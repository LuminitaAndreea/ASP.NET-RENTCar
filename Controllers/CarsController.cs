using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RentCWeb.Models;
using System.Web.Http.Cors;
using System.Data;
using Newtonsoft.Json;

namespace RentCWeb.Controllers
{
    public class CarsController : Controller
    {
        private readonly MyDbContext db;
        public string conString = @"Server=DESKTOP-VDTQMNM;Database=RentCWeb;Trusted_Connection=True;ConnectRetryCount=0";

        public CarsController(MyDbContext context)
        {
            db = context;
        }
        //[Authorize(Roles = "manager,admin,salesperson")]
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public object AvailableCars(DateTime startdate, DateTime enddate, string location, IList<Car> cars)
        //{
        //    using (SqlConnection con = new SqlConnection(conString))
        //    {

        //        using (SqlCommand cmd = new SqlCommand("SELECT DISTINCT Cars.Plate, Cars.Manufacturer, Cars.Model, Cars.PricePerDay, Cars.Location" +
        //            " FROM Cars JOIN Reservations ON (Cars.CarID = Reservations.CarID AND Reservations.Location ='" + location + "' AND " +
        //            "('" + startdate + "'> Reservations.EndDate OR '" + enddate + "'< Reservations.StartDate)OR(Cars.CarID!=Reservations.CarID AND " +
        //            "Cars.Location='" + location + "')) ", con))
        //        {
        //            con.Open();
        //            SqlDataReader reader = cmd.ExecuteReader();
        //            cars.Add(reader);
        //        }
        //        return RedirectToAction(nameof(AvailableCarsList));
        //    }

        //}
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "manager,admin,salesperson")]
        public IActionResult AvailableCars(SearchEL search)
        {
            List<Car> cars = new List<Car>();
            
            if (search.startdate != null && search.enddate != null && search.location != null)
                {
                    using (SqlConnection con = new SqlConnection(conString))
                    {
                        using (SqlCommand cmd = new SqlCommand("Select * from Cars WHERE Location = '" + search.location + "' AND CarID NOT IN" +
                                "(Select CarID FROM Reservations WHERE NOT (StartDate < '" + search.enddate + "') OR (EndDate > '" + search.startdate + "'))", con))
                        {
                            con.Open();
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                            if (reader.HasRows)
                                {
                                    while (reader.Read()) {
                                        Car car = new Car();
                                        car.CarID =(int) reader["CarID"];
                                        car.Plate = reader["Plate"] as string;
                                        car.Manufacturer =reader["Manufacturer"] as string;
                                        car.Model =reader["Model"] as string;
                                        car.PricePerDay = reader["PricePerDay"] as string;
                                        car.Location = reader["Location"]as string;
                                        cars.Add(car);
                                    }
                                }
                            }
                        }
                    }
                return RedirectToAction("AvailableCarsList", new { serializedModel = JsonConvert.SerializeObject(cars.ToList()) });
                }
            
            return View("AvailableCarsList");
        }
        
        public IActionResult AvailableCars()
        {
            return View();
        }

        [Authorize(Roles = "manager,admin,salesperson")]
        public IActionResult AvailableCarsList(string serializedModel)
        {
            List<Car> model = JsonConvert.DeserializeObject<List<Car>>(serializedModel);
            return View(model);
        }

        // GET: Cars
        [Authorize(Roles = "manager,admin,salesperson")]
        public async Task<IActionResult> Index()
        {
            return View(await db.Cars.ToListAsync());
        }

        // GET: Cars/Details/5
        [Authorize(Roles = "manager,admin,salesperson")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await db.Cars
                .FirstOrDefaultAsync(m => m.CarID == id);
            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }
        [Authorize(Roles = "manager,admin")]
        // GET: Cars/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Cars/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "manager,admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CarID,Plate,Manufacturer,Model,PricePerDay,Location")] Car car)
        {
            if (ModelState.IsValid)
            {
                db.Add(car);
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(car);
        }

        // GET: Cars/Edit/5
        [Authorize(Roles = "manager,admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await db.Cars.FindAsync(id);
            if (car == null)
            {
                return NotFound();
            }
            return View(car);
        }

        // POST: Cars/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "manager,admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CarID,Plate,Manufacturer,Model,PricePerDay,Location")] Car car)
        {
            if (id != car.CarID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    db.Update(car);
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarExists(car.CarID))
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
            return View(car);
        }

        // GET: Cars/Delete/5
        [Authorize(Roles = "manager,admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await db.Cars
                .FirstOrDefaultAsync(m => m.CarID == id);
            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }

        // POST: Cars/Delete/5
        [Authorize(Roles = "manager,admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var car = await db.Cars.FindAsync(id);
            db.Cars.Remove(car);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CarExists(int id)
        {
            return db.Cars.Any(e => e.CarID == id);
        }
    }
}
