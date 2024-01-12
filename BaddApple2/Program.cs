using GleamTech.VideoUltimate;
using System.Drawing;
using System.Text;

namespace BaddApple2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Fullscreen");
            Console.ReadLine();

            using (var videoFrameReader = new VideoFrameReader(@"C:\Users\Jason\Downloads\Bad Apple!! - Full Version w_video [Lyrics in Romaji, Translation in English].mp4"))
            {
                var test = videoFrameReader.GetFrame();
                int width = test.Width;
                int height = test.Height;

                while(videoFrameReader.Read())
                {
                    using(var image = videoFrameReader.GetFrame())
                    {
                        Bitmap bmp = (Bitmap)image;
                        StringBuilder _sb = new StringBuilder();
                        for(int i = 0; i < Console.BufferWidth; i++)
                        {
                            for (int j = 0; j < Console.BufferHeight; j++)
                            {
                                Color colorAt = bmp.GetPixel((i / Console.BufferWidth) * width, (i / Console.BufferHeight) * height);
                                _sb.Append(colorAt.R > 128 ? ' ' : '█');
                            }
                        }

                        Console.SetCursorPosition(0,0);
                        Console.Clear();
                        Console.Write(_sb);
                    }
                }
            }
        }
    }
}
