namespace Coursework.Entities;

public class MaterialsCalculation
{
    public int SidingId { get; set; }

    public int CalculationId { get; set; }

    public decimal Count { get; set; }

    public decimal CurrentPrice { get; set; }

    public virtual Calculation Calculation { get; set; } = null!;

    public virtual Siding Siding { get; set; } = null!;
}