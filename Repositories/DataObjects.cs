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
    public class DataObjects : BaseRepository
    {
        public DataObjects(IConfiguration config) : base(config) { }
        public void AddNewDataObject(DataObject obj)
        {
            using (IDbConnection conn = new SqlConnection(GetConnectionString()))
            {
                string sqlquery = "insert into DataObject (Id, ReferenceId) values (@id, @refid);";
                conn.Execute(sqlquery, new
                {
                    @id = obj.Id,
                    @refid = obj.ReferenceId
                });
            }
        }

        public DataObject DeleteDataObjectByReference(string referenceId)
        {
            using (IDbConnection conn = new SqlConnection(GetConnectionString()))
            {
                string sqlquery = "delete from DataObject where ReferenceId=@refid";
                return conn.Query<DataObject>(sqlquery, new
                {
                    @refid = referenceId
                }).FirstOrDefault();
            }
        }

        public void DeleteDataObjectById(string objId)
        {
            using (IDbConnection conn = new SqlConnection(GetConnectionString()))
            {
                string sqlquery = "delete from DataObject where Id=@dobid";
                conn.Execute(sqlquery, new
                {
                    @dobid = objId
                });
            }
        }

        public string GetReference(string objId)
        {
            using (IDbConnection conn = new SqlConnection(GetConnectionString()))
            {
                string sqlquery = "select ReferenceId from DataObject where Id=@objid";
                return conn.Query<string>(sqlquery, new
                {
                    @objid = objId
                }).FirstOrDefault();
            }
        }

        public List<DataObject> GetAllDataObject(string refId)
        {
            using (IDbConnection conn = new SqlConnection(GetConnectionString()))
            {
                string sqlquery = "select * from DataObject where ReferenceId=@refid";
                return conn.Query<DataObject>(sqlquery, new
                {
                    @refid = refId
                }).ToList();
            }
        }

        public DataObject RecentlyCreatedDataObject(string refId)
        {
            using (IDbConnection conn = new SqlConnection(GetConnectionString()))
            {
                string sqlquery = "select top 1 * from DataObject where ReferenceId=@refid order by Id desc";
                return conn.Query<DataObject>(sqlquery, new
                {
                    @refid = refId
                }).FirstOrDefault();
            }
        }
    }
}
