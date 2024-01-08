using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace SchoolManagement.Models;

public partial class SchoolContext : DbContext
{
    public SchoolContext()
    {
    }

    public SchoolContext(DbContextOptions<SchoolContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<BoMon> BoMons { get; set; }

    public virtual DbSet<ChamDiem> ChamDiems { get; set; }

    public virtual DbSet<DangKyKyNang> DangKyKyNangs { get; set; }

    public virtual DbSet<DangKyLopKyNang> DangKyLopKyNangs { get; set; }

    public virtual DbSet<GiaoVien> GiaoViens { get; set; }

    public virtual DbSet<KetQua> KetQuas { get; set; }

    public virtual DbSet<KhoaTruong> KhoaTruongs { get; set; }

    public virtual DbSet<KyNang> KyNangs { get; set; }

    public virtual DbSet<Lop> Lops { get; set; }

    public virtual DbSet<LopKyNang> LopKyNangs { get; set; }

    public virtual DbSet<Phong> Phongs { get; set; }

    public virtual DbSet<SinhVien> SinhViens { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=DESKTOP-MVU19RK\\SQLEXPRESS;Database=School;Integrated Security=true;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.MaAdmin).HasName("PK__Admin__49341E385629CD9C");

            entity.ToTable("Admin");

            entity.Property(e => e.MaAdmin)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.TenAdmin).HasMaxLength(255);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.User).WithMany(p => p.Admins)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Admin__UserID__5812160E");
        });

        modelBuilder.Entity<BoMon>(entity =>
        {
            entity.HasKey(e => e.MaBoMon).HasName("PK__BoMon__B783EFA6300424B4");

            entity.ToTable("BoMon");

            entity.Property(e => e.MaBoMon)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.MaKhoaTruong)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.TenBoMon).HasMaxLength(255);

            entity.HasOne(d => d.MaKhoaTruongNavigation).WithMany(p => p.BoMons)
                .HasForeignKey(d => d.MaKhoaTruong)
                .HasConstraintName("FK__BoMon__MaKhoaTru__31EC6D26");
        });

        modelBuilder.Entity<ChamDiem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ChamDiem__3214EC2760A75C0F");

            entity.ToTable("ChamDiem");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.Dat).HasMaxLength(50);
            entity.Property(e => e.MaGiaoVien)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.MaKyNang)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.MaSinhVien)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.HasOne(d => d.MaGiaoVienNavigation).WithMany(p => p.ChamDiems)
                .HasForeignKey(d => d.MaGiaoVien)
                .HasConstraintName("FK__ChamDiem__MaGiao__628FA481");

            entity.HasOne(d => d.MaKyNangNavigation).WithMany(p => p.ChamDiems)
                .HasForeignKey(d => d.MaKyNang)
                .HasConstraintName("FK__ChamDiem__MaKyNa__6477ECF3");

            entity.HasOne(d => d.MaSinhVienNavigation).WithMany(p => p.ChamDiems)
                .HasForeignKey(d => d.MaSinhVien)
                .HasConstraintName("FK__ChamDiem__MaSinh__6383C8BA");
        });

        modelBuilder.Entity<DangKyKyNang>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__DangKyKy__3214EC275AEE82B9");

            entity.ToTable("DangKyKyNang");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.MaKyNang)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.MaSinhVien)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.NgayDangKy).HasColumnType("date");

            entity.HasOne(d => d.MaKyNangNavigation).WithMany(p => p.DangKyKyNangs)
                .HasForeignKey(d => d.MaKyNang)
                .HasConstraintName("FK__DangKyKyN__MaKyN__5DCAEF64");

            entity.HasOne(d => d.MaSinhVienNavigation).WithMany(p => p.DangKyKyNangs)
                .HasForeignKey(d => d.MaSinhVien)
                .HasConstraintName("FK__DangKyKyN__MaSin__5CD6CB2B");
        });

        modelBuilder.Entity<DangKyLopKyNang>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__DangKyLo__3214EC276754599E");

            entity.ToTable("DangKyLopKyNang");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("ID");
            entity.Property(e => e.MaGiaoVien)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.MaLopKyNang)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.HasOne(d => d.MaGiaoVienNavigation).WithMany(p => p.DangKyLopKyNangs)
                .HasForeignKey(d => d.MaGiaoVien)
                .HasConstraintName("FK__DangKyLop__MaGia__693CA210");

            entity.HasOne(d => d.MaLopKyNangNavigation).WithMany(p => p.DangKyLopKyNangs)
                .HasForeignKey(d => d.MaLopKyNang)
                .HasConstraintName("FK__DangKyLop__MaLop__6A30C649");
        });

        modelBuilder.Entity<GiaoVien>(entity =>
        {
            entity.HasKey(e => e.MaGiaoVien).HasName("PK__GiaoVien__8D374F503F466844");

            entity.ToTable("GiaoVien");

            entity.Property(e => e.MaGiaoVien)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.GioiTinh).HasMaxLength(10);
            entity.Property(e => e.MaBoMon)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.NgaySinh).HasColumnType("date");
            entity.Property(e => e.TenGiaoVien).HasMaxLength(255);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.MaBoMonNavigation).WithMany(p => p.GiaoViens)
                .HasForeignKey(d => d.MaBoMon)
                .HasConstraintName("FK__GiaoVien__MaBoMo__412EB0B6");

            entity.HasOne(d => d.User).WithMany(p => p.GiaoViens)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_GiaoVien_User");
        });

        modelBuilder.Entity<KetQua>(entity =>
        {
            entity.HasKey(e => e.MaKetQua).HasName("PK__KetQua__D5B3102A5165187F");

            entity.ToTable("KetQua");

            entity.Property(e => e.MaKetQua)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.MaSinhVien)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.HasOne(d => d.MaSinhVienNavigation).WithMany(p => p.KetQuas)
                .HasForeignKey(d => d.MaSinhVien)
                .HasConstraintName("FK__KetQua__MaSinhVi__534D60F1");
        });

        modelBuilder.Entity<KhoaTruong>(entity =>
        {
            entity.HasKey(e => e.MaKhoaTruong).HasName("PK__KhoaTruo__BB05FC672C3393D0");

            entity.ToTable("KhoaTruong");

            entity.Property(e => e.MaKhoaTruong)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.NgayThanhLap).HasColumnType("date");
            entity.Property(e => e.TenKhoaTruong).HasMaxLength(255);
        });

        modelBuilder.Entity<KyNang>(entity =>
        {
            entity.HasKey(e => e.MaKyNang).HasName("PK__KyNang__796CFDAF48CFD27E");

            entity.ToTable("KyNang");

            entity.Property(e => e.MaKyNang)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.MaLopKyNang)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.TenKyNang).HasMaxLength(255);

            entity.HasOne(d => d.MaLopKyNangNavigation).WithMany(p => p.KyNangs)
                .HasForeignKey(d => d.MaLopKyNang)
                .HasConstraintName("FK__KyNang__MaLopKyN__4AB81AF0");
        });

        modelBuilder.Entity<Lop>(entity =>
        {
            entity.HasKey(e => e.MaLop).HasName("PK__Lop__3B98D27334C8D9D1");

            entity.ToTable("Lop");

            entity.Property(e => e.MaLop)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.MaBoMon)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.TenLop).HasMaxLength(255);

            entity.HasOne(d => d.MaBoMonNavigation).WithMany(p => p.Lops)
                .HasForeignKey(d => d.MaBoMon)
                .HasConstraintName("FK__Lop__MaBoMon__36B12243");
        });

        modelBuilder.Entity<LopKyNang>(entity =>
        {
            entity.HasKey(e => e.MaLopKyNang).HasName("PK__LopKyNan__20B0074E440B1D61");

            entity.ToTable("LopKyNang");

            entity.Property(e => e.MaLopKyNang)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.MaBoMon)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.NgayBatDau).HasColumnType("date");
            entity.Property(e => e.TenLopKyNang).HasMaxLength(255);

            entity.HasOne(d => d.MaBoMonNavigation).WithMany(p => p.LopKyNangs)
                .HasForeignKey(d => d.MaBoMon)
                .HasConstraintName("FK__LopKyNang__MaBoM__45F365D3");
        });

        modelBuilder.Entity<Phong>(entity =>
        {
            entity.HasKey(e => e.MaPhong).HasName("PK__Phong__20BD5E5B4D94879B");

            entity.ToTable("Phong");

            entity.Property(e => e.MaPhong)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.MaLop)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.TenPhong).HasMaxLength(255);

            entity.HasOne(d => d.MaLopNavigation).WithMany(p => p.Phongs)
                .HasForeignKey(d => d.MaLop)
                .HasConstraintName("FK_Phong_Lop");
        });

        modelBuilder.Entity<SinhVien>(entity =>
        {
            entity.HasKey(e => e.MaSinhVien).HasName("PK__SinhVien__939AE775398D8EEE");

            entity.ToTable("SinhVien");

            entity.Property(e => e.MaSinhVien)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.DanToc)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.GioiTinh).HasMaxLength(10);
            entity.Property(e => e.MaLop)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.NgaySinh).HasColumnType("date");
            entity.Property(e => e.Ten).HasMaxLength(255);
            entity.Property(e => e.UserId).HasColumnName("UserID");

            entity.HasOne(d => d.MaLopNavigation).WithMany(p => p.SinhViens)
                .HasForeignKey(d => d.MaLop)
                .HasConstraintName("FK__SinhVien__MaLop__3B75D760");

            entity.HasOne(d => d.User).WithMany(p => p.SinhViens)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__SinhVien__UserID__3C69FB99");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__User__1788CCAC286302EC");

            entity.ToTable("User");

            entity.Property(e => e.UserId)
                .ValueGeneratedNever()
                .HasColumnName("UserID");
            entity.Property(e => e.Password).HasMaxLength(255);
            entity.Property(e => e.Role).HasMaxLength(20);
            entity.Property(e => e.Username).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
