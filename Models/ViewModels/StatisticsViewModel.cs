using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Garage2._0.Models.ViewModels
{
    public class StatisticsViewModel
    {
        [Display(Name = "Number of wheels parked: ")]
        public int NumberOfWheels { get; set; }

        [Display(Name = "Generated Revenue: ")]
        [DisplayFormat(DataFormatString = "{0:C0}", ApplyFormatInEditMode = true)]
        public double GeneratedRevenue { get; set; }
    }
}