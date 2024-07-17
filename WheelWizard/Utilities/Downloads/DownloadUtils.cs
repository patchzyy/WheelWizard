﻿using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using CT_MKWII_WPF.Views;

namespace CT_MKWII_WPF.Utilities.Downloads;

public static class DownloadUtils
{
    public static async Task DownloadFileWithWindow(string url, string filePath, ProgressWindow progressWindow, 
        string bottomWindowText = "")
    {
        if (filePath == null) throw new ArgumentNullException(nameof(filePath));
        var directory = Path.GetDirectoryName(filePath)!;
        if (!Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
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
                    $"Speed: {speedMBps:F2} MB/s | Estimated time remaining: {FormatTimeSpan(estimatedTimeRemaining)}";

                progressWindow.Dispatcher.Invoke(() =>
                {
                    progressWindow.UpdateProgress(progress, status, bottomWindowText);
                    progressWindow.BottomTextLabel.Text = bottomText;
                });
            }
        }
        catch (Exception x)
        {
            MessageBox.Show($"An error occurred while downloading the file: {x.Message}");
        }
    }

    private static string FormatTimeSpan(double seconds)
    {
        var timeSpan = TimeSpan.FromSeconds(seconds);
        if (timeSpan.TotalHours >= 1)
            return $"{timeSpan.TotalHours:F1} hours";
        else if (timeSpan.TotalMinutes >= 1)
            return $"{timeSpan.TotalMinutes:F1} minutes";
        else
            return $"{timeSpan.TotalSeconds:F0} seconds";
    }
}