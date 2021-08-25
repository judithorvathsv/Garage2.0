using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Garage2._0.Models.ViewModels
{
    public class UnParkResponseViewModel
    {
        [Display(Name = "Id")]
        public int Id { get; set; }

        [Display(Name = "VehicleType")]
        public VehicleTypes VehicleType { get; set; }


        [Display(Name = "Registration Plate")]
        public string VehicleRegistrationNumber { get; set; }


        [Display(Name = "Arrival Time")]
        public DateTime VehicleArrivalTime { get; set; }


        [Display(Name = "Departure Time")]
        public DateTime VehicleDepartureTime { get; set; }


        [Display(Name = "Duration Parked")]
        [DisplayFormat(DataFormatString = "{0:%d} day(s) {0:hh'h 'mm'm 'ss's'}", ApplyFormatInEditMode = true)]
        public TimeSpan VehicleParkDuration { get; set; }


        [Display(Name = "Total cost")]
        [DisplayFormat(DataFormatString = "{0:C0}", ApplyFormatInEditMode = true)]
        public double VehicleParkPrice { get; set; }
    }
}



