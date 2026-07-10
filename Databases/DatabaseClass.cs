using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Npgsql;
using Somethingnew.Models;
using System.Data;
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

        public string GenerateToken(Users user)
        {
            var claims = new[]
            {
        new Claim(ClaimTypes.Name, user.FullName),
        new Claim(ClaimTypes.Email, user.Email),
       new Claim(ClaimTypes.Role, user.Role.Substring(0,1).ToUpper() + user.Role.Substring(1).ToLower())
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

            string query = "SELECT * FROM RestaurantTables ORDER BY TableNumber";

            using var cmd = new NpgsqlCommand(query, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                tables.Add(new RestaurantTable
                {
                    TableId = Convert.ToInt32(reader["TableId"]),
                    TableNumber = Convert.ToInt32(reader["TableNumber"]),
                    Capacity = Convert.ToInt32(reader["Capacity"]),
                    Status = reader["Status"].ToString(),
                    CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
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
                    VALUES
                    (@TableNumber, @Capacity, @Status)";

            using var cmd = new NpgsqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@TableNumber", table.TableNumber);
            cmd.Parameters.AddWithValue("@Capacity", table.Capacity);
            cmd.Parameters.AddWithValue("@Status", table.Status);

            cmd.ExecuteNonQuery();

            return "Restaurant Table Added Successfully";
        }
        public string UpdateRestaurantTable(RestaurantTable table)
        {
            using var conn = GetConnection();
            conn.Open();

            string query = @"UPDATE RestaurantTables
                     SET
                        TableNumber=@TableNumber,
                        Capacity=@Capacity,
                        Status=@Status
                     WHERE TableId=@TableId";

            using var cmd = new NpgsqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@TableId", table.TableId);
            cmd.Parameters.AddWithValue("@TableNumber", table.TableNumber);
            cmd.Parameters.AddWithValue("@Capacity", table.Capacity);
            cmd.Parameters.AddWithValue("@Status", table.Status);

            cmd.ExecuteNonQuery();

            return "Restaurant Table Updated Successfully";
        }
        public string DeleteRestaurantTable(int tableId)
        {
            using var conn = GetConnection();
            conn.Open();

            string query = "DELETE FROM RestaurantTables WHERE TableId=@TableId";

            using var cmd = new NpgsqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@TableId", tableId);

            cmd.ExecuteNonQuery();

            return "Restaurant Table Deleted Successfully";
        }
        public string UpdateTableStatus(int tableId, string status)
        {
            using var conn = GetConnection();
            conn.Open();

            string query = @"UPDATE RestaurantTables
                     SET Status=@Status
                     WHERE TableId=@TableId";

            using var cmd = new NpgsqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@Status", status);
            cmd.Parameters.AddWithValue("@TableId", tableId);

            cmd.ExecuteNonQuery();

            return "Table Status Updated Successfully";
        }

        //The RestaurantTables related query ends here 


        //menu items related query starts here

        public List<MenuItem> GetMenuItems()
        {
            List<MenuItem> items = new List<MenuItem>();

            using var conn = GetConnection();
            conn.Open();

            string query = @"SELECT *
                     FROM MenuItems
                     ORDER BY ItemName";

            using var cmd = new NpgsqlCommand(query, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                items.Add(new MenuItem
                {
                    MenuItemId = Convert.ToInt32(reader["MenuItemId"]),
                    CategoryId = Convert.ToInt32(reader["CategoryId"]),
                    ItemName = reader["ItemName"].ToString(),
                    Description = reader["Description"].ToString(),
                    Price = Convert.ToDecimal(reader["Price"]),
                    ImageUrl = reader["ImageUrl"].ToString(),
                    IsAvailable = Convert.ToBoolean(reader["IsAvailable"]),
                    CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                });
            }

            return items;
        }
        public string AddMenuItem(MenuItem item)
        {
            using var conn = GetConnection();
            conn.Open();

            string query = @"INSERT INTO MenuItems
                    (CategoryId,ItemName,Description,Price,ImageUrl)

                    VALUES

                    (@CategoryId,@ItemName,@Description,@Price,@ImageUrl)";

            using var cmd = new NpgsqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@CategoryId", item.CategoryId);
            cmd.Parameters.AddWithValue("@ItemName", item.ItemName);
            cmd.Parameters.AddWithValue("@Description", item.Description);
            cmd.Parameters.AddWithValue("@Price", item.Price);
            cmd.Parameters.AddWithValue("@ImageUrl", (object?)item.ImageUrl ?? DBNull.Value);

            cmd.ExecuteNonQuery();

            return "Menu Item Added Successfully";
        }
        public string UpdateMenuItem(MenuItem item)
        {
            using var conn = GetConnection();
            conn.Open();

            string query = @"UPDATE MenuItems

                     SET

                     CategoryId=@CategoryId,

                     ItemName=@ItemName,

                     Description=@Description,

                     Price=@Price,

                     ImageUrl=@ImageUrl,

                     IsAvailable=@IsAvailable

                     WHERE MenuItemId=@MenuItemId";

            using var cmd = new NpgsqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@MenuItemId", item.MenuItemId);
            cmd.Parameters.AddWithValue("@CategoryId", item.CategoryId);
            cmd.Parameters.AddWithValue("@ItemName", item.ItemName);
            cmd.Parameters.AddWithValue("@Description", item.Description);
            cmd.Parameters.AddWithValue("@Price", item.Price);
            cmd.Parameters.AddWithValue("@ImageUrl", (object?)item.ImageUrl ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@IsAvailable", item.IsAvailable);

            cmd.ExecuteNonQuery();

            return "Menu Item Updated Successfully";
        }
        public string DeleteMenuItem(int menuItemId)
        {
            using var conn = GetConnection();
            conn.Open();

            string query = "DELETE FROM MenuItems WHERE MenuItemId=@MenuItemId";

            using var cmd = new NpgsqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@MenuItemId", menuItemId);

            cmd.ExecuteNonQuery();

            return "Menu Item Deleted Successfully";
        }
        public string UpdateMenuAvailability(int menuItemId, bool isAvailable)
        {
            using var conn = GetConnection();
            conn.Open();

            string query = @"UPDATE MenuItems
                     SET IsAvailable=@IsAvailable
                     WHERE MenuItemId=@MenuItemId";

            using var cmd = new NpgsqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@MenuItemId", menuItemId);
            cmd.Parameters.AddWithValue("@IsAvailable", isAvailable);

            cmd.ExecuteNonQuery();

            return "Menu Availability Updated Successfully";
        }

        //menu items get information query

        //order related query starts here

        public CreateOrderResponse CreateOrder(CreateOrderRequest request)
        {
            using var conn = GetConnection();
            conn.Open();

            using var transaction = conn.BeginTransaction();

            try
            {
                decimal totalAmount = 0;

                // Create Order
                string orderQuery = @"
            INSERT INTO Orders
            (TableId, WaiterId, OrderStatus, TotalAmount)
            VALUES
            (@TableId, @UserId, 'Pending', 0)
            RETURNING OrderId";

                int orderId;

                using (var cmd = new NpgsqlCommand(orderQuery, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@TableId", request.TableId);
                    cmd.Parameters.AddWithValue("@UserId", request.UserId);

                    orderId = Convert.ToInt32(cmd.ExecuteScalar());
                }

                // Insert Order Items
                foreach (var item in request.Items)
                {
                    decimal price = 0;

                    string priceQuery = @"
                SELECT Price
                FROM MenuItems
                WHERE MenuItemId=@MenuItemId";

                    using (var cmd = new NpgsqlCommand(priceQuery, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@MenuItemId", item.MenuItemId);

                        price = Convert.ToDecimal(cmd.ExecuteScalar());
                    }

                    string itemQuery = @"
                INSERT INTO OrderItems
                (OrderId, MenuItemId, Quantity, Price)

                VALUES

                (@OrderId,@MenuItemId,@Quantity,@Price)";

                    using (var cmd = new NpgsqlCommand(itemQuery, conn, transaction))
                    {
                        cmd.Parameters.AddWithValue("@OrderId", orderId);
                        cmd.Parameters.AddWithValue("@MenuItemId", item.MenuItemId);
                        cmd.Parameters.AddWithValue("@Quantity", item.Quantity);
                        cmd.Parameters.AddWithValue("@Price", price);

                        cmd.ExecuteNonQuery();
                    }

                    totalAmount += price * item.Quantity;
                }

                // Update Total Amount
                string updateQuery = @"
            UPDATE Orders
            SET TotalAmount=@TotalAmount
            WHERE OrderId=@OrderId";

                using (var cmd = new NpgsqlCommand(updateQuery, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@OrderId", orderId);
                    cmd.Parameters.AddWithValue("@TotalAmount", totalAmount);

                    cmd.ExecuteNonQuery();
                }

                // Update Table Status
                string tableQuery = @"
            UPDATE RestaurantTables
            SET Status='Occupied'
            WHERE TableId=@TableId";

                using (var cmd = new NpgsqlCommand(tableQuery, conn, transaction))
                {
                    cmd.Parameters.AddWithValue("@TableId", request.TableId);

                    cmd.ExecuteNonQuery();
                }

                transaction.Commit();

                return new CreateOrderResponse
                {
                    OrderId = orderId,
                    TotalAmount = totalAmount
                };
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception(ex.Message);
            }
        }

        public List<Order> GetOrders()
        {
            List<Order> orders = new List<Order>();

            using var conn = GetConnection();
            conn.Open();

            string query = @"SELECT *
                     FROM Orders
                     ORDER BY CreatedAt DESC";

            using var cmd = new NpgsqlCommand(query, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                orders.Add(new Order
                {
                    OrderId = Convert.ToInt32(reader["OrderId"]),
                    TableId = Convert.ToInt32(reader["TableId"]),
                    WaiterId = Convert.ToInt32(reader["WaiterId"]),
                    OrderStatus = reader["OrderStatus"].ToString(),
                    TotalAmount = Convert.ToDecimal(reader["TotalAmount"]),
                    CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                });
            }

            return orders;
        }
        public List<Order> GetOrdersByWaiter(int waiterId)
        {
            List<Order> orders = new List<Order>();

            using var conn = GetConnection();
            conn.Open();

            string query = @"SELECT *
                     FROM Orders
                     WHERE WaiterId=@WaiterId
                     ORDER BY CreatedAt DESC";

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
                    TotalAmount = Convert.ToDecimal(reader["TotalAmount"]),
                    CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                });
            }

            return orders;
        }
        public List<OrderItem> GetOrderItems()
        {
            List<OrderItem> items = new List<OrderItem>();

            using var conn = GetConnection();
            conn.Open();

            string query = @"SELECT *
                     FROM OrderItems
                     ORDER BY OrderId";

            using var cmd = new NpgsqlCommand(query, conn);

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                items.Add(new OrderItem
                {
                    OrderItemId = Convert.ToInt32(reader["OrderItemId"]),
                    OrderId = Convert.ToInt32(reader["OrderId"]),
                    MenuItemId = Convert.ToInt32(reader["MenuItemId"]),
                    Quantity = Convert.ToInt32(reader["Quantity"]),
                    Price = Convert.ToDecimal(reader["Price"]),
                    ItemStatus = reader["ItemStatus"].ToString()
                });
            }

            return items;
        }
        public List<OrderItem> GetOrderItemsByOrder(int orderId)
        {
            List<OrderItem> items = new List<OrderItem>();

            using var conn = GetConnection();
            conn.Open();

            string query = @"SELECT *
                     FROM OrderItems
                     WHERE OrderId=@OrderId";

            using var cmd = new NpgsqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@OrderId", orderId);

            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                items.Add(new OrderItem
                {
                    OrderItemId = Convert.ToInt32(reader["OrderItemId"]),
                    OrderId = Convert.ToInt32(reader["OrderId"]),
                    MenuItemId = Convert.ToInt32(reader["MenuItemId"]),
                    Quantity = Convert.ToInt32(reader["Quantity"]),
                    Price = Convert.ToDecimal(reader["Price"]),
                    ItemStatus = reader["ItemStatus"].ToString()
                });
            }

            return items;
        }
        public string UpdateOrderStatus(int orderId, string status)
        {
            using var conn = GetConnection();
            conn.Open();

            string query = @"UPDATE Orders
                     SET OrderStatus=@OrderStatus
                     WHERE OrderId=@OrderId";

            using var cmd = new NpgsqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@OrderId", orderId);
            cmd.Parameters.AddWithValue("@OrderStatus", status);

            cmd.ExecuteNonQuery();

            return "Order Status Updated Successfully";
        }
        public string UpdateOrderItemStatus(int orderItemId, string status)
        {
            using var conn = GetConnection();
            conn.Open();

            string query = @"UPDATE OrderItems
                     SET ItemStatus=@ItemStatus
                     WHERE OrderItemId=@OrderItemId";

            using var cmd = new NpgsqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@OrderItemId", orderItemId);
            cmd.Parameters.AddWithValue("@ItemStatus", status);

            cmd.ExecuteNonQuery();

            return "Order Item Status Updated Successfully";
        }
        public string DeleteOrder(int orderId)
        {
            using var conn = GetConnection();
            conn.Open();

            string query = @"DELETE FROM Orders
                     WHERE OrderId=@OrderId";

            using var cmd = new NpgsqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@OrderId", orderId);

            cmd.ExecuteNonQuery();

            return "Order Deleted Successfully";
        }

        //order related query ends here

        //category related query starts here

        public List<Category> GetCategories()
        {
            List<Category> categories = new List<Category>();

            using var conn = GetConnection();
            conn.Open();

            string query = "SELECT * FROM Categories ORDER BY CategoryName";

            using var cmd = new NpgsqlCommand(query, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                categories.Add(new Category
                {
                    CategoryId = Convert.ToInt32(reader["CategoryId"]),
                    CategoryName = reader["CategoryName"].ToString(),
                    IsActive = Convert.ToBoolean(reader["IsActive"]),
                    CreatedAt = Convert.ToDateTime(reader["CreatedAt"])
                });
            }

            return categories;
        }
        public string AddCategory(Category category)
        {
            using var conn = GetConnection();
            conn.Open();

            string query = @"INSERT INTO Categories
                    (CategoryName)
                    VALUES
                    (@CategoryName)";

            using var cmd = new NpgsqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@CategoryName", category.CategoryName);

            cmd.ExecuteNonQuery();

            return "Category Added Successfully";
        }

        public string UpdateCategory(Category category)
        {
            using var conn = GetConnection();
            conn.Open();

            string query = @"UPDATE Categories
                     SET
                        CategoryName=@CategoryName,
                        IsActive=@IsActive
                     WHERE CategoryId=@CategoryId";

            using var cmd = new NpgsqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@CategoryId", category.CategoryId);
            cmd.Parameters.AddWithValue("@CategoryName", category.CategoryName);
            cmd.Parameters.AddWithValue("@IsActive", category.IsActive);

            cmd.ExecuteNonQuery();

            return "Category Updated Successfully";
        }

        public string DeleteCategory(int categoryId)
        {
            using var conn = GetConnection();
            conn.Open();

            string query = "DELETE FROM Categories WHERE CategoryId=@CategoryId";

            using var cmd = new NpgsqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@CategoryId", categoryId);

            cmd.ExecuteNonQuery();

            return "Category Deleted Successfully";
        }
        public string UpdateCategoryStatus(int categoryId, bool isActive)
        {
            using var conn = GetConnection();
            conn.Open();

            string query = @"UPDATE Categories
                     SET IsActive=@IsActive
                     WHERE CategoryId=@CategoryId";

            using var cmd = new NpgsqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@CategoryId", categoryId);
            cmd.Parameters.AddWithValue("@IsActive", isActive);

            cmd.ExecuteNonQuery();

            return "Category Status Updated Successfully";
        }
        //category related query ends here


        //payment related query starts here


        public string AddPayment(Payment payment)
        {
            using var conn = GetConnection();
            conn.Open();

            string query = @"INSERT INTO Payments
                    (OrderId,Amount,PaymentMethod,PaymentStatus,PaidAt)

                    VALUES

                    (@OrderId,@Amount,@PaymentMethod,@PaymentStatus,@PaidAt)";

            using var cmd = new NpgsqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@OrderId", payment.OrderId);
            cmd.Parameters.AddWithValue("@Amount", payment.Amount);
            cmd.Parameters.AddWithValue("@PaymentMethod", payment.PaymentMethod);
            cmd.Parameters.AddWithValue("@PaymentStatus", payment.PaymentStatus);
            cmd.Parameters.AddWithValue("@PaidAt",
                payment.PaidAt.HasValue ? payment.PaidAt.Value : DBNull.Value);

            cmd.ExecuteNonQuery();

            return "Payment Added Successfully";
        }
        public List<Payment> GetPayments()
        {
            List<Payment> payments = new List<Payment>();

            using var conn = GetConnection();
            conn.Open();

            string query = @"SELECT *
                     FROM Payments
                     ORDER BY PaymentId DESC";

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
                    PaidAt = reader["PaidAt"] == DBNull.Value
                        ? null
                        : Convert.ToDateTime(reader["PaidAt"])
                });
            }

            return payments;
        }
        public Payment? GetPaymentByOrder(int orderId)
        {
            using var conn = GetConnection();
            conn.Open();

            string query = @"SELECT *
                     FROM Payments
                     WHERE OrderId=@OrderId";

            using var cmd = new NpgsqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@OrderId", orderId);

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
        public string UpdatePayment(Payment payment)
        {
            using var conn = GetConnection();
            conn.Open();

            string query = @"UPDATE Payments

                     SET

                     Amount=@Amount,

                     PaymentMethod=@PaymentMethod,

                     PaymentStatus=@PaymentStatus,

                     PaidAt=@PaidAt

                     WHERE PaymentId=@PaymentId";

            using var cmd = new NpgsqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@PaymentId", payment.PaymentId);
            cmd.Parameters.AddWithValue("@Amount", payment.Amount);
            cmd.Parameters.AddWithValue("@PaymentMethod", payment.PaymentMethod);
            cmd.Parameters.AddWithValue("@PaymentStatus", payment.PaymentStatus);
            cmd.Parameters.AddWithValue("@PaidAt",
                payment.PaidAt.HasValue ? payment.PaidAt.Value : DBNull.Value);

            cmd.ExecuteNonQuery();

            return "Payment Updated Successfully";
        }
        public string DeletePayment(int paymentId)
        {
            using var conn = GetConnection();
            conn.Open();

            string query = @"DELETE FROM Payments
                     WHERE PaymentId=@PaymentId";

            using var cmd = new NpgsqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@PaymentId", paymentId);

            cmd.ExecuteNonQuery();

            return "Payment Deleted Successfully";
        }
        public DataTable GetPaymentsByUser(int userId)
        {
            using var conn = GetConnection();
            conn.Open();

            string query = @"
        SELECT
            p.paymentid,
            p.orderid,
            p.amount,
            p.paymentmethod,
            p.status,
            p.paymentdate
        FROM Payments p
        INNER JOIN Orders o
            ON p.orderid = o.orderid
        WHERE o.userid = @UserId
        ORDER BY p.paymentid DESC";

            using var cmd = new NpgsqlCommand(query, conn);

            cmd.Parameters.AddWithValue("@UserId", userId);

            using var da = new NpgsqlDataAdapter(cmd);

            DataTable dt = new DataTable();

            da.Fill(dt);

            return dt;
        }
        //payment related query ends here
    }
}
