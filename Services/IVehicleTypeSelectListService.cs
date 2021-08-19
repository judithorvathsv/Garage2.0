using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace Garage2._0.Services
{
    public interface IVehicleTypeSelectListService
    {
        IEnumerable<SelectListItem> GetVehicleType();
    }
}
