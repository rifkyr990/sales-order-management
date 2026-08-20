using Microsoft.EntityFrameworkCore;
using SalesOrderService.Data;
using SalesOrderService.Models.DTOs;
using SalesOrderService.Models.Entities;
using SalesOrderService.Services.Interfaces;

namespace SalesOrderService.Services;

public class SalesOrderService : ISalesOrderService
{
    private readonly SalesOrderDbContext _context;

    public SalesOrderService(SalesOrderDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<OrderListDto>> GetOrdersAsync(string? search)
    {
        // Menjalankan Query Stored Procedure sp_get_orders
        var query = _context.Database.SqlQueryRaw<OrderListDto>(
            "EXEC sp_get_orders @Keyword = {0}", search ?? (object)DBNull.Value
        );

        return await query.ToListAsync();
    }

    public async Task<bool> CreateOrderAsync(CreateOrderDto dto)
    {
        var order = new SalesSo
        {
            SoNo = dto.SoNo,
            OrderDate = dto.OrderDate,
            ComCustomerId = dto.ComCustomerId,
            Address = dto.Address,
            Items = dto.Items.Select(i => new SalesSoLitem
            {
                ItemName = i.ItemName,
                Quantity = i.Quantity,
                Price = i.Price
            }).ToList()
        };

        _context.SalesSos.Add(order);
        return await _context.SaveChangesAsync() > 0;
    }
}