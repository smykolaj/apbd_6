using Microsoft.Data.SqlClient;
using WebApplication.Models.DTOs;

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

        await using var connection = new SqlConnection(_configuration.GetConnectionString("Docker"));
        await using var command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@ID", id);

        await connection.OpenAsync();

        var res = await command.ExecuteScalarAsync();

        if (res is not null)
            return true;
        throw new Exception("Product with the id you input does not exist");
    }

    public async Task<bool> WarehouseWithIdExists(int id)
    {
        var query = "SELECT 1 FROM Warehouse WHERE IdWarehouse = @ID";

        await using var connection = new SqlConnection(_configuration.GetConnectionString("Docker"));
        await using var command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@ID", id);

        await connection.OpenAsync();

        var res = await command.ExecuteScalarAsync();

        if (res is not null)
            return true;
        throw new Exception("Warehouse with the id you input does not exist");
    }

    public async Task<bool> IdProductAndAmountCombinationInOrderExists(int idProduct, int amount)
    {
        var query = "SELECT 1 FROM [Order] WHERE IdProduct = @IdProduct AND Amount = @Amount";

        await using var connection = new SqlConnection(_configuration.GetConnectionString("Docker"));
        await using var command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@IdProduct", idProduct);
        command.Parameters.AddWithValue("@Amount", amount);


        await connection.OpenAsync();

        var res = await command.ExecuteScalarAsync();

        if (res is not null)
            return true;
        throw new Exception("No record in Order table with such parameters exists");
    }

    public async Task<int> ReturnOrderId(int idProduct, int amount)
    {
        var query = "SELECT IdOrder FROM [Order] WHERE IdProduct = @IdProduct AND Amount = @Amount";

        await using var connection = new SqlConnection(_configuration.GetConnectionString("Docker"));
        await using var command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@IdProduct", idProduct);
        command.Parameters.AddWithValue("@Amount", amount);

        await connection.OpenAsync();
        var reader = await command.ExecuteReaderAsync();

        await reader.ReadAsync();

        if (!reader.HasRows) throw new Exception("No record in Order table with such parameters exists");
        var orderIdOrdinal = reader.GetOrdinal("IdOrder");
        var id = reader.GetInt32(orderIdOrdinal);

        return id;
    }

    public async Task<bool> CreatedAtOfOrderIsLowerThanCreatedAtOfRequest(int orderId, DateTime requestDate)
    {
        var query = "SELECT CreatedAt From [Order] WHERE IdOrder = @Id";
        await using var connection = new SqlConnection(_configuration.GetConnectionString("Docker"));
        await using var command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = query;
        command.Parameters.AddWithValue("@Id", orderId);

        await connection.OpenAsync();
        var reader = await command.ExecuteReaderAsync();

        await reader.ReadAsync();

        if (!reader.HasRows)
            throw new Exception("No order with such id exists");

        var createdAtOrdinal = reader.GetOrdinal("CreatedAt");
        var orderDate = reader.GetDateTime(createdAtOrdinal);

        if (orderDate.CompareTo(requestDate) < 0)
            return true;
        throw new Exception("Order has a newer date than your request");
    }

    public async Task<bool> IdOrderDoesNotAlreadyExist(int idOrder)
    {
        var query = "SELECT 1 FROM Product_Warehouse WHERE IdOrder = @IdOrder";

        await using var connection = new SqlConnection(_configuration.GetConnectionString("Docker"));
        await using var command = new SqlCommand();

        command.CommandText = query;
        command.Connection = connection;
        command.Parameters.AddWithValue("@IdOrder", idOrder);

        await connection.OpenAsync();

        var res = await command.ExecuteScalarAsync();

        if (res is null)
            return true;
        throw new Exception("There is already a record with such OrderId in the Product_Warehouse table");
    }

    public async Task UpdateFulfilledAtOfOrder(int idOrder)
    {
        var nonQuery = "UPDATE [Order] SET FulfilledAt = @CurrentDate WHERE IdOrder = @IdOrder";

        await using var connection = new SqlConnection(_configuration.GetConnectionString("Docker"));
        await using var command = new SqlCommand();

        command.CommandText = nonQuery;
        command.Connection = connection;
        command.Parameters.AddWithValue("@CurrentDate", DateTime.Now);
        command.Parameters.AddWithValue("@IdOrder", idOrder);

        await connection.OpenAsync();

        if (await command.ExecuteNonQueryAsync() < 1)
            throw new Exception("Could not update Order table");
    }

    public async Task<decimal> ReturnProductPrice(int productId)
    {
        var query = "SELECT Price FROM Product WHERE IdProduct = @productId";

        await using var connection = new SqlConnection(_configuration.GetConnectionString("Docker"));
        await using var command = new SqlCommand();

        command.CommandText = query;
        command.Connection = connection;
        command.Parameters.AddWithValue("@productId", productId);

        await connection.OpenAsync();

        var reader = await command.ExecuteReaderAsync();

        await reader.ReadAsync();

        if (!reader.HasRows)
            throw new Exception("Could not find price for this product");

        var priceOrdinal = reader.GetOrdinal("Price");

        var price = reader.GetDecimal(priceOrdinal);

        return price;
    }

    public async Task<int> InsertProduct_WarehouseRecord(DataToAccept dataToAccept, int idOrder)
    {
        var firstQuery = "INSERT INTO Product_Warehouse(IdWarehouse, IdProduct, IdOrder, Amount, Price, CreatedAt)" +
                         "VALUES (@IdWarehouse, @IdProduct, @IdOrder, @Amount, @Price, @CreatedAt )";

        await using var connection = new SqlConnection(_configuration.GetConnectionString("Docker"));
        await using var command = new SqlCommand();

        command.Connection = connection;
        command.CommandText = firstQuery;
        decimal totalPrice = dataToAccept.Amount * ReturnProductPrice(dataToAccept.IdProduct).Result;
        command.Parameters.AddWithValue("@IdWarehouse", dataToAccept.IdWarehouse);
        command.Parameters.AddWithValue("@IdProduct", dataToAccept.IdProduct);
        command.Parameters.AddWithValue("@IdOrder", idOrder);
        command.Parameters.AddWithValue("@Amount", dataToAccept.Amount);
        command.Parameters.AddWithValue("@Price", totalPrice);
        command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

        await connection.OpenAsync();
        if (await command.ExecuteNonQueryAsync() < 1)
            throw new Exception("Could not insert to the table");
        connection.Close();
        command.Cancel();


        await using var connection2 = new SqlConnection(_configuration.GetConnectionString("Docker"));
        await using var command2 = new SqlCommand();

        var secondQuery = "SELECT IdProductWarehouse" +
                          " From Product_Warehouse" +
                          " WHERE IdOrder = @IdOrder";
        command2.CommandText = secondQuery;
        command2.Connection = connection2;
        command2.Parameters.AddWithValue("@IdOrder", idOrder);

        await connection2.OpenAsync();

        var reader2 = await command2.ExecuteReaderAsync();
        if (!reader2.HasRows)
            throw new Exception("Could not retrieve new Id");

        var idOrdinal = reader2.GetOrdinal("IdProductWarehouse");
        await reader2.ReadAsync();
        var newId = reader2.GetInt32(idOrdinal);

        return newId;
    }

    public async Task<int> AddWithProcedures(DataToAccept dataToAccept)
    {
        var query = "EXEC AddProductToWarehouse @IdProduct, @IdWarehouse, @Amount, @CreatedAt ";

        await using var connection = new SqlConnection(_configuration.GetConnectionString("Docker"));
        await using var command = new SqlCommand();

        command.CommandText = query;
        command.Connection = connection;
        command.Parameters.AddWithValue("@IdProduct", dataToAccept.IdProduct);
        command.Parameters.AddWithValue("@IdWarehouse", dataToAccept.IdWarehouse);
        command.Parameters.AddWithValue("@Amount", dataToAccept.Amount);
        command.Parameters.AddWithValue("@CreatedAt", dataToAccept.Date);


        await connection.OpenAsync();
        var newId = await command.ExecuteScalarAsync();

        return Convert.ToInt32(newId);
    }
}