using System;
using System.Collections.Generic;

namespace SchoolManagement.Models;

public partial class KhoaTruong
{
    public string MaKhoaTruong { get; set; } = null!;

    public string? TenKhoaTruong { get; set; }

    public DateTime? NgayThanhLap { get; set; }

    public virtual ICollection<BoMon> BoMons { get; set; } = new List<BoMon>();
}
