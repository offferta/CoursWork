﻿using System;
using System.Collections.Generic;

namespace Coursework.Entities;

public partial class Window
{
    public int WindowId { get; set; }

    public int CalculationId { get; set; }

    public decimal Length { get; set; }

    public decimal Width { get; set; }

    public byte Count { get; set; }

    public bool IsDoor { get; set; }

    public virtual Calculation Calculation { get; set; } = null!;
}