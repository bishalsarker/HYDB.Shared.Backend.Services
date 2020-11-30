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
    public class DataModels : BaseRepository
    {
        public DataModels(IConfiguration config) : base(config) { }
        public void AddNewDataModel(DataModel newDataModel)
        {
            using (IDbConnection conn = new SqlConnection(GetConnectionString()))
            {
                string sqlquery = "insert into DataModels (Id, Name, CreatedBy) values (@id, @name, @createdby)";
                conn.Execute(sqlquery, new
                {
                    @id = newDataModel.Id,
                    @name = newDataModel.Name,
                    @createdby = newDataModel.CreatedBy
                });
            }
        }

        public void UpdateDataModel(DataModel newDataModel)
        {
            using (IDbConnection conn = new SqlConnection(GetConnectionString()))
            {
                string sqlquery = "update DataModels set Name=@name where Id=@id";
                conn.Execute(sqlquery, new
                {
                    @id = newDataModel.Id,
                    @name = newDataModel.Name
                });
            }
        }

        public DataModel GetDataModelById(string modelId)
        {
            using (IDbConnection conn = new SqlConnection(GetConnectionString()))
            {
                string sqlquery = "select * from DataModels where Id=@modelid";
                return conn.Query<DataModel>(sqlquery, new { @modelid = modelId }).FirstOrDefault();
            }
        }

        public DataModel GetAllDataModelByName(string modelName, string userId)
        {
            using (IDbConnection conn = new SqlConnection(GetConnectionString()))
            {
                string sqlquery = "select * from DataModels where Name=@name and CreatedBy=@createdby";
                return conn.Query<DataModel>(sqlquery, new { @name = modelName, @createdby = userId }).FirstOrDefault();
            }
        }

        public List<DataModel> GetAllDataModelByUserId(string userId)
        {
            using (IDbConnection conn = new SqlConnection(GetConnectionString()))
            {
                string sqlquery = "select * from DataModels where CreatedBy=@createdby";
                return conn.Query<DataModel>(sqlquery, new { @createdby = userId }).ToList();
            }
        }

        public DataModel RecentlyCreatedDataModel(int userId)
        {
            using (IDbConnection conn = new SqlConnection(GetConnectionString()))
            {
                string sqlquery = "select top 1 * from DataModels where Createdby=@userid order by Id desc";
                return conn.Query<DataModel>(sqlquery, new { @userid = userId }).FirstOrDefault();
            }
        }

        public void DeleteDataModel(string modelId)
        {
            using (IDbConnection conn = new SqlConnection(GetConnectionString()))
            {
                string sqlquery = "delete from DataModels where Id=@id";
                conn.Execute(sqlquery, new { @id = modelId });
            }
        }
    }
}
