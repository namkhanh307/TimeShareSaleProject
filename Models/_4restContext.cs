using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TimeShareProject.Models;

public partial class _4restContext : DbContext
{
    public _4restContext()
    {
    }

    public _4restContext(DbContextOptions<_4restContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Account> Accounts { get; set; }

    public virtual DbSet<Block> Blocks { get; set; }

    public virtual DbSet<Contact> Contacts { get; set; }

    public virtual DbSet<New> News { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<Property> Properties { get; set; }

    public virtual DbSet<Rate> Rates { get; set; }

    public virtual DbSet<Reservation> Reservations { get; set; }

    public virtual DbSet<Transaction> Transactions { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=tcp:swp4rest.database.windows.net,1433;Initial Catalog=4rest;Persist Security Info=False;User ID=swp4rest;Password=swpswp@1;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=60;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Account__3214EC27CAA521F4");

            entity.ToTable("Account");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .HasColumnName("password");
            entity.Property(e => e.Role).HasColumnName("role");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .HasColumnName("username");
        });

        modelBuilder.Entity<Block>(entity =>
        {
            entity.ToTable("Block");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.BlockNumber).HasColumnName("blockNumber");
            entity.Property(e => e.EndDay).HasColumnName("endDay");
            entity.Property(e => e.EndMonth).HasColumnName("endMonth");
            entity.Property(e => e.Proportion).HasColumnName("proportion");
            entity.Property(e => e.StartDay).HasColumnName("startDay");
            entity.Property(e => e.StartMonth).HasColumnName("startMonth");
        });

