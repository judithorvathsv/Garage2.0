using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Garage2._0.Models.ViewModels
{
    public class OverviewViewModel
    {
        public VehicleTypes VehicleType { get; set; }
        public string VehicleRegistrationNumber { get; set; }
        public DateTime VehicleArrivalTime { get; set; }
        public TimeSpan VehicleParkDuration { get; set; }
    }
}