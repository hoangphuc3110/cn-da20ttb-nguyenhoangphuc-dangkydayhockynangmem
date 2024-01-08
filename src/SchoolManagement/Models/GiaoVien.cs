using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace SchoolManagement.Models;

public partial class GiaoVien
{
    public string MaGiaoVien { get; set; } = null!;

    public int? UserId { get; set; }

    public string? TenGiaoVien { get; set; }
    [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
    public DateTime? NgaySinh { get; set; }

    public string? GioiTinh { get; set; }

    public string? MaBoMon { get; set; }

    public virtual ICollection<ChamDiem> ChamDiems { get; set; } = new List<ChamDiem>();

    public virtual ICollection<DangKyLopKyNang> DangKyLopKyNangs { get; set; } = new List<DangKyLopKyNang>();

    public virtual BoMon? MaBoMonNavigation { get; set; }

    public virtual User? User { get; set; }
}
