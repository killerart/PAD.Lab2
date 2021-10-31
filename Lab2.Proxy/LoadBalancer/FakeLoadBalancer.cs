using Lab2.Proxy.LoadBalancer.Abstractions;

namespace Lab2.Proxy.LoadBalancer {
    public class FakeLoadBalancer : ILoadBalancer {
        public string GetNextWarehouse() {
            return "https://localhost:5001";
        }
    }
}
