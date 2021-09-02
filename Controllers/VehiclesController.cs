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

        //Start page where search on reg nr is done
        public IActionResult Index()
        {
            return View();
        }

        // Search for Vehicle
        public async Task<IActionResult> Search(string searchText)
        {
            var exists = db.Vehicle.Any(v => v.RegistrationNumber == searchText);

            if (exists)
            {
                var model = await db.Vehicle.FirstOrDefaultAsync(v => v.RegistrationNumber == searchText);

                return RedirectToAction("Details", new { id = model.Id });
            }
            else
            {
                TempData["Regnumber"] = searchText.ToUpper();

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

        public async Task<IActionResult> Overview(int parkedStatus)
        {
            var model = new OverviewListViewModel();
            model.VehicleTypesSelectList = await GetVehicleTypesAsync();

            var allVehicles = db.Vehicle;

            //IQueryable<OverviewViewModel> vehicles = GetOverviewViewModel(allVehicles);
            string parkedStatusStr = parkedStatus.ToString();
            var vehicles = GetOverviewViewModelAsEnumerable(allVehicles);
            parkedStatus = ParkingStatus(parkedStatusStr, model, ref vehicles);

            if (parkedStatus == 3)
            {
                model.AllStatus = true;
                ViewData["PakedStatus"] = "3";
            }
            else if (parkedStatus == 2)
            {
                model.UnparkedStatus = true;
                ViewData["PakedStatus"] = "2";
                vehicles = vehicles.Where(u => u.VehicleParked.Equals(false));
            }
            else
            {
                model.ParkedStatus = true;
                ViewData["PakedStatus"] = "1";
                vehicles = vehicles.Where(u => u.VehicleParked.Equals(true));
            }
            model.VehicleTypesSelectList = await GetVehicleTypesAsync();
            model.Overview = vehicles;

            return View("Overview", model);
        }

        private async Task<IEnumerable<SelectListItem>> GetVehicleTypesAsync()
        {
            return await db.Vehicle
                        .Where(w => w.IsParked == true)
                        .Select(t => t.VehicleType)
                        .Distinct()
                        .Select(g => new SelectListItem
                        {
                            Text = g.ToString(),
                            Value = g.ToString()
                        })
                        .ToListAsync();
        }

        public async Task<IActionResult> Filter(OverviewListViewModel viewModel)
        {
            //var filtermodel = new OverviewListViewModel();
            var model = new OverviewListViewModel();
            var vehicles = await db.Vehicle.ToListAsync();

            var result = string.IsNullOrWhiteSpace(viewModel.Regnumber) ?
                           vehicles :
                           vehicles.Where(m => m.RegistrationNumber.StartsWith(viewModel.Regnumber.ToUpper()));

            result = viewModel.Types == null ?
                                    result :
                                    result.Where(v => v.VehicleType == viewModel.Types);

            IQueryable<OverviewViewModel> vehi = result.Select(v => new OverviewViewModel
            {
                VehicleId = v.Id,
                VehicleType = v.VehicleType,
                VehicleRegistrationNumber = v.RegistrationNumber,
                VehicleArrivalTime = v.TimeOfArrival,
                VehicleParkDuration = DateTime.Now - v.TimeOfArrival,
                VehicleParked = false

            }).AsQueryable();

            model.Overview = vehi.AsEnumerable();

            model.VehicleTypesSelectList = await GetVehicleTypesAsync();

            return View("Overview", model);
        }


        [HttpGet, ActionName("OverviewSort")]
        public async Task<IActionResult> OverviewSort(string sortingVehicle)
        {
            string parkedStatusStr = sortingVehicle.Split(",")[1];
            sortingVehicle = sortingVehicle.Split(",")[0];

            ViewData["VehicleTypeSorting"] = string.IsNullOrEmpty(sortingVehicle) ? "VehicleTypeSortingDescending" : "";
            ViewData["RegistrationNumberSorting"] = sortingVehicle == "RegistrationNumberSortingAscending" ? "RegistrationNumberSortingDescending" : "RegistrationNumberSortingAscending";
            ViewData["ArrivalTimeSorting"] = sortingVehicle == "ArrivalTimeSortingAscending" ? "ArrivalTimeSortingDescending" : "ArrivalTimeSortingAscending";
            ViewData["DurationParkedSorting"] = sortingVehicle == "DurationParkedSortingAscending" ? "DurationParkedSortingDescending" : "DurationParkedSortingAscending";

            var model = new OverviewListViewModel();
            var allVehicles = db.Vehicle;
            var vehicles = GetOverviewViewModelAsEnumerable(allVehicles);

            int parkedStatus = 0;
            parkedStatus = ParkingStatus(parkedStatusStr, model, ref vehicles);

            switch (sortingVehicle)
            {
                case "VehicleTypeSortingAscending":
                    vehicles = vehicles.OrderBy(x => x.VehicleType);
                    break;
                case "VehicleTypeSortingDescending":
                    vehicles = vehicles.OrderByDescending(x => x.VehicleType);
                    break;
                case "RegistrationNumberSortingAscending":
                    vehicles = vehicles.OrderBy(x => x.VehicleRegistrationNumber);
                    break;
                case "RegistrationNumberSortingDescending":
                    vehicles = vehicles.OrderByDescending(x => x.VehicleRegistrationNumber);
                    break;
                case "ArrivalTimeSortingAscending":
                    vehicles = vehicles.OrderBy(x => x.VehicleArrivalTime);
                    break;
                case "ArrivalTimeSortingDescending":
                    vehicles = vehicles.OrderByDescending(x => x.VehicleArrivalTime);
                    break;
                case "DurationParkedSortingAscending":
                    vehicles = vehicles.OrderBy(x => x.VehicleParkDuration.Days);
                    break;
                case "DurationParkedSortingDescending":
                    vehicles = vehicles.OrderByDescending(x => x.VehicleParkDuration.Days);
                    break;

                default:
                    vehicles = vehicles.OrderBy(x => x.VehicleType);
                    break;
            }

            model.Overview = vehicles;
            model.VehicleTypesSelectList = await GetVehicleTypesAsync();

            return PartialView(nameof(Overview), model);
        }

        private int ParkingStatus(string parkedStatusStr, OverviewListViewModel model, ref IEnumerable<OverviewViewModel> vehicles)
        {
            int parkedStatus;
            if (int.TryParse(parkedStatusStr, out parkedStatus))
            {
                if (parkedStatus == 3)
                {
                    model.AllStatus = true;
                    ViewData["PakedStatus"] = "3";
                }
                else if (parkedStatus == 2)
                {
                    model.UnparkedStatus = true;
                    ViewData["PakedStatus"] = "2";
                    vehicles = vehicles.Where(u => u.VehicleParked.Equals(false));
                }
                else
                {
                    model.ParkedStatus = true;
                    ViewData["PakedStatus"] = "1";
                    vehicles = vehicles.Where(u => u.VehicleParked.Equals(true));
                }
            };
            return parkedStatus;
        }

        private IQueryable<OverviewViewModel> GetOverviewViewModel(IQueryable<Vehicle> allVehicles)
        {
            return allVehicles.Select(v => new OverviewViewModel
            {
                VehicleParked = v.IsParked,
                VehicleId = v.Id,
                VehicleType = v.VehicleType,
                VehicleRegistrationNumber = v.RegistrationNumber,
                VehicleArrivalTime = v.TimeOfArrival,
                VehicleParkDuration = DateTime.Now - v.TimeOfArrival

            });
        }

        private IEnumerable<OverviewViewModel> GetOverviewViewModelAsEnumerable(IQueryable<Vehicle> allVehicles)
        {
            return allVehicles.Select(v => new OverviewViewModel
            {
                VehicleParked = v.IsParked,
                VehicleId = v.Id,
                VehicleType = v.VehicleType,
                VehicleRegistrationNumber = v.RegistrationNumber,
                VehicleArrivalTime = v.TimeOfArrival,
                VehicleParkDuration = DateTime.Now - v.TimeOfArrival

            }).AsEnumerable();
        }

        // POST: Vehicles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Park([Bind("Id,VehicleType,RegistrationNumber,Color,Brand,VehicleModel,NumberOfWheels,IsParked,TimeOfArrival")] Vehicle vehicle)
        {
            bool registeredvehicle = db.Vehicle.Any(v => v.RegistrationNumber == vehicle.RegistrationNumber);
            //var regnumber = vehicle.RegistrationNumber.ToUpper();

            if (!registeredvehicle)
            {
                var model = new Vehicle
                {
                    RegistrationNumber = vehicle.RegistrationNumber.ToUpper(),
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
                        else { }
                    }
                }
                return View(model);
            }
            else
            {
                var existingvehicle = await db.Vehicle.FirstOrDefaultAsync(v => v.RegistrationNumber.Contains(vehicle.RegistrationNumber));
                TempData["Message"] = "A vehicle with this registration number is already registered!";
                return RedirectToAction("Details", new { id = existingvehicle.Id });
            }

        }
        [HttpGet]
        public async Task<IActionResult> ParkRegisteredVehicle(int? id)
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
            return RedirectToAction("UnparkResponse", new { id = vehicle.Id, departureTime });
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

        public bool Equals(Vehicle b1, Vehicle b2)
        {
            if (b1.RegistrationNumber == b2.RegistrationNumber && b1.Color == b2.Color && b1.Brand == b2.Brand
                && b1.VehicleModel == b2.VehicleModel && b1.NumberOfWheels == b2.NumberOfWheels && b1.VehicleType == b2.VehicleType)
                return true;
            else
                return false;
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Change(int Id, Vehicle vehicle)
        {
            //från databasen
            var v1 = await db.Vehicle.AsNoTracking().FirstOrDefaultAsync(v => v.Id == Id);

            if (!Equals(v1, vehicle))
            {

                if (Id != vehicle.Id)
                {
                    return NotFound();
                }

                string str = vehicle.Color;
                vehicle.RegistrationNumber = v1.RegistrationNumber;
                vehicle.Color = FirstLetterToUpper(str);
                vehicle.TimeOfArrival = v1.TimeOfArrival;

                if (ModelState.IsValid)
                {
                    try
                    {
                        db.Update(vehicle);
                        await db.SaveChangesAsync();
                        TempData["ChangedVehicle"] = "The vehicle is changed!";
                        return RedirectToAction("Details", new { vehicle.Id });
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
                }
            }
            //return View(vehicle);
            return RedirectToAction("Details", new { vehicle.Id });
        }

        private string FirstLetterToUpper(string str)
        {
            if (str == null)
                return null;

            if (str.Length > 1)
                return char.ToUpper(str[0]) + str.Substring(1);

            return str.ToUpper();
        }



        /*
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
        */

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
        public async Task<IActionResult> UnParkResponse(int id, DateTime departureTime)
        {
            //    var v = db.Vehicle
            //.Select(v => v.Id);
            var model = await db.Vehicle
                .Select(v => new UnParkResponseViewModel
                {
                    Id = v.Id,
                    VehicleType = v.VehicleType,
                    VehicleRegistrationNumber = v.RegistrationNumber,
                    VehicleArrivalTime = v.TimeOfArrival,
                    VehicleDepartureTime = departureTime,
                    //VehicleParkDuration=v.
                    //VehicleParkPrice
                })
                .FirstOrDefaultAsync(v => v.Id == id);

            return View("UnParkResponse", model);
            //ViewBag.id = id;
            //ViewBag.departureTime = departureTime;
            //return View(nameof(UnParkResponse));
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
                VehicleParkPrice = (departureTime - vehicle.TimeOfArrival).TotalHours * 100
            };

            return View(model);
        }

        private OverviewViewModel OverviewViewModelBuilder(Vehicle vehicle)
        {
            return new OverviewViewModel
            {
                VehicleParked = vehicle.IsParked,
                VehicleId = vehicle.Id,
                VehicleType = vehicle.VehicleType,
                VehicleRegistrationNumber = vehicle.RegistrationNumber,
                VehicleArrivalTime = vehicle.TimeOfArrival,
                VehicleParkDuration = DateTime.Now - vehicle.TimeOfArrival
            };

        }

        public async Task<IActionResult> Statistics()
        {
            var vehicles = await db.Vehicle.ToListAsync();

            var model = new StatisticsViewModel
            {
                VehicleTypesData = Enum.GetValues(typeof(VehicleTypes))
                                       .Cast<VehicleTypes>()
                                       .ToDictionary(type => type.ToString(), type => vehicles
                                                                                        .Where(v => v.VehicleType == type && v.IsParked)
                                                                                        .Count()),

                NumberOfWheels = vehicles
                                    .Where(v => v.IsParked)
                                    .Select(v => v.NumberOfWheels)
                                    .Sum(),

                GeneratedRevenue = vehicles
                                    .Where(v => v.IsParked)
                                    .Select(v =>
                                        (DateTime.Now - v.TimeOfArrival).TotalHours
                                      + (DateTime.Now - v.TimeOfArrival).TotalDays * 24
                                        )
                                    .Sum() * 100
            };
            return View(model);
        }
    }
}