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

        [Display(Name = "Vehicle type")]
        [Required]
        public VehicleTypes VehicleType { get; set; }

        [Display(Name = "Registration number")]
        [Required(ErrorMessage = "Please enter registration number!")]
        [MaxLength(10)]
        [MinLength(3)]
        [RegularExpression("^([-a-zA-Z-0-9-]+)$", ErrorMessage = "Invalid registration number!")]
        public string RegistrationNumber { get; set; }

        [RegularExpression("^([a-zA-Z ]+)$", ErrorMessage = "Invalid color!")]
        [Required(ErrorMessage = "Please enter color!")]
        [MaxLength(20)]
        [MinLength(3)]
        public string Color { get; set; }

        [RegularExpression("^([a-zA-Z-0-9- ]+)$", ErrorMessage = "Invalid brand!")]
        [Required(ErrorMessage = "Please enter brand!")]
        [MaxLength(20)]
        [MinLength(3)]
        public string Brand { get; set; }

        [Display(Name = "Vehicle model")]
        [RegularExpression("^([-a-zA-Z-0-9- ]+)$", ErrorMessage = "Invalid model!")]
        [Required(ErrorMessage = "Please enter model!")]   
        [MaxLength(20)]
        [MinLength(1)]
        public string VehicleModel { get; set; }

        [Display(Name = "Number of wheels")]
        [Required(ErrorMessage = "Please enter number of wheels!")]
        [Range(0, 30, ErrorMessage = "Please enter correct number between 0-30!")]
        public int NumberOfWheels { get; set; }

        public bool IsParked { get; set; }
        
        public DateTime TimeOfArrival { get; set; }

        internal bool SequenceEqual(Vehicle vehicle)
        {
            throw new NotImplementedException();
        }

        //public TimeSpan TimeParked { get; set; }
    }
}
