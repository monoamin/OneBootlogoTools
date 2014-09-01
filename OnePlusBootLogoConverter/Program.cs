using System;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;

namespace OneBootlogoTools
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("\n\tDrag your image onto this window.\n\tPress enter if you see the file path.");
            string path = Console.ReadLine();

            var bitmap = (Bitmap) Image.FromFile(Path.GetDirectoryName(path) + "\\boot-logo.png");
            var fastBitmap = new FastBitmap(bitmap);
            fastBitmap.LockImage();

            var coverterWriter = new BinaryWriter(File.Open(Path.GetDirectoryName(path) + "\\..\\data\\image-converted_DO_NOT_FLASH.bin", FileMode.Truncate));

            for (int height = 0; height < bitmap.Height; height++)
            {
                for (int width = 0; width < bitmap.Width; width++)
                {
                    Color colorAtPixel = fastBitmap.GetPixel(width, height);
                    //string hexColor = HexConverter(colorAtPixel);
                    //writer.Write(hexColor);
                    int Rd = Convert.ToInt32(colorAtPixel.R.ToString("X2"), 16);
                    int Gd = Convert.ToInt32(colorAtPixel.G.ToString("X2"), 16);
                    int Bd = Convert.ToInt32(colorAtPixel.B.ToString("X2"), 16);

                    coverterWriter.Write((byte)Bd);
                    coverterWriter.Write((byte)Gd);
                    coverterWriter.Write((byte)Rd);
                }
            }

            fastBitmap.UnlockImage();

            coverterWriter.Close();
            coverterWriter.Dispose();


            byte[] convertedImage = File.ReadAllBytes(Path.GetDirectoryName(path) + "\\..\\data\\image-converted_DO_NOT_FLASH.bin");
            byte[] logo = File.ReadAllBytes(Path.GetDirectoryName(path) + "\\..\\data\\logo-orig.bin");

            for (int i = 0; i < logo.Length; i++)
            {
                if (i >= 512 && i < 6221312)
                {
                    logo[i] = convertedImage[i - 512];
                }
            }

            if (logo.Length != 9821696) return;
            File.WriteAllBytes(Path.GetDirectoryName(path) + "\\..\\output\\logo-modified.bin", logo);
            Console.WriteLine("\tThe size of the flashable file is correct! Everything is ok.");
            Console.Out.WriteLine("");
            Console.Out.WriteLine("\tDone! You may now flash /output/logo-modified.bin as LOGO in fastboot.");
            Console.ReadLine();
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
