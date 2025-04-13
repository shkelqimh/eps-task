using System.Net;
using System.Net.Sockets;
using System.Text;
using Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Server.Config;
using Shared.Enums;

namespace Server;

public class DiscountServer
{
    private readonly TcpListener _listener;
    private readonly IServiceProvider _services;
    private readonly Settings _settings;
    private readonly object _lock = new();

    public DiscountServer(IServiceProvider services)
    {
        _services = services;
        _settings = services.GetService<IOptions<Settings>>()!.Value;
        _listener = new TcpListener(IPAddress.Any, _settings.Port);
    }

    public async Task StartAsync(CancellationToken cancellationToken = default)
    {
        _listener.Start();
        Console.WriteLine($"Server listening on port {_settings.Port}");

        while (!cancellationToken.IsCancellationRequested)
        {
            var client = await _listener.AcceptTcpClientAsync(cancellationToken);
            _ = Task.Run(() => HandleClient(client));
        }
    }

    private void HandleClient(TcpClient client)
    {
        try
        {
            var remoteIp = ((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString();

            if (!_settings.WhiteListedIpAddresses.Contains(remoteIp))
            {
                Console.WriteLine($"Rejected connection from {remoteIp}");
                client.Close();
                return;
            }

            using var stream = client.GetStream();
            using var reader = new BinaryReader(stream, Encoding.ASCII, leaveOpen: true);
            using var writer = new BinaryWriter(stream, Encoding.ASCII, leaveOpen: true);

            byte messageType = reader.ReadByte();

            if (!Enum.IsDefined(typeof(MessageType), messageType))
            {
                return;
            }

            switch ((MessageType)messageType)
            {
                case MessageType.GenerateCode:
                    ushort count = reader.ReadUInt16();
                    byte length = reader.ReadByte();

                    if (!IsRequestValid(count, length))
                    {
                        return;
                    }

                    bool success = GenerateCodes(count, length);
                    writer.Write(success);
                    break;

                case MessageType.UseCode:
                    byte codeLength = reader.ReadByte();
                    byte[] codeBytes = reader.ReadBytes(codeLength);
                    string code = Encoding.ASCII.GetString(codeBytes);
                    byte result = UseCode(code);
                    writer.Write(result);
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error handling client: {ex.Message}");
        }
        finally
        {
            client.Close();
        }
    }

    public bool IsRequestValid(int count, byte length)
    {
        var codeRules = _settings.CodeRules;
        return count <= codeRules.MaxCodeGenerationWithinRequestAllowed &&
               length <= codeRules.MaxCodeLength &&
               length >= codeRules.MinCodeLength;
    }

    public bool GenerateCodes(int count, byte codeLength)
    {
        var codeRepository = _services.GetRequiredService<ICodeRepository>();

        lock (_lock)
        {
            return codeRepository.AddRange(count, codeLength);
        }
    }

    public byte UseCode(string code)
    {
        var codeRepository = _services.GetRequiredService<ICodeRepository>();

        lock (_lock)
        {
            if (!codeRepository.Exists(code))
            {
                return (byte)UseCodeResult.CodeDoesNotExist;
            }

            if (codeRepository.IsUsed(code))
            {
                return (byte)UseCodeResult.CodeAlreadyUsed;
            }

            bool hasAffected = codeRepository.UseCode(code);
            return (byte)(hasAffected ? UseCodeResult.Success : UseCodeResult.Fail);
        }
    }
}