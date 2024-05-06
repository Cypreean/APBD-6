using WebApplication4.Model;

namespace WebApplication4.Services;

public interface IWarehouseService
{
    public Task<int> AddProduct(int idProduct, int idWarehouse, int ammount, string createdAt);
    
}