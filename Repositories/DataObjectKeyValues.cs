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
    public class DataObjectKeyValues : BaseRepository
    {
        public DataObjectKeyValues(IConfiguration config) : base(config) {}

        public void AddNewDataObjectKeyValue(DataObjectKeyValue newDataObjectKeyValue)
        {
            using (IDbConnection conn = new SqlConnection(GetConnectionString()))
            {
                string sqlquery = "insert into DataObjectKeyValues (KeyString, Value, DataObjectId) " +
                                  "values (@keystring, @value, @objid)";
                conn.Execute(sqlquery, new
                {
                    @keystring = newDataObjectKeyValue.KeyString,
                    @value = newDataObjectKeyValue.Value,
                    @objid = newDataObjectKeyValue.DataObjectId
                });
            }
        }

        public void UpdateValue(string dataObjectId, string keyString, string newValue)
        {
            using (IDbConnection conn = new SqlConnection(GetConnectionString()))
            {
                string sqlquery = "update DataObjectKeyValues set Value=@newvalue where " +
                                  "DataObjectId=@dataobjectid and KeyString=@keystring";
                conn.Execute(sqlquery, new
                {
                    @keystring = keyString,
                    @dataobjectid = dataObjectId,
                    @newvalue = newValue
                });
            }
        }

        public void DeleteByKeyString(string keyString)
        {
            using (IDbConnection conn = new SqlConnection(GetConnectionString()))
            {
                string sqlquery = "delete from DataObjectKeyValues where KeyString=@keystring";
                conn.Execute(sqlquery, new
                {
                    @keystring = keyString
                });
            }
        }

        public void DeleteByObjectId(string objectId)
        {
            using (IDbConnection conn = new SqlConnection(GetConnectionString()))
            {
                string sqlquery = "delete from DataObjectKeyValues where DataObjectId=@dobid";
                conn.Execute(sqlquery, new
                {
                    @dobid = objectId
                });
            }
        }

        public List<DataObjectKeyValue> GetDataObjectValues(string dataObjectId)
        {
            using (IDbConnection conn = new SqlConnection(GetConnectionString()))
            {
                string sqlquery = "select * from DataObjectKeyValues where DataObjectId=@dataobjectid";
                return conn.Query<DataObjectKeyValue>(sqlquery, new
                {
                    @dataobjectid = dataObjectId
                }).ToList();
            }
        }
    }
}
