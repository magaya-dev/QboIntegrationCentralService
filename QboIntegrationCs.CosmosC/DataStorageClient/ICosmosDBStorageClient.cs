using Microsoft.Azure.Cosmos;
using QboIntegrationCS.Domain.CosmosEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace QboIntegrationCs.CosmosC.DataStorageClient
{
    public interface ICosmosDBStorageClient
    {
        Task<Container> GetCosmosContainer(string containerId, string partitionKeyName = "/PartitionKey");
        Task<ItemResponse<T>> AddItemAsync<T>(Container container, T item) where T : IEntity;
        Task<T> DeleteItemAsync<T>(Container container, string id, string partitionKey = null) where T : IEntity;
        Task<T> DeleteItemAsync<T>(Container container, string id, T item) where T : IEntity;
        Task<T> GetItemAsync<T>(Container container, string id) where T : IEntity;
        Task<TResult> GetItemBy<T, TResult>(Container container, Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> projection);
        Task<T> GetItemBy<T>(Container container, Expression<Func<T, bool>> predicate);
        Task<IEnumerable<TResult>> GetItemsBy<T, TResult>(Container container, Expression<Func<T, bool>> predicate, Expression<Func<T, TResult>> projection);
        Task<IEnumerable<TResult>> GetItemsBy<T, TResult>(Container container, Expression<Func<T, TResult>> projection);
        IAsyncEnumerable<IEnumerable<T>> GetItemsBy<T>(Container container);
        Task<IEnumerable<T>> GetItemsBy<T>(Container container, Expression<Func<T, bool>> predicate);
        Task ReplaceItemAsync<T>(Container container, string id, T item) where T : IEntity;
        Task UpsertItemAsync<T>(Container container, T item) where T : IEntity;
        Task<ItemResponse<T>> UpdateItemAsync<T>(Container container, T item) where T : IEntity;
    }
}
