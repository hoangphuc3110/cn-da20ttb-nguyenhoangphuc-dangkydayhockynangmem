using System;
using System.Collections.Generic;

namespace SchoolManagement.Models;

public partial class LopKyNang
{
    public string MaLopKyNang { get; set; } = null!;

    public string? TenLopKyNang { get; set; }

    public DateTime? NgayBatDau { get; set; }

    public string? MaBoMon { get; set; }

    public virtual ICollection<DangKyLopKyNang> DangKyLopKyNangs { get; set; } = new List<DangKyLopKyNang>();

    public virtual ICollection<KyNang> KyNangs { get; set; } = new List<KyNang>();

    public virtual BoMon? MaBoMonNavigation { get; set; }
}
