using Garage2._0.Data;
using Garage2._0.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Garage2._0.Services
{
    public class VehicleTypeSelectListService: IVehicleTypeSelectListService
    {
        private readonly Garage2_0Context db;

        public VehicleTypeSelectListService(Garage2_0Context db)
        {
            this.db = db;
        }

        public IEnumerable<SelectListItem> GetVehicleType()
        {
            return 
            Enum.GetValues(typeof(VehicleTypes))
            .Cast<VehicleTypes>()
            .Select(v => new SelectListItem
            {
            Text = v.ToString(),
            Value = v.ToString()
            }).ToList();
        }
    }
}
