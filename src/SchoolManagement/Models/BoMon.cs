using System;
using System.Collections.Generic;

namespace SchoolManagement.Models;

public partial class BoMon
{
    public string MaBoMon { get; set; } = null!;

    public string? TenBoMon { get; set; }

    public string? MaKhoaTruong { get; set; }

    public virtual ICollection<GiaoVien> GiaoViens { get; set; } = new List<GiaoVien>();

    public virtual ICollection<LopKyNang> LopKyNangs { get; set; } = new List<LopKyNang>();

    public virtual ICollection<Lop> Lops { get; set; } = new List<Lop>();

    public virtual KhoaTruong? MaKhoaTruongNavigation { get; set; }
}
