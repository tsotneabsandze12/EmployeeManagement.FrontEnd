using Client.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Client.Helpers
{
    public static class UiHelpers
    {
        public static List<SelectListItem> EntityToSelectListItems(
            this IReadOnlyList<dynamic> entities)
        {
            return entities.Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = x.Name
            }).ToList();    
        }
    }
}
