using Microsoft.AspNetCore.Mvc;

namespace TimeShareProject.Models
{
    public class Common
    {

        private readonly TimeShareProjectContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public Common(TimeShareProjectContext context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        public static List<Property> GetProperties(Project project)
        {
            using TimeShareProjectContext context = new TimeShareProjectContext();
            List<Property> list = context.Properties.Where(p => p.ProjectId == project.Id).OrderBy(p => p.Name).ToList();
            return list;
        }

        public static List<Block> GetBlocks()
        {
            using TimeShareProjectContext context = new TimeShareProjectContext();
            List<Block> blocks = context.Blocks.ToList();
            return blocks;
        }

        public static string GetProjectShortNameFromProperty(Property property)
        {
            using (var context = new TimeShareProjectContext())
            {
                var project = context.Projects.FirstOrDefault(p => p.Id == property.ProjectId);
                return project != null ? project.ShortName : "Unknown";
            }
        }

        public static List<int?> GetDistinctBedTypes()
        {
            using TimeShareProjectContext context = new();
            var distinctBedTypes = context.Properties.Select(p => p.Beds).Distinct().ToList();
            return distinctBedTypes;
        }

        public static List<int> GetDistinctReservation()
        {
            using TimeShareProjectContext context = new();
            var distinctReservation = context.Reservations.Select(r => r.Id).Distinct().ToList();
            return distinctReservation;
        }

        public static float Calculate(float unitprice)
        {
            return unitprice * 10;
        }

        public static int CountReservations(int propertyId, int blockId)
        {
            using TimeShareProjectContext context = new();
            int reservationCount = context.Reservations.Count(r => r.PropertyId == propertyId && r.BlockId == blockId);
            return reservationCount;
        }

        public static List<Block> GetAvailableBlocks(int propertyId)
        {
            using (TimeShareProjectContext context = new TimeShareProjectContext())
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
            using TimeShareProjectContext context = new TimeShareProjectContext();
            List<Property> reservedProperties = new List<Property>();
            reservedProperties = context.Reservations.Select(r => r.Property).Distinct().ToList();
            return reservedProperties;
        }

        public static List<Block> GetReservedBlocks()
        {
            using TimeShareProjectContext context = new();
            List<Block> reservedBlocks = new();
            reservedBlocks = context.Reservations.Select(r => r.Block).Distinct().ToList();
            return reservedBlocks;
        }

        public static List<Project> GetProjects()
        {
            using TimeShareProjectContext context = new();
            var projects = new List<Project>();
            projects = context.Projects.ToList();
            return projects;
        }

        public static string? GetPropertyName(int? Id)
        {
            using TimeShareProjectContext context = new();
            var propertyName = context.Properties.FirstOrDefault(p => p.Id == Id).Name;
            return propertyName;
        }
    }
}
