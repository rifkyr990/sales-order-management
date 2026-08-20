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

    public async Task<IEnumerable<OrderListDto>> GetOrdersAsync(string? keyword, DateTime? orderDate)
    {
        object dateParam = orderDate.HasValue ? orderDate.Value.Date : DBNull.Value;
        
        return await _context.Database.SqlQueryRaw<OrderListDto>(
            "EXEC sp_get_orders @Keyword = {0}, @OrderDate = {1}", 
            keyword ?? (object)DBNull.Value, 
            dateParam
        ).ToListAsync();
    }

    public async Task<OrderDetailDto?> GetOrderByIdAsync(int id)
    {
        var order = await _context.SalesSos
            .Include(o => o.Items)
            .AsNoTracking()
            .FirstOrDefaultAsync(o => o.SalesSoId == id);

        if (order == null) return null;

        var items = order.Items.Select(i => new OrderItemDetailDto(
            i.SalesSoLitemId,
            i.ItemName,
            i.Quantity,
            (decimal)i.Price,
            (decimal)(i.Quantity * i.Price)
        )).ToList();

        decimal grandTotal = items.Sum(i => i.Total);

        return new OrderDetailDto(
            order.SalesSoId,
            order.SoNo,
            order.OrderDate,
            order.ComCustomerId,
            "Customer Name Placeholder",
            order.Address,
            grandTotal,
            items
        );
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