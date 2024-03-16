using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Swp4RestWeb.Models
{
    public partial class PropertyDetail : DbContext
    {
        public PropertyDetail() { }

        public PropertyDetail(DbContextOptions<PropertyDetail> options)
            : base(options) 
        { }

        public int Id { get; set; }
        public string? Name { get; set; }
        public bool? SaleStatus { get; set; }
        public DateTime? SaleDate { get; set; }
        public double? UnitPrice { get; set; }
        public int ProjectId { get; set; }
        public int? Beds { get; set; }
        public string? Occupancy { get; set; }
        public string? Size { get; set; }
        public string? Bathroom { get; set; }
        public string? Views { get; set; }
        public string? UniqueFeature { get; set; }
        public string? ViewImage { get; set; }
        public string? FrontImage { get; set; }
        public string? InsideImage { get; set; }
        public string? SideImage { get; set; }
        public bool? Status { get; set; }

        public virtual Property? Property { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=THAO-LAPTOP\\SQLTHAO;Database=TimeShareProject;User ID=sa;Password=12345;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;");
            }
        }
        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
