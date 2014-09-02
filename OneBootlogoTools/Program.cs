using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;

namespace OneBootlogoTools
{
    class Program
    {
        static readonly string _inputDir = Path.Combine(Environment.CurrentDirectory, "input");
        static readonly string _dataDir = Path.Combine(Environment.CurrentDirectory, "data");
        static readonly string _outputDir = Path.Combine(Environment.CurrentDirectory, "output");

        static void Main(string[] args)
        {

            var bootLogo = (Bitmap)Image.FromFile(Path.Combine(_inputDir,"bootlogo.png"));
            var fastBootLogo = (Bitmap)Image.FromFile(Path.Combine(_inputDir, "fastboot.png"));
            
            byte[] logo = File.ReadAllBytes(Path.Combine(_dataDir, "logo-original.bin"));

            List<byte> bootLogoRaw = ConvertToRaw("bootlogo.raw", bootLogo);
            List<byte> fastBootLogoRaw = ConvertToRaw("fastboot.raw", fastBootLogo);

            for (int i = 0; i < logo.Length; i++)
            {
                if (i >= 512 && i < 6221312)
                {
                    logo[i] = bootLogoRaw[i - 512];
                }

                if (i >= 7234560 && i < 7549560)
                {
                    logo[i] = fastBootLogoRaw[i - 7234560];
                }
            }

            if (logo.Length != 9821696) return;
            File.WriteAllBytes(Path.Combine(_outputDir, "logo-modified.bin"), logo);
        }

        static List<byte> ConvertToRaw(string filename, Bitmap image)
        {
            var coverterWriter = new BinaryWriter(File.Open(Path.Combine(_dataDir, filename), FileMode.Truncate));
            var fastBitmap = new FastBitmap(image);
            var bytes = new List<byte>();
            fastBitmap.LockImage();

            for (int height = 0; height < image.Height; height++)
            {
                for (int width = 0; width < image.Width; width++)
                {
                    Color colorAtPixel = fastBitmap.GetPixel(width, height);

                    int rdec = Convert.ToInt32(colorAtPixel.R.ToString("X2"), 16);
                    int gdec = Convert.ToInt32(colorAtPixel.G.ToString("X2"), 16);
                    int bdec = Convert.ToInt32(colorAtPixel.B.ToString("X2"), 16);

                    coverterWriter.Write((byte)bdec);
                    coverterWriter.Write((byte)gdec);
                    coverterWriter.Write((byte)rdec);

                    bytes.Add((byte) bdec);
                    bytes.Add((byte) gdec);
                    bytes.Add((byte) rdec);
                }
            }

            fastBitmap.UnlockImage();

            coverterWriter.Close();
            coverterWriter.Dispose();
            return bytes;
        }
        static string ColorConverter(Color c)
        {
            return c.B.ToString("X2") + c.G.ToString("X2") + c.R.ToString("X2");
        }

        private static String HexConverter(System.Drawing.Color c)
        {
            return c.B.ToString("X2") + c.G.ToString("X2") + c.R.ToString("X2");
        }
    }
}
