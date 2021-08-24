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
            var exists = db.Vehicle.Any(v => v.RegistrationNumber == searchText);

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
                // Fordonet finns inte
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
            var model = db.Vehicle.Select(o => new OverviewViewModel
            {
                VehicleId = o.Id,
                VehicleType = o.VehicleType,
                VehicleRegistrationNumber = o.RegistrationNumber,
                VehicleArrivalTime = o.TimeOfArrival,
                VehicleParkDuration = DateTime.Now - o.TimeOfArrival
            });

            return View(await model.ToListAsync());
        }

        // GET: Vehicles/Create
        public IActionResult Park()
        {
            return View();
        }
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
                db.Add(model);
                await db.SaveChangesAsync();
                return RedirectToAction("Details", new { id = model.Id });
            }
            return View(model);
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


        public async Task<IActionResult> Filter(string registrationNumber)
        {
            var model = string.IsNullOrWhiteSpace(registrationNumber) ?
                            db.Vehicle :
                            db.Vehicle.Where(m => m.RegistrationNumber.StartsWith(registrationNumber));
            return View(nameof(Index), await model.ToListAsync());
        }



        [HttpGet]
        public async Task<IActionResult> Overview(string sortingVehicle)
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


            var model = await allVehicles.Select(v => new OverviewViewModel
            {
                VehicleParked = v.IsParked,
                VehicleId = v.Id,
                VehicleType = v.VehicleType,
                VehicleRegistrationNumber = v.RegistrationNumber,
                VehicleArrivalTime = v.TimeOfArrival,
                VehicleParkDuration = DateTime.Now - v.TimeOfArrival

            }).ToListAsync();

            return View(model);
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