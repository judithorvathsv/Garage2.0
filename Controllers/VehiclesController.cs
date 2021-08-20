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

        // GET: Vehicles/Create
        public IActionResult Park()
        {
            return View();
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
                return RedirectToAction(nameof(Index));
            }
            return View(model);
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
                return RedirectToAction(nameof(Index));
            }
            return View(vehicle);
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
    }
}
