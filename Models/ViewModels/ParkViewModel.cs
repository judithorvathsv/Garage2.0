using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Garage2._0.Models.ViewModels
{
    public class ParkViewModel
    {
  
        public VehicleTypes VehicleType { get; set; }

        public string RegistrationNumber { get; set; }

        public string Color { get; set; }

        public string Brand { get; set; }

        public string VehicleModel { get; set; }

        public int NumberOfWheels { get; set; }

        public bool IsParked { get; set; }

        public DateTime TimeOfArrival { get; set; }
    }
}
