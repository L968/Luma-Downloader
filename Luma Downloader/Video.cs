using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Models.MediaStreams;

namespace Luma_Downloader
{
    public class Video
    {
        public string Id { get; set; }
        public string DestinationFolder { get; set; }
        public string Title { get; set; }

        public Video(string id, string destinationFolder, string title)
        {
            Id = id;
            DestinationFolder = destinationFolder;
            Title = title;
        }

        public async Task Download(IProgress<double> progress, CancellationToken ct)
        {
            var client = new YoutubeClient();
            // Get metadata for all streams in this video
            var streamInfoSet = await client.GetVideoMediaStreamInfosAsync(Id);

            // Select one of the streams, e.g. highest quality muxed stream
            var streamInfo = streamInfoSet
                            .Audio
                            .Where(video => video.Container.GetFileExtension().Contains("mp4"))
                            .First();

            // Get file extension based on stream's container
            var ext = streamInfo.Container.GetFileExtension();
            var filePath = $@"{DestinationFolder}\{Title}.{ext}";

            // Download stream to file
            await client.DownloadMediaStreamAsync(streamInfo, filePath, progress, ct);
        }


    }
}
