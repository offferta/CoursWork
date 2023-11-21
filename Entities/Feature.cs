using System.Collections.Generic;

namespace Coursework.Entities;

public class Feature
{
    public int FeaturesId { get; set; }

    public string TypeParsing { get; set; } = null!;

    public string Title { get; set; } = null!;

    public virtual ICollection<FeaturesMaterial> FeaturesMaterials { get; set; } = new List<FeaturesMaterial>();
}