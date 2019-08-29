using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using YoutubeExplode;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;
using Humanizer;
using Luma_Downloader.Properties;
using Luma_Downloader.FolderSelector;

namespace Luma_Downloader.UI
{
    public partial class frmMain : Form
    {
        private CancellationTokenSource _cts;

        public frmMain()
        {
            InitializeComponent();
            lblProgress.Text = null;
            txtDestinationFolder.Text = Settings.Default.DestinationFolder == "" ? Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) : Settings.Default.DestinationFolder;
            txtUrl.Select();
        }

        #region Buttons
        private void btnBrowse_Click(object sender, EventArgs e)
        {
            var browser = new FolderSelectDialog("Select Destination Folder", txtDestinationFolder.Text);
            if (browser.ShowDialog(IntPtr.Zero))
            {
                txtDestinationFolder.Text = browser.FileName;

                // Saving path to settings
                Settings.Default.DestinationFolder = browser.FileName;
                Settings.Default.Save();
            }
        }

        private void btnOpenFolder_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtDestinationFolder.Text))
                {
                    Process.Start(txtDestinationFolder.Text);
                }
                else
                {
                    AddLogMessage("Select a folder!", Message.Warning);
                }
            }
            catch (Exception)
            {
                AddLogMessage("Not a valid path!", Message.Warning);
            }
        }

        private async void btnDownload_Click(object sender, EventArgs e)
        {
            var st = new Stopwatch();

            try
            {
                ValidateBeforeDownload();
                UpdateUIBeforeDownload();
                st.Start();

                var progress = new Progress<double>(p => prgDownloadProgress.Value = Convert.ToInt32(p * 100));
                _cts = new CancellationTokenSource();

                if (rdDownloadPlaylist.Checked)
                {
                    var playlistId = YoutubeClient.ParsePlaylistId(txtUrl.Text);
                    await DownloadPlaylist(playlistId, txtDestinationFolder.Text, progress, _cts.Token);
                }
                else if (rdDownloadVideo.Checked)
                {
                    var videoId = YoutubeClient.ParseVideoId(txtUrl.Text);
                    var videoTitle = await Util.GetFilteredVideoTitle(videoId);
                    var video = new Video(videoId, txtDestinationFolder.Text, videoTitle);

                    await DownloadVideo(video, progress, _cts.Token);
                }

                st.Stop();
                AddLogMessage($"Download completed in {TimeSpanHumanizeExtensions.Humanize(st.Elapsed)}!", Message.Success);
            }
            catch (FormatException ex)
            {
                if (ex.Message.StartsWith("Could not parse playlist"))
                {
                    AddLogMessage("Invalid youtube playlist URL!", Message.Warning);
                    return;
                }
                else
                {
                    AddLogMessage("Invalid youtube video URL!", Message.Warning);
                    return;
                }
            }
            catch (UnauthorizedAccessException)
            {
                AddLogMessage("Access to the path is denied! Try selecting another folder...", Message.Warning);
                return;
            }
            catch (ApplicationException ex)
            {
                AddLogMessage(ex.Message, Message.Warning);
                return;
            }
            catch (OperationCanceledException)
            {
                st.Stop();
                AddLogMessage($"Operation cancelled after {TimeSpanHumanizeExtensions.Humanize(st.Elapsed)}!", Message.Warning);
                prgDownloadProgress.Value = 0;
                return;
            }
            finally
            {
                UpdateUIAfterDownload();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            _cts.Cancel();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtUrl.Text = null;
            lstDownloadLog.Items.Clear();
        }
        #endregion

        #region Downloaders
        private async Task DownloadPlaylist(string playlistId, string destinationFolder, IProgress<double> progress, CancellationToken ct)
        {
            var client = new YoutubeClient();
            var playlist = await client.GetPlaylistAsync(playlistId);

            var totalVideosInPlaylist = playlist.Videos.Count();

            lblProgress.Text = $"Total: 0/{totalVideosInPlaylist}";

            var i = 1;
            foreach (var item in playlist.Videos)
            {
                ct.ThrowIfCancellationRequested();
                try
                {
                    var videoTitle = Util.FilterVideoTitle(item.Title);
                    var video = new Video(item.Id, destinationFolder, videoTitle);

                    if (!File.Exists($@"{destinationFolder}\{video.Title}.mp4"))
                    {
                        await DownloadVideo(video, progress, ct);
                    }
                    else
                    {
                        AddLogMessage($"Already downloaded: {video.Title}", Message.Warning);
                    }
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    AddLogMessage($"ERROR on downloading this video: {ex.Message}", Message.Warning);
                }
                lblProgress.Text = $"Total: {i}/{totalVideosInPlaylist}";
                i++;
            }
        }

        private async Task DownloadVideo(Video video, IProgress<double> progress, CancellationToken ct)
        {    
            AddLogMessage($"Downloading: {video.Title}");

            await video.Download(progress, ct);

            AddLogMessage($"Downloaded!", Message.Success);

            prgDownloadProgress.Value = 0;
        }
        #endregion

        /// <summary>
        /// Validate UI components before start the download
        /// </summary>
        private void ValidateBeforeDownload()
        {
            if (string.IsNullOrWhiteSpace(txtDestinationFolder.Text))
            {
                throw new ApplicationException("Choose the download destination folder!");
            }

            if (!Directory.Exists(txtDestinationFolder.Text))
            {
                throw new ApplicationException("Not a existing/valid folder path!");
            }

            if (string.IsNullOrWhiteSpace(txtUrl.Text))
            {
                throw new ApplicationException("Select the playlist link!");
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
        private void AddLogMessage(string message, Message messageType)
        {
            switch (messageType)
            {
                case Message.Success:
                    lstDownloadLog.Items.Add(new Dictionary<string, object> {
                        { "Text", message},
                        { "ForeColor", Color.Green}
                    });
                    break;
                case Message.Warning:
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

        private void lstDownloadLog_DoubleClick(object sender, EventArgs e)
        {
            var selectedLine = lstDownloadLog.Items[lstDownloadLog.SelectedIndex] as Dictionary<string, object>;
            var lineMessage = selectedLine["Text"] as string;
            MessageBox.Show(lineMessage);
        }
        
    }

    public enum Message
    {
        Success,
        Warning
    }
}
