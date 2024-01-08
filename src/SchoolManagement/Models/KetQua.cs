using System;
using System.Collections.Generic;

namespace SchoolManagement.Models;

public partial class KetQua
{
    public string MaKetQua { get; set; } = null!;

    public bool? Dat { get; set; }

    public double? Diem { get; set; }

    public string? MaSinhVien { get; set; }

    public virtual SinhVien? MaSinhVienNavigation { get; set; }
}
