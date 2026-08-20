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

    public async Task<(bool Success, int SalesSoId, string Message, List<string>? Errors)> CreateOrderAsync(CreateOrderDto dto)
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(dto.SoNo)) errors.Add("Nomor SO wajib diisi.");
        if (dto.CustomerId <= 0) errors.Add("Customer wajib dipilih.");
        if (dto.Items == null || !dto.Items.Any()) errors.Add("Order minimal harus memiliki 1 item.");

        var isDuplicate = await _context.SalesSos.AnyAsync(o => o.SoNo.ToLower() == dto.SoNo.ToLower());
        if (isDuplicate) errors.Add($"Nomor SO '{dto.SoNo}' sudah digunakan.");

        if (errors.Any()) return (false, 0, "Validasi gagal", errors);

        var order = new SalesSo
        {
            SoNo = dto.SoNo,
            OrderDate = dto.OrderDate,
            ComCustomerId = dto.CustomerId,
            Address = dto.Address,
            Items = dto.Items!.Select(i => new SalesSoLitem
            {
                ItemName = i.ItemName,
                Quantity = i.Quantity,
                Price = (double)i.Price
            }).ToList()
        };

        _context.SalesSos.Add(order);
        await _context.SaveChangesAsync();

        return (true, order.SalesSoId, "Order berhasil dibuat", null);
    }

    public async Task<(bool Success, string Message)> UpdateOrderAsync(int id, CreateOrderDto dto)
    {
        var existingOrder = await _context.SalesSos
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.SalesSoId == id);

        if (existingOrder == null) return (false, "Order tidak ditemukan.");

        existingOrder.SoNo = dto.SoNo;
        existingOrder.OrderDate = dto.OrderDate;
        existingOrder.ComCustomerId = dto.CustomerId;
        existingOrder.Address = dto.Address;

        _context.SalesSoLitems.RemoveRange(existingOrder.Items);
        existingOrder.Items = dto.Items.Select(i => new SalesSoLitem
        {
            ItemName = i.ItemName,
            Quantity = i.Quantity,
            Price = (double)i.Price
        }).ToList();

        await _context.SaveChangesAsync();
        return (true, "Order berhasil diperbarui");
    }

    public async Task<bool> DeleteOrderAsync(int id)
    {
        var order = await _context.SalesSos.FindAsync(id);
        if (order == null) return false;

        _context.SalesSos.Remove(order);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<byte[]> ExportOrdersToExcelAsync(string? keyword, DateTime? orderDate)
    {
        var orders = await GetOrdersAsync(keyword, orderDate);

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Sales Orders");

        // Header Table
        worksheet.Cell(1, 1).Value = "Sales SO ID";
        worksheet.Cell(1, 2).Value = "SO No";
        worksheet.Cell(1, 3).Value = "Order Date";
        worksheet.Cell(1, 4).Value = "Customer Name";
        worksheet.Cell(1, 5).Value = "Address";
        worksheet.Cell(1, 6).Value = "Grand Total";

        int row = 2;
        foreach (var order in orders)
        {
            worksheet.Cell(row, 1).Value = order.SalesSoId;
            worksheet.Cell(row, 2).Value = order.SoNo;
            worksheet.Cell(row, 3).Value = order.OrderDate.ToString("yyyy-MM-dd");
            worksheet.Cell(row, 4).Value = order.CustomerName;
            worksheet.Cell(row, 5).Value = order.Address;
            worksheet.Cell(row, 6).Value = order.GrandTotal;
            row++;
        }

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}