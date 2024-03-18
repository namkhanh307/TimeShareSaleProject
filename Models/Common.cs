using Microsoft.AspNetCore.Mvc;

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
        public static List<Block> GetAvailableBlocks(int propertyId)
        {
            using (_4restContext context = new _4restContext())
            {
                List<Block> availableBlocks = new List<Block>();

                var reservations = context.Reservations
                    .Where(r => r.PropertyId == propertyId)
                    .Select(r => r.BlockId) // Select only the BlockIds
                    .Distinct() // Remove duplicate BlockIds
                    .ToList();

                if (reservations.Any())
                {
                    // Fetch blocks which are not reserved
                    availableBlocks = context.Blocks
                        .Where(b => !reservations.Contains(b.Id))
                        .ToList();
                }
                else
                {
                    // If no reservations, all blocks are available
                    availableBlocks = context.Blocks.ToList();
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
    }
}
