using System;
using System.IO;
using QRCoder;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            PrintHelp();
            return;
        }

        string? data = null;
        string output = "qr.png";
        int pixels = 10;

        for (int i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "-f":
                case "--file":
                    if (i + 1 >= args.Length)
                    {
                        Console.WriteLine("Xatolik: Fayl manzili ko‘rsatilmagan.");
                        return;
                    }
                    if (!File.Exists(args[i + 1]))
                    {
                        Console.WriteLine($"Xatolik: Fayl topilmadi → {args[i + 1]}");
                        return;
                    }
                    data = File.ReadAllText(args[i + 1]).Trim();
                    i++;
                    break;

                case "-o":
                case "--out":
                    if (i + 1 >= args.Length)
                    {
                        Console.WriteLine("Xatolik: Chiqish fayli berilmagan.");
                        return;
                    }
                    output = args[i + 1];
                    i++;
                    break;

                case "-s":
                case "--size":
                    if (i + 1 >= args.Length || !int.TryParse(args[i + 1], out pixels))
                    {
                        Console.WriteLine("Xatolik: Noto‘g‘ri size qiymati.");
                        return;
                    }
                    i++;
                    break;

                default:
                    if (data == null)
                        data = args[i];
                    break;
            }
        }

        if (string.IsNullOrWhiteSpace(data))
        {
            Console.WriteLine("Xatolik: Matn topilmadi.");
            return;
        }

        if (!output.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("Xatolik: Faqat PNG format qo‘llab-quvvatlanadi.");
            return;
        }

        GenerateQR(data, output, pixels);
    }

    static void GenerateQR(string text, string path, int pixelSize)
    {
        try
        {
            var generator = new QRCodeGenerator();
            var qrData = generator.CreateQrCode(text, QRCodeGenerator.ECCLevel.Q); // yuqori sifat
            var pngQr = new PngByteQRCode(qrData);

            byte[] bytes = pngQr.GetGraphic(pixelSize);

            File.WriteAllBytes(path, bytes);
            Console.WriteLine($"✓ QR code tayyor: {path}");
        }
        catch (Exception ex)
        {
            Console.WriteLine("QR yaratishda xatolik:");
            Console.WriteLine(ex.Message);
        }
    }

    static void PrintHelp()
    {
        Console.WriteLine("QR Console Generator (C#)");
        Console.WriteLine();
        Console.WriteLine("Foydalanish:");
        Console.WriteLine("  dotnet run -- \"Matn\" -o qr.png");
        Console.WriteLine("  dotnet run -- -f text.txt -o qr.png");
        Console.WriteLine();
        Console.WriteLine("Parametrlar:");
        Console.WriteLine("  -f, --file <path>     Fayldan matn yuklash");
        Console.WriteLine("  -o, --out <file>      Chiqariladigan PNG nomi");
        Console.WriteLine("  -s, --size <int>      QR piksel o‘lchami (default: 10)");
        Console.WriteLine();
    }
}
