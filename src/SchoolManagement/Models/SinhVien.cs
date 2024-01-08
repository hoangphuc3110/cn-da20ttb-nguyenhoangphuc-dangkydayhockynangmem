using System;
using System.Collections.Generic;

namespace SchoolManagement.Models;

public partial class SinhVien
{
    public string MaSinhVien { get; set; } = null!;

    public int? UserId { get; set; }

    public string? Ten { get; set; }

    public DateTime? NgaySinh { get; set; }

    public string? GioiTinh { get; set; }

    public string? MaLop { get; set; }

    public string? DanToc { get; set; }

    public virtual ICollection<ChamDiem> ChamDiems { get; set; } = new List<ChamDiem>();

    public virtual ICollection<DangKyKyNang> DangKyKyNangs { get; set; } = new List<DangKyKyNang>();

    public virtual ICollection<KetQua> KetQuas { get; set; } = new List<KetQua>();

    public virtual Lop? MaLopNavigation { get; set; }

    public virtual User? User { get; set; }
}
