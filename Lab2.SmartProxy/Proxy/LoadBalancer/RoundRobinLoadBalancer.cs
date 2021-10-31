using Lab2.SmartProxy.Proxy.LoadBalancer.Abstractions;

namespace Lab2.SmartProxy.Proxy.LoadBalancer {
    public class RoundRobinLoadBalancer : ILoadBalancer {
        private readonly RoundRobinCollectionEnumerator<string> _roundRobinCollectionEnumerator;

        private readonly ProxyConfig _config;

        public RoundRobinLoadBalancer(ProxyConfig config) {
            _config = config;

            _roundRobinCollectionEnumerator = new RoundRobinCollectionEnumerator<string>(config.Nodes);
        }

        public int Count => _config.Nodes.Count;

        public string GetNextWarehouse() {
            return _roundRobinCollectionEnumerator.GetNext();
        }
    }
}
