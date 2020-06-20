using System;
using System.IO;
using Humanizer;
using System.Linq;
using System.Drawing;
using YoutubeExplode;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Collections.Generic;
using Luma.Properties;
using Luma.FolderSelector;

namespace Luma.UI
{
    public partial class frmMain : Form
    {
        private CancellationTokenSource _cancellationTokenSource;

        public frmMain()
        {
            InitializeComponent();
            lblProgress.Text = null;
            txtUrl.Select();
        }

        #region Buttons
        private async void btnDownload_Click(object sender, EventArgs e)
        {
            var stopWatch = new Stopwatch();

            try
            {
                ValidateBeforeDownload();
                var destinationFolder = OpenSelectFolderDialog();
                var url = txtUrl.Text;
                UpdateUIBeforeDownload();
                stopWatch.Start();

                var progress = new Progress<double>(p => prgDownloadProgress.Value = Convert.ToInt32(p * 100));
                _cancellationTokenSource = new CancellationTokenSource();

                AddLogMessage("Downloading in: " + destinationFolder);

                if (rdDownloadPlaylist.Checked)
                {
                    await DownloadPlaylist(url, destinationFolder, progress, _cancellationTokenSource.Token);
                }
                else if (rdDownloadVideo.Checked)
                {
                    var videoTitle = await Util.GetFilteredVideoTitle(url);
                    var video = new Video(url, destinationFolder, videoTitle);

                    await DownloadVideo(video, progress, _cancellationTokenSource.Token);
                }

                stopWatch.Stop();
                AddLogMessage($"Download completed in {TimeSpanHumanizeExtensions.Humanize(stopWatch.Elapsed)}!", MessageType.Success);
            }
            catch (UnauthorizedAccessException)
            {
                AddLogMessage("Access to the path is denied! Try selecting another folder", MessageType.Error);
                return;
            }
            catch (ApplicationException ex)
            {
                AddLogMessage(ex.Message, MessageType.Error);
                return;
            }
            catch (OperationCanceledException)
            {
                AddLogMessage($"Operation cancelled after {TimeSpanHumanizeExtensions.Humanize(stopWatch.Elapsed)}!", MessageType.Warning);
                return;
            }
            catch(Exception ex)
            {
                AddLogMessage("Error: " + ex.Message, MessageType.Error);
            }
            finally
            {
                UpdateUIAfterDownload();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _cancellationTokenSource.Cancel();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            lstDownloadLog.Items.Clear();
        }
        #endregion

        #region Downloaders
        private async Task DownloadPlaylist(string playlistUrl, string destinationFolder, IProgress<double> progress, CancellationToken ct)
        {
            var client = new YoutubeClient();
            var videos = await client.Playlists.GetVideosAsync(playlistUrl);

            var totalVideosInPlaylist = videos.Count();

            lblProgress.Text = $"Total: 0/{totalVideosInPlaylist}";

            var i = 1;
            foreach (var item in videos)
            {
                ct.ThrowIfCancellationRequested();
                try
                {
                    var videoTitle = Util.FilterVideoTitle(item.Title);
                    var video = new Video(item.Id, destinationFolder, videoTitle);

                    if (!File.Exists($@"{destinationFolder}\{video.Title}.mp3"))
                    {
                        await DownloadVideo(video, progress, ct);
                    }
                    else
                    {
                        AddLogMessage($"There's already a .mp3 file with this name: {video.Title}", MessageType.Warning);
                    }
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    AddLogMessage($"ERROR on downloading this video: {ex.Message}", MessageType.Error);
                }
                lblProgress.Text = $"Total: {i}/{totalVideosInPlaylist}";
                i++;
            }
        }

        private async Task DownloadVideo(Video video, IProgress<double> progress, CancellationToken ct)
        {
            AddLogMessage($"Downloading: {video.Title}");

            await video.Download(progress, ct);

            AddLogMessage($"Downloaded successfully!", MessageType.Success);

            await StartConvertingToMp3(video);

            prgDownloadProgress.Value = 0;
        }
        #endregion

        /// <summary>
        /// Validate UI components before start the download
        /// </summary>
        private void ValidateBeforeDownload()
        {
            if (string.IsNullOrWhiteSpace(txtUrl.Text))
            {
                throw new ApplicationException("Select the playlist/video url!");
            }
        }

        private void UpdateUIBeforeDownload()
        {
            lblProgress.Text = null;
            btnCancel.Enabled = true;
            btnDownload.Enabled = false;
        }

        private void UpdateUIAfterDownload()
        {
            btnDownload.Enabled = true;
            btnCancel.Enabled = false;
            prgDownloadProgress.Value = 0;
        }

        /// <summary>
        /// Adds a standart message in the UI download log
        /// </summary>
        /// <param name="message">The message to be shown</param>
        private void AddLogMessage(string message)
        {
            lstDownloadLog.Items.Add(new Dictionary<string, object> {
                { "Text", message}
            });

            if (chkAutoScroll.Checked)
            {
                lstDownloadLog.SelectedIndex = lstDownloadLog.Items.Count - 1;
                lstDownloadLog.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// Add a custom color message in the UI download log
        /// </summary>
        /// <param name="message">The message to be shown</param>
        /// <param name="messageType">The message type</param>
        private void AddLogMessage(string message, MessageType messageType)
        {
            switch (messageType)
            {
                case MessageType.Success:
                    lstDownloadLog.Items.Add(new Dictionary<string, object> {
                        { "Text", message},
                        { "ForeColor", Color.Green}
                    });
                    break;
                case MessageType.Warning:
                    lstDownloadLog.Items.Add(new Dictionary<string, object> {
                        { "Text", message},
                        { "ForeColor", Color.DarkOrange}
                    });
                    break;
                case MessageType.Error:
                    lstDownloadLog.Items.Add(new Dictionary<string, object> {
                        { "Text", message},
                        { "ForeColor", Color.Red}
                    });
                    break;
            }

            if (chkAutoScroll.Checked)
            {
                lstDownloadLog.SelectedIndex = lstDownloadLog.Items.Count - 1;
                lstDownloadLog.SelectedIndex = -1;
            }
        }

        /// <summary>
        /// The UI function to insert rows in the listbox with different colors
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lstDownloadLog_DrawItem(object sender, DrawItemEventArgs e)
        {
            try
            {
                Graphics g = e.Graphics;
                Dictionary<string, object> props = lstDownloadLog.Items[e.Index] as Dictionary<string, object>;
                SolidBrush backgroundBrush = new SolidBrush(props.ContainsKey("BackColor") ? (Color)props["BackColor"] : e.BackColor);
                SolidBrush foregroundBrush = new SolidBrush(props.ContainsKey("ForeColor") ? (Color)props["ForeColor"] : e.ForeColor);
                Font textFont = props.ContainsKey("Font") ? (Font)props["Font"] : e.Font;
                string text = props.ContainsKey("Text") ? (string)props["Text"] : string.Empty;
                RectangleF rectangle = new RectangleF(new PointF(e.Bounds.X, e.Bounds.Y), new SizeF(e.Bounds.Width, g.MeasureString(text, textFont).Height));

                g.FillRectangle(backgroundBrush, rectangle);
                g.DrawString(text, textFont, foregroundBrush, rectangle);

                backgroundBrush.Dispose();
                foregroundBrush.Dispose();
                g.Dispose();
            }
            catch (Exception)
            {
            }
        }

        private void lstDownloadLog_DoubleClick(object sender, EventArgs e)
        {
            if (lstDownloadLog.SelectedIndex != -1)
            {
                var selectedLine = lstDownloadLog.Items[lstDownloadLog.SelectedIndex] as Dictionary<string, object>;
                var lineMessage = selectedLine["Text"] as string;
                MessageBox.Show(lineMessage);
            }
        }

        private string OpenSelectFolderDialog()
        {
            var folder = string.IsNullOrWhiteSpace(Settings.Default.DestinationFolder) ? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) : Settings.Default.DestinationFolder;
            var browser = new FolderSelectDialog("Select Destination Folder", folder);

            if (browser.ShowDialog(IntPtr.Zero))
            {
                SaveDestinationFolder(browser.FileName);
                return browser.FileName;
            }
            else
            {
                throw new ApplicationException("");
            }
        }

        private void SaveDestinationFolder(string destinationFolder)
        {
            Settings.Default.DestinationFolder = destinationFolder;
            Settings.Default.Save();
        }

        private async Task StartConvertingToMp3(Video video)
        {
            AddLogMessage("Converting to mp3...Do not close the application");

            var filePath = $@"{video.DestinationFolder}\{video.Title}.mp4";
            prgDownloadProgress.Style = ProgressBarStyle.Marquee;

            await ConvertToMp3(filePath);

            AddLogMessage("Converted successfully!", MessageType.Success);
            prgDownloadProgress.Style = ProgressBarStyle.Blocks;
        }

        private Task ConvertToMp3(string filePath)
        {
            return Task.Factory.StartNew(() =>
            {
                var converter = new NReco.VideoConverter.FFMpegConverter();
                converter.ConvertMedia(filePath, filePath.Replace(".mp4", ".mp3"), "mp3");
                File.Delete(filePath); // Deletes the old mp4 file
            });
        }

    }

    public enum MessageType
    {
        Success,
        Warning,
        Error
    }
}
