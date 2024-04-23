using WebApplication.Models.DTOs;

namespace WebApplication.Repositories;

public interface IWarehouseRepository
{
    Task<bool> ProductWithIdExists(int id);

    Task<bool> WarehouseWithIdExists(int id);

    Task<bool> IdProductAndAmountCombinationInOrderExists(int idProduct, int amount);
    
    Task<int> ReturnOrderId(int idProduct, int amount);
    
    Task<bool> CreatedAtOfOrderIsLowerThanCreatedAtOfRequest(int orderId, DateTime requestDate);
    
    Task<bool> IdOrderDoesNotAlreadyExist(int idOrder);

    Task UpdateFulfilledAtOfOrder(int idOrder);

    Task<decimal> ReturnProductPrice(int productId);
    
    Task<int> InsertProduct_WarehouseRecord(DataToAccept dataToAccept, int idOrder );

    Task<int> AddWithProcedures(DataToAccept dataToAccept);
}