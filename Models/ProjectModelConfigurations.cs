using Microsoft.EntityFrameworkCore;
using TimeShareProject.Models;

namespace TimeShareProject.Data.Configurations
{
    public static class ProjectModelConfigurations
    {
        public static void ConfigureProjectModel(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Property>()
                .HasOne(p => p.Project)
                .WithMany(p => p.Properties)
                .HasForeignKey(p => p.ProjectId)
                .OnDelete(DeleteBehavior.Cascade); 
        }
    }
}