using System;
using System.Collections.Generic;

namespace SchoolManagement.Models;

public partial class Admin
{
    public string MaAdmin { get; set; } = null!;

    public string? TenAdmin { get; set; }

    public int? UserId { get; set; }

    public virtual User? User { get; set; }
}
