using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using ViViD.VimeoPoco;

namespace ViViD
{
    class Program
    {
        const string searchTag = "window.vimeo.clip_page_config = ";

        static void Main()
        {
            Console.Title = "ViViD - Vimeo Video Downloader";

            Console.Write("Enter Vimeo Video URL: ");
            var url = Console.ReadLine();

            /*
             * This part is pretty much all the use we have for HtmlAgilityPack.
             * Feel free to do without it if you like...
            */
            var doc = GetHtmlDocument(url);

            var node = doc.DocumentNode.Descendants()
                .FirstOrDefault(n => n.Name == "script" &&
                n.InnerText.Contains(searchTag));

            if (node == null)
            {
                Console.WriteLine("Unable to find base config. Press any key to exit.");
                Console.ReadKey();
                return;
            }

            /*
             * Yuck! Using Jurassic would have been cooler to extract the clips'
             * json metadata, but it seems to brittle for use with webpage js.
            */
            var json = node.InnerText
                    .SubstringAfter(searchTag)
                    .SubstringBefore(";\n\n    function autoplayOnsiteReferrals()");

            var clipPageConfig = JsonConvert.DeserializeObject<ClipPageConfig>(json);
            var playerConfig = GetJsonObject<PlayerConfig>(clipPageConfig.Player.ConfigUrl);

            /*
             * The approach below (Progressive) should be more consistent and
             * provide just one URL for download - which is simpler. But what's
             * the fun in that?
            */
            if (playerConfig.Request.Files.Dash == null)
            {
                var progressive = playerConfig.Request.Files.Progressive.OrderByDescending(f => f.Width).FirstOrDefault();

                using (var webClient = new WebClient())
                {
                    webClient.DownloadFile(progressive.Url, $"{playerConfig.Video.Title}.mp4");
                }
            }
            else
            {
                /*
                 * The provided config url ends with a base64_init=1 query string.
                 * This sets the video[x]>init_segment value to a base-64 encoded
                 * mp4 video header. Without this parameter the init_segment points
                 * to a file, similar to the other segments.
                */
                var masterConfig = GetJsonObject<MasterConfig>(playerConfig.Request.Files.Dash.Cdns.FastlySkyfire.Url.SubstringBefore("?"));

                var uri = new Uri(playerConfig.Request.Files.Dash.Cdns.FastlySkyfire.Url);
                var path = Path.Combine(Path.GetDirectoryName(uri.AbsolutePath), masterConfig.BaseUrl);

                var vidInfo = masterConfig.Video.OrderByDescending(v => v.AverageBitrate).FirstOrDefault();
                Console.WriteLine("Downloading video stream...");
                DownloadAudioVideoSegments(vidInfo, uri, path, "video.mp4");

                var audioInfo = masterConfig.Audio.OrderByDescending(v => v.AverageBitrate).FirstOrDefault();
                Console.WriteLine("Downloading audio stream...");
                DownloadAudioVideoSegments(audioInfo, uri, path, "audio.mp4");

                var ffmpegBinPath = ConfigurationManager.AppSettings["FFmpegBinPath"];
                if (!string.IsNullOrWhiteSpace(ffmpegBinPath))
                {
                    Console.WriteLine("Found FFmpeg... Merging streams...");

                    using (var proc = Process.Start(
                        new ProcessStartInfo(Path.Combine(ffmpegBinPath, "ffmpeg.exe"), $"-i video.mp4 -i audio.mp4 -c copy \"{playerConfig.Video.Title}.mp4\"")
                        {
                            CreateNoWindow = true
                        }))
                    {
                        proc.WaitForExit();
                    }
                }
            }

            Console.Write("Done! Press any key to exit...");
            Console.ReadKey();
        }

        private static void DownloadAudioVideoSegments(AudioVideo av, Uri basePath, string relativePath, string savePath)
        {
            var segments = av.Segments.Select(s => s.Url).ToList();
            segments.Insert(0, av.InitialSegment);

            using (var fs = new FileStream(savePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                for (int i = 0; i < segments.Count; i++)
                {
                    Console.WriteLine($"\tFetching segment {i + 1} of {segments.Count}...");

                    /*
                     * There must be a more decent approach to doing this...
                     * But, alas, we're working with paths if this was
                     * a local path, and then finally changing back to URL.
                     * 
                     * Path.GetFullPath() is used to resolve special paths,
                     * such as ../
                    */
                    var tmpPath = Path.Combine(relativePath, av.BaseUrl);
                    tmpPath = Path.Combine(tmpPath, segments[i]);
                    tmpPath = Path.GetFullPath(tmpPath);
                    tmpPath = tmpPath.SubstringAfter(Path.GetPathRoot(tmpPath))
                        .Replace("\\", "/");

                    var uri = new Uri($"{basePath.Scheme}://{basePath.Host}/{tmpPath}");

                    using (var webClient = new WebClient())
                    {
                        var data = webClient.DownloadData(uri);
                        fs.Write(data, 0, data.Length);
                    }
                }

                fs.Flush();
                fs.Close();
            }
        }

        private static HtmlDocument GetHtmlDocument(string url)
        {
            var doc = new HtmlDocument();

            using (var webClient = new WebClient())
            {
                // Load() by Url method is error-prone
                doc.LoadHtml(webClient.DownloadString(url));
            }

            return doc;
        }

        private static T GetJsonObject<T>(string url)
        {
            using (var webClient = new WebClient())
            {
                return JsonConvert.DeserializeObject<T>(webClient.DownloadString(url));
            }
        }
    }
}