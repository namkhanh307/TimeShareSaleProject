using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Claims;
using TimeShareProject.Controllers;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TimeShareProject.Models
{
    public class Common
    {

        private readonly _4restContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public Common(_4restContext context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        public static List<Property> GetProperties(Project project)
        {
            using _4restContext context = new _4restContext();
            List<Property> list = context.Properties.Where(p => p.ProjectId == project.Id).OrderBy(p => p.Name).ToList();
            return list;
        }

        public static List<Block> GetBlocks()
        {
            using _4restContext context = new _4restContext();
            List<Block> blocks = context.Blocks.ToList();
            return blocks;
        }

        public static string GetProjectShortNameFromProperty(Property property)
        {
            using (var context = new _4restContext())
            {
                var project = context.Projects.FirstOrDefault(p => p.Id == property.ProjectId);
                return project != null ? project.ShortName : "Unknown";
            }
        }

        public static List<int?> GetDistinctBedTypes()
        {
            using _4restContext context = new();
            var distinctBedTypes = context.Properties.Select(p => p.Beds).Distinct().ToList();
            return distinctBedTypes;
        }

        public static List<int> GetDistinctReservation()
        {
            using _4restContext context = new();
            var distinctReservation = context.Reservations.Select(r => r.Id).Distinct().ToList();
            return distinctReservation;
        }

        public static float Calculate(float unitprice)
        {
            return unitprice * 10;
        }

        public static int CountReservations(int propertyId, int blockId)
        {
            using _4restContext context = new();
            int reservationCount = context.Reservations.Count(r => r.PropertyId == propertyId && r.BlockId == blockId);
            return reservationCount;
        }
        public static DateTime GetSaleDate(int id)
        {
            using _4restContext context = new();
            var property = context.Properties.FirstOrDefault(p => p.Id == id);
            return (DateTime)property.SaleDate;
        }
        public static Reservation GetPropertyFromReservation(int id)
        {
            using _4restContext context = new();
            var reservation = context.Reservations.Include(p => p.Block).Include(p => p.User).Include(p => p.Property).FirstOrDefault(p => p.Id == id);
            return reservation;
        }
        public static List<Block> GetAvailableBlocks(int userId, int propertyId)
        {
            using (_4restContext context = new _4restContext())
            {
                List<Block> availableBlocks = new List<Block>();

                var reservations = context.Reservations.Where(r => r.PropertyId == propertyId).ToList();
                var blocks = context.Blocks.ToList();
                availableBlocks.AddRange(blocks);
                if (reservations.Count > 0)
                {
                    foreach (var block in blocks)
                    {
                        foreach (var reservation in reservations)
                        {
                            int blockId = reservation.BlockId;
                            int? type = reservation.Type;

                            if ((type == 1 && reservation.UserId == userId && block.Id == blockId) || (block.Id == blockId && type == 2))
                            {
                                availableBlocks.Remove(block);
                                break;
                            }
                        }
                    }
                }
                else
                {
                    availableBlocks.AddRange(blocks);
                }

                return availableBlocks;
            }
        }


        public static List<Property> GetReservedProperties()
        {
            using _4restContext context = new _4restContext();
            List<Property> reservedProperties = new List<Property>();
            reservedProperties = context.Reservations.Select(r => r.Property).Distinct().ToList();
            return reservedProperties;
        }

        public static List<Block> GetReservedBlocks()
        {
            using _4restContext context = new();
            List<Block> reservedBlocks = new();
            reservedBlocks = context.Reservations.Select(r => r.Block).Distinct().ToList();
            return reservedBlocks;
        }

        public static List<Project> GetProjects()
        {
            using _4restContext context = new();
            var projects = new List<Project>();
            projects = context.Projects.ToList();
            return projects;
        }

        public static string? GetPropertyName(int? Id)
        {
            using _4restContext context = new();
            var propertyName = context.Properties.FirstOrDefault(p => p.Id == Id).Name;
            return propertyName;
        }

        public static string? GetTransactionCode(int? id)
        {
            if (id == null)
            {
                return null;
            }

            using (var context = new _4restContext())
            {
                var transaction = context.Transactions
                    .Include(t => t.Reservation)
                        .ThenInclude(r => r.Block)
                    .Include(t => t.Reservation)
                        .ThenInclude(r => r.Property)
                    .Include(t => t.Reservation)
                        .ThenInclude(r => r.User)
                            .ThenInclude(u => u.Account)
                    .FirstOrDefault(t => t.Id == id);

                if (transaction == null || transaction.Reservation == null)
                {
                    return null;
                }

                var user = transaction.Reservation.User;
                var userName = user?.Account?.Username ?? "UnknownUser";
                var propertyName = transaction.Reservation.Property?.Name ?? "UnknownProperty";
                var blockId = transaction.Reservation.Block?.Id ?? 0;

                int propertyID = (int)transaction.Reservation.PropertyId;

                var type = "";
                switch (transaction.Type)
                {
                    case -1:
                        type = "Reserve";
                        break;
                    case 0:
                        type = "Deposit";
                        CreateTermPayments(user.Id,transaction.Reservation.Id, propertyID);
                        break;
                    case 1:
                        type = "FirstPayment";
                        break;
                    case 2:
                        type = "SecondPayment";
                        break;
                    case 3:
                        type = "ThirdPayment";
                        break;
                    default:
                        type = "Unknown";
                        break;
                }


                return $"{userName}_{propertyName}_{blockId}_{type}";
            }
        }
        public static int? GetReservationId(int transactionId)
        {
            using _4restContext context = new();
            var transaction = context.Transactions
                                       .FirstOrDefault(t => t.Id == transactionId);

            return transaction?.ReservationId; 
        }
        public static bool? GetTransactionStatus(int transactionId)
        {
            using _4restContext context = new();
            var transaction = context.Transactions
                                       .FirstOrDefault(t => t.Id == transactionId);

            return transaction?.Status;
        }
        public static DateTime? GetTransactionDeadline(int transactionId)
        {
            using _4restContext context = new();
            var transaction = context.Transactions
                                       .FirstOrDefault(t => t.Id == transactionId);

            return transaction?.ResolveDate;
        }
        public static bool CheckNotAvailable(bool? status, int? type, DateTime? deadlineDate, DateTime? resolveDate)
        {
            using _4restContext context = new();
            DateTime today = DateTime.Today;
            if (status == false && (type == -1 || type == 0 || type == 1 || type == 2 || type == 3) && deadlineDate == today && resolveDate <= today)
            {
                return true;
            }
            return false;
        }
        public static void CreateTermPayments(int userID, int reservationID, int propertyID)
        {
            using _4restContext _context = new _4restContext();
            var propertyId = propertyID; 
            var reservationId = reservationID;
            var reservation = _context.Reservations.FirstOrDefault(r => r.Id == reservationID);
            var property = _context.Properties.FirstOrDefault(p => p.Id == propertyId);

            DateTime DeadlineDate1 = Common.GetSaleDate(propertyId).AddDays(7);
            DateTime DeadlineDate2 = Common.GetSaleDate(propertyId).AddDays(365);
            DateTime DeadlineDate3 = Common.GetSaleDate(propertyId).AddDays(730);

            try
            {
                using (var dbTransaction = _context.Database.BeginTransaction())
                {
                    var firstTermTransaction = new Transaction()
                    {
                        Date = DateTime.Now,
                        Amount = property.UnitPrice * 3,
                        Status = false,
                        TransactionCode = null,
                        ReservationId = reservationId,
                        Type = 1,
                        DeadlineDate = DeadlineDate1,
                        ResolveDate = DeadlineDate1
                    };

                    _context.Transactions.Add(firstTermTransaction);

                    var secondTermTransaction = new Transaction()
                    {
                        Date = DateTime.Now,
                        Amount = property.UnitPrice * 3,
                        Status = false,
                        TransactionCode = null,
                        ReservationId = reservationId,
                        Type = 2,
                        DeadlineDate = DeadlineDate2,
                        ResolveDate = DeadlineDate1
                    };

                    _context.Transactions.Add(secondTermTransaction);

                    var thirdTermTransaction = new Transaction()
                    {
                        Date = DateTime.Now,
                        Amount = property.UnitPrice * 3,
                        Status = false,
                        TransactionCode = null,
                        ReservationId = reservationId,
                        Type = 3,
                        DeadlineDate = DeadlineDate3,
                        ResolveDate = DeadlineDate1
                    };

                    _context.Transactions.Add(thirdTermTransaction);
                    _context.SaveChanges();
                    dbTransaction.Commit();
                    NewsController.CreateNewForAll(userID, firstTermTransaction.Id, DeadlineDate1, 3);
                    NewsController.CreateNewForAll(userID, secondTermTransaction.Id, DeadlineDate2, 4);
                    NewsController.CreateNewForAll(userID, thirdTermTransaction.Id, DeadlineDate3, 5);
                    
                }
            }
            catch (Exception ex)
            {
                
                throw; 
            }
        }
        public static bool CheckReservation(System.Security.Claims.ClaimsPrincipal user, int propertyId)
        {
            using _4restContext context = new();
            string username = user.Identity.Name;


            var reservation = context.Users
                                 .Include(u => u.Reservations)
                                 .ThenInclude(r => r.Transactions)
                                 .FirstOrDefault(u => u.Account.Username == username);
            if (reservation != null)
            {
                int userId = reservation.Id;
                var userReservations = context.Reservations.Include(r => r.Property)
                    .Where(r => r.UserId == userId && r.PropertyId == propertyId)
                    .ToList();
                if (userReservations.Any())
                {
                    return true;
                }
            }
            return false;
        }
        public static void AddProjectTotalUnit(int id)
        {
            using (_4restContext context = new _4restContext())
            {
                var existingProject = context.Projects.FindAsync(id).Result;

                if (existingProject != null)
                {
                    existingProject.TotalUnit++;
                    context.Projects.Update(existingProject);
                    context.SaveChangesAsync();
                }
            }
        }
        public static int? GetTypeOfTransaction(int transactionId)
        {
            using _4restContext context = new();
            var transaction = context.Transactions
                                      .FirstOrDefault(t => t.Id == transactionId);

            return transaction?.Type;
        }
        public static int GetTransactionByReservation(int ReservationID)
        {
            using _4restContext context = new();
            var transaction = context.Transactions
                                      .FirstOrDefault(t => t.Reservation.Id == ReservationID);

            return transaction.Id;
        }
        public static bool CheckDeposit(int id)
        {
            using _4restContext context = new();
            var transaction = context.Transactions
             .FirstOrDefault(t => t.ReservationId == id && t.Type == 0 && t.Status == true);
            if (transaction != null)
            {
                return false;
            }
            return true;
        }
    }
}
