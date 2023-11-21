using System.Collections.Generic;

namespace Coursework.Entities;

public class Role
{
    public int RoleId { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Worker> Workers { get; set; } = new List<Worker>();
}