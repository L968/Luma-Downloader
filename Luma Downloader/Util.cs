using System.IO;
using System.Threading.Tasks;
using YoutubeExplode;

namespace Luma_Downloader
{
    internal static class Util
    {

        /// <summary>
        /// Get's the video title already filtered by invalid path/file characters
        /// </summary>
        /// <param name="videoId">The video URL Id</param>
        /// <returns></returns>
        public static async Task<string> GetFilteredVideoTitle(string videoId)
        {
            var client = new YoutubeClient();
            var videoInfo = await client.Videos.GetAsync(videoId);
            var videoTitle = videoInfo.Title;

            videoTitle = FilterVideoTitle(videoTitle);

            return videoTitle;
        }

        /// <summary>
        /// Filters invalid file characters in order to save the video file with it's title
        /// </summary>
        /// <param name="videoTitle">The video URL Id</param>
        /// <returns></returns>
        public static string FilterVideoTitle(string videoTitle)
        {
            string invalid = new string(Path.GetInvalidFileNameChars()) + new string(Path.GetInvalidPathChars());
            foreach (var c in invalid)
            {
                videoTitle = videoTitle.Replace(c.ToString(), "");
            }

            return videoTitle;
        }

    }
}
