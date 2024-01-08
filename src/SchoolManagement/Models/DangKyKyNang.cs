using System;
using System.Collections.Generic;

namespace SchoolManagement.Models;

public partial class DangKyKyNang
{
    public int Id { get; set; }

    public string? MaSinhVien { get; set; }

    public string? MaKyNang { get; set; }

    public DateTime? NgayDangKy { get; set; }

    public virtual KyNang? MaKyNangNavigation { get; set; }

    public virtual SinhVien? MaSinhVienNavigation { get; set; }
}
