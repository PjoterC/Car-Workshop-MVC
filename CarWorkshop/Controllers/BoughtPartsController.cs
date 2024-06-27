using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CarWorkshop.Data;
using CarWorkshop.Models;
using Microsoft.AspNetCore.Identity;
using CarWorkshop.Areas.Identity.Data;
using System.Net.Sockets;

namespace CarWorkshop.Controllers
{
    public class BoughtPartsController : Controller
    {
        private readonly AuthDbContext _context;
        private readonly UserManager<User> _userManager;

        int ticketId;

        public BoughtPartsController(AuthDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: BoughtParts
        public async Task<IActionResult> Index(int Id) // id 0 means no ticket is selected
        {
            ticketId = Id;
            Console.WriteLine("Ticket ID: " + Id);
            List<BoughtPart> allParts = await _context.BoughtPart.ToListAsync();
            
            List<BoughtPart> ticketparts = allParts.Where(p => p.TicketId == Id).ToList();

            if(Id == 0)
            {
                return View(allParts);
            }
            else
            {
                return View(ticketparts);
            }   
        }

        // GET: BoughtParts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boughtPart = await _context.BoughtPart
                .FirstOrDefaultAsync(m => m.Id == id);
            if (boughtPart == null)
            {
                return NotFound();
            }

            return View(boughtPart);
        }

        // GET: BoughtParts/Create
        public IActionResult Create(int ticketID)
        {
            var ticket = _context.Ticket.Find(ticketID);
            if (ticket == null)
            {
                return NotFound();
            }
            var user = _userManager.FindByNameAsync(User.Identity.Name).Result;

            if(user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            if (user.Id != ticket.EmployeeID)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.TicketId = ticketID;
            var boughtPart = new BoughtPart { TicketId = ticketID };
            return View(boughtPart);
        }

        // POST: BoughtParts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TicketId,Name,Amount,Price")] BoughtPart boughtPart)
        {
            
            if (ModelState.IsValid)
            {
                _context.Add(boughtPart);
                await _context.SaveChangesAsync();
                return RedirectToAction("Edit", "Tickets", new { id = boughtPart.TicketId });
            }
            return View(boughtPart);
        }

        // GET: BoughtParts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boughtPart = await _context.BoughtPart.FindAsync(id);
            if (boughtPart == null)
            {
                return NotFound();
            }
            return View(boughtPart);
        }

        // POST: BoughtParts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TicketId,Name,Amount,Price")] BoughtPart boughtPart)
        {
            if (id != boughtPart.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(boughtPart);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BoughtPartExists(boughtPart.Id))
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
            return View(boughtPart);
        }

        // GET: BoughtParts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var boughtPart = await _context.BoughtPart
                .FirstOrDefaultAsync(m => m.Id == id);
            if (boughtPart == null)
            {
                return NotFound();
            }

            return View(boughtPart);
        }

        // POST: BoughtParts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var boughtPart = await _context.BoughtPart.FindAsync(id);
            if (boughtPart != null)
            {
                _context.BoughtPart.Remove(boughtPart);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BoughtPartExists(int id)
        {
            return _context.BoughtPart.Any(e => e.Id == id);
        }
    }
}
