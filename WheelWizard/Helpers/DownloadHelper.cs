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
    public static async Task DownloadToLocation(string url, string filePath, string windowTitle, string extraText = "")
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

        try
        {
            using var client = new HttpClient();
            using var response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);
            var totalBytes = response.Content.Headers.ContentLength ?? -1;
            progressPopupWindow.SetGoal(totalBytes / (1024.0 * 1024.0));
            
            await using var downloadStream = await response.Content.ReadAsStreamAsync();
            await using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None,
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
        }
        catch (Exception x)
        {
            // I rather not translate this message, makes it easier to check where a given error came from
            MessageBox.Show($"An error occurred while downloading the file: {x.Message}");
        }
    }
}
