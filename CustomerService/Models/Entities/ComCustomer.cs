using System.ComponentModel.DataAnnotations.Schema;

namespace CustomerService.Models.Entities;

[Table("COM_CUSTOMER")]
public class ComCustomer
{
    [Column("COM_CUSTOMER_ID")]
    public int ComCustomerId { get; set; }

    [Column("CUSTOMER_NAME")]
    public string CustomerName { get; set; } = string.Empty;
}