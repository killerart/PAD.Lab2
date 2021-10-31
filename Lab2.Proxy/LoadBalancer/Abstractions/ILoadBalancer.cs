namespace Lab2.Proxy.LoadBalancer.Abstractions {
    public interface ILoadBalancer {
        string? GetNextWarehouse();
    }
}
