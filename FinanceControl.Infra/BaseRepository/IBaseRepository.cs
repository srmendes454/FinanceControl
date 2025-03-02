using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace FinanceControl.Infra.BaseRepository;

public interface IBaseRepository<TCollection>
{
    Task InsertOneAsync(TCollection model);
    Task InsertManyAsync(List<TCollection> model);
    Task DeleteOneAsync(FilterDefinition<TCollection> filter);
    Task UpdateOneAsync(UpdateDefinition<TCollection> update, FilterDefinition<TCollection> filter);
}