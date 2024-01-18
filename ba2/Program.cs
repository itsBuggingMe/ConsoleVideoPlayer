using GleamTech.VideoUltimate;
using Spectre.Console;
using System.Diagnostics;
using NAudio;
using NAudio.Wave;

namespace ba2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Fullscreen console. Enter file path or leave empty for bad apple");

            string option = Console.ReadLine();
            option = @"G:\My Drive\APCSA\testVideo.mp4";
            option = @"C:\Users\Jason\Downloads\out.mp4";

            option = option.Replace("\"", "");

            var asb = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string finalPath = string.IsNullOrEmpty(option) ? $"{Path.GetDirectoryName(asb)}\\Bad Apple!! - Full Version w_video [Lyrics in Romaji, Translation in English].mp4" : option;

            ConsoleVideoPlayer player = new ConsoleVideoPlayer(finalPath, titleStats: true);
            player.Play();
            player.Dispose();
        }
    }
}
