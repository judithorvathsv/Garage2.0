using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Garage2._0.Models.ViewModels
{
    public class OverviewListModel
    {
        public IEnumerable<SelectListItem> VehicleTypesSelectList { get; set; }
        public IEnumerable<OverviewViewModel> Overview { get; set; }

        public string Regnumber{ get; set; }
        public VehicleTypes? Types { get; set; }
    }
}
