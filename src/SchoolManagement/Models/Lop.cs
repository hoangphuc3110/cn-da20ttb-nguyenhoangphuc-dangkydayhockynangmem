using System;
using System.Collections.Generic;

namespace SchoolManagement.Models;

public partial class Lop
{
    public string MaLop { get; set; } = null!;

    public string? TenLop { get; set; }

    public string? MaBoMon { get; set; }

    public virtual BoMon? MaBoMonNavigation { get; set; }

    public virtual ICollection<Phong> Phongs { get; set; } = new List<Phong>();

    public virtual ICollection<SinhVien> SinhViens { get; set; } = new List<SinhVien>();
}
