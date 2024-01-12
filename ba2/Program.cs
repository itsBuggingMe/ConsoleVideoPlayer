using GleamTech.VideoUltimate;
using System.Text;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Spectre.Console;

namespace ba2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Fullscreen console. Enter file path or leave empty for bad apple");

            //string option = Console.ReadLine();
            string option = @"G:\My Drive\APCSA\testVideo.mp4";
            option = @"G:\My Drive\APCSA\videoplayback.mp4";

            using (var videoFrameReader = new VideoFrameReader(string.IsNullOrEmpty(option) ? $"{Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().FullName)}\\Bad Apple!! - Full Version w_video [Lyrics in Romaji, Translation in English].mp4" : option))
            {
                videoFrameReader.Read();
                var firstFrame = videoFrameReader.GetFrame();
                int width = firstFrame.Width;
                int height = firstFrame.Height;

                Console.Clear();


                while (videoFrameReader.Read())
                {
                    int consoleBufferWidth = Console.WindowWidth;
                    int consoleBufferHeight = Console.WindowHeight + 2;

                    var canvas = new Canvas(consoleBufferWidth >> 1, consoleBufferHeight);

                    int widthM = width / consoleBufferWidth;
                    int heightM = height / consoleBufferHeight;

                    using (var image = videoFrameReader.GetFrame())
                    {                        
                        byte[] data = image.ToPixelData();

                        for (int j = 0; j < consoleBufferHeight - 1; j++)
                        {
                            for (int i = 0; i < consoleBufferWidth - 1; i++)
                            {
                                int screenx = (i * widthM);
                                int screeny = (j * heightM);
                                int index = (screeny * image.Width + screenx) * 3;
                                    
                                canvas.SetPixel(i >> 1,j, new Spectre.Console.Color(
                                    data[index + 2],
                                    data[index + 1],
                                    data[index]
                                    ));
                            }
                        }

                        data = null;
                        GC.Collect();

                        //Console.Clear();

                        Console.SetCursorPosition(0,0);
                        AnsiConsole.Write(canvas);
                        //Thread.Sleep(50);
                    }
                }
            }
        }
    }
}
