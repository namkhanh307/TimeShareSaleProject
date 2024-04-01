
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Query;
using TimeShareProject.Models;

namespace TimeShareProject.Controllers
{
    public class NewsController : Controller
    {
        private readonly _4restContext _context;


        public NewsController(_4restContext context)
        {
            _context = context;

        }
        [HttpPost]
        public async Task<IActionResult> CompleteTransaction(int reservationId)
        {
           
            var transactions = _context.Transactions.Where(t => t.ReservationId == reservationId);

            if (transactions.Any())
            {
                foreach (var transaction in transactions)
                {
                    
                    transaction.Status = true;
                }

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Transaction completed successfully." });
            }

            return Json(new { success = false, message = "No transactions found for the given reservation ID." });
        }


        public async Task<IActionResult> NotificationPopup()
        {
            string username = User.Identity.Name;

            var user = _context.Users
                                 .FirstOrDefault(u => u.Account.Username == username);


            if (user != null)
            {

                var newsItems = await _context.News
                                              .Where(n => n.UserId == user.Id)
                                              .ToListAsync();

                ViewBag.Context = _context;
                return PartialView("_NotificationPopup", newsItems);
            }
            else
            {
                return PartialView("_NotificationPopup", new List<New>());
            }
        }
        [HttpPost]
        public IActionResult DeleteNews(int newsId)
        {
            try
            {
                var news = _context.News.Find(newsId);
                if (news != null)
                {
                    _context.News.Remove(news);
                    _context.SaveChanges();
                    return Ok();
                }
                else
                {
                    return NotFound();
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred while deleting the news item: {ex.Message}");
            }
        }
        public static void CreateNewForAll(int userId, int transactionID,int type)
        {
            using (_4restContext _context = new _4restContext())
            {
                string title = string.Empty;
                string content = string.Empty;
                var transaction = _context.Transactions.FirstOrDefault(t => t.Id == transactionID);
                switch (type)
                {
                    case 1:
                        title = "Deadline for reservation payment";
                        content = "Please complete your transaction before 12pm";
                        break;
                    case 2:
                        title = "Deadline for deposit payment";
                        content = "Please complete your transaction before 12pm ";
                        break;
                    case 3:
                        title = "Deadline for first term payment";
                        content = "Please complete your transaction before 12pm ";
                        break;
                    case 4:
                        title = "Deadline for second term payment";
                        content = "Please complete your transaction before 12pm ";
                        break;
                    case 5:
                        title = "Deadline for third term payment";
                        content = "Please complete your transaction before 12pm ";
                        break;
                    case 6:
                        title = "Your reservation had been recorded";
                        content = "To finish your reservation, please visit us on " + Common.GetSaleDateofPropertyDByPropertyID(transaction.Reservation.PropertyId);
                        break;
                    case 7:
                        title = "Today is Opening Day. ";
                        content = "To complete registration, we recommend paying the deposit by " + Common.GetSaleDateofPropertyDByPropertyID(transaction.Reservation.PropertyId);
                        break;
                    case 8:
                        title = "Your reservation had been CANCELLED";
                        content = "Your reservation fee payment is overdue!!!!!";
                        break;
                    case 9:
                        title = "Your reservation had been CANCELLED";
                        content = "Your deposit payment is overdue!!!!! ";
                        break;
                    case 10:
                        title = "Your reservation had been CANCELLED";
                        content = "Your first term payment is overdue!!!! ";
                        break;
                    case 11:
                        title = "Your reservation had been CANCELLED";
                        content = "Your second term payment is overdue!!!! ";
                        break;
                    case 12:
                        title = "Your reservation had been CANCELLED";
                        content = "Your third term payment is overdue!!!! ";
                        break;
                    case 13:
                        title = "Your reservation had been CANCELLED";
                        content = "Your reservation fee payment has been cancelled";
                        break;
                    case 14:
                        title = "Your reservation had been CANCELLED";
                        content = "Your deposit fee payment has been cancelled";
                        break;
                    case 15:
                        title = "Your reservation had been CANCELLED";
                        content = "Your  first termn fee payment has been cancelled";
                        break;
                    case 16:
                        title = "Your reservation had been CANCELLED";
                        content = "Your second term fee payment has been cancelled";
                        break;
                    case 17:
                        title = "Your reservation had been CANCELLED";
                        content = "Your third term fee payment has been cancelled";
                        break;

                
                    default:
                        // Handle any other cases or throw an exception
                        break;
                }

                var news = new New
                {
                    UserId = userId,
                    TransactionId = transactionID,
                    Title = title,
                    Content = content,
                    Type = type,
                };

                _context.News.Add(news);
                _context.SaveChanges();
            }
        }
        public static void CreateFinishNews(int userId)
        {
            using _4restContext _context = new _4restContext();
            var news = new New
            {
                UserId = userId,
                TransactionId = 0,
                Date = DateTime.Today,
                Title = "Your reservation had been Paied",
            Content = "Enjoy your time. ",
            Type = 13
            };

            _context.News.Add(news);
            _context.SaveChanges();
        }
      
    }
}


