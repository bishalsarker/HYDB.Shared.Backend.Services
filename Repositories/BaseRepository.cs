using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace HYDB.Services.Repositories
{
    public class BaseRepository
    {
        private readonly string _dbConn;

        public BaseRepository(IConfiguration config)
        {
            _dbConn = config.GetSection("DbConfig").GetSection("connString").Value;
        }

        public string GetConnectionString()
        {
            return _dbConn;
        }
    }
}
