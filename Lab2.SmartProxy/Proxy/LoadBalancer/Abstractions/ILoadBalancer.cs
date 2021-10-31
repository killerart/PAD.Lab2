namespace Lab2.SmartProxy.Proxy.LoadBalancer.Abstractions {
    public interface ILoadBalancer {
        int    Count { get; }
        string GetNextWarehouse();
    }
}
