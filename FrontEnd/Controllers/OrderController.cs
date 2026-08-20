using Microsoft.AspNetCore.Mvc;
using FrontEnd.Models;

namespace FrontEnd.Controllers;

public class OrderController : Controller
{
    private readonly IHttpClientFactory _clientFactory;

    public OrderController(IHttpClientFactory clientFactory)
    {
        _clientFactory = clientFactory;
    }

    // List Order
    [HttpGet]
    public async Task<IActionResult> Index(string? keyword, string? orderDate)
    {
        var client = _clientFactory.CreateClient("SalesOrderService");
        var orders = await client.GetFromJsonAsync<List<OrderListDto>>($"api/orders?keyword={keyword}&orderDate={orderDate}");

        ViewBag.Keyword = keyword;
        ViewBag.OrderDate = orderDate;

        return View(orders ?? new List<OrderListDto>());
    }

    // Form Tambah Order
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var customerClient = _clientFactory.CreateClient("CustomerService");
        ViewBag.Customers = await customerClient.GetFromJsonAsync<List<CustomerOptionDto>>("api/customers");

        return View(new CreateOrderViewModel());
    }

    // Submit Tambah Order (Proxy ke Backend)
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateOrderViewModel model)
    {
        var client = _clientFactory.CreateClient("SalesOrderService");
        var response = await client.PostAsJsonAsync("api/orders", model);
        var content = await response.Content.ReadAsStringAsync();

        return StatusCode((int)response.StatusCode, content);
    }

    // Form Edit Order
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var customerClient = _clientFactory.CreateClient("CustomerService");
        ViewBag.Customers = await customerClient.GetFromJsonAsync<List<CustomerOptionDto>>("api/customers");

        var client = _clientFactory.CreateClient("SalesOrderService");
        var order = await client.GetFromJsonAsync<OrderDetailViewModel>($"api/orders/{id}");
        
        if (order == null) return NotFound();

        return View(order);
    }

    // Submit Edit Order (Proxy ke Backend)
    [HttpPost]
    public async Task<IActionResult> Edit(int id, [FromBody] CreateOrderViewModel model)
    {
        var client = _clientFactory.CreateClient("SalesOrderService");
        var response = await client.PutAsJsonAsync($"api/orders/{id}", model);
        var content = await response.Content.ReadAsStringAsync();

        return StatusCode((int)response.StatusCode, content);
    }

    // Submit Delete Order
    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var client = _clientFactory.CreateClient("SalesOrderService");
        var response = await client.DeleteAsync($"api/orders/{id}");
        var content = await response.Content.ReadAsStringAsync();

        return StatusCode((int)response.StatusCode, content);
    }
}