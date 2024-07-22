using CT_MKWII_WPF.Views;
using CT_MKWII_WPF.Views.Popups;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CT_MKWII_WPF.Helpers;

public static class DownloadHelper
{
    // Not providing a progress window will just use a default one
    public static async Task DownloadToLocation(string url, string filePath)
    {
        var progressWindow = new ProgressWindow();
        progressWindow.Show();
        await DownloadToLocation(url, filePath, progressWindow);
        progressWindow.Close();
    }

    public static async Task DownloadToLocation(string url, string filePath, ProgressWindow progressWindow)
    {
        var directory = Path.GetDirectoryName(filePath)!;
        if (!Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        try
        {
            using var client = new HttpClient();
            using var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
            var totalBytes = response.Content.Headers.ContentLength ?? -1;
            await using var downloadStream = await response.Content.ReadAsStreamAsync();
            await using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None,
                bufferSize: 8192, useAsync: true);
            var downloadedBytes = 0L;
            var bufferSize = 8192;
            var buffer = new byte[bufferSize];
            int bytesRead;
            var stopwatch = Stopwatch.StartNew();

            while ((bytesRead = await downloadStream.ReadAsync(buffer, 0, bufferSize)) != 0)
            {
                await fileStream.WriteAsync(buffer, 0, bytesRead);
                downloadedBytes += bytesRead;

                var progress = totalBytes == -1 ? 0 : (int)((float)downloadedBytes / totalBytes * 100);
                var downloadedMb = downloadedBytes / (1024.0 * 1024.0);
                var totalMb = totalBytes / (1024.0 * 1024.0);
                var status = $"Downloading... {downloadedMb:F2}/{totalMb:F2} MB";

                var elapsedSeconds = stopwatch.Elapsed.TotalSeconds;
                var speedMBps = downloadedMb / elapsedSeconds;
                var remainingMb = totalMb - downloadedMb;
                var estimatedTimeRemaining = remainingMb / speedMBps;

                var bottomText =
                    $"Speed: {speedMBps:F2} MB/s | Estimated time remaining: " +
                    Humanizer.HumanizeTimeSpan(TimeSpan.FromSeconds(estimatedTimeRemaining));

                progressWindow.Dispatcher.Invoke(() =>
                {
                    progressWindow.UpdateProgress(progress, status, bottomText);
                });
            }
        }
        catch (Exception x)
        {
            MessageBox.Show($"An error occurred while downloading the file: {x.Message}");
        }
    }
}
