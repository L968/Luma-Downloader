using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;

namespace Luma
{
    public class Video
    {
        public string Url { get; set; }
        public string DestinationFolder { get; set; }
        public string Title { get; set; }

        public Video(string url, string destinationFolder, string title)
        {
            Url = url;
            DestinationFolder = destinationFolder;
            Title = title;
        }

        public async Task Download(IProgress<double> progress, CancellationToken ct)
        {
            var client = new YoutubeClient();
            // Get metadata for all streams in this video
            var streamInfoSet = await client.Videos.Streams.GetManifestAsync(Url);

            // Select one of the streams, e.g. highest quality muxed stream
            var streamInfo = streamInfoSet
                            .GetAudioOnly()
                            .Where(video => video.Container == Container.Mp4)
                            .WithHighestBitrate();

            var filePath = $@"{DestinationFolder}\{Title}.mp4";

            // Download stream to file
            await client.Videos.Streams.DownloadAsync(streamInfo, filePath, progress, ct);
        }


    }
}
