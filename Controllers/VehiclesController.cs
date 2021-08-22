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
                    return RedirectToAction("ResponsePark", new { id = model.Id });

                }
                else
                {
                    // Gå till en vy där man kan parkera
                    return RedirectToAction("ResponsePark", new { id = model.Id });
                }
            }
            else
            {
                TempData["Regnumber"] = searchText;
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
        public async Task<PartialViewResult> _ParkDetails(int? id)
        {
            var vehicle = await db.Vehicle
                .FirstOrDefaultAsync(m => m.Id == id);

            return PartialView(vehicle);
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
            bool registeredvehicle = db.Vehicle.Any(v => v.RegistrationNumber == vehicle.RegistrationNumber);

            string str = vehicle.Color;
            vehicle.Color = FirstLetterToUpper(str);

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
                        return RedirectToAction("ResponsePark", new { id = model.Id });
                    }
                    catch (Exception ex)
                    {
                        if (ex.GetType() == typeof(DbEntityValidationException))
                        {
                            //Exception thrown from System.Data.Entity.DbContext.SaveChanges when validating entities fails.
                        }
                        else
                        if (ex.GetType() == typeof(DbUnexpectedValidationException))
                        {
                            //Exception thrown from System.Data.Entity.DbContext.GetValidationErrors when an
                            //exception is thrown from the validation code.
                        }
                        else
                        {
                            //All remaining exception here 
                        }
                    }
                }
                return View(model);
            }
            else
            {
                var existingvehicle = await db.Vehicle.FirstOrDefaultAsync(v => v.RegistrationNumber.Contains(vehicle.RegistrationNumber));
                TempData["Message"] = "Your vehicle is alredy registred!";
                return RedirectToAction("ResponsePark", new { id = existingvehicle.Id });
            }
    
        }

        public async Task<IActionResult> ResponsePark(int? id)
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
            return RedirectToAction("ResponsePark", new { id = vehicle.Id });
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
            return PartialView(nameof(_ParkChange),vehicle);        
        }


        public async Task<IActionResult> _ParkChange(int? Id)
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
            return PartialView(vehicle);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Change(int Id, [Bind("Id,VehicleType,RegistrationNumber,Color,Brand,VehicleModel,NumberOfWheels,IsParked,TimeOfArrival")] Vehicle vehicle)
        {                     
            if (Id != vehicle.Id)
            {
                return NotFound();
            }
            //Make the first letter to upper charcter;

            string str = vehicle.Color;
            vehicle.Color = FirstLetterToUpper(str);
            //str = char.ToUpper(str[0]) + str.Substring(1);
            //vehicle.Color = str;

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
                return RedirectToAction("ResponsePark", new { id = vehicle.Id });
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


        public async Task<IActionResult> Filter(string registrationNumber)
        {
            var model = string.IsNullOrWhiteSpace(registrationNumber) ?
                            db.Vehicle :
                            db.Vehicle.Where(m => m.RegistrationNumber.StartsWith(registrationNumber));
            return View(nameof(Index), await model.ToListAsync());        
        }


        public async Task<IActionResult> Order()
        {
            var model = db.Vehicle.OrderBy(o => o.RegistrationNumber);
            return View(nameof(Index), await model.ToListAsync());
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
