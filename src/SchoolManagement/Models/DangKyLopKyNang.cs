using System;
using System.Collections.Generic;

namespace SchoolManagement.Models;

public partial class DangKyLopKyNang
{
    public int Id { get; set; }

    public string? MaGiaoVien { get; set; }

    public string? MaLopKyNang { get; set; }

    public virtual GiaoVien? MaGiaoVienNavigation { get; set; }

    public virtual LopKyNang? MaLopKyNangNavigation { get; set; }
}
