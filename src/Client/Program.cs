using Shared.Enums;

namespace Client;

class Program
{
    private const string ServerAddress = "127.0.0.1";
    private const int ServerPort = 50051;

    static async Task Main()
    {
        var tcpClient = new DiscountClient(ServerAddress, ServerPort);

        Console.WriteLine("1. Generate codes\n2. Use code\n3. Press Q to quit\nChoose option:");
        string option = Console.ReadLine() ?? string.Empty;

        while (option.ToLower() != "q")
        {
            switch (option)
            {
                case "1":
                    await HandleGenerateCode(tcpClient);
                    break;

                case "2":
                    await HandleUseCode(tcpClient);
                    break;

                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }

            Console.WriteLine("1. Generate codes\n2. Use code\n3. Press Q to quit\nChoose option:");
            option = Console.ReadLine() ?? string.Empty;
        }
    }

    private static async Task HandleGenerateCode(DiscountClient tcpClient)
    {
        try
        {
            Console.Write("Enter number of codes to generate: ");
            ushort count = ushort.Parse(Console.ReadLine() ?? "0");

            Console.Write("Enter code length (7 or 8): ");
            byte length = byte.Parse(Console.ReadLine() ?? "0");

            bool result = await tcpClient.GenerateCodesAsync(count, length);
            Console.WriteLine($"Generate request result: {result}");
        }
        catch (FormatException ex)
        {
            Console.WriteLine($"Invalid input: {ex.Message}");
        }
    }

    private static async Task HandleUseCode(DiscountClient tcpClient)
    {
        try
        {
            Console.Write("Enter discount code (7 or 8 chars): ");
            string code = Console.ReadLine() ?? string.Empty;

            byte result = await tcpClient.UseCodeAsync(code);

            string resultMessage = result switch
            {
                (byte)UseCodeResult.CodeDoesNotExist => "Code does not exist",
                (byte)UseCodeResult.CodeAlreadyUsed => "Code already used",
                (byte)UseCodeResult.Fail => "Failed during code usage",
                (byte)UseCodeResult.Success => "Code used successfully",
                _ => "Unknown error"
            };

            Console.WriteLine(resultMessage);
        }
        catch (FormatException ex)
        {
            Console.WriteLine($"Invalid input: {ex.Message}");
        }
    }
}