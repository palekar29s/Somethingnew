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
        // Validate User Login
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

        // Generate JWT Token
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

       

    }
}
