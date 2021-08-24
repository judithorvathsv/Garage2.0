using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Garage2._0.Data;
using Garage2._0.Models;
using Garage2._0.Models.ViewModels;
using System.Data.Entity.Validation;

namespace Garage2._0.Controllers
{
    public class VehiclesController : Controller
    {
        private readonly Garage2_0Context db;

        public VehiclesController(Garage2_0Context context)
        {
            db = context;
        }

        // Search for Vehicle
        public async Task<IActionResult> Search(string searchText)
        {
            var exists =  db.Vehicle.Any(v => v.RegistrationNumber == searchText);

            if (exists)
            {
                var model = await db.Vehicle.FirstOrDefaultAsync(v => v.RegistrationNumber == searchText);

                if (model.IsParked)
                {
                    // Visa parkerat fordon
                    return RedirectToAction("Details", new { id = model.Id });

                }
                else
                {
                    // Gå till en vy där man kan parkera
                    return RedirectToAction("Details", new { id = model.Id });
                }
            }
            else
            {
                TempData["Regnumber"] = searchText.ToUpper();
                // Fordonet finns inte
                //ViewBag.RegNr = searchText.ToUpper();
                return View(nameof(Park));
            }
        }


        // GET: Vehicles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicle = await db.Vehicle
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vehicle == null)
            {
                return NotFound();
            }

