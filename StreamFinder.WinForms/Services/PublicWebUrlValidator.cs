using System;
using System.Net;

namespace StreamFinder.WinForms.Services
{
    public static class PublicWebUrlValidator
    {
        public static bool TryValidate(string value, out Uri uri)
        {
            uri = null;
            if (!Uri.TryCreate(value, UriKind.Absolute, out var candidate) ||
                (candidate.Scheme != Uri.UriSchemeHttp && candidate.Scheme != Uri.UriSchemeHttps) ||
                string.IsNullOrWhiteSpace(candidate.Host) ||
                !string.IsNullOrWhiteSpace(candidate.UserInfo))
            {
                return false;
            }

            if (IsLocalHostName(candidate.Host) || IsPrivateAddress(candidate.Host))
            {
                return false;
            }

            uri = candidate;
            return true;
        }

        private static bool IsLocalHostName(string host)
        {
            return string.Equals(host, "localhost", StringComparison.OrdinalIgnoreCase) ||
                host.EndsWith(".localhost", StringComparison.OrdinalIgnoreCase) ||
                host.EndsWith(".local", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(host, "ip6-localhost", StringComparison.OrdinalIgnoreCase);
        }

        private static bool IsPrivateAddress(string host)
        {
            IPAddress address;
            if (!IPAddress.TryParse(host, out address))
            {
                return false;
            }

            var bytes = address.GetAddressBytes();
            if (address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {
                return bytes[0] == 10 ||
                    bytes[0] == 127 ||
                    (bytes[0] == 172 && bytes[1] >= 16 && bytes[1] <= 31) ||
                    (bytes[0] == 192 && bytes[1] == 168) ||
                    (bytes[0] == 169 && bytes[1] == 254) ||
                    (bytes[0] == 0 && bytes[1] == 0 && bytes[2] == 0 && bytes[3] == 0);
            }

            return IPAddress.IsLoopback(address) ||
                address.Equals(IPAddress.IPv6Any) ||
                (bytes.Length > 0 && (bytes[0] & 0xFE) == 0xFC) ||
                (bytes.Length > 0 && bytes[0] == 0xFE && (bytes[1] & 0xC0) == 0x80);
        }
    }
}
