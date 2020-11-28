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
    public class DataModelProperties : BaseRepository
    {
        public DataModelProperties(IConfiguration config) : base(config) { }
        public void AddNewDataModelProperty(DataModelProperty newDataModelProperty)
        {
            using (IDbConnection conn = new SqlConnection(GetConnectionString()))
            {
                string sqlquery = "insert into DataModelProperties (Id, Name, Type, DataModelId) values (@id, @name, @type, @datamodelid)";
                conn.Execute(sqlquery, new
                {
                    @id = newDataModelProperty.Id,
                    @name = newDataModelProperty.Name,
                    @type = newDataModelProperty.Type,
                    @datamodelid = newDataModelProperty.DataModelId
                });
            }
        }

        public DataModelProperty EditDataModelProperty(DataModelProperty updateProperty)
        {
            using (IDbConnection conn = new SqlConnection(GetConnectionString()))
            {
                string sqlquery = "update DataModelProperties set Name=@name, Type=@type where Id=@propid " +
                                  "select * from DataModelProperties where Id=@propid";
                return conn.Query<DataModelProperty>(sqlquery, new
                {
                    @propid = updateProperty.Id,
                    @name = updateProperty.Name,
                    @type = updateProperty.Type,
                    @datamodelid = updateProperty.DataModelId
                }
                ).FirstOrDefault();
            }
        }

        public void DeleteDataModelProperty(string propertyId)
        {
            using (IDbConnection conn = new SqlConnection(GetConnectionString()))
            {
                string sqlquery = "delete from DataModelProperties where Id=@id";
                conn.Execute(sqlquery, new { @id = propertyId });
            }
        }

        public DataModelProperty GetDataModelProperty(string propertyId)
        {
            using (IDbConnection conn = new SqlConnection(GetConnectionString()))
            {
                string sqlquery = "select * from DataModelProperties where Id=@id ";
                return conn.Query<DataModelProperty>(sqlquery, new { @id = propertyId }).FirstOrDefault();
            }
        }

        public DataModelProperty GetDataModelProperty(string propertyName, int dataModelId)
        {
            using (IDbConnection conn = new SqlConnection(GetConnectionString()))
            {
                string sqlquery = "select * from DataModelProperties where Name=@propname and DataModelId=@datamodelid";
                return conn.Query<DataModelProperty>(sqlquery, new
                {
                    @propname = propertyName,
                    @datamodelid = dataModelId
                }).FirstOrDefault();
            }
        }

        public List<DataModelProperty> GetDataModelProperties(string dataModelId)
        {
            using (IDbConnection conn = new SqlConnection(GetConnectionString()))
            {
                string sqlquery = "select * from DataModelProperties where DataModelId=@datamodelid ";
                return conn.Query<DataModelProperty>(sqlquery, new { @datamodelid = dataModelId }).ToList();
            }
        }
    }
}
