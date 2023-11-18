using System;
using System.Collections.Generic;

namespace Coursework.Entities;

public partial class FeaturesMaterial
{
    public int FeaturesId { get; set; }

    public int SidingId { get; set; }

    public string Value { get; set; } = null!;

    public virtual Feature Features { get; set; } = null!;

    public virtual Siding Siding { get; set; } = null!;
}
