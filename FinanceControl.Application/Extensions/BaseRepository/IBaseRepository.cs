using System.Threading.Tasks;
using MongoDB.Driver;

namespace FinanceControl.WebApi.Extensions.BaseRepository;

public interface IBaseRepository<TCollection>
{
    Task InsertOneAsync(TCollection model);
    Task DeleteOneAsync(FilterDefinition<TCollection> filter);
    Task UpdateOneAsync(UpdateDefinition<TCollection> update, FilterDefinition<TCollection> filter);
}