using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TimeShareProject.Models;

namespace TimeShareProject.Controllers
{
    public class TransactionsController : Controller
    {
        private readonly _4restContext _context;

        public TransactionsController(_4restContext context)
        {
            _context = context;
        }

        // GET: Transactions
        public async Task<IActionResult> Index()
        {
            var timeShareProjectContext = _context.Transactions.Include(t => t.Reservation);
            return View(await timeShareProjectContext.ToListAsync());
        }

        // GET: Transactions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions
                .Include(t => t.Reservation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // GET: Transactions/Create
        public IActionResult Create()
        {
            ViewData["ReservationId"] = new SelectList(_context.Reservations, "Id", "Id");
            return View();
        }

        // POST: Transactions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Date,Amount,Status,TransactionCode,FirstTerm,SecondTerm,ThirdTerm,ReservationId,Type")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                _context.Add(transaction);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ReservationId"] = new SelectList(_context.Reservations, "Id", "Id", transaction.ReservationId);
            return View(transaction);
        }

        // GET: Transactions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }
            ViewData["ReservationId"] = new SelectList(_context.Reservations, "Id", "Id", transaction.ReservationId);
            return View(transaction);
        }

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Date,Amount,Status,TransactionCode,FirstTerm,SecondTerm,ThirdTerm,ReservationId,Type")] Transaction transaction)
        {
            if (id != transaction.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(transaction);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TransactionExists(transaction.Id))
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
            ViewData["ReservationId"] = new SelectList(_context.Reservations, "Id", "Id", transaction.ReservationId);
            return View(transaction);
        }

        // GET: Transactions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transactions
                .Include(t => t.Reservation)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var transaction = await _context.Transactions.FindAsync(id);
            if (transaction != null)
            {
                _context.Transactions.Remove(transaction);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TransactionExists(int id)
        {
            return _context.Transactions.Any(e => e.Id == id);
        }
        public IActionResult CreateTransaction(int id)
        {
            var reservation = Common.GetPropertyFromReservation(id);
            try
            {
                using var transaction = _context.Database.BeginTransaction();
                var newDepositTransaction = new Transaction()
                {
                    Date = DateTime.Now,
                    Amount = reservation.Property.UnitPrice * reservation.Block.Proportion,
                    Status = false,
                    TransactionCode = null,
                    ReservationId = id,
                    Type = 0,
                    

                };
                _context.Transactions.Add(newDepositTransaction);

                var newFirstTermTransaction = new Transaction()
                {
                    Date = DateTime.Now,
                    Amount = reservation.Property.UnitPrice * 3 * reservation.Block.Proportion,
                    Status = false,
                    TransactionCode = null,
                    ReservationId = id,
                    Type = 1,
                   

                };
                _context.Transactions.Add(newFirstTermTransaction);

                var newSecondTermTransaction = new Transaction()
                {
                    Date = DateTime.Now,
                    Amount = reservation.Property.UnitPrice * 3 * reservation.Block.Proportion,
                    Status = false,
                    TransactionCode = null,
                    ReservationId = id,
                    Type = 2,
                   
                };
                _context.Transactions.Add(newSecondTermTransaction);


                var newThirdTermTransaction = new Transaction()
                {
                    Date = DateTime.Now,
                    Amount = reservation.Property.UnitPrice * 3 * reservation.Block.Proportion ,
                    Status = false,
                    TransactionCode = null,
                    ReservationId = id,
                    Type = 3,
                  
                };
                _context.Transactions.Add(newThirdTermTransaction);
                _context.SaveChanges();
                transaction.Commit();
                TempData["Message"] = "Reservation confirmed successfully!";

                
                
                    NewsController.CreateNewForAll(reservation.UserId, reservation.Property.Id, 1, DateTime.Today);
                
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while confirming the reservation.";
            }
            return RedirectToAction("Index", "Reservations");
        }
    }
}
