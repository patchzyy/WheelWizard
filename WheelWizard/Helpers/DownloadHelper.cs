﻿using CT_MKWII_WPF.Views;
using CT_MKWII_WPF.Views.Popups;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CT_MKWII_WPF.Helpers
{
    public static class DownloadHelper
    {
        private const int MaxRetries = 5;
        private const int TimeoutInSeconds = 30; // Adjust as needed

        // Not providing a progress window will just use a default one
        public static async Task DownloadToLocation(string url, string filePath, string windowTitle,
            string extraText = "")
        {
            var progressWindow = new ProgressWindow(windowTitle).SetExtraText(extraText);
            progressWindow.Show();
            await DownloadToLocation(url, filePath, progressWindow);
            progressWindow.Close();
        }

        public static async Task DownloadToLocation(string url, string filePath, ProgressWindow progressPopupWindow)
        {
            var directory = Path.GetDirectoryName(filePath)!;
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            int attempt = 0;
            bool success = false;

            while (attempt < MaxRetries && !success)
            {
                try
                {
                    using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(TimeoutInSeconds) };

                    using var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
                    response.EnsureSuccessStatusCode();

                    // Extract file name from Content-Disposition or URL
                    var fileName = Path.GetFileName(filePath);
                    if (response.Content.Headers.ContentDisposition?.FileName != null)
                    {
                        fileName = response.Content.Headers.ContentDisposition.FileName.Trim('"');
                    }
                    else if (!Path.HasExtension(fileName))
                    {
                        // Add extension from URL if missing in file path
                        var urlExtension = Path.GetExtension(new Uri(url).AbsolutePath);
                        if (!string.IsNullOrEmpty(urlExtension))
                        {
                            fileName += urlExtension;
                        }
                    }

                    filePath = Path.Combine(directory, fileName); // Update filePath with extension if added

                    var totalBytes = response.Content.Headers.ContentLength ?? -1;
                    progressPopupWindow.SetGoal(totalBytes / (1024.0 * 1024.0));

                    await using var downloadStream = await response.Content.ReadAsStreamAsync();
                    await using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write,
                        FileShare.None,
                        bufferSize: 8192, useAsync: true);
                    var downloadedBytes = 0L;
                    const int bufferSize = 8192;
                    var buffer = new byte[bufferSize];
                    int bytesRead;

                    while ((bytesRead = await downloadStream.ReadAsync(buffer, 0, bufferSize)) != 0)
                    {
                        await fileStream.WriteAsync(buffer, 0, bytesRead);
                        downloadedBytes += bytesRead;

                        var progress = totalBytes == -1 ? 0 : (int)((float)downloadedBytes / totalBytes * 100);
                        progressPopupWindow.Dispatcher.Invoke(() => progressPopupWindow.UpdateProgress(progress));
                    }

                    success = true; // Download completed successfully
                }
                catch (TaskCanceledException ex) when (!ex.CancellationToken.IsCancellationRequested)
                {
                    attempt++;
                    if (attempt >= MaxRetries)
                    {
                        MessageBox.Show($"Download timed out after {MaxRetries} attempts.");
                        break;
                    }
                    else
                    {
                        int delay = (int)Math.Pow(2, attempt) * 1000;
                        await Task.Delay(delay);
                        progressPopupWindow.Dispatcher.Invoke(() =>
                            progressPopupWindow.SetExtraText($"Retrying... Attempt {attempt + 1} of {MaxRetries}"));
                    }
                }
                catch (HttpRequestException ex)
                {
                    attempt++;
                    if (attempt >= MaxRetries)
                    {
                        MessageBox.Show($"An HTTP error occurred after {MaxRetries} attempts: {ex.Message}");
                        break;
                    }
                    else
                    {
                        int delay = (int)Math.Pow(2, attempt) * 1000;
                        await Task.Delay(delay);
                        progressPopupWindow.Dispatcher.Invoke(() =>
                            progressPopupWindow.SetExtraText($"Retrying... Attempt {attempt + 1} of {MaxRetries}"));
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"An error occurred while downloading the file: {ex.Message}");
                    break;
                }
            }
        }
    }
}
