using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Garage2._0.Models
{
    public class Vehicle
    {

        public int Id { get; set; }
        [Required]
        public VehicleTypes VehicleType { get; set; }

        [Required(ErrorMessage = "Please enter registration number!")]
        [MaxLength(10)]
        [RegularExpression("^([a-zA-Z-0-9]+)$", ErrorMessage = "Invalid registration number!")]
        public string RegistrationNumber { get; set; }

        [MaxLength(20)]
        public string Color { get; set; }

        [MaxLength(20)]
        public string Brand { get; set; }

        [MaxLength(30)]
        public string VehicleModel { get; set; }

        [Range(0, 10, ErrorMessage = "Please enter correct number between 0-10!")]
        public int NumberOfWheels { get; set; }

        public bool IsParked { get; set; }
        
        public DateTime TimeOfArrival { get; set; }
    }
}
