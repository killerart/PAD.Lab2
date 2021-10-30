using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using Cassandra;
using Cassandra.Data.Linq;
using Lab2.Shared.Exceptions;
using Lab2.Warehouse.Entities.Abstractions;
using Lab2.Warehouse.Repositories.Abstractions;

namespace Lab2.Warehouse.Repositories {
    public class CassandraRepository<TModel> : IRepository<TModel> where TModel : Entity {
        private readonly Table<TModel> _table;

        public CassandraRepository(ISession session) {
            _table = new Table<TModel>(session);
        }

        public async Task<IEnumerable<TModel>> GetAllAsync() {
            return await _table.ExecuteAsync();
        }

        public async Task<TModel> GetByIdAsync(Guid id) {
            var model = await _table.First(m => m.Id == id).ExecuteAsync();
            if (model == null) {
                throw new RequestException($"{typeof(TModel).Name} with id = {id} not found", HttpStatusCode.NotFound);
            }

            return model;
        }

        public async Task<TModel> InsertAsync(TModel model) {
            await _table.Insert(model).ExecuteAsync();
            return model;
        }

        public async Task UpdateByIdAsync(Guid id, Expression<Func<TModel, TModel>> updateExpression) {
            await GetByIdAsync(id);
            await _table.Where(m => m.Id == id).Select(updateExpression).Update().ExecuteAsync();
        }

        public async Task DeleteByIdAsync(Guid id) {
            await GetByIdAsync(id);
            await _table.Where(m => m.Id == id).Delete().ExecuteAsync();
        }
    }
}
