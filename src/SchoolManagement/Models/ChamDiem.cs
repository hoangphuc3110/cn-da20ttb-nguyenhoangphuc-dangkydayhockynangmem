using System;
using System.Collections.Generic;

namespace SchoolManagement.Models;

public partial class ChamDiem
{
    public int Id { get; set; }

    public string? MaGiaoVien { get; set; }

    public string? MaSinhVien { get; set; }

    public string? MaKyNang { get; set; }

    public double? Diem { get; set; }

    public string? Dat { get; set; }

    public virtual GiaoVien? MaGiaoVienNavigation { get; set; }

    public virtual KyNang? MaKyNangNavigation { get; set; }

    public virtual SinhVien? MaSinhVienNavigation { get; set; }
}
