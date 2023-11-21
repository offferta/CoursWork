namespace Coursework.Entities;

public class Wall
{
    public int WallId { get; set; }

    public int CalculationId { get; set; }

    public decimal Length { get; set; }

    public decimal Width { get; set; }

    public byte Count { get; set; }

    public virtual Calculation Calculation { get; set; } = null!;
}