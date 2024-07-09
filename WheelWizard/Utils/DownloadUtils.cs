using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using CT_MKWII_WPF.Pages;

namespace CT_MKWII_WPF.Utils;

public class DownloadUtils
{
    public static async Task DownloadFileWithWindow(string url, string filePath, ProgressWindow progressWindow)
    {
        using (HttpClient client = new HttpClient())
        {
            using (var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
            {
                var totalBytes = response.Content.Headers.ContentLength ?? -1;
                using (var downloadStream = await response.Content.ReadAsStreamAsync())
                using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None, bufferSize: 8192, useAsync: true))
                {
                    var downloadedBytes = 0;
                    var bufferSize = 8192;
                    var buffer = new byte[bufferSize];
                    int bytesRead;

                    while ((bytesRead = await downloadStream.ReadAsync(buffer, 0, bufferSize)) != 0)
                    {
                        await fileStream.WriteAsync(buffer, 0, bytesRead);
                        downloadedBytes += bytesRead;

                        var progress = totalBytes == -1 ? 0 : (int)((float)downloadedBytes / totalBytes * 100);
                        var status = $"Downloading... {downloadedBytes}/{totalBytes} bytes";

                        progressWindow.Dispatcher.Invoke(() =>
                        {
                            progressWindow.UpdateProgress(progress, status);
                        });
                    }
                }
            }
        }
    }
}