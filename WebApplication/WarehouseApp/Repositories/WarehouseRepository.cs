using Microsoft.Data.SqlClient;

namespace WebApplication.Repositories;

public class WarehouseRepository : IWarehouseRepository
{
    private readonly IConfiguration _configuration;
    
    public WarehouseRepository(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public async Task<bool> ProductWithIdExists(int id)
    {
        var query = "SELECT 1 FROM Product WHERE IdProduct = @ID";

        using var connection = new SqlConnection(); 
        
        throw new NotImplementedException();
    }

    public async Task<bool> WarehouseWithIdExists(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> IdProductAndAmountCombinationInOrderExists(int idProduct, int amount)
    {
        throw new NotImplementedException();
    }

    public async Task<int> ReturnOrderId(int idProduct, int amount)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> CreatedAtOfOrderIsLowerThanCreatedAtOfRequest(int orderId, DateTime requestDate)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> IdOrderDoesNotAlreadyExist(int idOrder)
    {
        throw new NotImplementedException();
    }

    public async Task UpdateFulfilledAtOfOrder(int idOrder)
    {
        throw new NotImplementedException();
    }

    public async Task<double> ReturnProductPrice(int productId)
    {
        throw new NotImplementedException();
    }

    public async Task<int> InsertProduct_WarehouseRecord(int idWarehouse, int idProduct, int idOrder, int amount)
    {
        throw new NotImplementedException();
    }
}