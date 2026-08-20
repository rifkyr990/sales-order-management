using System.ComponentModel.DataAnnotations.Schema;

namespace SalesOrderService.Models.Entities;

[Table("SALES_SO")]
public class SalesSo
{
    [Column("SALES_SO_ID")]
    public int SalesSoId { get; set; }

    [Column("SO_NO")]
    public string SoNo { get; set; } = string.Empty;

    [Column("ORDER_DATE")]
    public DateTime OrderDate { get; set; }

    [Column("COM_CUSTOMER_ID")]
    public int ComCustomerId { get; set; }

    [Column("ADDRESS")]
    public string? Address { get; set; }

    public ICollection<SalesSoLitem> Items { get; set; } = new List<SalesSoLitem>();
}