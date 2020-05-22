using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using RentCWeb.Models;

namespace RentCWeb.Controllers
{
    public class ReservationsController : Controller
    {
        private readonly MyDbContext _context;

        public ReservationsController(MyDbContext context)
        {
            _context = context;
        }

        // GET: Reservations
        [Authorize(Roles = "manager,admin,salesperson")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.Reservations.ToListAsync());
        }
        [Authorize(Roles = "manager,admin,salesperson")]
        // GET: Reservations/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations
                .FirstOrDefaultAsync(m => m.ReservationId == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // GET: Reservations/Create
        [Authorize(Roles = "manager,admin,salesperson")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Reservations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "manager,admin,salesperson")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string plate, int customerid, DateTime startdate, DateTime enddate, string location, string cuponcode, Reservation reservation)
        {
            if (_context.Cars.Where(c => c.Plate == plate && c.Location == location).FirstOrDefault() != null)
            {
                Car MyCar = _context.Cars.Where(c => c.Plate == plate && c.Location == location).FirstOrDefault();
                reservation.CarID = MyCar.CarID;
                reservation.Plate = MyCar.Plate;
                reservation.Location = MyCar.Location;
                if (_context.Customers.Where(c => c.CustomerID == customerid).FirstOrDefault() != null)
                {
                    Customer MyCustomer = _context.Customers.Where(c => c.CustomerID == customerid).FirstOrDefault();
                    reservation.CustomerID = MyCustomer.CustomerID;
                    reservation.StartDate = startdate;
                    reservation.EndDate = enddate;
                    if ((reservation.StartDate <= reservation.EndDate) && (reservation.StartDate >= DateTime.Now))
                    {
                        if (_context.Reservations.Where(c => (c.EndDate < reservation.StartDate || c.StartDate > reservation.EndDate)
                        && c.Plate == reservation.Plate).Any())
                        {
                            _context.Add(reservation);
                            await _context.SaveChangesAsync();
                            return RedirectToAction(nameof(Index));
                        }
                        else if (_context.Reservations.Where(c => c.Plate == reservation.Plate).Any() == false)
                        {
                            _context.Add(reservation);
                            await _context.SaveChangesAsync();
                            return RedirectToAction(nameof(Index));
                        }
                    }
                }

            }
            return View(reservation);
        }

        // GET: Reservations/Edit/5
        [Authorize(Roles = "manager,admin,salesperson")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }
            return View(reservation);
        }

        // POST: Reservations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "manager,admin,salesperson")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string plate, int customerid, DateTime startdate, DateTime enddate, string location, string cuponcode, Reservation reservation)
        {
            reservation.ReservationId = id;
            if (_context.Cars.Where(c => c.Plate == plate && c.Location == location).FirstOrDefault() != null)
            {
                Car MyCar = _context.Cars.Where(c => c.Plate == plate && c.Location == location).FirstOrDefault();
                reservation.CarID = MyCar.CarID;
                reservation.Plate = MyCar.Plate;
                reservation.Location = MyCar.Location;
                if (_context.Customers.Where(c => c.CustomerID == customerid).FirstOrDefault() != null)
                {
                    Customer MyCustomer = _context.Customers.Where(c => c.CustomerID == customerid).FirstOrDefault();
                    reservation.CustomerID = MyCustomer.CustomerID;
                    reservation.StartDate = startdate;
                    reservation.EndDate = enddate;
                    if ((reservation.StartDate < reservation.EndDate) && (reservation.StartDate >= DateTime.Now))
                    {
                        if ((_context.Reservations.Where(c => !((c.EndDate < reservation.StartDate) || (c.StartDate > reservation.EndDate))
                         && (c.ReservationId != reservation.ReservationId) && (c.Plate == reservation.Plate)).Any()) == false)
                        {
                            _context.Update(reservation);
                            await _context.SaveChangesAsync();
                            return RedirectToAction(nameof(Index));
                        }
                        else if (_context.Reservations.Where(c => c.Plate == reservation.Plate).Count() == 1)
                        {
                            _context.Update(reservation);
                            await _context.SaveChangesAsync();
                            return RedirectToAction(nameof(Index));
                        }
                        else
                        {
                            return NotFound();
                        }

                    }

                }

            }
            return View(reservation);
        }

        // GET: Reservations/Delete/5
        [Authorize(Roles = "manager,admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations
                .FirstOrDefaultAsync(m => m.ReservationId == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // POST: Reservations/Delete/5
        [Authorize(Roles = "manager,admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReservationExists(int id)
        {
            return _context.Reservations.Any(e => e.ReservationId == id);
        }
    }
}
