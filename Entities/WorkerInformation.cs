using System;
using System.Collections.Generic;

namespace Coursework.Entities;

public partial class WorkerInformation
{
    public int WorkerInformation1 { get; set; }

    public int WorkerId { get; set; }

    public string FirstName { get; set; } = null!;

    public string SecondName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? Email { get; set; }

    public string? Phone { get; set; }

    public virtual Worker Worker { get; set; } = null!;
}
