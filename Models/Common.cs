using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        public static List<Block> GetAvailableBlocks(int propertyId)
        {
            using (_4restContext context = new _4restContext())
            {
                List<Block> availableBlocks = new List<Block>();

                var reservations = context.Reservations
                    .Where(r => r.PropertyId == propertyId).ToList();
                var blocks = context.Blocks.ToList();
                if (reservations.Count > 0)
                {
                    foreach (var reservation in reservations)
                    {
                        int blockId = reservation.BlockId;
                        int? type = reservation.Type;
                        foreach (var block in blocks)
                        {
                            if ((type == 1) || (block.Id != blockId && type == 2))
                            {
                                availableBlocks.Add(block);
                            }
                        }
                    }
                }
                else
                {
                    foreach (var block in blocks)
                    {
                        availableBlocks.Add(block);
                    }
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

                var type = "";
                switch (transaction.Type)
                {
                    case -1:
                        type = "Reserve";
                        break;
                    case 0:
                        type = "Deposit";
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

    }
}
