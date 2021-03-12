using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Client.Helpers;
using Microsoft.AspNetCore.Mvc;
using Client.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace Client.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _config;

        public HomeController(IConfiguration config)
        {
            _config = config;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string firstName, string lastName)
        {
            ViewBag.Filters = new ArrayList
            {
               firstName,
               lastName
            };

            try
            {

                var employees = await GetEmployeesAsync(firstName, lastName);
                return View(employees);
            }
            catch (Exception)
            {
                // some error view thingy needs to be done here 
                return Redirect("https://stackoverflow.com/questions/22626837/how-to-connect-multiple-virtual-machines-in-lan");
            }
        }

        [HttpGet]
        public async Task<IReadOnlyList<EmployeeModel>> GetEmployeesAsync(string firstName, string lastName)
        {
            var token = HttpContext.Session.GetString("token");
            var client = new HttpClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync($"{_config["BaseApiUrl"]}api/Employees?FirstName={firstName}&LastName={lastName}");


            if (!response.IsSuccessStatusCode) throw new Exception(nameof(response));

            var dataString = await response.Content.ReadAsStringAsync();
            var employees = JsonConvert.DeserializeObject<IReadOnlyList<EmployeeModel>>(dataString);

            return employees;
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            //error handling needs to  be implemented here
            var jwt = HttpContext.Session.GetString("token");

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

            var strContent = await client.GetStringAsync($"{_config["BaseApiUrl"]}api/Employees/get/{id}");

            var employee = JsonConvert.DeserializeObject<EmployeeModel>(strContent);


            return View(employee);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {

            var token = HttpContext.Session.GetString("token");

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var content = await client.GetAsync($"{_config["BaseApiUrl"]}api/Employees/get/{id}");

            if (content.IsSuccessStatusCode)
            {
                var stringContent = await content.Content.ReadAsStringAsync();
                var employeeToDelete = JsonConvert.DeserializeObject<EmployeeModel>(stringContent);

                return View(employeeToDelete);
            }

            // error handling here as well
            return Redirect("https://www.google.com/search?q=not+found&sxsrf=ALeKk03yzk1gLRw4BSkLd5GG6qZEbHGZdw:1615479505263&source=lnms&tbm=isch&sa=X&ved=2ahUKEwidg7aQ0qjvAhUEzoUKHanUBk4Q_AUoAXoECBQQAw&biw=1366&bih=663#imgrc=uOy188n5ILXlUM");

        }

        //httpDelete
        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var token = HttpContext.Session.GetString("token");

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var res = await client.DeleteAsync($"{_config["BaseApiUrl"]}api/Employees/delete/{id}");

            if (res.IsSuccessStatusCode)
                return RedirectToAction(nameof(Index));


            // error handling here as well
            return Redirect("https://www.google.com/search?q=not+found&sxsrf=ALeKk03yzk1gLRw4BSkLd5GG6qZEbHGZdw:1615479505263&source=lnms&tbm=isch&sa=X&ved=2ahUKEwidg7aQ0qjvAhUEzoUKHanUBk4Q_AUoAXoECBQQAw&biw=1366&bih=663#imgrc=uOy188n5ILXlUM");
        }

        [HttpGet]
        public async Task<IActionResult> AddEmployee()
        {
            var positions = await GetPositionsAsync();
            ViewBag.Positions = positions.Select(n => new SelectListItem
            {
                Value = n.Id.ToString(),
                Text = n.Name
            }).ToList();


            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddEmployee(AddEmployeeModel model)
        {
            if (!ModelState.IsValid) return View(model);

            if (await ApiUtilities.CheckIfFieldExists($"{_config["BaseApiUrl"]}api/Employees/personalIdExists?personalId={model.PersonalId}"))
            {
                ModelState.AddModelError(string.Empty, $"{model.PersonalId} belongs to someone else");
                return View(model);
            }
            
            if (await ApiUtilities.CheckIfFieldExists($"{_config["BaseApiUrl"]}api/Employees/phoneExists?number={model.PhoneNumber}"))
            {
                ModelState.AddModelError(string.Empty, $"{model.PhoneNumber} belongs to someone else");
                return View(model);
            }

            var token = HttpContext.Session.GetString("token");

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

            var result = await client.PostAsync($"{_config["BaseApiUrl"]}api/Employees/add", stringContent);

            if (result.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            //error handling
            return Redirect("https://www.google.com/search?q=not+found&sxsrf=ALeKk03yzk1gLRw4BSkLd5GG6qZEbHGZdw:1615479505263&source=lnms&tbm=isch&sa=X&ved=2ahUKEwidg7aQ0qjvAhUEzoUKHanUBk4Q_AUoAXoECBQQAw&biw=1366&bih=663#imgrc=uOy188n5ILXlUM");
        }

        
        [HttpGet]
        public async Task<IReadOnlyList<PositionModel>> GetPositionsAsync()
        {
            var token = HttpContext.Session.GetString("token");

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync($"{_config["BaseApiUrl"]}api/Employees/getPositions");

            if (!response.IsSuccessStatusCode) throw new Exception(nameof(response));

            var dataString = await response.Content.ReadAsStringAsync();
            var positions = JsonConvert.DeserializeObject<IReadOnlyList<PositionModel>>(dataString);

            return positions;
        }

        
        // [HttpGet]
        // [HttpPost]
        // public async Task<IActionResult> CheckIfPersonalIdExists(string personalId)
        // {
        //     var client = new HttpClient();
        //     var responseString = await client.GetStringAsync($"{_config["BaseApiUrl"]}api/Employees/personalIdExists?personalId={personalId}");
        //
        //     var result = JsonConvert.DeserializeObject<bool>(responseString);
        //
        //     return result ? Json(true) : Json(false);
        // }
        //
        //
        // [HttpGet]
        // [HttpPost]
        // public async Task<IActionResult> CheckIfPhoneNumberExists(string phone)
        // {
        //     var client = new HttpClient();
        //     var responseString = await client.GetStringAsync($"{_config["BaseApiUrl"]}api/Employees/phoneExists?number={phone}");
        //
        //     var result = JsonConvert.DeserializeObject<bool>(responseString);
        //
        //     return result ? Json(true) : Json(false);
        // }
    }
}
