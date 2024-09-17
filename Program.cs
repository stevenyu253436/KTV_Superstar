using System;
using System.Globalization;
using System.Windows.Forms;
using System.Data.SQLite;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Ports;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using OfficeOpenXml; // 引用 EPPlus 的命名空間
using DirectShowLib;
using DirectShowLib.Dvd;
using DirectShowLib.BDA;
using DirectShowLib.DES;
using DirectShowLib.DMO;
using ZXing;
using ZXing.QrCode;
using WMPLib;
using NAudio.Wave;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Ink;
using Microsoft.Win32;
using System.Diagnostics;
using DualScreenDemo;

namespace DualScreenDemo
{
    public static class Program
    {
        // 定义全局变量
        internal static SongListManager songListManager;
        internal static ArtistManager artistManager;
        internal static SerialPortManager serialPortManager;
        private static PrimaryForm primaryForm; // 儲存實例的參考

        [STAThread]
        static void Main()
        {
            try
            {
                // Initialize COM for the application
                int hr = ComInterop.CoInitializeEx(IntPtr.Zero, ComInterop.COINIT_APARTMENTTHREADED);
                if (hr < 0) // Check for S_OK or S_FALSE (already initialized)
                {
                    Console.WriteLine("Failed to initialize COM library.");
                    return;
                }

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                // 初始化歌曲列表管理器
                songListManager = new SongListManager();
                artistManager = new ArtistManager();

                // 初始化串行端口管理器
                var commandHandler = new CommandHandler(songListManager);
                serialPortManager = new SerialPortManager(commandHandler);
                serialPortManager.InitializeSerialPort();
                
                // Print the virtual screen information
                Console.WriteLine(String.Format("Virtual Screen: {0}", SystemInformation.VirtualScreen));

                // Iterate over each screen and print its resolution
                foreach (var screen in Screen.AllScreens)
                {
                    Console.WriteLine(String.Format("Screen: {0} Resolution: {1}x{2}", screen.DeviceName, screen.Bounds.Width, screen.Bounds.Height));
                }

                // Start servers
                Task.Run(() => HttpServerManager.StartServer());
                Task.Run(() => TCPServerManager.StartServer());

                // 當應用程序關閉時，關閉 SerialPort
                Application.ApplicationExit += (sender, e) => SerialPortManager.CloseSerialPortSafely();
                // 註冊顯示設置更改事件
                SystemEvents.DisplaySettingsChanged += OnDisplaySettingsChanged;

                primaryForm = new PrimaryForm();
                primaryForm.allSongs = songListManager.AllSongs;
                primaryForm.allArtists = artistManager.AllArtists;

                // 設置 PrimaryForm 的位置和大小
                primaryForm.StartPosition = FormStartPosition.Manual;
                primaryForm.Location = new Point(0, 0); // 將窗體放置於屏幕左上角
                primaryForm.Size = new Size(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height); // 設置為滿屏

                InitializeSecondaryScreen();

                primaryForm.Show();
                // primaryForm.ShowSendOffScreen();
                
                Application.Run(primaryForm);
            }
            catch (Exception ex)
            {
                // 当发生异常时，将异常信息写入日志文件
                WriteLog(ex.ToString());
            }
            finally
            {
                SystemEvents.DisplaySettingsChanged -= OnDisplaySettingsChanged;
            }
        }

        private static void InitializeSecondaryScreen()
        {
            if (Screen.AllScreens.Length > 1)
            {
                var secondaryScreen = Screen.AllScreens.FirstOrDefault(s => !s.Primary);
                if (secondaryScreen != null)
                {
                    // 确保 primaryForm 和 videoPlayerForm 已经正确初始化
                    if (primaryForm.videoPlayerForm == null)
                    {
                        primaryForm.videoPlayerForm = new VideoPlayerForm();
                    }

                    // 设置 videoPlayerForm 的位置和大小
                    // primaryForm.videoPlayerForm.StartPosition = FormStartPosition.Manual;
                    // primaryForm.videoPlayerForm.Location = secondaryScreen.WorkingArea.Location;
                    // primaryForm.videoPlayerForm.Size = secondaryScreen.WorkingArea.Size;

                    // 显示 videoPlayerForm 在第二显示器
                    primaryForm.videoPlayerForm.Show();

                    // 初始化公共播放列表
                    primaryForm.videoPlayerForm.InitializePublicPlaylist(primaryForm.publicSongList);
                }
            }
        }

        private static void OnDisplaySettingsChanged(object sender, EventArgs e)
        {
            // UI操作應該放在try-catch塊中
            try
            {
                if (Screen.AllScreens.Length > 1)
                {
                    primaryForm.Invoke(new System.Action(() =>
                    {
                        if (primaryForm.videoPlayerForm == null)
                        {
                            var filePath = @"C:\\video\\100015-周杰倫&aMei-不該-國語-vL-100-11000001.mpg";
                            if (File.Exists(filePath))
                            {
                                Screen secondaryScreen = Screen.AllScreens.FirstOrDefault(s => !s.Primary);
                                if (secondaryScreen != null)
                                {
                                    primaryForm.videoPlayerForm = new VideoPlayerForm();
                                    // primaryForm.primaryMediaPlayerForm = new PrimaryMediaPlayerForm(primaryForm, primaryForm.secondaryMediaPlayerForm);
                                    primaryForm.videoPlayerForm.InitializePublicPlaylist(primaryForm.publicSongList);
                                    primaryForm.videoPlayerForm.Show();
                                }
                            }
                            else
                            {
                                Console.WriteLine("File not found.");
                            }
                        }
                    }));
                }
            }
            catch (Exception ex)
            {
                WriteLog("Error during display settings changed: " + ex.Message);
            }
        }

        static void WriteLog(string message)
        {
            // 指定日志文件的路径
            string logFilePath = "mainlog.txt"; // 请根据需要修改文件路径

            try
            {
                // 使用 StreamWriter 来向日志文件追加文本
                using (StreamWriter writer = new StreamWriter(logFilePath, true))
                {
                    writer.WriteLine(String.Format("[{0}] {1}", DateTime.Now, message));
                }
            }
            catch (Exception ex)
            {
                // 如果写入日志文件时发生错误，这里可以处理这些异常
                // 例如：打印到控制台
                Console.WriteLine(String.Format("Error writing to log file: {0}", ex.Message));
            }
        }

        private static Form CreatePrimaryForm()
        {
            return new Form
            {
                WindowState = FormWindowState.Maximized,
                FormBorderStyle = FormBorderStyle.None
            };
        }

        private static Form CreateSecondaryForm(Screen screen)
        {
            return new Form
            {
                Text = "Secondary Screen Form",
                StartPosition = FormStartPosition.Manual,
                Bounds = screen.Bounds,
                WindowState = FormWindowState.Maximized,
                FormBorderStyle = FormBorderStyle.None
            };
        }
    }
}