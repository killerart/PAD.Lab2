using System;

namespace Lab2.Warehouse.Core {
    public abstract class Entity {
        public Guid Id { get; set; } = Guid.NewGuid();
    }
}
