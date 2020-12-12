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
    public class DataServices : BaseRepository
    {
        public DataServices(IConfiguration config) : base(config) { }
        public void AddNewDataService(DataService newDataService)
        {
            using (IDbConnection conn = new SqlConnection(GetConnectionString()))
            {
                string sqlquery = "insert into DataServices (Id, Name, CreatedBy) values (@id, @name, @createdby)";
                conn.Execute(sqlquery, new
                {
                    @id = newDataService.Id,
                    @name = newDataService.Name,
                    @createdby = newDataService.CreatedBy
                });
            }
        }

        public void UpdateDataService(DataService newDataService)
        {
            using (IDbConnection conn = new SqlConnection(GetConnectionString()))
            {
                string sqlquery = "update DataServices set Name=@name where Id=@id";
                conn.Execute(sqlquery, new
                {
                    @id = newDataService.Id,
                    @name = newDataService.Name
                });
            }
        }

        public DataService GetDataServiceById(string modelId)
        {
            using (IDbConnection conn = new SqlConnection(GetConnectionString()))
            {
                string sqlquery = "select * from DataServices where Id=@modelid";
                return conn.Query<DataService>(sqlquery, new { @modelid = modelId }).FirstOrDefault();
            }
        }

        public DataService GetDataServiceByName(string modelName, string userId)
        {
            using (IDbConnection conn = new SqlConnection(GetConnectionString()))
            {
                string sqlquery = "select * from DataServices where Name=@name and CreatedBy=@createdby";
                return conn.Query<DataService>(sqlquery, new { @name = modelName, @createdby = userId }).FirstOrDefault();
            }
        }

        public List<DataService> GetAllDataServiceByUserId(string userId)
        {
            using (IDbConnection conn = new SqlConnection(GetConnectionString()))
            {
                string sqlquery = "select * from DataServices where CreatedBy=@createdby";
                return conn.Query<DataService>(sqlquery, new { @createdby = userId }).ToList();
            }
        }

        public DataService RecentlyCreatedDataService(int userId)
        {
            using (IDbConnection conn = new SqlConnection(GetConnectionString()))
            {
                string sqlquery = "select top 1 * from DataServices where Createdby=@userid order by Id desc";
                return conn.Query<DataService>(sqlquery, new { @userid = userId }).FirstOrDefault();
            }
        }

        public void DeleteDataService(string modelId)
        {
            using (IDbConnection conn = new SqlConnection(GetConnectionString()))
            {
                string sqlquery = "delete from DataServices where Id=@id";
                conn.Execute(sqlquery, new { @id = modelId });
            }
        }
    }
}
