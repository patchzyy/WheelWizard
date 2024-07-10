﻿using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Diagnostics;
using CT_MKWII_WPF.Pages;

namespace CT_MKWII_WPF.Utils;

public class DownloadUtils
{
    public static async Task DownloadFileWithWindow(string url, string filePath, ProgressWindow progressWindow, string extratext = "")
    {
        using (HttpClient client = new HttpClient())
        {
            using (var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
            {
                var totalBytes = response.Content.Headers.ContentLength ?? -1;
                using (var downloadStream = await response.Content.ReadAsStreamAsync())
                using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 8192, useAsync: true))
                {
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
                        var downloadedMB = downloadedBytes / (1024.0 * 1024.0);
                        var totalMB = totalBytes / (1024.0 * 1024.0);
                        var status = $"Downloading... {downloadedMB:F2}/{totalMB:F2} MB";

                        var elapsedSeconds = stopwatch.Elapsed.TotalSeconds;
                        var speedMBps = downloadedMB / elapsedSeconds;
                        var remainingMB = totalMB - downloadedMB;
                        var estimatedTimeRemaining = remainingMB / speedMBps;

                        var bottomText = $"Speed: {speedMBps:F2} MB/s | Estimated time remaining: {FormatTimeSpan(estimatedTimeRemaining)}";

                        progressWindow.Dispatcher.Invoke(() =>
                        {
                            progressWindow.UpdateProgress(progress, status, extratext);
                            progressWindow.BottomTextLabel.Text = bottomText;
                        });
                    }
                }
            }
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