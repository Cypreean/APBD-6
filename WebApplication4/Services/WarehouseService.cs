using System.Data.SqlClient;
using WebApplication4.Model;

namespace WebApplication4.Services;

public class WarehouseService : IWarehouseService
{
    private async Task<SqlConnection> GetConnection()
    {
        var connection = new SqlConnection("Server=localhost;Database=master;User Id=SA;Password=yourStrong(!)Password;");
        if (connection.State != System.Data.ConnectionState.Open)
        {
            await connection.OpenAsync();
        }
        return connection;
        
    }

    public async Task<int> AddProduct(int idProduct, int idWarehouse, int ammount, string createdAt)
    {
        double price;
        using var connection = await GetConnection();
        using var command =
            new SqlCommand(
                "Select * from Product where IdProduct = @IdProduct",
                connection);
        command.Parameters.AddWithValue("@IdProduct", idProduct);
        using (var reader = await command.ExecuteReaderAsync())
        {
            if (!reader.HasRows)
            {
                return -1;
            }
            reader.Read();
            price = (double)reader.GetDecimal(3);   
        }
        using var command2 =
            new SqlCommand(
                "Select * from Warehouse where IdWarehouse = @IdWarehouse",
                connection);
        command2.Parameters.AddWithValue("@IdWarehouse", idWarehouse);
        using (var reader2 = await command2.ExecuteReaderAsync())
        {
            if (!reader2.HasRows)
            {
                return -1;
            }
        }
        using var command3 =
            new SqlCommand(
                "Select * from \"Order\" where IdProduct = @IdProduct and Amount = @Ammount ",
                connection);
        command3.Parameters.AddWithValue("@IdProduct", idProduct);
        command3.Parameters.AddWithValue("@Ammount", ammount);
        command3.Parameters.AddWithValue("@CreatedAt", createdAt);
        int OrderId = 0;
        using (var reader3 = await command3.ExecuteReaderAsync())
        {
            
            if (!reader3.HasRows)
            {
                return -1;
            }
            reader3.Read();
            OrderId = reader3.GetInt32(0);
            
            
        }

        if (OrderId != 0)
        {
            using var command4 =
                new SqlCommand(
                    "Select * from Product_Warehouse where IdOrder = @OrderId",
                    connection);
            command4.Parameters.AddWithValue("@OrderId", OrderId);
            using (var reader4 = await command4.ExecuteReaderAsync())
            {
                if (reader4.HasRows)
                {
                    return -1;
                }
            }
        }
       
        var date = DateTime.Now.ToString("yyyy-MM-dd");
        using var command5 =
            new SqlCommand(
                "Update \"Order\" set FulfilledAt = @NowDate where IdOrder = @OrderId",
                connection);
        command5.Parameters.AddWithValue("@NowDate", date);
        command5.Parameters.AddWithValue("@OrderId", OrderId);
        await command5.ExecuteNonQueryAsync();
        
        using var command6 =
            new SqlCommand(
                "Insert into Product_Warehouse (IdWarehouse,IdProduct,IdOrder,Amount,Price,CreatedAt) values (@IdWarehouse,@IdProduct,@IdOrder,@Amount,@Price,@CreatedAt)",
                connection);
        command6.Parameters.AddWithValue("@IdWarehouse", idWarehouse);
        command6.Parameters.AddWithValue("@IdProduct", idProduct);
        command6.Parameters.AddWithValue("@IdOrder", OrderId);
        command6.Parameters.AddWithValue("@Amount", ammount);
        command6.Parameters.AddWithValue("@Price", price*ammount); 
        command6.Parameters.AddWithValue("@CreatedAt", date);
        await command6.ExecuteNonQueryAsync();

        using var command7 =
            new SqlCommand(
                "Select * from Product_Warehouse where IdWarehouse = @IdWarehouse and IdProduct = @IdProduct and IdOrder = @IdOrder and Amount = @Amount and Price = @Price and CreatedAt = @CreatedAt",
                connection);
        command7.Parameters.AddWithValue("@IdWarehouse", idWarehouse);
        command7.Parameters.AddWithValue("@IdProduct", idProduct);
        command7.Parameters.AddWithValue("@IdOrder", OrderId);
        command7.Parameters.AddWithValue("@Amount", ammount);
        command7.Parameters.AddWithValue("@Price", price*ammount);
        command7.Parameters.AddWithValue("@CreatedAt", date);
        using (var reader7 = await command7.ExecuteReaderAsync())
        {
            if (!reader7.HasRows)
            {
                return -1;
            }
            reader7.Read();
            return reader7.GetInt32(0);
        }
        
    }
}