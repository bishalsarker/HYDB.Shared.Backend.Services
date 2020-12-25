using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Dapper;
using HYDB.Services.Models;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace HYDB.Services.Repositories
{
    public class UserAccounts : BaseRepository
    {
        public UserAccounts(IConfiguration config) : base(config) {}

        public IEnumerable<UserAccount> GetByUserNamePassword(string userName, string password)
        {
            string sql = "select * from UserAccounts where UserName=@username and Password=@password";
            using (IDbConnection conn = new SqlConnection(GetConnectionString()))
            {
                return conn.Query<UserAccount>(sql, new { username = userName, password = password });
            }
        }

        public IEnumerable<UserAccount> GetByUsername(string userName)
        {
            string sql = "select * from UserAccounts where UserName=@username";
            using (IDbConnection conn = new SqlConnection(GetConnectionString()))
            {
                return conn.Query<UserAccount>(sql, new { username = userName });
            }
        }

        public IEnumerable<UserAccount> GetById(string userId)
        {
            string sql = "select * from UserAccounts where Ide=@id";
            using (IDbConnection conn = new SqlConnection(GetConnectionString()))
            {
                return conn.Query<UserAccount>(sql, new { @id = userId });
            }
        }

        public void AddUser(UserAccount userAccount)
        {
            string sql = "insert into UserAccounts(Id, UserName, Email, Password, IsEmailVerified, IsActive, CreationDate) " +
                         "values (@id, @username, @email, @password, @isemailverified, @isactive, @creationdate)";
            using (IDbConnection conn = new SqlConnection(GetConnectionString()))
            {
                conn.Execute(sql, new
                {
                    id = userAccount.Id,
                    username = userAccount.UserName,
                    email = userAccount.Email,
                    password = userAccount.Password,
                    isemailverified = userAccount.IsEmailVerified,
                    isactive = userAccount.IsActive,
                    creationdate = userAccount.CreationDate
                });
            }
        }
    }
}
