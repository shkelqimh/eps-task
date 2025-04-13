using System.Net.Sockets;
using System.Text;
using Shared.Enums;

namespace Client;

public class DiscountClient
{
    private readonly string _serverAddress;
    private readonly int _serverPort;

    public DiscountClient(string serverAddress, int serverPort)
    {
        _serverAddress = serverAddress;
        _serverPort = serverPort;
    }

    public async Task<bool> GenerateCodesAsync(ushort count, byte length, CancellationToken cancellationToken = default)
    {
        try
        {
            using var client = new TcpClient();
            await client.ConnectAsync(_serverAddress, _serverPort, cancellationToken);
            using var stream = client.GetStream();
            using var writer = new BinaryWriter(stream);
            using var reader = new BinaryReader(stream);

            writer.Write((byte)MessageType.GenerateCode);
            writer.Write(count);
            writer.Write(length);

            return await Task.Run(() => reader.ReadBoolean(), cancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error generating codes: {ex.Message}");
            return false;
        }
    }

    public async Task<byte> UseCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        try
        {
            using var client = new TcpClient();
            await client.ConnectAsync(_serverAddress, _serverPort, cancellationToken);
            using var stream = client.GetStream();
            using var writer = new BinaryWriter(stream);
            using var reader = new BinaryReader(stream);

            writer.Write((byte)MessageType.UseCode);
            writer.Write((byte)code.Length);
            writer.Write(Encoding.ASCII.GetBytes(code));

            return await Task.Run(() => reader.ReadByte(), cancellationToken);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error using code: {ex.Message}");
            return (byte)UseCodeResult.Fail;
        }
    }
}