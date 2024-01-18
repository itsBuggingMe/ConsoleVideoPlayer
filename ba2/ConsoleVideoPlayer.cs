using GleamTech.VideoUltimate;
using NAudio.Wave;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ba2
{
    internal class ConsoleVideoPlayer : IDisposable
    {
        private VideoFrameReader _videoFrameReader;

        int width;
        int height;

        double msPerFrame;

        Stopwatch stopwatch;

        int consoleBufferWidth;
        int consoleBufferHeight;
        Canvas? canvas;
        int widthM;
        int heightM;

        private string path;
        private float Volume;
        private bool titleStats;

        public ConsoleVideoPlayer(string path, float initalVolume = 1, bool titleStats = false)
        {
            this.titleStats = titleStats;
            Volume = initalVolume;
            this.path = path;

            _videoFrameReader = new VideoFrameReader(path);

            msPerFrame = (1000 / _videoFrameReader.FrameRate);
            width = _videoFrameReader.Width;
            height = _videoFrameReader.Height;

            stopwatch = new();

            RegenerateInfo();
        }

        public void Play()
        {
            Task.Run(PlayAudio);

            while (_videoFrameReader.Read())
            {
                stopwatch.Start();

                using (var image = _videoFrameReader.GetFrame())
                {
                    AddPixelData(image);

                    Console.SetCursorPosition(0, 0);
                    AnsiConsole.Write(canvas ?? throw new NullReferenceException());

                    if (consoleBufferWidth != Console.WindowWidth || consoleBufferHeight != Console.WindowHeight - 1)
                        RegenerateInfo();

                    if(titleStats)
                    {
                        Console.Title = $"Frame: {_videoFrameReader.CurrentFrameNumber} Volume: {(int)Math.Round(Volume * 100)}%";
                    }

                    double currentMs = stopwatch.ElapsedMilliseconds;
                    while (currentMs > msPerFrame)
                    {//running slowly
                        currentMs -= msPerFrame;
                        _videoFrameReader.Read();
                    }

                    while (stopwatch.ElapsedMilliseconds < msPerFrame)
                    {/*running quickly*/}

                    stopwatch.Reset();
                }
            }
        }

        private void PlayAudio()
        {
            using (var audioFile = new AudioFileReader(path))
            using (var outputDevice = new WaveOutEvent())
            {
                outputDevice.Init(audioFile);
                outputDevice.Play();

                while (outputDevice.PlaybackState == PlaybackState.Playing)
                {
                    if (Console.KeyAvailable)
                    {
                        var k = Console.ReadKey();
                        float locV = Volume;
                        if (k.Key == ConsoleKey.LeftArrow)
                            locV -= 0.1f;
                        if (k.Key == ConsoleKey.RightArrow)
                            locV += 0.1f;

                        Volume = Math.Clamp(locV, 0, 1);
                    }

                    if (Volume != outputDevice.Volume)
                    {
                        outputDevice.Volume = Volume;
                    }

                    Thread.Sleep(100);
                }
            }
        }

        private void RegenerateInfo()
        {
            consoleBufferWidth = Console.WindowWidth;
            consoleBufferHeight = Console.WindowHeight - 1;

            canvas = new Canvas(consoleBufferWidth >> 1, consoleBufferHeight);

            widthM = width / consoleBufferWidth;
            heightM = height / consoleBufferHeight;
        }

        private void AddPixelData(GleamTech.Drawing.Image image)
        {
            var data = image.ToPixelData();

            for (int j = 0; j < consoleBufferHeight - 1; j++)
            {
                for (int i = 0; i < consoleBufferWidth - 1; i++)
                {
                    int screenx = (i * widthM);
                    int screeny = (j * heightM);
                    int index = (screeny * image.Width + screenx) * 3;

                    canvas?.SetPixel(i >> 1, j, new Spectre.Console.Color(
                        data[index + 2],
                        data[index + 1],
                        data[index]
                        ));
                }
            }
        }

        public void Dispose()
        {
            _videoFrameReader.Dispose();
        }
    }
}
