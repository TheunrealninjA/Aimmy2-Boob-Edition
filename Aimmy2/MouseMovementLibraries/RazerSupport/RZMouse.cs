using Aimmy2.Other;
using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Windows;
using Visuality;

namespace Aimmy2.MouseMovementLibraries.RazerSupport
{
    internal class RZMouse
    {
        #region Razer Variables

        private const string rzctlpath = "rzctl.dll";
        private const string rzctlDownloadUrl = "https://github.com/MarsQQ/rzctl/releases/download/1.0.0/rzctl.dll";

        [DllImport(rzctlpath, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool init();

        [DllImport(rzctlpath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void mouse_move(int x, int y, bool starting_point);

        [DllImport(rzctlpath, CallingConvention = CallingConvention.Cdecl)]
        public static extern void mouse_click(int up_down);

        private static List<string> Razer_HID = [];

        #endregion Razer Variables


        public static async Task InstallRazerSynapse()
        {
            using HttpClient httpClient = new();
            var response = await httpClient.GetAsync(new Uri("https://rzr.to/synapse-new-pc-download-beta"));

            if (!response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsByteArrayAsync();
                await File.WriteAllBytesAsync($"{Path.GetTempPath()}\\rz.exe", content);

                Process.Start(new ProcessStartInfo
                {
                    WindowStyle = ProcessWindowStyle.Hidden,
                    FileName = "cmd.exe",
                    Arguments = "/C start rz.exe",
                    WorkingDirectory = Path.GetTempPath()
                });

                new NoticeBar("Razer Synapse downloaded, please look for UAC prompt and install Razer Synapse.", 4000).Show();
            }
        }

        private static async Task downloadrzctl()
        {
            try
            {
                FileManager.LogWarning($"{rzctlpath} is missing, attempting to download {rzctlpath}.", true);

                using HttpClient httpClient = new();
                using var response = await httpClient.GetAsync(new Uri(rzctlDownloadUrl), HttpCompletionOption.ResponseHeadersRead);

                if (response.IsSuccessStatusCode)
                {
                    using var contentStream = await response.Content.ReadAsStreamAsync();
                    using var fileStream = new FileStream(rzctlpath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true);
                    await contentStream.CopyToAsync(fileStream);
                    FileManager.LogInfo($"{rzctlpath} has downloaded successfully, please re-select Razer Synapse to load the DLL.", true, 4000);
                }
            }
            catch
            {
                FileManager.LogError($"{rzctlpath} has failed to install, please try a different Mouse Movement Method.", true);
            }
        }

        public static async Task<bool> Load()
        {
            if (!await RequirementsManager.CheckRazerSynapseInstall())
            {
                return false;
            }

            if (!File.Exists(rzctlpath))
            {
                await downloadrzctl();
                return false;
            }

            if (!RequirementsManager.CheckForRazerDevices(Razer_HID))
            {
                MessageBox.Show("No Razer Peripheral is detected, this Mouse Movement Method is unusable.", "Aimmy");
                return false;
            }

            try
            {
                return init();
            }
            catch (DllNotFoundException dllEx)
            {
                MessageBox.Show($"DLL loading error: {dllEx}\nEnsure {rzctlpath} is present and correctly loaded.", "Aimmy");
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Unfortunately, Razer Synapse mode cannot be ran sufficiently.\n{ex}", "Aimmy");
                return false;
            }
        }
    }
}