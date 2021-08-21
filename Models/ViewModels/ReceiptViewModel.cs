﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Garage2._0.Models.ViewModels
{
    public class ReceiptViewModel
    {
        public int VehicleId { get; set; }
        
        [Display(Name = "Registration Plate")]
        public string VehicleRegistrationNumber { get; set; }
        
        [Display(Name = "Arrival Time")]
        public DateTime VehicleArrivalTime { get; set; }
        
        [Display(Name = "Departure Time")]
        public DateTime VehicleDepartureTime { get; set; }

        [DisplayFormat(DataFormatString = "{0:%d} day(s) {0:hh'h 'mm'm 'ss's'}", ApplyFormatInEditMode = true)]
        [Display(Name = "Duration Parked")]
        public TimeSpan VehicleParkDuration { get; set; }
    }
}