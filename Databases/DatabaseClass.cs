using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using Somethingnew.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Somethingnew.Databases
{
    public class DatabaseClass
    {
        private readonly IConfiguration _configuration;

        public DatabaseClass(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public NpgsqlConnection GetConnection()
        {
            return new NpgsqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));
        }


        //Users Login and Registration related query starts here

        //user get information query 
        public List<Users> GetUsers()
        {
            List<Users> users = new List<Users>();

            using var conn = GetConnection();
            conn.Open();

            string query = "SELECT * FROM Users";

            using var cmd = new NpgsqlCommand(query, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                users.Add(new Users
                {
                    UserId = Convert.ToInt32(reader["UserId"]),
                    FullName = reader["FullName"].ToString(),
                    Email = reader["Email"].ToString(),
                    PasswordHash = reader["PasswordHash"].ToString(),
                    Role = reader["Role"].ToString(),
                    IsActive = Convert.ToBoolean(reader["IsActive"]),
                    CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                });
            }

            return users;
        }


        //delete user query to delete user from the database
        public string DeleteUser(int userId)
        {
            using var conn = GetConnection();
            conn.Open();

            string query = "DELETE FROM Users WHERE UserId = @UserId";

            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@UserId", userId);

            int rows = cmd.ExecuteNonQuery();

            return rows > 0 ? "User deleted successfully" : "Delete failed";
        }

        //jwt token generation query for user authentication
        public Users ValidateUser(string email, string password)
        {
            using var conn = GetConnection();
            conn.Open();

            string query = "SELECT * FROM Users WHERE Email=@Email AND PasswordHash=@Password";

            using var cmd = new NpgsqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@Email", email);
            cmd.Parameters.AddWithValue("@Password", password);

            using var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return new Users
                {
                    UserId = Convert.ToInt32(reader["UserId"]),
                    FullName = reader["FullName"].ToString(),
                    Email = reader["Email"].ToString(),
                    Role = reader["Role"].ToString()
                };
            }

            return null;
        }

        //user information new user registration query to add new user to the database
        public bool AddUser(Users user)
        {
            using var conn = GetConnection();
            conn.Open();

            string query = @"INSERT INTO Users 
                    (FullName, Email, PasswordHash, Role, IsActive, CreatedAt)
                    VALUES
                    (@FullName, @Email, @PasswordHash, @Role, @IsActive, @CreatedAt)";

            using var cmd = new NpgsqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@FullName", user.FullName);
            cmd.Parameters.AddWithValue("@Email", user.Email);
            cmd.Parameters.AddWithValue("@PasswordHash", user.PasswordHash);
            cmd.Parameters.AddWithValue("@Role", user.Role);
            cmd.Parameters.AddWithValue("@IsActive", user.IsActive);
            cmd.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

            int rows = cmd.ExecuteNonQuery();

            return rows > 0;
        }

        //Users Login and Registration related query ends here

        // Generate JWT Token query for user authentication
        public string GenerateToken(Users user)
        {
            var claims = new[]
            {
        new Claim(ClaimTypes.Name, user.FullName),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Role, user.Role)
    };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var creds = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        //ends the jwt token generation query

        //restaurant table deatils 
        public List<RestaurantTable> GetRestaurantTables()
        {
            List<RestaurantTable> tables = new List<RestaurantTable>();

            using var conn = GetConnection();
            conn.Open();

            string query = "SELECT * FROM RestaurantTables";

            using var cmd = new NpgsqlCommand(query, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                tables.Add(new RestaurantTable
                {
                    TableId = Convert.ToInt32(reader["TableId"]),
                    TableNumber = Convert.ToInt32(reader["TableNumber"]),
                    Capacity = Convert.ToInt32(reader["Capacity"]),
                    Status = reader["Status"].ToString()
                });
            }

            return tables;
        }
        public string AddRestaurantTable(RestaurantTable table)
        {
            using var conn = GetConnection();
            conn.Open();

            string query = @"INSERT INTO RestaurantTables 
                     (TableNumber, Capacity, Status) 
                     VALUES (@TableNumber, @Capacity, @Status)";

            using var cmd = new NpgsqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@TableNumber", table.TableNumber);
            cmd.Parameters.AddWithValue("@Capacity", table.Capacity);
            cmd.Parameters.AddWithValue("@Status", table.Status);

            int rows = cmd.ExecuteNonQuery();

            return rows > 0 ? "Table added successfully" : "Failed to add table";
        }


        public string UpdateTableStatus(int tableId, string status)
        {
            using var conn = GetConnection();
            conn.Open();

            string query = @"UPDATE RestaurantTables 
                     SET Status = @Status
                     WHERE TableId = @TableId";

            using var cmd = new NpgsqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@Status", status);
            cmd.Parameters.AddWithValue("@TableId", tableId);

            int rows = cmd.ExecuteNonQuery();

            return rows > 0 ? "Table status updated successfully" : "Update failed";
        }

        public string DeleteRestaurantTable(int tableId)
        {
            using var conn = GetConnection();
            conn.Open();

            string query = @"DELETE FROM RestaurantTables 
                     WHERE TableId = @TableId";

            using var cmd = new NpgsqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@TableId", tableId);

            int rows = cmd.ExecuteNonQuery();

            return rows > 0 ? "Table deleted successfully" : "Delete failed";
        }
        //The RestaurantTables related query ends here 


        //menu items related query starts here

        public List<MenuItem> GetMenuItems()
        {
            List<MenuItem> menuItems = new List<MenuItem>();

            using var conn = GetConnection();
            conn.Open();

            string query = "SELECT * FROM MenuItems";

            using var cmd = new NpgsqlCommand(query, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                menuItems.Add(new MenuItem
                {
                    MenuItemId = Convert.ToInt32(reader["MenuItemId"]),
                    CategoryId = Convert.ToInt32(reader["CategoryId"]),
                    ItemName = reader["ItemName"].ToString(),
                    Price = Convert.ToDecimal(reader["Price"]),
                    IsAvailable = Convert.ToBoolean(reader["IsAvailable"])
                });
            }

            return menuItems;
        }
        public string AddMenuItem(MenuItem item)
        {
            using var conn = GetConnection();
            conn.Open();

            string query = @"INSERT INTO MenuItems
                    (CategoryId, ItemName, Price, IsAvailable)
                    VALUES
                    (@CategoryId, @ItemName, @Price, @IsAvailable)";

            using var cmd = new NpgsqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@CategoryId", item.CategoryId);
            cmd.Parameters.AddWithValue("@ItemName", item.ItemName);
            cmd.Parameters.AddWithValue("@Price", item.Price);
            cmd.Parameters.AddWithValue("@IsAvailable", item.IsAvailable);

            int rows = cmd.ExecuteNonQuery();

            return rows > 0 ? "Menu item added successfully" : "Failed to add menu item";
        }

        public string UpdateMenuItem(MenuItem item)
        {
            using var conn = GetConnection();
            conn.Open();

            string query = @"UPDATE MenuItems
                     SET CategoryId = @CategoryId,
                         ItemName = @ItemName,
                         Price = @Price,
                         IsAvailable = @IsAvailable
                     WHERE MenuItemId = @MenuItemId";

            using var cmd = new NpgsqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@MenuItemId", item.MenuItemId);
            cmd.Parameters.AddWithValue("@CategoryId", item.CategoryId);
            cmd.Parameters.AddWithValue("@ItemName", item.ItemName);
            cmd.Parameters.AddWithValue("@Price", item.Price);
            cmd.Parameters.AddWithValue("@IsAvailable", item.IsAvailable);

            int rows = cmd.ExecuteNonQuery();

            return rows > 0 ? "Menu item updated successfully" : "Update failed";
        }

        public string DeleteMenuItem(int menuItemId)
        {
            using var conn = GetConnection();
            conn.Open();

            string query = @"DELETE FROM MenuItems 
                     WHERE MenuItemId = @MenuItemId";

            using var cmd = new NpgsqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@MenuItemId", menuItemId);

            int rows = cmd.ExecuteNonQuery();

            return rows > 0 ? "Menu item deleted successfully" : "Delete failed";
        }
        //menu items get information query

        //order related query starts here
        public string AddOrder(Order order)
        {
            using var conn = GetConnection();
            conn.Open();

            string query = @"INSERT INTO Orders
                    (TableId, WaiterId, OrderStatus, CreatedAt)
                    VALUES
                    (@TableId, @WaiterId, @OrderStatus, @CreatedAt)";

            using var cmd = new NpgsqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@TableId", order.TableId);
            cmd.Parameters.AddWithValue("@WaiterId", order.WaiterId);
            cmd.Parameters.AddWithValue("@OrderStatus", order.OrderStatus);
            cmd.Parameters.AddWithValue("@CreatedAt", order.CreatedAt);

            int rows = cmd.ExecuteNonQuery();

            return rows > 0 ? "Order added successfully" : "Failed to add order";
        }
        public string UpdateOrder(Order order)
        {
            using var conn = GetConnection();
            conn.Open();

            string query = @"UPDATE Orders
                     SET TableId = @TableId,
                         WaiterId = @WaiterId,
                         OrderStatus = @OrderStatus
                     WHERE OrderId = @OrderId";

            using var cmd = new NpgsqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@OrderId", order.OrderId);
            cmd.Parameters.AddWithValue("@TableId", order.TableId);
            cmd.Parameters.AddWithValue("@WaiterId", order.WaiterId);
            cmd.Parameters.AddWithValue("@OrderStatus", order.OrderStatus);

            int rows = cmd.ExecuteNonQuery();

            return rows > 0 ? "Order updated successfully" : "Update failed";
        }

        public string DeleteOrder(int orderId)
        {
            using var conn = GetConnection();
            conn.Open();

            string query = "DELETE FROM Orders WHERE OrderId = @OrderId";

            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@OrderId", orderId);

            int rows = cmd.ExecuteNonQuery();

            return rows > 0 ? "Order deleted successfully" : "Delete failed";
        }

        public List<Order> GetOrdersByWaiterId(int waiterId)
        {
            List<Order> orders = new List<Order>();

            using var conn = GetConnection();
            conn.Open();

            string query = "SELECT * FROM Orders WHERE WaiterId = @WaiterId";

            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@WaiterId", waiterId);

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                orders.Add(new Order
                {
                    OrderId = Convert.ToInt32(reader["OrderId"]),
                    TableId = Convert.ToInt32(reader["TableId"]),
                    WaiterId = Convert.ToInt32(reader["WaiterId"]),
                    OrderStatus = reader["OrderStatus"].ToString(),
                    CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                });
            }

            return orders;
        }
        //order related query ends here

        //category related query starts here
        public string AddCategory(Category category)
        {
            using var conn = GetConnection();
            conn.Open();

            string query = "INSERT INTO Categories (CategoryName) VALUES (@CategoryName)";

            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@CategoryName", category.CategoryName);

            int rows = cmd.ExecuteNonQuery();

            return rows > 0 ? "Category added successfully" : "Failed to add category";
        }
        public List<Category> GetCategories()
        {
            List<Category> categories = new List<Category>();

            using var conn = GetConnection();
            conn.Open();

            string query = "SELECT * FROM Categories";

            using var cmd = new NpgsqlCommand(query, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                categories.Add(new Category
                {
                    CategoryId = Convert.ToInt32(reader["CategoryId"]),
                    CategoryName = reader["CategoryName"].ToString()
                });
            }

            return categories;
        }

        public string UpdateCategory(Category category)
        {
            using var conn = GetConnection();
            conn.Open();

            string query = @"UPDATE Categories
                     SET CategoryName = @CategoryName
                     WHERE CategoryId = @CategoryId";

            using var cmd = new NpgsqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@CategoryId", category.CategoryId);
            cmd.Parameters.AddWithValue("@CategoryName", category.CategoryName);

            int rows = cmd.ExecuteNonQuery();

            return rows > 0 ? "Category updated successfully" : "Update failed";
        }
        public string DeleteCategory(int categoryId)
        {
            using var conn = GetConnection();
            conn.Open();

            string query = "DELETE FROM Categories WHERE CategoryId = @CategoryId";

            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@CategoryId", categoryId);

            int rows = cmd.ExecuteNonQuery();

            return rows > 0 ? "Category deleted successfully" : "Delete failed";
        }

        //category related query ends here


        //payment related query starts here
        public string AddPayment(Payment payment)
        {
            using var conn = GetConnection();
            conn.Open();

            string query = @"INSERT INTO Payments
                    (OrderId, Amount, PaymentMethod, PaymentStatus, PaidAt)
                    VALUES
                    (@OrderId, @Amount, @PaymentMethod, @PaymentStatus, @PaidAt)";

            using var cmd = new NpgsqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@OrderId", payment.OrderId);
            cmd.Parameters.AddWithValue("@Amount", payment.Amount);
            cmd.Parameters.AddWithValue("@PaymentMethod", payment.PaymentMethod);
            cmd.Parameters.AddWithValue("@PaymentStatus", payment.PaymentStatus);
            cmd.Parameters.AddWithValue("@PaidAt", payment.PaidAt ?? (object)DBNull.Value);

            int rows = cmd.ExecuteNonQuery();

            return rows > 0 ? "Payment added successfully" : "Failed to add payment";
        }
        public List<Payment> GetPayments()
        {
            List<Payment> payments = new List<Payment>();

            using var conn = GetConnection();
            conn.Open();

            string query = "SELECT * FROM Payments";

            using var cmd = new NpgsqlCommand(query, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                payments.Add(new Payment
                {
                    PaymentId = Convert.ToInt32(reader["PaymentId"]),
                    OrderId = Convert.ToInt32(reader["OrderId"]),
                    Amount = Convert.ToDecimal(reader["Amount"]),
                    PaymentMethod = reader["PaymentMethod"].ToString(),
                    PaymentStatus = reader["PaymentStatus"].ToString(),
                    PaidAt = reader["PaidAt"] == DBNull.Value ? null : Convert.ToDateTime(reader["PaidAt"])
                });
            }

            return payments;
        }
        public string UpdatePayment(Payment payment)
        {
            using var conn = GetConnection();
            conn.Open();

            string query = @"UPDATE Payments
                     SET Amount = @Amount,
                         PaymentMethod = @PaymentMethod,
                         PaymentStatus = @PaymentStatus,
                         PaidAt = @PaidAt
                     WHERE PaymentId = @PaymentId";

            using var cmd = new NpgsqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@PaymentId", payment.PaymentId);
            cmd.Parameters.AddWithValue("@Amount", payment.Amount);
            cmd.Parameters.AddWithValue("@PaymentMethod", payment.PaymentMethod);
            cmd.Parameters.AddWithValue("@PaymentStatus", payment.PaymentStatus);
            cmd.Parameters.AddWithValue("@PaidAt", payment.PaidAt ?? (object)DBNull.Value);

            int rows = cmd.ExecuteNonQuery();

            return rows > 0 ? "Payment updated successfully" : "Update failed";
        }
        public string DeletePayment(int paymentId)
        {
            using var conn = GetConnection();
            conn.Open();

            string query = "DELETE FROM Payments WHERE PaymentId = @PaymentId";

            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@PaymentId", paymentId);

            int rows = cmd.ExecuteNonQuery();

            return rows > 0 ? "Payment deleted successfully" : "Delete failed";
        }

        public List<Payment> GetPaymentsByWaiterId(int waiterId)
        {
            List<Payment> payments = new List<Payment>();

            using var conn = GetConnection();
            conn.Open();

            string query = @"
        SELECT p.*
        FROM Payments p
        INNER JOIN Orders o ON p.OrderId = o.OrderId
        WHERE o.WaiterId = @WaiterId";

            using var cmd = new NpgsqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@WaiterId", waiterId);

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                payments.Add(new Payment
                {
                    PaymentId = Convert.ToInt32(reader["PaymentId"]),
                    OrderId = Convert.ToInt32(reader["OrderId"]),
                    Amount = Convert.ToDecimal(reader["Amount"]),
                    PaymentMethod = reader["PaymentMethod"].ToString(),
                    PaymentStatus = reader["PaymentStatus"].ToString(),
                    PaidAt = reader["PaidAt"] == DBNull.Value
                                ? null
                                : Convert.ToDateTime(reader["PaidAt"])
                });
            }

            return payments;
        }

        public Payment GetPaymentById(int waiterId, int paymentId)
        {
            using var conn = GetConnection();
            conn.Open();

            string query = @"
        SELECT p.*
        FROM Payments p
        INNER JOIN Orders o ON p.OrderId = o.OrderId
        WHERE o.WaiterId = @WaiterId
        AND p.PaymentId = @PaymentId";

            using var cmd = new NpgsqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@WaiterId", waiterId);
            cmd.Parameters.AddWithValue("@PaymentId", paymentId);

            using var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return new Payment
                {
                    PaymentId = Convert.ToInt32(reader["PaymentId"]),
                    OrderId = Convert.ToInt32(reader["OrderId"]),
                    Amount = Convert.ToDecimal(reader["Amount"]),
                    PaymentMethod = reader["PaymentMethod"].ToString(),
                    PaymentStatus = reader["PaymentStatus"].ToString(),
                    PaidAt = reader["PaidAt"] == DBNull.Value
                                ? null
                                : Convert.ToDateTime(reader["PaidAt"])
                };
            }

            return null;
        }
        //payment related query ends here
    }
}
