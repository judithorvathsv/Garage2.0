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

        // GET: Vehicles
        public async Task<IActionResult> Index()
        {
            return View(await db.Vehicle.ToListAsync());
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
            var model =  db.Vehicle.Select(o => new OverviewViewModel
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
        public async Task<IActionResult>ParkRegisteredVehicle(int? id)
        {
            var vehicle = await db.Vehicle.FirstOrDefaultAsync(x => x.Id == id);
            vehicle.IsParked = true;

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
            return RedirectToAction("Receipt");
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


        public async Task<IActionResult> Order(int id)
        {
            if (id == 1) { 
            var model = db.Vehicle.OrderBy(o => o.RegistrationNumber);           
            return View(nameof(Index), await model.ToListAsync());
            }

            if (id == 2){


         



                 var model = db.Vehicle.OrderBy(o => o.VehicleType);
                return View(nameof(Index), await model.ToListAsync());
            }
            if (id == 3)
            {
                var model = db.Vehicle.OrderBy(o => o.Color);
                return View(nameof(Index), await model.ToListAsync());
            }
            if (id == 4)
            {
                var model = db.Vehicle.OrderBy(o => o.Brand);
                return View(nameof(Index), await model.ToListAsync());
            }
            if (id == 5)
            {
                var model = db.Vehicle.OrderBy(o => o.VehicleModel);
                return View(nameof(Index), await model.ToListAsync());
            }
            if (id == 6)
            {
                var model = db.Vehicle.OrderBy(o => o.NumberOfWheels);
                return View(nameof(Index), await model.ToListAsync());
            }
            if (id == 7)
            {
                var model = db.Vehicle.OrderBy(o => o.IsParked);
                return View(nameof(Index), await model.ToListAsync());
            }
            if (id == 8)
            {
                var model = db.Vehicle.OrderBy(o => o.TimeOfArrival);
                return View(nameof(Index), await model.ToListAsync());
            }

            return View(nameof(Index));
        }




        // GET: Vehicles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicle = await db.Vehicle.FindAsync(id);
            if (vehicle == null)
            {
                return NotFound();
            }
            return View(vehicle);
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

        public IActionResult Receipt()
        {
            return View();
        }
    }
}
