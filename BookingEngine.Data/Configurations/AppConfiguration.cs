using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Microsoft.Extensions.Configuration;


namespace BookingEngine.Data.Configuration
{
    public class AppConfiguration
    {
        // constructor
        public AppConfiguration()
        {
            // ConfigurationBuilder() is used to obtain configuration settings from the json file
            var configBuilder = new ConfigurationBuilder();
            // The path to get to the configuration string
            var path = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");
            configBuilder.AddJsonFile(path, false);
            var root = configBuilder.Build();

            var appSetting = root.GetSection("ConnectionStrings:DefaultConnection");
            

            // allocate the connection string to the variable
            sqlConnectionString = appSetting.Value;
        }

        public string sqlConnectionString { get; }

    }
}