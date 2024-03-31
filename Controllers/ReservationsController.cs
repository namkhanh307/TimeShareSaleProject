using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TimeShareProject.Models;
using TimeShareProject.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Transaction = TimeShareProject.Models.Transaction;

namespace TimeShareProject.Controllers
{


    public class ReservationsController : Controller
    {
        private readonly _4restContext _context;
        private readonly PaypalClient _paypalClient;

        public ReservationsController(_4restContext context, PaypalClient paypalClient)
        {
            _context = context;
            _paypalClient = paypalClient;
        }
        public async Task<IActionResult> CancelReservation(int id, int userID)
        {
            var reservation = await _context.Reservations.FindAsync(id);
 
           
            if (reservation != null)
            {
                reservation.Status = 2;
                _context.Reservations.Update(reservation);
           
            }
            await _context.SaveChangesAsync();
            if (reservation.Type == 1) {
<<<<<<< HEAD
                NewsController.CreateNewForAll(userID, Common.GetReservTransactionIDByResevationID(id), DateTime.Now, 13);
                NewsController.CreateNewForAll(userID, Common.GetDepositIDByResevationID(id), DateTime.Now, 14);
            }
            if (reservation.Type == 2) {
                NewsController.CreateNewForAll(userID, Common.GetDepositIDByResevationID(id), DateTime.Now, 14);
=======
                NewsController.CreateNewForAll(userID, Common.GetReservTransactionIDByResevationID(id),  13);
                NewsController.CreateNewForAll(userID, Common.GetDepositIDByResevationID(id), 14);
            }
            if (reservation.Type == 2) {
                NewsController.CreateNewForAll(userID, Common.GetDepositIDByResevationID(id), 14);
>>>>>>> e3198606b022648766b066fe24d25a0643bacd8f
            }

            return RedirectToAction("GetUserReservation", "User");
        }
        public async Task<IActionResult> UpdateReservationStatus(int id, int status)
        {
            var reservation = await _context.Reservations.FindAsync(id);

            if (reservation == null)
            {
                return NotFound();
            }

            reservation.Status = status;
            _context.Update(reservation);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        public IActionResult FilterDuplicate()
        {
            var reservations = _context.Reservations.Include(r => r.Block).Include(r => r.Property).Include(r => r.User).ToList();

            var duplicateReservations = reservations
                .Where(r => r.Type == 1)
                .GroupBy(r => new { r.PropertyId, r.BlockId })
                .Where(g => g.Count() > 1)
                .Select(g => g.First());

            return View(duplicateReservations);
        }

        public IActionResult ViewAllDuplicates(int propertyId, int blockId)
        {
            var duplicateReservations = _context.Reservations
                .Include(r => r.Block)
                .Include(r => r.Property)
                .Include(r => r.User)
                .Where(r => r.Type == 1 && r.PropertyId == propertyId && r.BlockId == blockId)
                .ToList();

            return View(duplicateReservations);
        }


        #region Paypal payment


        [Authorize]
        [HttpPost("/Reservations/create-paypal-order")]
        public async Task<IActionResult> CreatePaypalOrder([FromBody] Transaction transaction, CancellationToken cancellationToken)
        {
            double total = Math.Round((double)transaction.Amount / 23000, 2);
            var totalString = total.ToString();
            var currency = "USD";
            var transactionCode = Common.GetTransactionCode(transaction.Id);
            string username = User.Identity.Name;
            var user = _context.Users.FirstOrDefault(u => u.Account.Username == username);
            try
            {
                var response = await _paypalClient.CreateOrder(totalString, currency, transactionCode);
                if (response != null)
                {
                    var newTrasaction = await _context.Transactions.FindAsync(transaction.Id);
                    try
                    {
                        newTrasaction.Status = true;
                        newTrasaction.TransactionCode = transactionCode;
                        _context.Update(newTrasaction);
                        await _context.SaveChangesAsync();

                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "Error occurred while updating the transaction.");
                    }

                    
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                var error = new { ex.GetBaseException().Message };
                return BadRequest(error);
            }
        }

        [Authorize]
        [HttpPost("/Reservations/capture-paypal-order")]

        public async Task<IActionResult> CapturePaypalOrder(string orderID, CancellationToken cancellationToken)
        {
            try
            {
                var response = await _paypalClient.CaptureOrder(orderID);
             
                return Ok(response);

            }
            catch (Exception ex)
            {
                var error = new { ex.GetBaseException().Message };
                return BadRequest(error);
            }
        }
        #endregion


        // GET: Reservations
        [Authorize(Roles = "1,2")]
        public async Task<IActionResult> Index()
        {
            List<int> distinctReservation = Common.GetDistinctReservation();
            ViewBag.DistinctReservation = distinctReservation;
            var timeShareProjectContext = _context.Reservations.Include(r => r.Block).Include(r => r.Property).Include(r => r.User);
            return View(await timeShareProjectContext.ToListAsync());
        }
        // GET: Reservations/Details/5

        public async Task<IActionResult> TransactionDetail(int id)
        {
            var timeShareProjectContext = _context.Transactions.Include(t => t.Reservation).Where(m => m.ReservationId == id);
            ViewBag.count = timeShareProjectContext.Count();
            return View(await timeShareProjectContext.ToListAsync());
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations
                .Include(r => r.Block)
                .Include(r => r.Property)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        public IActionResult SelectRoom(int blockSelect, int propertyId, string saleStatus, int projectId, int bedSelect)
        {
            if (!User.Identity.IsAuthenticated)
            {
                var returnUrl = Url.Action("SelectRoom", "Reservations", new { blockSelect, propertyId, saleStatus, projectId, bedSelect });
                return RedirectToAction("Login", "Login", new { returnUrl });
            }

            var block = _context.Blocks.FirstOrDefault(b => b.Id == blockSelect);
            var property = _context.Properties.FirstOrDefault(p => p.Id == propertyId);

            if (block == null || property == null)
            {
                return RedirectToAction("GetProperty", "Properties", new { ID = property.Id });
            }
            ViewBag.SaleStatus = saleStatus;
            ViewBag.Block = block;
            ViewBag.Property = property;
            ViewBag.ProjectId = projectId;
            ViewBag.BedSelect = bedSelect;
            return View();
        }

        public IActionResult ConfirmReservation(int propertyId, int blockSelect, string transactionCode, string saleStatus, int order)
        {
            string username = User.Identity.Name;
            var user = _context.Users.FirstOrDefault(u => u.Account.Username == username);
            var property = _context.Properties.FirstOrDefault(p => p.Id == propertyId);
            order++;
            int reservationID = 0;
            int transactionId = 0;
            int depositId = 0;
            int reservationType = 2;
            int transactionType = 0;
            if (saleStatus == "Reserve")
            {
                reservationType = 1;
                transactionType = -1;
            }
            try
            {
                using (var transaction = _context.Database.BeginTransaction())
                {
                    var newReservation = new Reservation()
                    {
                        PropertyId = propertyId,
                        BlockId = blockSelect,
                        UserId = user.Id,
                        YearQuantity = 10,
                        RegisterDate = DateTime.Now,
                        Type = reservationType,
                        Status = 3,
                        Order = order
                    };

                    _context.Reservations.Add(newReservation);
                    _context.SaveChanges();

                    if (reservationType == 1)
                    {
                        var newReserveTransaction = new Transaction()
                        {
                            Date = DateTime.Today,
                            Amount = 500000,
                            Status = false,
                            TransactionCode = transactionCode,
                            ReservationId = newReservation.Id,
                            Type = transactionType,
                            //DeadlineDate = Common.GetSaleDate(propertyId),
                            //ResolveDate = Common.GetSaleDate(propertyId).AddDays(order - 1)
                        };
                        _context.Transactions.Add(newReserveTransaction);
                        
                        var newDepositTransaction = new Transaction()
                        {
                            Date = DateTime.Today,
                            Amount = property.UnitPrice,
                            Status = false,
                            TransactionCode = transactionCode,
                            ReservationId = newReservation.Id,
                            Type = 0,
                            //DeadlineDate = Common.GetSaleDate(propertyId).AddDays(order),
                            //ResolveDate = Common.GetSaleDate(propertyId).AddDays(order - 1)
                        };
                      
                       
                        _context.Transactions.Add(newDepositTransaction);
                       
                        _context.SaveChanges();
                        reservationID = newReserveTransaction.Id;
                        transactionId = newDepositTransaction.Id;
                    }

                    if (reservationType == 2)
                    {

                        var newDepositTransaction = new Transaction()
                        {
                            Date = DateTime.Today,
                            Amount = property.UnitPrice,
                            Status = false,
                            TransactionCode = transactionCode,
                            ReservationId = newReservation.Id,
                            Type = transactionType,
                            //DeadlineDate = DateTime.Today.AddDays(1),
                            //ResolveDate = DateTime.Today
                        }; _context.Transactions.Add(newDepositTransaction);
                        
                        
                        _context.SaveChanges();
                        depositId = newDepositTransaction.Id;
                    }
                    transaction.Commit();
                    TempData["Message"] = "Reservation confirmed successfully!";
                    if (reservationType == 1)
                    {
                        NewsController.CreateNewForAll(user.Id, reservationID, 1);
                        NewsController.CreateNewForAll(user.Id, transactionId,  2);
                    }
                    if (reservationType == 2)
                    {
                        
<<<<<<< HEAD
                        NewsController.CreateNewForAll(user.Id, depositId, DateTime.Today.AddDays(1), 2);
=======
                        NewsController.CreateNewForAll(user.Id, depositId, 2);
>>>>>>> e3198606b022648766b066fe24d25a0643bacd8f
                    }

                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while confirming the reservation.";
            }
            return RedirectToAction("Index", "Home");
        }

        // GET: Reservations/Create
        public IActionResult Create()
        {
            ViewData["BlockId"] = new SelectList(_context.Blocks, "Id", "Id");
            ViewData["PropertyId"] = new SelectList(_context.Properties, "Id", "Id");
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Reservations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,PropertyId,UserId,RegisterDate,YearQuantity,Type,BlockId")] Reservation reservation)
        {
            if (ModelState.IsValid)
            {
                _context.Add(reservation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["BlockId"] = new SelectList(_context.Blocks, "Id", "Id", reservation.BlockId);
            ViewData["PropertyId"] = new SelectList(_context.Properties, "Id", "Id", reservation.PropertyId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", reservation.UserId);
            return View(reservation);
        }

        // GET: Reservations/Edit/5
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
            ViewData["BlockId"] = new SelectList(_context.Blocks, "Id", "Id", reservation.BlockId);
            ViewData["PropertyId"] = new SelectList(_context.Properties, "Id", "Id", reservation.PropertyId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", reservation.UserId);
            return View(reservation);
        }

        // POST: Reservations/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PropertyId,UserId,RegisterDate,YearQuantity,Type,BlockId")] Reservation reservation)
        {
            if (id != reservation.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reservation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservationExists(reservation.Id))
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
            ViewData["BlockId"] = new SelectList(_context.Blocks, "Id", "Id", reservation.BlockId);
            ViewData["PropertyId"] = new SelectList(_context.Properties, "Id", "Id", reservation.PropertyId);
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", reservation.UserId);
            return View(reservation);
        }

        // GET: Reservations/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations
                .Include(r => r.Block)
                .Include(r => r.Property)
                .Include(r => r.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // POST: Reservations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            var transaction = _context.Transactions.Where(t => t.ReservationId == id).ToList();
            if (transaction != null)
            {
                foreach (var item in transaction)
                {
                    var news = item.News.Where(r => r.TransactionId == item.Id);
                    foreach (var item1 in news)
                    {
                        _context.News.Remove(item1);
                        _context.Transactions.Remove(item);
                    }

                }
            }
            if (reservation != null)
            {
                _context.Reservations.Remove(reservation);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        private bool ReservationExists(int id)
        {
            return _context.Reservations.Any(e => e.Id == id);
        }
        public PartialViewResult FilterReservation(int? propertyId, int? blockId, int? type)
        {
            using (_4restContext context = new _4restContext())
            {
                var query = context.Reservations.AsQueryable();
                query = query.Include(r => r.Block).Include(r => r.Property).Include(r => r.User);

                if (propertyId != null)
                {
                    query = query.Where(r => r.PropertyId == propertyId);
                }

                if (blockId != null)
                {
                    query = query.Where(r => r.BlockId == blockId);
                }

                if (type != null)
                {
                    query = query.Where(r => r.Type == type);
                }

                var filteredReservations = query.ToList();

                return PartialView("_FilteredReservations", filteredReservations);
            }
        }
        public PartialViewResult Search(string searchTerm)
        {
            // Filter reservations based on search term
            var filteredReservations = _context.Reservations.Include(r => r.Block).Include(r => r.User).Include(r => r.Property)
                .Where(r => r.Property.Name.Contains(searchTerm) || r.User.Name.Contains(searchTerm))
                .ToList();

            return PartialView("_FilteredReservations", filteredReservations);
        }
    }
}
