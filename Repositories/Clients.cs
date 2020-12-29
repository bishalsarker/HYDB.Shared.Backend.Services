using Dapper;
using HYDB.Services.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace HYDB.Services.Repositories
{
    public class Clients : BaseRepository
    {
        public Clients(IConfiguration config) : base(config) { }
        public void AddNewClient(Client newDataModel)
        {
            using (IDbConnection conn = new SqlConnection(GetConnectionString()))
            {
                string sqlquery = "insert into Clients (Id, Name, ApiKey, CreatedBy) values (@id, @name, @apikey, @createdby)";
                conn.Execute(sqlquery, new
                {
                    @id = newDataModel.Id,
                    @name = newDataModel.Name,
                    @apikey = newDataModel.ApiKey,
                    @createdby = newDataModel.CreatedBy
                });
            }
        }

        public IEnumerable<Client> GetAllClients(string userId)
        {
            using (IDbConnection conn = new SqlConnection(GetConnectionString()))
            {
                string sqlquery = "select * from Clients where CreatedBy=@userid";
                return conn.Query<Client>(sqlquery, new
                {
                    @userid = userId
                });
            }
        }

        public Client GetByName(string name)
        {
            using (IDbConnection conn = new SqlConnection(GetConnectionString()))
            {
                string sqlquery = "select * from Clients where Name=@name";
                return conn.Query<Client>(sqlquery, new
                {
                    @name = name
                }).FirstOrDefault();
            }
        }

        public Client GetByApiKey(string apiKey)
        {
            using (IDbConnection conn = new SqlConnection(GetConnectionString()))
            {
                string sqlquery = "select * from Clients where ApiKey=@key";
                return conn.Query<Client>(sqlquery, new
                {
                    @key = apiKey
                }).FirstOrDefault();
            }
        }

        public void DeleteClient(string Id)
        {
            using (IDbConnection conn = new SqlConnection(GetConnectionString()))
            {
                string sqlquery = "delete from Clients where Id=@id";
                conn.Execute(sqlquery, new { @id = Id });
            }
        }
    }
}
