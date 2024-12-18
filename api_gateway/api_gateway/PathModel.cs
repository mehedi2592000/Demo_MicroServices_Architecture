namespace api_gateway
{
    public class ReverseProxyConfig
    {
        public Dictionary<string, ReverseProxyRoute> Routes { get; set; }
        public Dictionary<string, ReverseProxyCluster> Clusters { get; set; }
    }

    public class ReverseProxyRoute
    {
        public string ClusterId { get; set; }
        public ReverseProxyMatch Match { get; set; }
    }

    public class ReverseProxyMatch
    {
        public string Path { get; set; }
    }

    public class ReverseProxyCluster
    {
        public Dictionary<string, ReverseProxyDestination> Destinations { get; set; }
    }

    public class ReverseProxyDestination
    {
        public string Address { get; set; }
    }

}
