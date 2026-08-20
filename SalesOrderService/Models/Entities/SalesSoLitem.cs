using System.ComponentModel.DataAnnotations.Schema;

namespace SalesOrderService.Models.Entities;

[Table("SALES_SO_LITEM")]
public class SalesSoLitem
{
    [Column("SALES_SO_LITEM_ID")]
    public int SalesSoLitemId { get; set; }

    [Column("SALES_SO_ID")]
    public int SalesSoId { get; set; }

    [Column("ITEM_NAME")]
    public string ItemName { get; set; } = string.Empty;

    [Column("QUANTITY")]
    public int Quantity { get; set; }

    [Column("PRICE")]
    public double Price { get; set; }

    public SalesSo? SalesSo { get; set; }
}