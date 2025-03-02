using MongoDB.Driver;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace FinanceControl.Infra.Context;

public interface IContextMongoDBDatabase
{
    ReadConcern GetReadConcernMajority();
    ReadConcern GetReadConcernAvailable();
    WriteConcern GetWriteConcern();
    IMongoClient GetMongoClient();
    IMongoDatabase GetMongoDatabase();
    IClientSessionHandle GetSession();
    DbContextOptions<ContextMongoDBDatabase> GetOptions();
    IConfiguration GetConfiguration();
}