        modelBuilder.Entity<Contact>(entity =>
        {
            entity.ToTable("Contact");

            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.Message).HasColumnName("message");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Phone)
                .HasMaxLength(50)
                .HasColumnName("phone");
            entity.Property(e => e.Status).HasColumnName("status");
        });

        modelBuilder.Entity<New>(entity =>
        {
            entity.ToTable("New");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Content).HasColumnName("content");
            entity.Property(e => e.Date)
                .HasColumnType("datetime")
                .HasColumnName("date");
            entity.Property(e => e.Title).HasColumnName("title");
            entity.Property(e => e.TransactionId).HasColumnName("transactionID");
            entity.Property(e => e.Type).HasColumnName("type");
            entity.Property(e => e.UserId).HasColumnName("userID");

            entity.HasOne(d => d.Transaction).WithMany(p => p.News)
                .HasForeignKey(d => d.TransactionId)
                .HasConstraintName("FK_New_Transaction");

            entity.HasOne(d => d.User).WithMany(p => p.News)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_New_User");
        });

        modelBuilder.Entity<Project>(entity =>
        {
            entity.ToTable("Project");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Address).HasColumnName("address");
            entity.Property(e => e.AddressImage).HasColumnName("addressImage");
            entity.Property(e => e.Area).HasColumnName("area");
            entity.Property(e => e.DetailDescription).HasColumnName("detailDescription");
            entity.Property(e => e.GeneralDescription).HasColumnName("generalDescription");
            entity.Property(e => e.Image1).HasColumnName("image1");
            entity.Property(e => e.Image2).HasColumnName("image2");
            entity.Property(e => e.Image3).HasColumnName("image3");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.ShortName).HasColumnName("shortName");
            entity.Property(e => e.Star).HasColumnName("star");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.TotalUnit).HasColumnName("totalUnit");
        });

        modelBuilder.Entity<Property>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_VacationList");

            entity.ToTable("Property");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Bathroom).HasColumnName("bathroom");
            entity.Property(e => e.Beds).HasColumnName("beds");
            entity.Property(e => e.FrontImage).HasColumnName("frontImage");
            entity.Property(e => e.InsideImage).HasColumnName("insideImage");
            entity.Property(e => e.Name).HasColumnName("name");
            entity.Property(e => e.Occupancy).HasColumnName("occupancy");
            entity.Property(e => e.ProjectId).HasColumnName("projectID");
            entity.Property(e => e.SaleDate)
                .HasColumnType("datetime")
                .HasColumnName("saleDate");
            entity.Property(e => e.SaleStatus).HasColumnName("saleStatus");
            entity.Property(e => e.SideImage).HasColumnName("sideImage");
            entity.Property(e => e.Size).HasColumnName("size");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.UniqueFeature).HasColumnName("uniqueFeature");
            entity.Property(e => e.UnitPrice).HasColumnName("unitPrice");
            entity.Property(e => e.ViewImage).HasColumnName("viewImage");
            entity.Property(e => e.Views).HasColumnName("views");

            entity.HasOne(d => d.Project).WithMany(p => p.Properties)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Vacation_Project");
        });

        modelBuilder.Entity<Rate>(entity =>
        {
            entity.HasKey(e => new { e.ProjectId, e.UserId }).HasName("PK__Rate__70B22D9023FDCE83");

            entity.ToTable("Rate");

            entity.Property(e => e.ProjectId).HasColumnName("projectID");
            entity.Property(e => e.UserId).HasColumnName("userID");
            entity.Property(e => e.DetailRate).HasColumnName("detailRate");
            entity.Property(e => e.StarRate).HasColumnName("starRate");

            entity.HasOne(d => d.Project).WithMany(p => p.Rates)
                .HasForeignKey(d => d.ProjectId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Rate_Project");

            entity.HasOne(d => d.User).WithMany(p => p.Rates)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Rate__userID__18EBB532");
        });

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_VacationRegistration");

            entity.ToTable("Reservation");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.BlockId).HasColumnName("blockID");
            entity.Property(e => e.Order).HasColumnName("order");
            entity.Property(e => e.PropertyId).HasColumnName("propertyID");
            entity.Property(e => e.RegisterDate)
                .HasColumnType("datetime")
                .HasColumnName("registerDate");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.Type).HasColumnName("type");
            entity.Property(e => e.UserId).HasColumnName("userID");
            entity.Property(e => e.YearQuantity).HasColumnName("yearQuantity");

            entity.HasOne(d => d.Block).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.BlockId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reservation_Block");

            entity.HasOne(d => d.Property).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.PropertyId)
                .HasConstraintName("FK_Reservation_Property");

            entity.HasOne(d => d.User).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Reservation_User");
        });

        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.ToTable("Transaction");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Amount).HasColumnName("amount");
            entity.Property(e => e.Date)
                .HasColumnType("datetime")
                .HasColumnName("date");
            entity.Property(e => e.DeadlineDate)
                .HasColumnType("datetime")
                .HasColumnName("deadlineDate");
            entity.Property(e => e.ReservationId).HasColumnName("reservationID");
            entity.Property(e => e.ResolveDate)
                .HasColumnType("datetime")
                .HasColumnName("resolveDate");
            entity.Property(e => e.Status).HasColumnName("status");
            entity.Property(e => e.TransactionCode).HasColumnName("transactionCode");
            entity.Property(e => e.Type).HasColumnName("type");

            entity.HasOne(d => d.Reservation).WithMany(p => p.Transactions)
                .HasForeignKey(d => d.ReservationId)
                .HasConstraintName("FK_Transaction_Reservation");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.AccountId).HasColumnName("accountID");
            entity.Property(e => e.Address).HasColumnName("address");
            entity.Property(e => e.DateOfBirth)
                .HasColumnType("datetime")
                .HasColumnName("dateOfBirth");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.IdbackImage).HasColumnName("IDBackImage");
            entity.Property(e => e.IdfrontImage).HasColumnName("IDFrontImage");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(50)
                .HasColumnName("phoneNumber");
            entity.Property(e => e.Sex).HasColumnName("sex");
            entity.Property(e => e.Status).HasColumnName("status");

            entity.HasOne(d => d.Account).WithMany(p => p.Users)
                .HasForeignKey(d => d.AccountId)
                .HasConstraintName("FK_User_Account");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
