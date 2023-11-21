using System.IO;
using System.Windows.Media.Imaging;
using Coursework.Entities;
using Microsoft.EntityFrameworkCore;

namespace Coursework.Context;

public partial class MyDbContext : DbContext
{
    public MyDbContext()
    {
    }

    public MyDbContext(DbContextOptions<MyDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Calculation> Calculations { get; set; }

    public virtual DbSet<Feature> Features { get; set; }

    public virtual DbSet<FeaturesMaterial> FeaturesMaterials { get; set; }

    public virtual DbSet<MaterialsCalculation> MaterialsCalculations { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Siding> Sidings { get; set; }

    public virtual DbSet<Wall> Walls { get; set; }

    public virtual DbSet<Window> Windows { get; set; }

    public virtual DbSet<Worker> Workers { get; set; }

    public virtual DbSet<WorkerInformation> WorkerInformations { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https: //go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer(
            "Server=localhost;Database=coursework;User Id=admin;Password=qAz1!;TrustServerCertificate=true;Trusted_Connection=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Calculation>(entity =>
        {
            entity.HasKey(e => e.CalculationId).HasName("PK__Calculat__F289C5C1F28803C2");

            entity.ToTable("Calculation");

            entity.Property(e => e.CalculationId).HasColumnName("calculationId");
            entity.Property(e => e.DateOrder)
                .HasColumnType("date")
                .HasColumnName("dateOrder");
            entity.Property(e => e.Title)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("title");
            entity.Property(e => e.WorkerId).HasColumnName("workerId");

            entity.HasOne(d => d.Worker).WithMany(p => p.Calculations)
                .HasForeignKey(d => d.WorkerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKCalculation645376");
        });

        modelBuilder.Entity<Feature>(entity =>
        {
            entity.HasKey(e => e.FeaturesId).HasName("PK__Features__63DEC89BFD2C619F");

            entity.Property(e => e.FeaturesId).HasColumnName("featuresId");
            entity.Property(e => e.Title)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("title");
            entity.Property(e => e.TypeParsing)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("typeParsing");
        });

        modelBuilder.Entity<FeaturesMaterial>(entity =>
        {
            entity.HasKey(e => new { e.FeaturesId, e.SidingId, e.Value }).HasName("PK__Features__F4E24F7EB8B59579");

            entity.ToTable("Features_Materials");

            entity.Property(e => e.FeaturesId).HasColumnName("featuresId");
            entity.Property(e => e.SidingId).HasColumnName("sidingId");
            entity.Property(e => e.Value)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("value");

            entity.HasOne(d => d.Features).WithMany(p => p.FeaturesMaterials)
                .HasForeignKey(d => d.FeaturesId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Features_Materials_Features");

            entity.HasOne(d => d.Siding).WithMany(p => p.FeaturesMaterials)
                .HasForeignKey(d => d.SidingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Features_Materials_Siding");
        });

        modelBuilder.Entity<MaterialsCalculation>(entity =>
        {
            entity.HasKey(e => new { e.SidingId, e.CalculationId }).HasName("PK__Material__D8EB5CA75D479259");

            entity.ToTable("Materials_Calculation");

            entity.Property(e => e.SidingId).HasColumnName("sidingId");
            entity.Property(e => e.CalculationId).HasColumnName("calculationId");
            entity.Property(e => e.Count)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("count");
            entity.Property(e => e.CurrentPrice)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("currentPrice");

            entity.HasOne(d => d.Calculation).WithMany(p => p.MaterialsCalculations)
                .HasForeignKey(d => d.CalculationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKMaterials_109362");

            entity.HasOne(d => d.Siding).WithMany(p => p.MaterialsCalculations)
                .HasForeignKey(d => d.SidingId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKMaterials_148670");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__CD98462AFFF50735");

            entity.Property(e => e.RoleId).HasColumnName("roleId");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Siding>(entity =>
        {
            entity.HasKey(e => e.SidingId).HasName("PK__Siding__D7C3C0FBFE3A2790");

            entity.ToTable("Siding");

            entity.Property(e => e.SidingId).HasColumnName("sidingId");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("description");
            entity.Property(x => x.Image)
                .HasConversion(
                    v => ConvertToByteArray(v),
                    v => ConvertToBitmapImage(v)
                );
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");
            entity.Property(e => e.Title)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("title");
        });

        modelBuilder.Entity<Wall>(entity =>
        {
            entity.HasKey(e => e.WallId).HasName("PK__Walls__BB1ABCCF73FA4952");

            entity.Property(e => e.WallId).HasColumnName("wallId");
            entity.Property(e => e.CalculationId).HasColumnName("calculationId");
            entity.Property(e => e.Count).HasColumnName("count");
            entity.Property(e => e.Length)
                .HasColumnType("decimal(4, 1)")
                .HasColumnName("length");
            entity.Property(e => e.Width)
                .HasColumnType("decimal(4, 1)")
                .HasColumnName("width");

            entity.HasOne(d => d.Calculation).WithMany(p => p.Walls)
                .HasForeignKey(d => d.CalculationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKWalls123685");
        });

        modelBuilder.Entity<Window>(entity =>
        {
            entity.HasKey(e => e.WindowId).HasName("PK__Windows__3E52924B43C654B5");

            entity.Property(e => e.WindowId).HasColumnName("windowId");
            entity.Property(e => e.CalculationId).HasColumnName("calculationId");
            entity.Property(e => e.Count).HasColumnName("count");
            entity.Property(e => e.IsDoor).HasColumnName("isDoor");
            entity.Property(e => e.Length)
                .HasColumnType("decimal(4, 1)")
                .HasColumnName("length");
            entity.Property(e => e.Width)
                .HasColumnType("decimal(4, 1)")
                .HasColumnName("width");

            entity.HasOne(d => d.Calculation).WithMany(p => p.Windows)
                .HasForeignKey(d => d.CalculationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKWindows682599");
        });

        modelBuilder.Entity<Worker>(entity =>
        {
            entity.HasKey(e => e.WorkerId).HasName("PK__Workers__3CF20591C656365E");

            entity.Property(e => e.WorkerId).HasColumnName("workerId");
            entity.Property(e => e.Login)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("login");
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.RoleId).HasColumnName("roleId");

            entity.HasOne(d => d.Role).WithMany(p => p.Workers)
                .HasForeignKey(d => d.RoleId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKWorkers125598");
        });

        modelBuilder.Entity<WorkerInformation>(entity =>
        {
            entity.HasKey(e => e.WorkerInformation1).HasName("PK__WorkerIn__242A65166F1B7626");

            entity.ToTable("WorkerInformation");

            entity.Property(e => e.WorkerInformation1).HasColumnName("workerInformation");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("firstName");
            entity.Property(e => e.LastName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("lastName");
            entity.Property(e => e.Phone)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("phone");
            entity.Property(e => e.SecondName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("secondName");
            entity.Property(e => e.WorkerId).HasColumnName("workerId");

            entity.HasOne(d => d.Worker).WithMany(p => p.WorkerInformations)
                .HasForeignKey(d => d.WorkerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FKWorkerInfo191972");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    // Метод для преобразования byte[] в BitmapImage
    public static BitmapImage ConvertToBitmapImage(byte[] byteArray)
    {
        if (byteArray == null || byteArray.Length == 0) return null;

        using (var stream = new MemoryStream(byteArray))
        {
            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = stream;
            bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapImage.EndInit();
            return bitmapImage;
        }
    }

// Метод для преобразования BitmapImage в byte[]
    private byte[] ConvertToByteArray(BitmapImage bitmapImage)
    {
        if (bitmapImage == null) return null;

        using (var stream = new MemoryStream())
        {
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
            encoder.Save(stream);
            return stream.ToArray();
        }
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}