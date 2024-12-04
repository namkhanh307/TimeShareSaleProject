using Microsoft.EntityFrameworkCore;
using TimeShareProject.Controllers;

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

        public static void CreateBlock()
        {
            using _4restContext context = new _4restContext();
            Random random = new Random();
            int id = 1;
            for (int i = 1; i <= 12; i++)//thang dau tien
            {
                int? swap = 1;

                for (int j = i; j <= 4; j++)//
                {
                    Block block = new Block()
                    {
                        Id = id,
                        StartDay = swap,
                        StartMonth = i,
                        EndDay = j * 7,
                        EndMonth = i,
                        BlockNumber = i * j,
                        Proportion = random.NextInt64(70, 90)
                    };
                    id++;
                    swap = block.EndDay;
                    context.Blocks.Add(block);
                }
            }
            context.SaveChanges();
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

        public static double Calculate(double? unitprice, int num, int blockId)
        {
            using _4restContext context = new();
            var block = context.Blocks
           .FirstOrDefault(b => b.Id == blockId);
            return (double)(unitprice * num * block.Proportion / 100);
        }

        public static int CountReservations(int propertyId, int blockId)
        {
            using _4restContext context = new();
            int reservationCount = context.Reservations.Count(r => r.PropertyId == propertyId && r.BlockId == blockId && r.Status == 3);
            return reservationCount;
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

                // Get all reservations for the specified property
                var reservations = context.Reservations
                    .Include(r => r.Block)
                    .Include(r => r.Transactions)
                    .Where(r => r.PropertyId == propertyId && r.Status == 3)
                    .ToList();

                // Get all blocks for the property
                var blocks = context.Blocks.ToList();
                var checkProperty = context.Properties.FirstOrDefault(p => p.Id == propertyId && p.SaleDate > DateTime.Today);
                if (checkProperty != null)
                {
                    return blocks;
                }
                foreach (var block in blocks)
                {
                    // Check if the block is available
                    bool isAvailable = true;

                    foreach (var reservation in reservations)
                    {
                        // Check if the block is reserved by the current user
                        if (reservation.UserId == userId && reservation.BlockId == block.Id)
                        {
                            isAvailable = false;
                            break;
                        }

                        // Check if the block is booked or paid during the reserve flow
                        if (reservation.BlockId == block.Id && reservation.Type == 1 && reservation.Transactions != null)
                        {
                            foreach (var transaction in reservation.Transactions)
                            {
                                if (transaction.Type == 0 && transaction.Status == true)
                                {
                                    isAvailable = false;
                                    break;
                                }
                                if (transaction.Type == -1 && transaction.Status == true && transaction.DeadlineDate == DateTime.Today)
                                {
                                    isAvailable = false;
                                    break;
                                }
                            }
                        }

                        // Check if the block is booked by another user
                        if (reservation.BlockId == block.Id && reservation.Type == 2)
                        {
                            isAvailable = false;
                            break;
                        }
                    }

                    if (isAvailable)
                    {
                        availableBlocks.Add(block);
                    }
                }

                return availableBlocks;
            }
        }
        //public static List<Block> GetAvailableBlocks(int userId, int propertyId)
        //{
        //    using (_4restContext context = new _4restContext())
        //    {
        //        List<Block> availableBlocks = new List<Block>();

        //        var reservations = context.Reservations.Where(r => r.PropertyId == propertyId).ToList();
        //        var blocks = context.Blocks.ToList();
        //        availableBlocks.AddRange(blocks);
        //        if (reservations.Count > 0)
        //        {
        //            //foreach (var block in blocks)
        //           // {
        //            foreach (var reservation in reservations)
        //            {
        //                if (reservation.Status == 3)
        //                {
        //                    var block = new Block();
        //                    int blockId = reservation.BlockId;
        //                    int? type = reservation.Type;
        //                    var transaction = context.Transactions.FirstOrDefault(t => t.ReservationId == reservation.Id);
        //                    if (transaction != null && transaction.Type == 0)
        //                    {
        //                        if ((type == 1 && reservation.UserId == userId && block.Id == blockId)//get the block of current user 
        //                     || (block.Id == blockId && type == 2)// get the block which has been booked 
        //                     || (type == 1 && transaction.Status == true)//get the block that have been pay during the reserve flow
        //                        )
        //                        {
        //                            availableBlocks.Remove(block);
        //                            break;
        //                        }
        //                    } else if (   transaction != null && transaction.Type == -1 ) { 

        //                    }

        //                }
        //            }
        //            //}
        //        }
        //        else
        //        {
        //            availableBlocks.AddRange(blocks);
        //        }

        //        return availableBlocks;
        //    }
        //}


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
                        CreateFirstTermPayment(user.Id, transaction.Reservation.Id, propertyID);
                        break;
                    case 1:
                        type = "FirstPayment";
                        CreateSecondTermPayment(user.Id, transaction.Reservation.Id, propertyID);
                        break;
                    case 2:
                        type = "SecondPayment";
                        CreateThirdTermPayment(user.Id, transaction.Reservation.Id, propertyID);
                        break;
                    case 3:
                        type = "ThirdPayment";
                        UpdateFinishStatus(transaction.Reservation.Id);
                        break;
                    default:
                        type = "Unknown";
                        break;
                }


                return $"{userName}_{propertyName}_{blockId}_{type}";
            }
        }
        public static int GetReservationId(int transactionId)
        {
            using _4restContext context = new();
            var transaction = context.Transactions
                                       .FirstOrDefault(t => t.Id == transactionId);

            return transaction.ReservationId;
        }
        public static bool? GetTransactionStatus(int transactionId)
        {
            using _4restContext context = new();
            var transaction = context.Transactions
                                       .FirstOrDefault(t => t.Id == transactionId);

            return transaction?.Status;
        }
        public static int? GetReservationStatus(int reservationId)
        {
            using _4restContext context = new();
            var reservation = context.Reservations
                                       .FirstOrDefault(t => t.Id == reservationId);

            return reservation.Status;
        }
        public static DateTime? GetTransactionDeadline(int transactionId)
        {
            using _4restContext context = new();
            var transaction = context.Transactions
                                       .FirstOrDefault(t => t.Id == transactionId);

            return transaction?.DeadlineDate;
        }
        public static int? GetReservationStatusByTransactionID(int transactionId)
        {
            using (_4restContext context = new _4restContext())
            {
                var transaction = context.Transactions
                    .Include(t => t.Reservation) // Include the Reservation navigation property
                    .FirstOrDefault(t => t.Id == transactionId);

                // Check if transaction or reservation is null
                if (transaction == null || transaction.Reservation == null)
                {
                    return null;
                }

                // Return the status of the associated reservation
                return transaction.Reservation.Status;
            }
        }

        public static bool CheckNotAvailable(bool? status, int? type, DateTime? deadlineDate)
        {
            using _4restContext context = new();
            DateTime today = DateTime.Today;
            if (status == false && (type == -1 || type == 0 || type == 1 || type == 2 || type == 3) && deadlineDate == today)
            {
                return true;
            }
            return false;
        }
        public static void CreateFirstTermPayment(int userID, int reservationID, int propertyID)
        {
            using _4restContext _context = new _4restContext();
            var propertyId = propertyID;
            var reservationId = reservationID;
            var reservation = _context.Reservations.FirstOrDefault(r => r.Id == reservationID);
            var property = _context.Properties.FirstOrDefault(p => p.Id == propertyId);

            try
            {
                using (var dbTransaction = _context.Database.BeginTransaction())
                {
                    var firstTermTransaction = new Transaction()
                    {
                        Date = DateTime.Now,
                        Amount = Common.Calculate(property.UnitPrice, 3, reservation.BlockId),
                        Status = false,
                        TransactionCode = null,
                        ReservationId = reservationId,
                        Type = 1,
                        DeadlineDate = DateTime.Today.AddDays(7)

                    };

                    _context.Transactions.Add(firstTermTransaction);
                    _context.SaveChanges();
                    dbTransaction.Commit();
                    NewsController.CreateNewForAll(userID, firstTermTransaction.Id, 3, DateTime.Today.AddDays(7));


                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public static void CreateSecondTermPayment(int userID, int reservationID, int propertyID)
        {
            using _4restContext _context = new _4restContext();
            var propertyId = propertyID;
            var reservationId = reservationID;
            var reservation = _context.Reservations.FirstOrDefault(r => r.Id == reservationID);
            var property = _context.Properties.FirstOrDefault(p => p.Id == propertyId);



            try
            {
                using (var dbTransaction = _context.Database.BeginTransaction())
                {


                    var secondTermTransaction = new Transaction()
                    {
                        Date = DateTime.Now,
                        Amount = Common.Calculate(property.UnitPrice, 3, reservation.BlockId),
                        Status = false,
                        TransactionCode = null,
                        ReservationId = reservationId,
                        Type = 2,
                        DeadlineDate = DateTime.Today.AddDays(365)
                    };

                    _context.Transactions.Add(secondTermTransaction);
                    _context.SaveChanges();
                    dbTransaction.Commit();

                    NewsController.CreateNewForAll(userID, secondTermTransaction.Id, 4, DateTime.Today.AddDays(365));


                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        public static void CreateThirdTermPayment(int userID, int reservationID, int propertyID)
        {
            using _4restContext _context = new _4restContext();
            var propertyId = propertyID;
            var reservationId = reservationID;
            var reservation = _context.Reservations.FirstOrDefault(r => r.Id == reservationID);
            var property = _context.Properties.FirstOrDefault(p => p.Id == propertyId);



            try
            {
                using (var dbTransaction = _context.Database.BeginTransaction())
                {


                    var thirdTermTransaction = new Transaction()
                    {
                        Date = DateTime.Now,
                        Amount = Common.Calculate(property.UnitPrice, 3, reservation.BlockId),
                        Status = false,
                        TransactionCode = null,
                        ReservationId = reservationId,
                        Type = 3,
                        DeadlineDate = DateTime.Today.AddDays(750)

                    };

                    _context.Transactions.Add(thirdTermTransaction);
                    _context.SaveChanges();
                    dbTransaction.Commit();
                    NewsController.CreateNewForAll(userID, thirdTermTransaction.Id, 5, DateTime.Today.AddDays(730));

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
        public static void MinusProjectTotalUnit(int id)
        {
            using (_4restContext context = new _4restContext())
            {
                var existingProject = context.Projects.FindAsync(id).Result;

                if (existingProject != null)
                {
                    existingProject.TotalUnit--;
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
        public static int GetReservTransactionIDByResevationID(int reservationID)
        {
            using _4restContext context = new();
            var transaction = context.Transactions
             .FirstOrDefault(t => t.ReservationId == reservationID && t.Type == -1);
            return transaction.Id;
        }
        public static int GetDepositIDByResevationID(int reservationID)
        {
            using _4restContext context = new();
            var transaction = context.Transactions
             .FirstOrDefault(t => t.ReservationId == reservationID && t.Type == 0);
            return transaction.Id;
        }
        public static bool GetDepositStatusByResevationID(int reservationID)
        {
            using _4restContext context = new();
            var transaction = context.Transactions
             .FirstOrDefault(t => t.ReservationId == reservationID && t.Type == 0);
            return transaction.Status.Value;
        }
        public static string GetPropertyNameByTransactionID(int transactionID)
        {
            using _4restContext context = new();
            var transaction = context.Transactions.FirstOrDefault(t => t.Id == transactionID);

            var property = context.Properties
             .FirstOrDefault(p => p.Id == transaction.Reservation.PropertyId);
            return property.Name;
        }
        public static DateTime GetSaleDateofPropertyDByPropertyID(int propertyID)
        {
            using _4restContext context = new();
            var property = context.Properties
             .FirstOrDefault(p => p.Id == propertyID);
            return property.SaleDate;
        }
        public static void UpdateOrderOnOpeningDay(int propertyId)
        {
            using (var context = new _4restContext())
            {
                var reservations = context.Reservations
                    .Include(r => r.Transactions)
                    .Where(r => r.PropertyId == propertyId)
                    .OrderBy(r => r.RegisterDate)
                    .ToList();

                int order = 0;
                foreach (var reservation in reservations)
                {
                    // Check if it's the opening day
                    if (reservation.Property.SaleDate == DateTime.Today)
                    {
                        // Check if the reservation fee has been paid
                        var reservationFeePaid = reservation.Transactions
                            .Any(t => t.Type == 0 && t.Status == true); // Assuming Type 0 is for reservation fee

                        // Update the order based on whether the fee has been paid
                        if (!reservationFeePaid)
                        {
                            reservation.Order = 0; // If fee not paid, set order to 0
                        }
                        else
                        {
                            reservation.Order = order++; // Increment order for each paid reservation
                        }
                    }
                }

                // Save changes to the database
                context.SaveChanges();
            }
        }
        public static void UpdateFinishStatus(int reservationID)
        {
            using _4restContext context = new();
            var reservation = context.Reservations
            .FirstOrDefault(r => r.Id == reservationID);
            if (reservation != null)
            {
                reservation.Status = 4; // Set status to 4 (finished)
                context.SaveChanges();
            }
            else
            {

                throw new ArgumentException($"Reservation with ID {reservationID} not found.");
            }
        }
        public static string GetPercentage(double proportion)
        {
            if (proportion > 100)
            {
                return $"(already + {proportion - 100}%)";
            }
            else if (proportion < 100)
            {
                return $"(already - {100 - proportion}%)";
            }
            return "";
        }

    }
}
