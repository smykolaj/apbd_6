namespace WebApplication.Repositories;

public interface IWarehouseRepository
{
    //TODO: We check if the product with the given id exists
    Task<bool> ProductWithIdExists(int id);

//TODO: check if the warehouse with the given id exists
    Task<bool> WarehouseWithIdExists(int id);

//TODO:check if there is a record in the Order table with IdProduct and Amount that matches our request
    Task<bool> IdProductAndAmountCombinationInOrderExists(int idProduct, int amount);
    
    //TODO: return orderId that matches our request
    Task<int> ReturnOrderId(int idProduct, int amount);
    
//TODO:CreatedAt of the order should be lower than the CreatedAt in the request
    Task<bool> CreatedAtOfOrderIsLowerThanCreatedAtOfRequest(int orderId, DateTime requestDate);
    
//TODO:check if there is no row with the given IdOrder in the Product_Warehouse table.
    Task<bool> IdOrderDoesNotAlreadyExist(int idOrder);

//TODO:update the FullfilledAt column of the order with the current date and time.
    Task UpdateFulfilledAtOfOrder(int idOrder);

    Task<double> ReturnProductPrice(int productId);
    
//TODO:insert a record into the Product_Warehouse table.
    Task<int> InsertProduct_WarehouseRecord(int idWarehouse, int idProduct, int idOrder, int amount);
//The Price column should corresponds to the price of the product multiplied by amount value from our requestnsert the CreatedAt value according to the current time.
//TODO:return the value of the primary key generated for the record inserted into the Product_Warehouse table.
}