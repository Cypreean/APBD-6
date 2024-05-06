using Microsoft.AspNetCore.Mvc;
using WebApplication4.Model;
using WebApplication4.Services;

namespace WebApplication4.Controllers;
[ApiController]
[Route("api/[controller]")]
public class WarehouseController : ControllerBase
{
    private readonly IWarehouseService _warehouseService;
    
    public WarehouseController(IWarehouseService warehouseService)
    {
        _warehouseService = warehouseService;
    }
    
    
    [HttpPost]
    public async Task<ActionResult<Warehouse>> AddProduct([FromBody] Warehouse warehouse)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var addedProduct = await _warehouseService.AddProduct(warehouse.IdProduct, warehouse.IdWarehouse, warehouse.Ammount, warehouse.CreatedAt);
            if (addedProduct == -1)
            {
                return NotFound();
            }
            return Ok(addedProduct);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
