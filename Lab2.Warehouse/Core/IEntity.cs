using System;

namespace Lab2.Warehouse.Domain.Entities.Abstractions {
    public interface IEntity {
        public Guid Id { get; set; }
    }
}
