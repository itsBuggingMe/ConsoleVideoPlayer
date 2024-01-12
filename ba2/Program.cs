using GleamTech.VideoUltimate;
using System.Text;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace ba2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Fullscreen console. Enter file path or leave empty for bad apple");
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

                                _sb.Append(data[(screeny * image.Width + screenx) * 3] < 128 ? ' ' : '█');
                            }
                        }

                        Console.SetCursorPosition(0, 0);
                        Console.Write(_sb);
                        _sb.Clear();

                        Thread.Sleep(50);
                    }
                }
            }
        }
    }
}
