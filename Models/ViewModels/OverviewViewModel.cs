using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Garage2._0.Models.ViewModels
{
    public class OverviewViewModel
    {
        public bool VehicleParked { get; set; }
        
        public int VehicleId { get; set; }

        [Display(Name = "Vehicle Type")]
        public VehicleTypes? VehicleType { get; set; }
        
        [Display(Name = "Registration Plate")]
        public string VehicleRegistrationNumber { get; set; }
        
        [Display(Name = "Arrival Time")]
        public DateTime VehicleArrivalTime { get; set; }
        
        [DisplayFormat(DataFormatString = "{0:%d} day(s) {0:hh'h 'mm'm 'ss's'}", ApplyFormatInEditMode = true)]
        [Display(Name = "Duration Parked")]
        public TimeSpan VehicleParkDuration { get; set; }
    }
}