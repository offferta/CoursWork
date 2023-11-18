using System;
using System.Collections.Generic;

namespace Coursework.Entities;

public partial class Calculation
{
    public int CalculationId { get; set; }

    public int WorkerId { get; set; }

    public string Title { get; set; } = null!;

    public DateTime DateOrder { get; set; }

    public virtual ICollection<MaterialsCalculation> MaterialsCalculations { get; set; } = new List<MaterialsCalculation>();

    public virtual ICollection<Wall> Walls { get; set; } = new List<Wall>();

    public virtual ICollection<Window> Windows { get; set; } = new List<Window>();

    public virtual Worker Worker { get; set; } = null!;
}
