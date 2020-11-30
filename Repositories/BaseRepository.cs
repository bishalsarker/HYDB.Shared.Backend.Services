using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace HYDB.Services.Repositories
{
    public class BaseRepository
    {
        private readonly bool _devMode;
        private readonly string _devDbConn;
        private readonly string _prodDbConn;

        public BaseRepository(IConfiguration config)
        {
            _devMode = bool.Parse(config.GetSection("DevMode").Value);
            _devDbConn = config.GetSection("DbConfig").GetSection("Dev").GetSection("connString").Value;
            _prodDbConn = config.GetSection("DbConfig").GetSection("Prod").GetSection("connString").Value;
        }

        public string GetConnectionString()
        {
            if(_devMode)
            {
                return _devDbConn;
            }
            else
            {
                return _prodDbConn;
            }
        }
    }
}
