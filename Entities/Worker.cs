﻿using System;
using System.Collections.Generic;

namespace Coursework.Entities;

public partial class Worker
{
    public int WorkerId { get; set; }

    public int RoleId { get; set; }

    public string Login { get; set; } = null!;

    public string Password { get; set; } = null!;

    public virtual ICollection<Calculation> Calculations { get; set; } = new List<Calculation>();

    public virtual Role Role { get; set; } = null!;

    public virtual ICollection<WorkerInformation> WorkerInformations { get; set; } = new List<WorkerInformation>();
}