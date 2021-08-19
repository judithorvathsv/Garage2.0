using Garage2._0.Data;
using Garage2._0.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Garage2._0.Services
{
    public class VehicleTypeSelectListService: IVehicleTypeSelectListService
    {
        private readonly Garage2_0Context db;

        public VehicleTypeSelectListService(Garage2_0Context db)
        {
            this.db = db;
        }

        public async Task<IEnumerable<SelectListItem>> GetVehicleTypeAsync()
        {
            return Enum.GetValues(typeof(VehicleTypes))
             .Cast<VehicleTypes>()
             .Select(v => new SelectListItem
             {
                 Text = v.ToString(),
                 Value = v.ToString()
             }).ToList();
        }


            //var vehicleTypes = new List<SelectListItem>();
            //foreach (var v in Enum.GetValues(typeof(VehicleTypes)))
            //{

            //    vehicleTypes.Add(v.ToString()); ;
            //}
            //Enum.GetValues(typeof(VehicleTypes))

            //return await Enum.GetValues(typeof(VehicleTypes))
            // .Cast<VehicleTypes>()
            // .Select(v => new SelectListItem
            // {
            //     Text = v.ToString(),
            //     Value = v.ToString()
            // }).ToList();
     
    }
}
