﻿using System;

namespace Lab2.Warehouse.Entities.Abstractions {
    public abstract class Entity {
        public Guid Id { get; set; } = Guid.NewGuid();
    }
}
