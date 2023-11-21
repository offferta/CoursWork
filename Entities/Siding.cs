using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Windows.Media.Imaging;

namespace Coursework.Entities;

public class Siding
{
    public int SidingId { get; set; }

    public string Title { get; set; } = null!;

    public string Description { get; set; } = null!;

    public decimal Price { get; set; }

    [Column("image")] public BitmapImage? Image { get; set; }

    public virtual ICollection<FeaturesMaterial> FeaturesMaterials { get; set; } = new List<FeaturesMaterial>();

    public virtual ICollection<MaterialsCalculation> MaterialsCalculations { get; set; } =
        new List<MaterialsCalculation>();
}