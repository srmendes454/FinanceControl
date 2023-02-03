using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using MongoDB.Driver.Core.Events;
using Serilog;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using ILogger = Serilog.ILogger;

namespace FinanceControl.WebApi.Extensions.Context;

public class ContextMongoDBDatabase : DbContext, IContextMongoDBDatabase
{
    #region [ Fields ]
    public readonly ILogger _logger;
    public readonly IMongoClient _client;
    public readonly IMongoDatabase _database;
    public readonly IConfiguration _configuration;
    public readonly IHostEnvironment _environment;
    public readonly DbContextOptions<ContextMongoDBDatabase> _options;
    #endregion

    #region [ Constructor ]

    public ContextMongoDBDatabase(IConfiguration configuration, IHostEnvironment environment, DbContextOptions<ContextMongoDBDatabase> options)
    {
        if (configuration != null)
            try
            {
                _options = options;
                _environment = environment;
                _configuration = configuration;

                _logger = Log.Logger.ForContext<ContextMongoDBDatabase>();

                _client = getMongoClient();

                _database = _client.GetDatabase(configuration["Providers:DataBase"].ToString()
                );
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "ContextMongoDBDatabase");
            }
    }

    #endregion

    #region [ Public Methods ]
    public IMongoClient GetMongoClient() => _client;
    public IMongoDatabase GetMongoDatabase() => _database;
    public ReadConcern GetReadConcernMajority() => ReadConcern.Majority;
    public ReadConcern GetReadConcernAvailable() => ReadConcern.Available;
    public WriteConcern GetWriteConcern() => WriteConcern.WMajority;
    public DbContextOptions<ContextMongoDBDatabase> GetOptions() => _options;
    public IConfiguration GetConfiguration() => _configuration;
    public IClientSessionHandle GetSession()
    {
        return GetMongoClient().StartSession();
    }
    public class SecretDocumentDbValue
    {
        public string username { get; set; }
        public string engine { get; set; }
        public string password { get; set; }
        public string host { get; set; }
        public string port { get; set; }
        public string ssl { get; set; }
        public string dbClusterIdentifier { get; set; }
    }
    #endregion

    #region [ Private Methods ]

    private MongoClient getMongoClient()
    {
        var urlMongoDb = _configuration["Providers:UrlMongoDB"];

        var mongoUrl = new MongoUrl(urlMongoDb);
        var settingsDB = MongoClientSettings.FromUrl(mongoUrl);

        settingsDB.ConnectTimeout = TimeSpan.FromMinutes(1);
        settingsDB.MinConnectionPoolSize = 2;
        settingsDB.MaxConnectionPoolSize = 10;
        
        return new MongoClient(settingsDB);
    }
    
    #endregion
}