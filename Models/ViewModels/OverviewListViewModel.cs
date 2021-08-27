using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Garage2._0.Models.ViewModels
{
    public class OverviewListViewModel
    {
        public IEnumerable<SelectListItem> VehicleTypesSelectList { get; set; }
        public IEnumerable<OverviewViewModel> Overview { get; set; }

        //SelectListBox
        public string Regnumber { get; set; }
        public VehicleTypes? Types { get; set; }

        // Radiobuttons
        public bool ParkedStatus { get; set; }
        public bool UnparkedStatus { get; set; }
        public bool AllStatus { get; set; }

        public class InputProperties
        {
            public string Name { get; set; }
            public string ElementId { get; set; }
            public bool IsSelected { get; set; }
            public bool IsDisabled { get; set; }
        }

        [BindProperty]
        public List<InputProperties> RadioButtons { get; set; }

        [BindProperty]
        public string SelectedRadio { get; set; }

        public void OnGet()
        {
            RadioButtons = new List<InputProperties>()
            {
                new InputProperties() { Name = "ParkedStatus", ElementId = "Parked" },
                new InputProperties() { Name = "UnparkedStatus", ElementId = "UnParked" },
                new InputProperties() { Name = "AllStatus", ElementId = "ParkedAll" }
            };
            SelectedRadio = "Warning";
        }


    }
}
