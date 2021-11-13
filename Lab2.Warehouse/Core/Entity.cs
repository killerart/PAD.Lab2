using System;

namespace Lab2.Warehouse.Domain.Entities.Abstractions {
    public abstract class Entity : IEntity {
        public Guid Id { get; set; } = Guid.NewGuid();
    }
}
