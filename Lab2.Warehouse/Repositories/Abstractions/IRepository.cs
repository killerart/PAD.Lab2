﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Lab2.Warehouse.Entities.Abstractions;

namespace Lab2.Warehouse.Repositories.Abstractions {
    public interface IRepository<TModel> where TModel : Entity {
        Task<IEnumerable<TModel>> GetAllAsync();

        Task<TModel> GetByIdAsync(Guid  id);
        Task<TModel> InsertAsync(TModel model);

        Task UpdateByIdAsync(Guid id, Expression<Func<TModel, TModel>> updateExpression);
        Task DeleteByIdAsync(Guid id);
    }
}
