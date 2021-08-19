using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Garage2._0.Services
{
    public interface IVehicleTypeSelectListService
    {
        Task<IEnumerable<SelectListItem>> GetVehicleTypeAsync();
    }
}