            return View(vehicle);
        }

        public async Task<IActionResult> Overview()
        {
            var model = new OverviewListModel();
            model.Overview = await db.Vehicle.Select(v => new OverviewViewModel
            {
                VehicleId = v.Id,
                VehicleType = v.VehicleType,
                VehicleRegistrationNumber = v.RegistrationNumber,
                VehicleArrivalTime = v.TimeOfArrival,
                VehicleParkDuration = DateTime.Now - v.TimeOfArrival
            }).ToListAsync();
            model.VehicleTypesSelectList = await GetVehicleTypesAsync();
            return View("Overview", model);
        }

        private async Task<IEnumerable<SelectListItem>> GetVehicleTypesAsync()
        {
            return await db.Vehicle
                        .Select(t => t.VehicleType)
                        .Distinct()
                        .Select(g => new SelectListItem
                        {
                            Text = g.ToString(),
                            Value = g.ToString()
                        })
                        .ToListAsync();
        }

        public async Task<IActionResult> Filter(OverviewListModel viewModel)
        {
            var model = new OverviewListModel();
            var result = string.IsNullOrWhiteSpace(viewModel.Regnumber) ?
                           db.Vehicle :
                           db.Vehicle.Where(m => m.RegistrationNumber.StartsWith(viewModel.Regnumber));

            result = viewModel.Types == null ?
                                    result :
                                    result.Where(v => v.VehicleType == viewModel.Types);

            model.Overview = await result.Select(v => new OverviewViewModel
            {
                VehicleId = v.Id,
                VehicleType = v.VehicleType,
                VehicleRegistrationNumber = v.RegistrationNumber,
                VehicleArrivalTime = v.TimeOfArrival,
                VehicleParkDuration = DateTime.Now - v.TimeOfArrival
            }).ToListAsync();
            model.VehicleTypesSelectList = await GetVehicleTypesAsync();

            return View(nameof(Overview), model);
        }

        [HttpGet, ActionName("Overview")]
        public async Task<IActionResult> OverviewSort(string sortingVehicle)
        {
            ViewData["VehicleTypeSorting"] = string.IsNullOrEmpty(sortingVehicle) ? "VehicleTypeSortingDescending" : "";
            ViewData["RegistrationNumberSorting"] = sortingVehicle == "RegistrationNumberSortingAscending" ? "RegistrationNumberSortingDescending" : "RegistrationNumberSortingAscending";
            ViewData["ArrivalTimeSorting"] = sortingVehicle == "ArrivalTimeSortingAscending" ? "ArrivalTimeSortingDescending" : "ArrivalTimeSortingAscending";
            ViewData["DurationParkedSorting"] = sortingVehicle == "DurationParkedSortingAscending" ? "DurationParkedSortingDescending" : "DurationParkedSortingAscending";


            var allVehicles = db.Vehicle.Select(v => v);

            switch (sortingVehicle)
            {
                case "VehicleTypeSortingDescending":
                    allVehicles = allVehicles.OrderByDescending(x => x.VehicleType);
                    break;


                case "RegistrationNumberSortingAscending":
                    allVehicles = allVehicles.OrderBy(x => x.RegistrationNumber);
                    break;
                case "RegistrationNumberSortingDescending":
                    allVehicles = allVehicles.OrderByDescending(x => x.RegistrationNumber);
                    break;


                case "ArrivalTimeSortingAscending":
                    allVehicles = allVehicles.OrderBy(x => x.TimeOfArrival);
                    break;
                case "ArrivalTimeSortingDescending":
                    allVehicles = allVehicles.OrderByDescending(x => x.TimeOfArrival);
                    break;


                case "DurationParkedSortingAscending":
                    allVehicles = allVehicles.OrderBy(x => x.TimeOfArrival);
                    break;
                case "DurationParkedSortingOrderByDescending":
                    allVehicles = allVehicles.OrderByDescending(x => x.TimeOfArrival);
                    break;

                default:
                    allVehicles = allVehicles.OrderBy(x => x.VehicleType);
                    break;
            }

            var model = new OverviewListModel();
            model.Overview = await allVehicles.Select(v => new OverviewViewModel
            {
                VehicleId = v.Id,
                VehicleType = v.VehicleType,
                VehicleRegistrationNumber = v.RegistrationNumber,
                VehicleArrivalTime = v.TimeOfArrival,
                VehicleParkDuration = DateTime.Now - v.TimeOfArrival

            }).ToListAsync();

            model.VehicleTypesSelectList = await GetVehicleTypesAsync();

            return View(model);
        }

        // GET: Vehicles/Create
        public IActionResult Park()
        {
            return View();
        }

        //Start page where search on reg nr is done
        public IActionResult Index()
        {
            return View();
        }
        // POST: Vehicles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Park([Bind("Id,VehicleType,RegistrationNumber,Color,Brand,VehicleModel,NumberOfWheels,IsParked,TimeOfArrival")] Vehicle vehicle)
        {
            bool registeredvehicle = db.Vehicle.Any(v => v.RegistrationNumber == vehicle.RegistrationNumber);
 
            if (!registeredvehicle)
            {
                var model = new Vehicle
                {
                    RegistrationNumber = vehicle.RegistrationNumber,
                    VehicleType = vehicle.VehicleType,
                    Brand = vehicle.Brand,
                    Color = vehicle.Color,
                    VehicleModel = vehicle.VehicleModel,
                    NumberOfWheels = vehicle.NumberOfWheels,
                    IsParked = true,
                    TimeOfArrival = DateTime.Now
                };

                if (ModelState.IsValid)
                {
                    try
                    {
                        db.Add(model);
                        await db.SaveChangesAsync();
                        TempData["Message"] = "";
                        return RedirectToAction("Details", new { id = model.Id });
                    }
                    catch (Exception ex)
                    {
                        if (ex.GetType() == typeof(DbEntityValidationException))
                        {
                            //Exception thrown from System.Data.Entity.DbContext.SaveChanges when validating entities fails.
                        }
                        if (ex.GetType() == typeof(DbUnexpectedValidationException))
                        {
                            //Exception thrown from System.Data.Entity.DbContext.GetValidationErrors when an
                            //exception is thrown from the validation code.
                        }
                    }
                }
                return View(model);
            }
            else
            {
                var existingvehicle = await db.Vehicle.FirstOrDefaultAsync(v => v.RegistrationNumber.Contains(vehicle.RegistrationNumber));
                TempData["Message"] = "Your vehicle is alredy registred!";
                return RedirectToAction("Details", new { id = existingvehicle.Id });
            }
    
        }
        [HttpGet]
        public async Task<IActionResult>ParkRegisteredVehicle(int? id)
        {
            var vehicle = await db.Vehicle.FirstOrDefaultAsync(x => x.Id == id);
            vehicle.IsParked = true;
            vehicle.TimeOfArrival = DateTime.Now;

            try
            {
                db.Update(vehicle);
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {

                if (vehicle.Id != id)
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction("Details", new { id = vehicle.Id });
        }
        [HttpGet]
        public async Task<IActionResult> UnPark(int? id)
        {
            var vehicle = await db.Vehicle.FirstOrDefaultAsync(x => x.Id == id);
            vehicle.IsParked = false;
            var departureTime = DateTime.Now;

            try
            {
                db.Update(vehicle);
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {

                if (vehicle.Id != id)
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction("Receipt", new { id = vehicle.Id, departureTime });
        }

        public async Task<IActionResult> Change(int? Id)
        {         
             if (Id == null)
            {
                return NotFound();
            }

             var vehicle = await db.Vehicle.FindAsync(Id);

            if (vehicle == null)
            {
                return NotFound();
            }
            return View(vehicle);        
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Change(int Id, [Bind("Id,VehicleType,RegistrationNumber,Color,Brand,VehicleModel,NumberOfWheels,IsParked,TimeOfArrival")] Vehicle vehicle)
        {                     
            if (Id != vehicle.Id)
            {
                return NotFound();
            }

            string str = vehicle.Color;
            vehicle.Color = FirstLetterToUpper(str);

            if (ModelState.IsValid)
            {
                try
                {
                    db.Update(vehicle);
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {

                    if (!VehicleExists(vehicle.Id))                  
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }         
                return RedirectToAction("Details", new { id = vehicle.Id });
            }
            return View(vehicle);    
        }

        private string FirstLetterToUpper(string str)
        {
            if (str == null)
                return null;

            if (str.Length > 1)
                return char.ToUpper(str[0]) + str.Substring(1);

            return str.ToUpper();
        }

        
        // POST: Vehicles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,VehicleType,RegistrationNumber,Color,Brand,VehicleModel,NumberOfWheels,IsParked,TimeOfArrival")] Vehicle vehicle)
        {
            if (id != vehicle.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    db.Update(vehicle);
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VehicleExists(vehicle.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(vehicle);
        }
        

        // GET: Vehicles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicle = await db.Vehicle
                .FirstOrDefaultAsync(m => m.Id == id);
            if (vehicle == null)
            {
                return NotFound();
            }

            return View(vehicle);
        }

        // POST: Vehicles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var vehicle = await db.Vehicle.FindAsync(id);
            db.Vehicle.Remove(vehicle);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VehicleExists(int id)
        {
            return db.Vehicle.Any(e => e.Id == id);
        }

        public async Task<IActionResult> Receipt(int id, DateTime departureTime)
        {
            var vehicle = await db.Vehicle.FindAsync(id);

            var model = new ReceiptViewModel
            {
                VehicleRegistrationNumber = vehicle.RegistrationNumber,
                VehicleArrivalTime = vehicle.TimeOfArrival,
                VehicleDepartureTime = departureTime, 
                VehicleParkDuration = vehicle.TimeOfArrival - departureTime,
                VehicleParkPrice = (DateTime.Now - vehicle.TimeOfArrival).TotalHours * 100
            };

            return View(model);
        }
    }
}