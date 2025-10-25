using System.Buffers.Binary;
using System.Net;
using System.Net.Sockets;
using System.Text;

static class Proxy
{
    public static async Task<IPEndPoint?> TryReadProxyHeaderAsync(NetworkStream stream, int maxHeaderBytes = 108)
    {
        byte[] buffer = new byte[maxHeaderBytes];
        int read = 0;

        try
        {
            read = await stream.ReadAsync(buffer, 0, buffer.Length);
            if (read == 0) return null;
        }
        catch
        {
            return null;
        }

        const string v1Prefix = "PROXY ";
        if (read >= v1Prefix.Length)
        {
            string start = Encoding.ASCII.GetString(buffer, 0, Math.Min(read, v1Prefix.Length));
            if (start == v1Prefix)
            {
                int eol = Array.IndexOf(buffer, (byte)'\n', 0, read);

                while (eol == -1 && read < buffer.Length)
                {
                    int additional = await stream.ReadAsync(buffer, read, buffer.Length - read);
                    if (additional == 0) break;
                    read += additional;
                    eol = Array.IndexOf(buffer, (byte)'\n', 0, read);
                }

                if (eol != -1)
                {
                    string headerLine = Encoding.ASCII.GetString(buffer, 0, eol + 1).Trim();

                    var parts = headerLine.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 6)
                    {
                        string proto = parts[1]; // TCP4, TCP6 or UNKNOWN
                        string src = parts[2];
                        string srcPortStr = parts[4];
                        if (IPAddress.TryParse(src, out var ip) && int.TryParse(srcPortStr, out int port))
                        {
                            return new IPEndPoint(ip, port);
                        }
                    }
                }

                return null;
            }
        }

        byte[] v2sig = new byte[] { 0x0d, 0x0a, 0x0d, 0x0a, 0x00, 0x0d, 0x0a, 0x51, 0x55, 0x49, 0x54, 0x0a };
        if (read >= v2sig.Length)
        {
            bool isV2 = true;
            for (int i = 0; i < v2sig.Length; i++)
            {
                if (buffer[i] != v2sig[i]) { isV2 = false; break; }
            }

            if (isV2)
            {
                int headerNeeded = 16;
                while (read < headerNeeded)
                {
                    int additional = await stream.ReadAsync(buffer, read, buffer.Length - read);
                    if (additional == 0) break;
                    read += additional;
                }
                if (read < headerNeeded) return null;

                byte verCmd = buffer[12];
                byte famProto = buffer[13];
                ushort len = BinaryPrimitives.ReadUInt16BigEndian(buffer.AsSpan(14, 2));

                int totalNeeded = 16 + len;
                while (read < totalNeeded && read < buffer.Length)
                {
                    int additional = await stream.ReadAsync(buffer, read, buffer.Length - read);
                    if (additional == 0) break;
                    read += additional;
                }
                if (read < totalNeeded) return null;

                int family = (famProto & 0xF0) >> 4;

                if (family == 0x1 && len >= 12)
                {
                    var srcAddr = new byte[4];
                    Array.Copy(buffer, 16, srcAddr, 0, 4);
                    var srcPort = BinaryPrimitives.ReadUInt16BigEndian(buffer.AsSpan(24, 2));
                    IPAddress ip = new IPAddress(srcAddr);
                    return new IPEndPoint(ip, srcPort);
                }

                if (family == 0x2 && len >= 36)
                {
                    var srcAddr = new byte[16];
                    Array.Copy(buffer, 16, srcAddr, 0, 16);
                    var srcPort = BinaryPrimitives.ReadUInt16BigEndian(buffer.AsSpan(48, 2));
                    IPAddress ip = new IPAddress(srcAddr);
                    return new IPEndPoint(ip, srcPort);
                }

                return null;
            }
        }

        return null;
    }
}
