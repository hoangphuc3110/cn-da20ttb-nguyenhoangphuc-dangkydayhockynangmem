using System;
using System.Collections.Generic;

namespace SchoolManagement.Models;

public partial class KyNang
{
    public string MaKyNang { get; set; } = null!;

    public string? TenKyNang { get; set; }


    public string? MaLopKyNang { get; set; }

    public virtual ICollection<ChamDiem> ChamDiems { get; set; } = new List<ChamDiem>();

    public virtual ICollection<DangKyKyNang> DangKyKyNangs { get; set; } = new List<DangKyKyNang>();

    public virtual LopKyNang? MaLopKyNangNavigation { get; set; }
}
