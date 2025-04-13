using System.Net.Sockets;
using System.Text;
using Infrastructure.Persistence.Repositories;
using Shared.Enums;

namespace Server.IntegrationTests;

public class DiscountServerTests : IClassFixture<TcpServerFixture>
{
    private const string ServerAddress = "127.0.0.1";
    private const int ServerPort = 50051;
    private readonly TcpServerFixture _fixture;

    public DiscountServerTests(TcpServerFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void GenerateCode_ReturnsTrue_WhenRequestIsValid()
    {
        using var client = new TcpClient(ServerAddress, ServerPort);
        using var stream = client.GetStream();
        using var writer = new BinaryWriter(stream);
        using var reader = new BinaryReader(stream);

        writer.Write((byte)MessageType.GenerateCode);
        writer.Write((ushort)10); // count
        writer.Write((byte)7);    // length

        bool result = reader.ReadBoolean();

        Assert.True(result);
    }

    [Fact]
    public void UseCode_ReturnsSuccess_WhenCodeExists()
    {
        var repo = _fixture.Server.Services.GetRequiredService<ICodeRepository>();
        repo.Add("TEST123");

        using var client = new TcpClient(ServerAddress, ServerPort);
        using var stream = client.GetStream();
        using var writer = new BinaryWriter(stream);
        using var reader = new BinaryReader(stream);

        writer.Write((byte)MessageType.UseCode);
        writer.Write((byte)7);
        writer.Write(Encoding.ASCII.GetBytes("TEST123"));

        byte result = reader.ReadByte();

        Assert.Equal((byte)UseCodeResult.Success, result);
    }
}