namespace IterumApi.Models
{
    public class SessionConnectionInfo
    {
        public SessionConnectionInfo(string sessionCode, int connectionPort, string connectionIp, List<long> playerUserIds, long hostId)
        {
            SessionCode = sessionCode;
            ConnectionPort = connectionPort;
            ConnectionIp = connectionIp;
            PlayerUserIds = playerUserIds;
            HostId = hostId;
        }

        public string SessionCode { get; set; }
        public int ConnectionPort { get; set; }
        public string ConnectionIp { get; set; }
        public List<long> PlayerUserIds { get; set; }
        public long HostId { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj is not SessionConnectionInfo other)
                return false;

            return string.Equals(ConnectionIp, other.ConnectionIp, StringComparison.OrdinalIgnoreCase) &&
                   Equals(ConnectionPort, other.ConnectionPort);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + (ConnectionIp != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(ConnectionIp) : 0);
                hash = hash * 23 + (ConnectionPort != null ? StringComparer.OrdinalIgnoreCase.GetHashCode(ConnectionPort) : 0);
                return hash;
            }
        }
    }
}
