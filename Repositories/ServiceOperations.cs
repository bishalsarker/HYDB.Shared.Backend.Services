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
    public class ServiceOperations : BaseRepository
    {
        public ServiceOperations(IConfiguration config) : base(config) { }
        public void AddNewServiceOperation(ServiceOperation newServiceOperation)
        {
            using (IDbConnection conn = new SqlConnection(GetConnectionString()))
            {
                string sqlquery = "insert into ServiceOperations (Id, Name, Type, ServiceId) values (@id, @name, @type, @serviceid)";
                conn.Execute(sqlquery, new
                {
                    @id = newServiceOperation.Id,
                    @name = newServiceOperation.Name,
                    @type = newServiceOperation.Type,
                    @serviceid = newServiceOperation.ServiceId
                });
            }
        }

        public ServiceOperation EditServiceOperation(ServiceOperation updateProperty)
        {
            using (IDbConnection conn = new SqlConnection(GetConnectionString()))
            {
                string sqlquery = "update ServiceOperations set Name=@name, Type=@type where Id=@opid " +
                                  "select * from ServiceOperations where Id=@opid";
                return conn.Query<ServiceOperation>(sqlquery, new
                {
                    @opid = updateProperty.Id,
                    @name = updateProperty.Name,
                    @type = updateProperty.Type,
                    @serviceid = updateProperty.ServiceId
                }
                ).FirstOrDefault();
            }
        }

        public void DeleteServiceOperation(string serviceId)
        {
            using (IDbConnection conn = new SqlConnection(GetConnectionString()))
            {
                string sqlquery = "delete from ServiceOperations where Id=@id";
                conn.Execute(sqlquery, new { @id = serviceId });
            }
        }

        public ServiceOperation GetServiceOperationById(string serviceId)
        {
            using (IDbConnection conn = new SqlConnection(GetConnectionString()))
            {
                string sqlquery = "select * from ServiceOperations where Id=@id ";
                return conn.Query<ServiceOperation>(sqlquery, new { @id = serviceId }).FirstOrDefault();
            }
        }

        public ServiceOperation GetServiceOperationByName(string serviceName, string dataModelId)
        {
            using (IDbConnection conn = new SqlConnection(GetConnectionString()))
            {
                string sqlquery = "select * from ServiceOperations where Name=@opname and ServiceId=@serviceid";
                return conn.Query<ServiceOperation>(sqlquery, new
                {
                    @opname = serviceName,
                    @serviceid = dataModelId
                }).FirstOrDefault();
            }
        }

        public List<ServiceOperation> GetServiceOperations(string dataModelId)
        {
            using (IDbConnection conn = new SqlConnection(GetConnectionString()))
            {
                string sqlquery = "select * from ServiceOperations where ServiceId=@serviceid ";
                return conn.Query<ServiceOperation>(sqlquery, new { @serviceid = dataModelId }).ToList();
            }
        }
    }
}
