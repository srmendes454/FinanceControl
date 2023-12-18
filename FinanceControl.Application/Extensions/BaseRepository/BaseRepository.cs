using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FinanceControl.WebApi.Extensions.BaseRepository;
using FinanceControl.WebApi.Extensions.Context;
using MongoDB.Driver;
using ILogger = Serilog.ILogger;

namespace FinanceControl.Extensions.BaseRepository;

public class BaseRepository<TCollection> : IBaseRepository<TCollection>, IDisposable
{
    #region [ Fields ]
    public void Dispose()
    {
    }

    public ILogger _logger;
    public IMongoClient _mongoClient;
    public IContextMongoDBDatabase _mongoDb;
    public IClientSessionHandle _mongoSession;
    public IMongoCollection<TCollection> _mongoCollection;
    private string _collectionName;

    public IMongoCollection<TCollection> GetMongoCollection() => _mongoCollection;
    #endregion

    #region [ Constructor ]

    public BaseRepository(IContextMongoDBDatabase mongoDb, ILogger logger, string collectionName)
    {
        _logger = logger;
        _mongoDb = mongoDb;
        _collectionName = collectionName;
        _mongoClient = mongoDb.GetMongoClient();


        var isDev = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == null ? true : Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT").ToLower().Equals("dev") || Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT").ToLower().Equals("development");
        var isAvailableReadConcern = (Environment.GetEnvironmentVariable("ASPNETCORE_READCONCERN") ?? "").Equals("available");

        if (isDev && isAvailableReadConcern)
        {
            _mongoCollection = mongoDb.GetMongoDatabase()
                .GetCollection<TCollection>(collectionName)
                .WithWriteConcern(mongoDb.GetWriteConcern())
                .WithReadConcern(mongoDb.GetReadConcernAvailable());
        }
        else
        {
            _mongoCollection = mongoDb.GetMongoDatabase()
                .GetCollection<TCollection>(collectionName)
                .WithWriteConcern(mongoDb.GetWriteConcern())
                .WithReadConcern(mongoDb.GetReadConcernMajority());
        }
    }

    #endregion

    #region [ Public Methods ]


    public async Task InsertOneAsync(TCollection model)
    {
        await GetMongoCollection().InsertOneAsync(model);
    }

    public async Task InsertManyAsync(List<TCollection> model)
    {
        await GetMongoCollection().InsertManyAsync(model);
    }

    public async Task DeleteOneAsync(FilterDefinition<TCollection> filter)
    {
        await GetMongoCollection().DeleteOneAsync(filter);
    }

    public async Task DeleteManyAsync(FilterDefinition<TCollection> filter)
    {
        await GetMongoCollection().DeleteManyAsync(filter);
    }

    public async Task UpdateOneAsync(UpdateDefinition<TCollection> update, FilterDefinition<TCollection> filter)
    {
        //_logger.Information($"BaseRepository :: [UpdateOneAsync] :: {_collectionName} :: {update.ToString()} :: {filter.ToString()}");
        await GetMongoCollection().UpdateOneAsync(filter, update);
    }

    public async Task UpdateManyAsync(UpdateDefinition<TCollection> update, FilterDefinition<TCollection> filter)
    {
        await GetMongoCollection().UpdateManyAsync(filter, update);
    }

    public async Task BeginTransactionAsync()
    {
        _mongoSession = await _mongoClient.StartSessionAsync();
        _mongoSession.StartTransaction();
    }

    public async Task CommitTransactionAsync()
    {
        if (_mongoSession.IsInTransaction)
        {
            await _mongoSession.CommitTransactionAsync();
        }
    }

    public async Task AbortTransactionAsync()
    {
        if (_mongoSession.IsInTransaction)
        {
            await _mongoSession.AbortTransactionAsync();
        }
    }
    #endregion
}