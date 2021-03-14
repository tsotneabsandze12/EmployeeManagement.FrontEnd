using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Client.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Client.Helpers;

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

            var employees = await GetEmployeesAsync(firstName, lastName);
            return View(employees);
        }

        [HttpGet]
        public async Task<IReadOnlyList<EmployeeModel>> GetEmployeesAsync(string firstName, string lastName)
        {
            var token = HttpContext.Session.GetString("token");
            var client = new HttpClient();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response =
                await client.GetAsync(
                    $"{_config["BaseApiUrl"]}api/Employees?FirstName={firstName}&LastName={lastName}");

            
            var dataString = await response.Content.ReadAsStringAsync();
            var employees = JsonConvert.DeserializeObject<IReadOnlyList<EmployeeModel>>(dataString);

            return employees;
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var jwt = HttpContext.Session.GetString("token");

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

            var response = await client.GetAsync($"{_config["BaseApiUrl"]}api/Employees/get/{id}");

            if (!response.IsSuccessStatusCode) return View("EmployeeNotFound", id);


            var strContent = await response.Content.ReadAsStringAsync();

            var employee = JsonConvert.DeserializeObject<EmployeeModel>(strContent);

            return View(employee);
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var token = HttpContext.Session.GetString("token");

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync($"{_config["BaseApiUrl"]}api/Employees/get/{id}");

            if (!response.IsSuccessStatusCode) return View("EmployeeNotFound", id);

            var stringContent = await response.Content.ReadAsStringAsync();
            var employeeToDelete = JsonConvert.DeserializeObject<EmployeeModel>(stringContent);

            return View(employeeToDelete);
        }


        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var token = HttpContext.Session.GetString("token");

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var res = await client.DeleteAsync($"{_config["BaseApiUrl"]}api/Employees/delete/{id}");

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> AddEmployee()
        {
            var positions = await GetPositionsAsync();
            var employee = new AddEmployeeModel
            {
                Positions = positions.EntityToSelectListItems()
            };

            return View(employee);
        }

        [HttpPost]
        public async Task<IActionResult> AddEmployee(AddEmployeeModel model)
        {
            if (ModelState.IsValid)
            {
                var token = HttpContext.Session.GetString("token");

                var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var stringContent =
                    new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

                var result = await client.PostAsync($"{_config["BaseApiUrl"]}api/Employees/add", stringContent);

                if (result.IsSuccessStatusCode)
                    return RedirectToAction(nameof(Index));
                
            }

            ModelState.AddModelError(string.Empty, "invalid attempt");
            var positions = await GetPositionsAsync();
            model.Positions = positions.EntityToSelectListItems();

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EditEmployee(int id)
        {
            var token = HttpContext.Session.GetString("token");

            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await client.GetAsync($"{_config["BaseApiUrl"]}api/Employees/get/{id}");

            if (!response.IsSuccessStatusCode) return View("EmployeeNotFound", id);
            
            var stringContent = await response.Content.ReadAsStringAsync();
            var employeeToEdit = JsonConvert.DeserializeObject<EditEmployeeModel>(stringContent);


            var positions = await GetPositionsAsync();
            employeeToEdit.Positions = positions.EntityToSelectListItems();


            return View(employeeToEdit);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditEmployee(int id, EditEmployeeModel model)
        {
            if (ModelState.IsValid)
            {
                var token = HttpContext.Session.GetString("token");

                var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var stringContent =
                    new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

                var response =
                    await client.PutAsync($"{_config["BaseApiUrl"]}api/Employees/update/{id}", stringContent);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Details), new {id = id});
                }
            }


            ModelState.AddModelError(string.Empty, "invalid update attempt");
            return View(model);
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

        [HttpGet]
        [HttpPost(Name = "CheckIfPersonalIdExists")]
        public async Task<JsonResult> CheckIfPersonalIdExists([FromForm(Name = "vm.data.PersonalId")]
            string personalId)
        {
            var client = new HttpClient();
            var responseString =
                await client.GetStringAsync(
                    $"{_config["BaseApiUrl"]}api/Employees/personalIdExists?personalId={personalId}");

            var result = JsonConvert.DeserializeObject<bool>(responseString);

            return result ? Json($"personal id {personalId}  belongs to someone else") : Json(true);
        }


        [HttpGet]
        [HttpPost]
        public async Task<JsonResult> CheckIfPhoneNumberExists(string phone)
        {
            var client = new HttpClient();
            var responseString =
                await client.GetStringAsync($"{_config["BaseApiUrl"]}api/Employees/phoneExists?number={phone}");

            var result = JsonConvert.DeserializeObject<bool>(responseString);

            return result ? Json($"phone number {phone}  belongs to someone else") : Json(true);
        }
    }
}