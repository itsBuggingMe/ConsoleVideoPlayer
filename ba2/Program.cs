using GleamTech.VideoUltimate;
using System.Text;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ba2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Fullscreen console. Enter file path or leave empty for bad apple");
            FConsole.Initialize("Console Media Player");
            string option = Console.ReadLine();

            using (var videoFrameReader = new VideoFrameReader(string.IsNullOrEmpty(option) ? $"{Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().FullName)}\\Bad Apple!! - Full Version w_video [Lyrics in Romaji, Translation in English].mp4" : option))
            {
                StringBuilder _sb = new StringBuilder();

                videoFrameReader.Read();
                var firstFrame = videoFrameReader.GetFrame();
                int width = firstFrame.Width;
                int height = firstFrame.Height;

                while (videoFrameReader.Read())
                {
                    int consoleBufferWidth = Console.BufferWidth;
                    int consoleBufferHeight = Console.BufferHeight;

                    int widthM = width / consoleBufferWidth;
                    int heightM = height / consoleBufferHeight;

                    using (var image = videoFrameReader.GetFrame())
                    {                        
                        byte[] data = image.ToPixelData();

                        for (int j = 0; j < consoleBufferHeight; j++)
                        {
                            for (int i = 0; i < consoleBufferWidth; i++)
                            {
                                int screenx = (i * widthM);
                                int screeny = (j * heightM);
                                int index = (screeny * image.Width + screenx) * 3;

                                FConsole.SetChar(
                                    (short)i, 
                                    (short)j, 
                                    new PixelValue(
                                        ClosestConsoleColor(data[index],
                                            data[index + 1], 
                                            data[index + 2])
                                        )
                                    );
                            }
                        }

                        data = null;
                        GC.Collect();

                        FConsole.DrawBuffer();
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ConsoleColor FromColor(byte r, byte g, byte b)
        {
            int index = (r > 128 | g > 128 | b > 128) ? 8 : 0; // Bright bit
            index |= (r > 64) ? 4 : 0; // Red bit
            index |= (r > 64) ? 2 : 0; // Green bit
            index |= (b > 64) ? 1 : 0; // Blue bit
            return (ConsoleColor)index;
        }

        static ConsoleColor ClosestConsoleColor(byte r, byte g, byte b)
        {
            ConsoleColor ret = 0;
            double rr = r, gg = g, bb = b, delta = double.MaxValue;

            foreach (ConsoleColor cc in Enum.GetValues(typeof(ConsoleColor)))
            {
                var n = Enum.GetName(typeof(ConsoleColor), cc);
                var c = System.Drawing.Color.FromName(n == "DarkYellow" ? "Orange" : n); // bug fix
                var t = Math.Pow(c.R - rr, 2.0) + Math.Pow(c.G - gg, 2.0) + Math.Pow(c.B - bb, 2.0);
                if (t == 0.0)
                    return cc;
                if (t < delta)
                {
                    delta = t;
                    ret = cc;
                }
            }
            return ret;
        }
    }
}
