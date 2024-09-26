using System;
using System.Collections.Generic;
using System.IO; // For StreamWriter
using System.Drawing; // For Size
using System.Linq; // For LINQ methods like Any
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Threading.Tasks;
using DirectShowLib;

namespace DualScreenDemo
{
    public class VideoPlayerForm : Form
    {
        #region 防止閃屏
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;
                return cp;
            }
        }
        #endregion 

        // 单例实例
        public static VideoPlayerForm Instance { get; private set; }
        
        // 导入user32.dll API
        [DllImport("user32.dll")]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        // Windows API 函數
        [DllImport("user32.dll")]
        static extern IntPtr GetDesktopWindow();

        [DllImport("user32.dll")]
        static extern IntPtr MonitorFromWindow(IntPtr hwnd, uint dwFlags);

        [DllImport("user32.dll")]
        static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO lpmi);

        // MONITORINFO 結構
        [StructLayout(LayoutKind.Sequential)]
        struct MONITORINFO
        {
            public int cbSize;
            public RECT rcMonitor;
            public RECT rcWork;
            public uint dwFlags;
        }

        // RECT 結構
        [StructLayout(LayoutKind.Sequential)]
        struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_TOPMOST = 0x00000008;
        private const uint SWP_NOZORDER = 0x0004;

        private IGraphBuilder graphBuilderPrimary;
        private IGraphBuilder graphBuilderSecondary;
        private IMediaControl mediaControlPrimary;
        private IMediaControl mediaControlSecondary;
        private static IBaseFilter videoRendererSecondary;
        private static IBaseFilter videoRendererPrimary;
        private static VideoMixingRenderer9 vmr9Secondary;
        private static VideoMixingRenderer9 vmr9Primary;
        private IBaseFilter lavSplitterPrimary;
        private IBaseFilter lavSplitterSecondary;
        private IBaseFilter lavVideoDecoderPrimary;
        private IBaseFilter lavVideoDecoderSecondary;
        private static IBaseFilter lavAudioDecoderSecondary;
        // private IPin outputPinPrimary;
        private IPin outputPinSecondary;
        private static IBaseFilter audioRenderer;
        private IVideoWindow videoWindowSecondary;
        private IVideoWindow videoWindowPrimary;
        private IMediaEventEx mediaEventExPrimary;
        private IMediaEventEx mediaEventExSecondary;
        private int videoWidth;
        private int videoHeight;
        private static bool isInitializationComplete = false;

        public static OverlayForm overlayForm; // Instance of the overlay form
        public static List<SongData> playingSongList; // 用于存储播放列表的变量
        public static List<SongData> publicPlaylist; // 公播歌单
        public static int currentSongIndex = 0; // 当前播放歌曲的索引
        private static bool isUserPlaylistPlaying = false; // 标记当前是否在播放用户歌单
        public bool isMuted = false;
        public int previousVolume = -1000; // 初始音量值设置为静音状态
        public bool isPaused = false;
        private bool isSyncToPrimaryMonitor = false; // 控制是否需要同步到主屏幕的布尔变量

        public bool IsSyncToPrimaryMonitor
        {
            get { return isSyncToPrimaryMonitor; }
            set { isSyncToPrimaryMonitor = value; }
        }

        private static Screen secondMonitor;

        public VideoPlayerForm()
        {
            Instance = this;
            // this.DoubleBuffered = true;

            InitializeComponent();
            this.Load += VideoPlayerForm_Load;
            this.Shown += VideoPlayerForm_Shown;
            this.FormClosing += VideoPlayerForm_FormClosing;  // 确保处理窗体关闭事件
            InitializeOverlayForm(secondMonitor);
            BringOverlayToFront();

            // 订阅HttpServer的事件
            HttpServer.OnDisplayBarrage += DisplayBarrageOnOverlay;
        }

        private void InitializeComponent()
        {

        }

        private void VideoPlayerForm_Load(object sender, EventArgs e)
        {
            secondMonitor = ScreenHelper.GetSecondMonitor();
            if (secondMonitor != null)
            {
                this.FormBorderStyle = FormBorderStyle.None; // 设置窗体没有边框
                this.StartPosition = FormStartPosition.Manual;
                this.Location = secondMonitor.Bounds.Location;
                this.Size = secondMonitor.Bounds.Size;
                // this.DoubleBuffered = true;
            }

            CheckMonitor(); // 調用 CheckMonitor 函數
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            try
            {
                // 调整窗口位置到第二显示器
                if (secondMonitor != null)
                {
                    // 使用 SetWindowPos 函数将窗口移动到第二个显示器并设置为最顶层窗口
                    SetWindowPos(this.Handle, IntPtr.Zero, secondMonitor.Bounds.X, secondMonitor.Bounds.Y, 
                                secondMonitor.Bounds.Width, secondMonitor.Bounds.Height, 0);
                }

                // 设置窗口为最顶层窗口
                IntPtr exStyle = GetWindowLong(this.Handle, GWL_EXSTYLE);
                // SetWindowLong(this.Handle, GWL_EXSTYLE, (IntPtr)(exStyle.ToInt32() | WS_EX_TOPMOST));
                SetWindowLong(this.Handle, GWL_EXSTYLE, (IntPtr)(exStyle.ToInt32()));
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred in OnShown: " + ex.Message);
            }
        }

        private void VideoPlayerForm_Shown(object sender, EventArgs e)
        {
            // 初始化COM库
            int hr = CoInitializeEx(IntPtr.Zero, COINIT.APARTMENTTHREADED);
            if (hr < 0)
            {
                Console.WriteLine("Failed to initialize COM library.");
                return;
            }

            InitializeGraphBuilderPrimary();
            InitializeGraphBuilderSecondary();

            // 在后台线程启动事件监听
            Task.Run(() => MonitorMediaEvents());
        }

        private void VideoPlayerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // 清理COM
            if (videoWindowPrimary != null)
            {
                videoWindowPrimary.put_Visible(OABool.False);
                videoWindowPrimary.put_Owner(IntPtr.Zero);
                Marshal.ReleaseComObject(videoWindowPrimary);
                videoWindowPrimary = null;
            }

            if (videoWindowSecondary != null)
            {
                videoWindowSecondary.put_Visible(OABool.False);
                videoWindowSecondary.put_Owner(IntPtr.Zero);
                Marshal.ReleaseComObject(videoWindowSecondary);
                videoWindowSecondary = null;
            }
            // 清理COM
            CoUninitialize();
        }

        // COM API函数声明
        [DllImport("ole32.dll")]
        private static extern int CoInitializeEx(IntPtr pvReserved, COINIT dwCoInit);

        [DllImport("ole32.dll")]
        private static extern void CoUninitialize();

        // CoInitializeEx() 可以选择的参数
        private enum COINIT : int
        {
            APARTMENTTHREADED = 0x2,
            MULTITHREADED = 0x0
        }

        private void InitializeGraphBuilderPrimary()
        {
            graphBuilderPrimary = (IGraphBuilder)new FilterGraph();
            if (graphBuilderPrimary == null)
            {
                Console.WriteLine("Failed to create FilterGraph for primary monitor.");
                throw new Exception("Failed to create FilterGraph for primary monitor.");
            }
            // 其他图形初始化代码
            try
            {
                // 添加 LAV Splitter
                lavSplitterPrimary = AddFilterByClsid(graphBuilderPrimary, "LAV Splitter", Clsid.LAVSplitter);
                // 添加 LAV Video Decoder
                lavVideoDecoderPrimary = AddFilterByClsid(graphBuilderPrimary, "LAV Video Decoder", Clsid.LAVVideoDecoder);

                // 创建 VideoMixingRenderer9 实例
                vmr9Primary = new VideoMixingRenderer9();

                // 显式转换为 IBaseFilter 接口并添加到图形中
                videoRendererPrimary = (IBaseFilter)vmr9Primary;
                int hr = graphBuilderPrimary.AddFilter(videoRendererPrimary, "Primary Video Renderer");
                DsError.ThrowExceptionForHR(hr);

                // 获取媒体控制接口
                mediaControlPrimary = (IMediaControl)graphBuilderPrimary;
                if (mediaControlPrimary == null)
                {
                    Console.WriteLine("Failed to get Media Control for primary monitor.");
                    return;
                }

                // 获取媒体事件接口
                mediaEventExPrimary = (IMediaEventEx)graphBuilderPrimary;
                if (mediaEventExPrimary == null)
                {
                    Console.WriteLine("Failed to get Media Event Ex for primary monitor.");
                    return;
                }

                // 初始化操作...
                // isInitializationComplete = true;  // 设置初始化完成标志

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error initializing graph builder for primary monitor: " + ex.Message);
            }
        }

        private void InitializeGraphBuilderSecondary()
        {
            graphBuilderSecondary = (IGraphBuilder)new FilterGraph();
            if (graphBuilderSecondary == null)
            {
                Console.WriteLine("Failed to create FilterGraph");
                throw new Exception("Failed to create FilterGraph");
            }
            // 其他图形初始化代码
            try
            {
                // 添加 LAV Splitter
                lavSplitterSecondary = AddFilterByClsid(graphBuilderSecondary, "LAV Splitter", Clsid.LAVSplitter);
                // 添加 LAV Video Decoder
                lavVideoDecoderSecondary = AddFilterByClsid(graphBuilderSecondary, "LAV Video Decoder", Clsid.LAVVideoDecoder);
                // 添加 LAV Audio Decoder
                lavAudioDecoderSecondary = AddFilterByClsid(graphBuilderSecondary, "LAV Audio Decoder", Clsid.LAVAudioDecoder);

                // 获取输出引脑
                outputPinSecondary = FindPin(lavAudioDecoderSecondary, "Output");

                // 创建 VideoMixingRenderer9 实例
                vmr9Secondary = new VideoMixingRenderer9();

                // 显式转换为 IBaseFilter 接口并添加到图形中
                videoRendererSecondary = (IBaseFilter)vmr9Secondary;
                int hr = graphBuilderSecondary.AddFilter(videoRendererSecondary, "Secondary Video Renderer");
                DsError.ThrowExceptionForHR(hr);

                // 添加系统默认音频渲染器
                var clsidAudioRenderer = new Guid("79376820-07D0-11CF-A24D-0020AFD79767");  // CLSID for DirectSound Renderer
                audioRenderer = (IBaseFilter)Activator.CreateInstance(Type.GetTypeFromCLSID(clsidAudioRenderer));
                hr = graphBuilderSecondary.AddFilter(audioRenderer, "Default DirectSound Device");
                DsError.ThrowExceptionForHR(hr);

                // 获取媒体控制接口
                mediaControlSecondary = (IMediaControl)graphBuilderSecondary;
                if (mediaControlSecondary == null)
                {
                    Console.WriteLine("Failed to get Media Control");
                    return;
                }

                // 获取媒体事件接口
                mediaEventExSecondary = (IMediaEventEx)graphBuilderSecondary;
                if (mediaEventExSecondary == null)
                {
                    Console.WriteLine("Failed to get Media Event Ex");
                    return;
                }

                // 初始化操作...
                isInitializationComplete = true;  // 设置初始化完成标志

                // 其他视频渲染配置
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error initializing graph builder with second monitor: " + ex.Message);
            }
        }

        private void ConfigureSampleGrabber(IBaseFilter sampleGrabberFilter)
        {
            ISampleGrabber sampleGrabber = (ISampleGrabber)sampleGrabberFilter;
            AMMediaType mediaType = new AMMediaType
            {
                majorType = MediaType.Video,
                subType = MediaSubType.RGB24,
                formatType = FormatType.VideoInfo
            };
            sampleGrabber.SetMediaType(mediaType);
            DsUtils.FreeAMMediaType(mediaType);

            sampleGrabber.SetBufferSamples(false);
            sampleGrabber.SetOneShot(false);
            sampleGrabber.SetCallback(new SampleGrabberCallback(this), 1);
        }

        private int ConnectFilters(IGraphBuilder graphBuilder, IBaseFilter sourceFilter, string sourcePinName, IBaseFilter destFilter, string destPinName)
        {
            IPin outPin = FindPin(sourceFilter, sourcePinName);
            IPin inPin = FindPin(destFilter, destPinName);
            if (outPin == null || inPin == null)
            {
                Console.WriteLine(String.Format("Cannot find pins: {0} or {1}", sourcePinName, destPinName));
                return -1;
            }

            // PrintPinMediaTypes(outPin, "Output Pin");
            // PrintPinMediaTypes(inPin, "Input Pin");

            int hr = graphBuilder.Connect(outPin, inPin);
            return hr;
        }

        private void PrintPinMediaTypes(IPin pin, string pinName)
        {
            IEnumMediaTypes enumMediaTypes;
            pin.EnumMediaTypes(out enumMediaTypes);
            AMMediaType[] mediaTypes = new AMMediaType[1];
            IntPtr fetched = IntPtr.Zero;

            Console.WriteLine(String.Format("Media types for {0}:", pinName));

            while (enumMediaTypes.Next(1, mediaTypes, fetched) == 0)
            {
                AMMediaType mediaType = mediaTypes[0];
                Console.WriteLine(String.Format("  Major type: {0}, Subtype: {1}", mediaType.majorType, mediaType.subType));
                DsUtils.FreeAMMediaType(mediaType);
            }
        }

        // 查找特定名称的引脑
        private IPin FindPin(IBaseFilter filter, string pinName)
        {
            IEnumPins enumPins;
            IPin[] pins = new IPin[1];

            filter.EnumPins(out enumPins);
            enumPins.Reset();

            while (enumPins.Next(1, pins, IntPtr.Zero) == 0)
            {
                PinInfo pinInfo;
                pins[0].QueryPinInfo(out pinInfo);
                Console.WriteLine(pinInfo);

                if (pinInfo.name == pinName)
                {
                    return pins[0];
                }
            }
            return null;
        }

        private void CheckMonitor()
        {
            // 获取当前窗体的屏幕
            Screen screen = Screen.FromHandle(this.Handle);

            // 打印当前窗体所在的显示器信息
            Console.WriteLine("Current Form is on: " + screen.DeviceName);
            Console.WriteLine("Bounds: " + screen.Bounds.ToString());
            Console.WriteLine("Primary Screen: " + screen.Primary.ToString());
        }

        private static void ListPins(IBaseFilter filter)
        {
            IEnumPins pinEnum;
            IPin[] pins = new IPin[1];
            IntPtr fetched = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(int))); // 分配内存

            Console.WriteLine("Listing pins for filter:");
            filter.EnumPins(out pinEnum);
            pinEnum.Reset();
            while (pinEnum.Next(1, pins, fetched) == 0)
            {
                int fetchedCount = Marshal.ReadInt32(fetched); // 从内存中读取引脚数量
                if (fetchedCount > 0)
                {
                    PinInfo pinInfo;
                    pins[0].QueryPinInfo(out pinInfo);
                    Console.WriteLine(String.Format("Pin name: {0}, Direction: {1}", pinInfo.name, pinInfo.dir));
                    Marshal.ReleaseComObject(pins[0]);
                }
            }
            Marshal.ReleaseComObject(pinEnum);
            Marshal.FreeCoTaskMem(fetched); // 释放内存
        }

        private static IBaseFilter AddFilterByClsid(IGraphBuilder graphBuilder, string name, Guid clsid)
        {
            try
            {
                // 使用 .NET 的 Activator 来创建 COM 对象实例
                Type filterType = Type.GetTypeFromCLSID(clsid);
                IBaseFilter filter = (IBaseFilter)Activator.CreateInstance(filterType);
                int hr = graphBuilder.AddFilter(filter, name);
                if (hr != 0)
                {
                    Console.WriteLine(String.Format("Failed to add filter {0} with CLSID {1}, HRESULT: {2}", name, clsid, hr));
                }
                DsError.ThrowExceptionForHR(hr);
                Console.WriteLine(String.Format("Successfully added filter {0} with CLSID {1}", name, clsid));
                return filter;
            }
            catch (Exception ex)
            {
                Console.WriteLine(String.Format("Exception in AddFilterByClsid: {0}", ex.Message));
                throw; // Rethrow the exception to handle it further up the call stack
            }
        }

        public void SyncToPrimaryMonitor()
        {
            // 显示主屏幕上的 primaryScreenPanel
            PrimaryForm.Instance.primaryScreenPanel.Visible = true;
            PrimaryForm.Instance.primaryScreenPanel.BringToFront();
            PrimaryForm.Instance.syncServiceBellButton.Visible = true;
            PrimaryForm.Instance.syncServiceBellButton.BringToFront();
            PrimaryForm.Instance.syncCutSongButton.Visible = true;
            PrimaryForm.Instance.syncCutSongButton.BringToFront();
            PrimaryForm.Instance.syncReplayButton.Visible = true;
            PrimaryForm.Instance.syncReplayButton.BringToFront();
            PrimaryForm.Instance.syncOriginalSongButton.Visible = true;
            PrimaryForm.Instance.syncOriginalSongButton.BringToFront();
            PrimaryForm.Instance.syncMuteButton.Visible = true;
            PrimaryForm.Instance.syncMuteButton.BringToFront();
            if (isPaused)
            {
                PrimaryForm.Instance.syncPlayButton.Visible = true;
                PrimaryForm.Instance.syncPlayButton.BringToFront();
            }
            else
            {
                PrimaryForm.Instance.syncPauseButton.Visible = true;
                PrimaryForm.Instance.syncPauseButton.BringToFront();
            }
            PrimaryForm.Instance.syncVolumeUpButton.Visible = true;
            PrimaryForm.Instance.syncVolumeUpButton.BringToFront();
            PrimaryForm.Instance.syncVolumeDownButton.Visible = true;
            PrimaryForm.Instance.syncVolumeDownButton.BringToFront();
            PrimaryForm.Instance.syncMicUpButton.Visible = true;
            PrimaryForm.Instance.syncMicUpButton.BringToFront();
            PrimaryForm.Instance.syncMicDownButton.Visible = true;
            PrimaryForm.Instance.syncMicDownButton.BringToFront();
            PrimaryForm.Instance.syncCloseButton.Visible = true;
            PrimaryForm.Instance.syncCloseButton.BringToFront();

            // 这里不需要停止和清理图形，只需要设置主屏幕的渲染器
            try
            {
                if (vmr9Primary == null)
                {
                    Console.WriteLine("VMR9 is not initialized.");
                    return;
                }

                videoWindowPrimary = (IVideoWindow)vmr9Primary;
                videoWindowPrimary.put_Owner(PrimaryForm.Instance.primaryScreenPanel.Handle); // 设置为 primaryScreenPanel 的句柄
                videoWindowPrimary.put_WindowStyle(WindowStyle.Child | WindowStyle.ClipChildren);
                videoWindowPrimary.SetWindowPosition(0, 0, 1210, 900);
                videoWindowPrimary.put_Visible(OABool.True);

                Console.WriteLine("Video window configured successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(String.Format("Error syncing to primary monitor: {0}", ex.Message));
                MessageBox.Show(String.Format("Error syncing to primary monitor: {0}", ex.Message));
            }
        }

        public void ClosePrimaryScreenPanel()
        {
            try
            {
                // 隐藏 primaryScreenPanel
                PrimaryForm.Instance.primaryScreenPanel.Visible = false;
                PrimaryForm.Instance.syncServiceBellButton.Visible = false;
                PrimaryForm.Instance.syncCutSongButton.Visible = false;
                PrimaryForm.Instance.syncReplayButton.Visible = false;
                PrimaryForm.Instance.syncOriginalSongButton.Visible = false;
                PrimaryForm.Instance.syncMuteButton.Visible = false;
                PrimaryForm.Instance.syncPauseButton.Visible = false;
                PrimaryForm.Instance.syncPlayButton.Visible = false;
                PrimaryForm.Instance.syncVolumeUpButton.Visible = false;
                PrimaryForm.Instance.syncVolumeDownButton.Visible = false;
                PrimaryForm.Instance.syncMicUpButton.Visible = false;
                PrimaryForm.Instance.syncMicDownButton.Visible = false;
                PrimaryForm.Instance.syncCloseButton.Visible = false;

                // 释放 videoWindowPrimary
                if (videoWindowPrimary != null)
                {
                    videoWindowPrimary.put_Visible(OABool.False);
                    // videoWindowPrimary.put_Owner(IntPtr.Zero);
                    // Marshal.ReleaseComObject(videoWindowPrimary);
                    // videoWindowPrimary = null;
                }

                // 更新同步状态
                IsSyncToPrimaryMonitor = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(String.Format("Error closing primary screen panel: {0}", ex.Message));
                MessageBox.Show(String.Format("Error closing primary screen panel: {0}", ex.Message));
            }
        }

        [DllImport("gdi32.dll", ExactSpelling = true)]
        public static extern bool BitBlt(IntPtr hdcDest, int nXDest, int nYDest, int nWidth, int nHeight,
        IntPtr hdcSrc, int nXSrc, int nYSrc, TernaryRasterOperations dwRop);

        public enum TernaryRasterOperations : uint
        {
            SRCCOPY = 0x00CC0020,
        }

        private void PrintVideoDimensions()
        {
            IPin videoOutputPin = FindVideoOutputPin(videoRendererSecondary);
            if (videoOutputPin != null)
            {
                AMMediaType mediaType = new AMMediaType();
                try
                {
                    videoOutputPin.ConnectionMediaType(mediaType);
                    if (mediaType.formatType == FormatType.VideoInfo) // 检查格式类型是否正确
                    {
                        VideoInfoHeader videoInfo = (VideoInfoHeader)Marshal.PtrToStructure(mediaType.formatPtr, typeof(VideoInfoHeader));
                        Console.WriteLine("Video Width: " + videoInfo.BmiHeader.Width);
                        Console.WriteLine("Video Height: " + videoInfo.BmiHeader.Height);
                        
                        // 适应上面 ConfigureVMR9ForFullScreen 中使用
                        videoWidth = videoInfo.BmiHeader.Width;
                        videoHeight = videoInfo.BmiHeader.Height;
                    }
                }
                finally
                {
                    DsUtils.FreeAMMediaType(mediaType);
                    Marshal.ReleaseComObject(videoOutputPin);
                }
            }
        }

        // 查找连接到渲染器的视频输出引脚
        private IPin FindVideoOutputPin(IBaseFilter filter)
        {
            IEnumPins pinEnum;
            filter.EnumPins(out pinEnum);
            IPin[] pins = new IPin[1];
            while (pinEnum.Next(1, pins, IntPtr.Zero) == 0)
            {
                PinInfo pinInfo;
                pins[0].QueryPinInfo(out pinInfo);
                if (pinInfo.dir == PinDirection.Output)
                {
                    pinEnum.Reset();
                    return pins[0];
                }
                Marshal.ReleaseComObject(pins[0]);
            }
            pinEnum.Reset();
            return null;
        }

        private void DisplayBarrageOnOverlay(string text)
        {
            if (overlayForm.InvokeRequired)
            {
                overlayForm.Invoke(new System.Action(() => overlayForm.DisplayBarrage(text)));
            }
            else
            {
                overlayForm.DisplayBarrage(text);
            }
        }

        public void InitializePublicPlaylist(List<SongData> initialPlaylist)
        {
            publicPlaylist = initialPlaylist; // 假设你已经有了一个初始化的歌曲列表
            PlayPublicPlaylist(); // 开始播放公播歌单
        }

        public void SetPlayingSongList(List<SongData> songList)
        {
            StopAndReleaseResources();

            playingSongList = songList;
            isUserPlaylistPlaying = playingSongList.Any();
            if (isUserPlaylistPlaying)
            {
                currentSongIndex = -1; // Reset index to start at the first song
                PlayNextSong();
            }
            else
            {
                PlayPublicPlaylist();
            }
        }

        public void PlayPublicPlaylist()
        {
            // 禁用循环播放
            // axWmp.settings.setMode("loop", false);

            isUserPlaylistPlaying = false;
            currentSongIndex = -1; // Reset index to start at the first song
            PlayNextSong();
        }

        private static async Task UpdateMarqueeTextForCurrentSong(SongData song)
        {
            string text;

            if (string.IsNullOrEmpty(song?.Song))
            {
                text = string.Empty;
            }
            else
            {
                text = String.Format("正在播放：{0} - 曲號：{1}", song.Song, song.SongNumber);
            }

            // 更新顯示
            overlayForm.UpdateSongDisplayLabel(text);

            // 等待一段时间，比如5秒
            await Task.Delay(5000);
        }

        public static async Task UpdateMarqueeTextForNextSong(SongData song)
        {
            string nextSongText = String.Format("下一首：{0} - 曲號：{1}", song.Song, song.SongNumber);
            // 确保在UI线程中执行更新
            if (overlayForm.InvokeRequired)
            {
                overlayForm.Invoke(new MethodInvoker(() => {
                    overlayForm.UpdateMarqueeText(nextSongText, OverlayForm.MarqueeStartPosition.Middle, Color.White);
                }));
            }
            else
            {
                overlayForm.UpdateMarqueeText(nextSongText, OverlayForm.MarqueeStartPosition.Middle, Color.White);
            }
            
            // 等待一段时间，比如5秒
            await Task.Delay(5000);

            // 重置跑马灯文本
            if (overlayForm.InvokeRequired)
            {
                overlayForm.Invoke(new MethodInvoker(() => {
                    overlayForm.ResetMarqueeTextToWelcomeMessage();
                }));
            }
            else
            {
                overlayForm.ResetMarqueeTextToWelcomeMessage();
            }
        }

        public async Task PlayNextSong()
        {
            // 等待初始化完成
            while (!isInitializationComplete)
            {
                await Task.Delay(100);  // 每隔100毫秒检查一次
            }

            Console.WriteLine("開始播放下一首歌曲...");
            // 确定使用哪个播放列表
            List<SongData> currentPlaylist = isUserPlaylistPlaying ? playingSongList : publicPlaylist;

            // 如果列表为空，则不继续
            if (!currentPlaylist.Any()) return;

            // 生成一个随机整数
            Random random = new Random();
            int randomOffset = random.Next(1, currentPlaylist.Count + 1);

            // 如果不是用户播放列表，currentSongIndex 加上随机整数
            if (!isUserPlaylistPlaying)
            {
                currentSongIndex = (currentSongIndex + randomOffset) % currentPlaylist.Count;
                Console.WriteLine(String.Format("隨機播放: currentSongIndex = {0}, currentPlaylist.Count = {1}", currentSongIndex, currentPlaylist.Count));
            }
            else
            {
                currentSongIndex = (currentSongIndex + 1) % currentPlaylist.Count;
            }

            var songToPlay = currentPlaylist[currentSongIndex];

            // 检查两个主机的文件路径，并确定哪个是有效的
            var pathToPlay = File.Exists(songToPlay.SongFilePathHost1) ? songToPlay.SongFilePathHost1 : songToPlay.SongFilePathHost2;
            if (!File.Exists(pathToPlay))
            {
                // MessageBox.Show("File does not exist on both hosts.");
                return; // 如果两个文件路径都不存在，则返回
            }

            // 在此调用 DisplayQRCodeOnOverlay 方法来显示 QR 码
            overlayForm.DisplayQRCodeOnOverlay(HttpServer.randomFolderPath);

            overlayForm.HidePauseLabel();

            // // 使用 MediaInfo 检查文件格式
            // var mediaInfo = new MediaInfo();
            // mediaInfo.Open(pathToPlay);

            // string format = mediaInfo.Get(StreamKind.Video, 0, "Format");
            // string codecID = mediaInfo.Get(StreamKind.Video, 0, "CodecID");

            // Console.WriteLine(String.Format("File Format: {0}", format));
            // Console.WriteLine(String.Format("Codec ID: {0}", codecID));

            // mediaInfo.Close();

            // 更新跑马灯文本
            UpdateMarqueeTextForCurrentSong(songToPlay);

            try
            {
                // StopAndReleaseResources();

                if (!File.Exists(pathToPlay))
                {
                    MessageBox.Show("Specified video file does not exist: " + pathToPlay);
                    return;
                }

                try
                {
                    // 重新建立和配置图
                    InitializeGraphBuilderPrimary();
                    InitializeGraphBuilderSecondary();
                    RenderMediaFilePrimary(pathToPlay);
                    RenderMediaFileSecondary(pathToPlay);

                    // 确保播放之前检查静音状态
                    if (isMuted)
                    {
                        SetVolume(-10000); // 静音
                    }

                    mediaControlPrimary.Run();
                    mediaControlSecondary.Run();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error adding source filter: " + ex.Message);
                    return;
                }

                videoWindowSecondary = (IVideoWindow)vmr9Secondary;

                // Set VMR9 window handle to second screen handle

                videoWindowSecondary.put_Owner(this.Handle);  // 使用正确的 Handle
                videoWindowSecondary.put_WindowStyle(WindowStyle.Child | WindowStyle.ClipChildren);
                videoWindowSecondary.SetWindowPosition(0, 0, secondMonitor.Bounds.Width, secondMonitor.Bounds.Height);
                videoWindowSecondary.put_Visible(OABool.True);

                if (isSyncToPrimaryMonitor)
                {
                    SyncToPrimaryMonitor(); // 确保同步到主屏幕
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(String.Format("Error playing song: {0}", ex.Message));
                MessageBox.Show(String.Format("Error playing song: {0}", ex.Message));
            }
        }

        public void SkipToNextSong()
        {
            StopAndReleaseResources();

            if (isUserPlaylistPlaying && playingSongList.Count > 0)
            {
                // 移除当前播放的歌曲
                playingSongList.RemoveAt(0);
                
                // 检查列表是否为空，如果为空，播放公播列表
                if (!playingSongList.Any())
                {
                    PrimaryForm.playStates[PrimaryForm.currentSongIndexInHistory] = PlayState.Played;
                    PlayPublicPlaylist();
                }
                else
                {
                    currentSongIndex = -1;
                    // 播放下一首用户请求的歌曲
                    PrimaryForm.playStates[PrimaryForm.currentSongIndexInHistory] = PlayState.Played;
                    PrimaryForm.currentSongIndexInHistory += 1;
                    PrimaryForm.playStates[PrimaryForm.currentSongIndexInHistory] = PlayState.Playing;
                    PlayNextSong();
                }
            }
            else
            {
                // 如果当前不是在播放用户歌单或者歌单为空，直接播放公播歌单
                PlayPublicPlaylist();
            }
        }

        public void ReplayCurrentSong()
        {
            // 确定使用哪个播放列表
            List<SongData> currentPlaylist = isUserPlaylistPlaying ? playingSongList : publicPlaylist;

            // 如果列表为空，则不继续
            if (!currentPlaylist.Any()) return;

            // 获取当前播放的歌曲
            var songToPlay = currentPlaylist[currentSongIndex];

            // 检查两个主机的文件路径，并确定哪个是有效的
            var pathToPlay = File.Exists(songToPlay.SongFilePathHost1) ? songToPlay.SongFilePathHost1 : songToPlay.SongFilePathHost2;
            if (!File.Exists(pathToPlay))
            {
                MessageBox.Show("File does not exist on both hosts.");
                return; // 如果两个文件路径都不存在，则返回
            }

            // 更新跑马灯文本
            UpdateMarqueeTextForCurrentSong(songToPlay);

            try
            {
                if (mediaControlPrimary != null)
                {
                    mediaControlPrimary.Stop();
                    RemoveAllFilters(graphBuilderPrimary);
                }
                // 停止当前播放并清理图形
                if (mediaControlSecondary != null)
                {
                    mediaControlSecondary.Stop();
                    RemoveAllFilters(graphBuilderSecondary);
                }

                try
                {
                    // 重新建立和配置图
                    InitializeGraphBuilderPrimary();
                    InitializeGraphBuilderSecondary();
                    RenderMediaFilePrimary(pathToPlay);
                    RenderMediaFileSecondary(pathToPlay);
                    mediaControlPrimary.Run();
                    mediaControlSecondary.Run();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error adding source filter: " + ex.Message);
                    return;
                }

                videoWindowSecondary = (IVideoWindow)vmr9Secondary;

                // 设置 VMR9 窗口句柄为第二屏幕的句柄
                videoWindowSecondary.put_Owner(this.Handle);  // 使用正确的 Handle
                videoWindowSecondary.put_WindowStyle(WindowStyle.Child | WindowStyle.ClipChildren);
                videoWindowSecondary.SetWindowPosition(0, 0, secondMonitor.Bounds.Width, secondMonitor.Bounds.Height);
                videoWindowSecondary.put_Visible(OABool.True);

                if (isSyncToPrimaryMonitor)
                {
                    SyncToPrimaryMonitor(); // 确保同步到主屏幕
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(String.Format("Error replaying song: {0}", ex.Message));
                MessageBox.Show(String.Format("Error replaying song: {0}", ex.Message));
            }
        }

        private void StopAndReleaseResources()
        {
            try
            {
                if (mediaControlPrimary != null)
                {
                    mediaControlPrimary.Stop();
                    RemoveAllFilters(graphBuilderPrimary);
                }
                if (mediaControlSecondary != null)
                {
                    mediaControlSecondary.Stop();
                    RemoveAllFilters(graphBuilderSecondary);
                }

                // if (videoWindowPrimary != null)
                // {
                //     videoWindowPrimary.put_Visible(OABool.False);
                //     videoWindowPrimary.put_Owner(IntPtr.Zero);
                //     Marshal.ReleaseComObject(videoWindowPrimary);
                //     videoWindowPrimary = null;
                // }

                // if (videoWindowSecondary != null)
                // {
                //     videoWindowSecondary.put_Visible(OABool.False);
                //     videoWindowSecondary.put_Owner(IntPtr.Zero);
                //     Marshal.ReleaseComObject(videoWindowSecondary);
                //     videoWindowSecondary = null;
                // }

                // 释放视频渲染器
                // if (videoRendererPrimary != null)
                // {
                //     Marshal.ReleaseComObject(videoRendererPrimary);
                //     videoRendererPrimary = null;
                // }
                // if (videoRendererSecondary != null)
                // {
                //     Marshal.ReleaseComObject(videoRendererSecondary);
                //     videoRendererSecondary = null;
                // }

                // 释放 LAV 分离器和解码器
                if (lavSplitterPrimary != null)
                {
                    Marshal.ReleaseComObject(lavSplitterPrimary);
                    lavSplitterPrimary = null;
                }
                if (lavSplitterSecondary != null)
                {
                    Marshal.ReleaseComObject(lavSplitterSecondary);
                    lavSplitterSecondary = null;
                }
                if (lavVideoDecoderPrimary != null)
                {
                    Marshal.ReleaseComObject(lavVideoDecoderPrimary);
                    lavVideoDecoderPrimary = null;
                }
                if (lavVideoDecoderSecondary != null)
                {
                    Marshal.ReleaseComObject(lavVideoDecoderSecondary);
                    lavVideoDecoderSecondary = null;
                }
                if (lavAudioDecoderSecondary != null)
                {
                    Marshal.ReleaseComObject(lavAudioDecoderSecondary);
                    lavAudioDecoderSecondary = null;
                }

                // 释放 VMR9 渲染器
                // if (vmr9Primary != null)
                // {
                //     Marshal.ReleaseComObject(vmr9Primary);
                //     vmr9Primary = null;
                // }
                // if (vmr9Secondary != null)
                // {
                //     Marshal.ReleaseComObject(vmr9Secondary);
                //     vmr9Secondary = null;
                // }
                
                // 释放输出引脚
                // if (outputPinPrimary != null)
                // {
                //     Marshal.ReleaseComObject(outputPinPrimary);
                //     outputPinPrimary = null;
                // }
                if (outputPinSecondary != null)
                {
                    Marshal.ReleaseComObject(outputPinSecondary);
                    outputPinSecondary = null;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(String.Format("Error stopping and releasing resources: {0}", ex.Message));
            }
        }

        public void ListAudioTracks()
        {
            if (mediaControlSecondary == null)
            {
                Console.WriteLine("Media control is null, cannot list audio tracks.");
                return;
            }

            IAMStreamSelect streamSelect = null;
            int hr = FindPinInterface(lavSplitterSecondary, typeof(IAMStreamSelect).GUID, out streamSelect);
            if (hr != 0 || streamSelect == null)
            {
                Console.WriteLine(String.Format("Failed to find IAMStreamSelect interface, HRESULT: {0}", hr));
                return;
            }

            int count = 0;
            hr = streamSelect.Count(out count);
            if (hr != 0)
            {
                Console.WriteLine(String.Format("Failed to count audio tracks, HRESULT: {0}", hr));
                return;
            }

            for (int i = 0; i < count; i++)
            {
                AMMediaType mediaType;
                AMStreamSelectInfoFlags flags;
                int enabled;
                int group;
                string trackName;
                object o1, o2;

                hr = streamSelect.Info(i, out mediaType, out flags, out enabled, out group, out trackName, out o1, out o2);
                if (hr != 0)
                {
                    Console.WriteLine(String.Format("Failed to get info for track {0}, HRESULT: {1}", i, hr));
                    continue;
                }

                Console.WriteLine(String.Format("Track {0}: {1}", i, trackName));
                DsUtils.FreeAMMediaType(mediaType);  // Always free AMMediaType when done.
            }
        }

        private static int FindPinInterface(IBaseFilter filter, Guid interfaceId, out IAMStreamSelect streamSelect)
        {
            streamSelect = null;
            if (filter == null)
            {
                Console.WriteLine("FindPinInterface: Provided filter is null.");
                return -1;  // Invalid filter
            }

            IEnumPins enumPins;
            int hr = filter.EnumPins(out enumPins);
            if (hr != 0)
            {
                Console.WriteLine("FindPinInterface: Failed to enumerate pins.");
                return hr;  // Failed to enumerate pins
            }

            IPin[] pins = new IPin[1];
            while (enumPins.Next(1, pins, IntPtr.Zero) == 0)
            {
                Console.WriteLine(String.Format("FindPinInterface: Checking pin {0} for IAMStreamSelect.", pins[0]));
                IPin pin = pins[0];
                // 尝试将pin转换为IAMStreamSelect接口
                IAMStreamSelect tmpStreamSelect = pin as IAMStreamSelect;
                if (tmpStreamSelect != null)
                {
                    streamSelect = tmpStreamSelect;
                    Console.WriteLine("FindPinInterface: Successfully found the IAMStreamSelect interface.");
                    Marshal.ReleaseComObject(pin);
                    enumPins.Reset();
                    return 0; // S_OK
                }
                
                Marshal.ReleaseComObject(pins[0]);
            }

            Console.WriteLine("FindPinInterface: IAMStreamSelect interface not found on any pins.");
            return -1;  // Interface not found
        }

        public void ToggleAudioTrack(int trackIndex)
        {
            IAMStreamSelect streamSelect = lavSplitterSecondary as IAMStreamSelect;

            if (streamSelect != null)
            {
                int count;
                int hr = streamSelect.Count(out count);
                if (hr == 0)
                {
                    Console.WriteLine(String.Format("Total tracks available: {0}", count));

                    // 首先禁用所有音轨
                    // streamSelect.Enable(1, AMStreamSelectEnableFlags.DisableAll);
                    // 尝试禁用每个音轨
                    for (int i = 0; i < count; i++)
                    {
                        hr = streamSelect.Enable(i, AMStreamSelectEnableFlags.DisableAll);
                        if (hr == 0)
                        {
                            Console.WriteLine(String.Format("Disabled track {0}", i));
                        }
                        else
                        {
                            Console.WriteLine(String.Format("Failed to disable track {0}, HRESULT: {1}", i, hr));
                        }
                    }

                    for (int i = 0; i < count; i++)
                    {
                        AMMediaType pmt;
                        AMStreamSelectInfoFlags flags;
                        int lcid, dwReserved1;
                        // int dwReserved2;
                        string pszName;
                        object ppObject, pvReserved2;
                        streamSelect.Info(i, out pmt, out flags, out lcid, out dwReserved1, out pszName, out ppObject, out pvReserved2);

                        // 启用指定音轨
                        if (i == trackIndex)
                        {
                            streamSelect.Enable(i, AMStreamSelectEnableFlags.Enable);
                        }
                    }
                }
            }
        }

        public void RenderMediaFilePrimary(string filePath)
        {
            int hr;

            try
            {
                // 添加源过滤器
                IBaseFilter sourceFilter;
                hr = graphBuilderPrimary.AddSourceFilter(filePath, "Source", out sourceFilter);
                DsError.ThrowExceptionForHR(hr);

                // 连接源过滤器的输出到 LAV Splitter 的输入
                hr = ConnectFilters(graphBuilderPrimary, sourceFilter, "Output", lavSplitterPrimary, "Input");
                DsError.ThrowExceptionForHR(hr);

                // 连接 LAV Splitter 的视频输出到 LAV Video Decoder
                hr = ConnectFilters(graphBuilderPrimary, lavSplitterPrimary, "Video", lavVideoDecoderPrimary, "Input");
                DsError.ThrowExceptionForHR(hr);

                // 连接 LAV Video Decoder 的输出到 Video Renderer
                hr = ConnectFilters(graphBuilderPrimary, lavVideoDecoderPrimary, "Output", videoRendererPrimary, "VMR Input0");
                DsError.ThrowExceptionForHR(hr);

                // 配置视频窗口
                videoWindowPrimary = (IVideoWindow)vmr9Primary;
                videoWindowPrimary.put_Owner(PrimaryForm.Instance.primaryScreenPanel.Handle); // 设置为 primaryScreenPanel 的句柄
                videoWindowPrimary.put_WindowStyle(WindowStyle.Child | WindowStyle.ClipChildren);
                videoWindowPrimary.SetWindowPosition(0, 0, 1210, 900);
                videoWindowPrimary.put_Visible(OABool.False);

                // 保存Graph文件以供调试
                SaveGraphFile(graphBuilderPrimary, "primary_graph.grf");

                if (hr == 0)
                {
                    Console.WriteLine("Primary file rendered successfully.");
                }
                else
                {
                    Console.WriteLine("Failed to render primary file.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to render primary file: " + ex.Message);
            }
        }

        public void RenderMediaFileSecondary(string filePath)
        {
            int hr = graphBuilderSecondary.RenderFile(filePath, null);
            DsError.ThrowExceptionForHR(hr);
            SaveGraphFile(graphBuilderSecondary, "secondary_graph.grf");
            if (hr == 0)
            {
                Console.WriteLine("Secondary File rendered successfully.");
            }
            else
            {
                Console.WriteLine("Failed to render secondary file.");
            }
        }

        public static void SaveGraphFile(IGraphBuilder graph, string filename)
        {
            var writer = new StreamWriter(filename);
            IFilterGraph2 graph2 = graph as IFilterGraph2;

            if (graph2 != null)
            {
                // 获取过滤器枚举器
                IEnumFilters enumFilters;
                graph2.EnumFilters(out enumFilters);

                enumFilters.Reset();
                IBaseFilter[] filters = new IBaseFilter[1];
                while (enumFilters.Next(1, filters, IntPtr.Zero) == 0)
                {
                    FilterInfo filterInfo;
                    filters[0].QueryFilterInfo(out filterInfo);
                    writer.WriteLine("Filter: " + filterInfo.achName);

                    // 枚举每个过滤器的引脚
                    IEnumPins enumPins;
                    filters[0].EnumPins(out enumPins);
                    enumPins.Reset();
                    IPin[] pins = new IPin[1];
                    while (enumPins.Next(1, pins, IntPtr.Zero) == 0)
                    {
                        PinInfo pinInfo;
                        pins[0].QueryPinInfo(out pinInfo);
                        writer.WriteLine("  Pin: " + pinInfo.name);
                        Marshal.ReleaseComObject(pins[0]);
                    }
                    Marshal.ReleaseComObject(enumPins);
                    Marshal.ReleaseComObject(filters[0]);
                }

                Marshal.ReleaseComObject(enumFilters);
            }

            writer.Close();
        }

        private static void RemoveAllFilters(IGraphBuilder graph)
        {
            IEnumFilters enumFilters;
            graph.EnumFilters(out enumFilters);
            IBaseFilter[] filters = new IBaseFilter[1];
            while (enumFilters.Next(1, filters, IntPtr.Zero) == 0)
            {
                graph.RemoveFilter(filters[0]);
                Marshal.ReleaseComObject(filters[0]);
            }
            Marshal.ReleaseComObject(enumFilters);
        }

        private void InitializeOverlayForm(Screen secondaryScreen)
        {
            overlayForm = new OverlayForm();

            // 使用 ScreenHelper 获取第二显示器信息
            Screen secondMonitor = ScreenHelper.GetSecondMonitor();

            // 確保 OverlayForm 出現在第二螢幕上
            // 您可能需要根據實際情況調整這裡的邏輯，以確保 secondaryScreen 確實是您想要顯示 OverlayForm 的螢幕
            if (secondMonitor != null)
            {
                overlayForm.Location = secondMonitor.WorkingArea.Location; // 將 OverlayForm 的位置設定為第二螢幕的起始位置
                overlayForm.StartPosition = FormStartPosition.Manual; // 防止窗體自動回到主螢幕
                // 根據需要調整 OverlayForm 的大小
                overlayForm.Size = new Size(secondMonitor.WorkingArea.Width, secondMonitor.WorkingArea.Height);
            }

            overlayForm.ShowInTaskbar = false;
            overlayForm.Owner = this;

            // Show the overlay form without stealing focus
            overlayForm.Show();
            this.Focus();
        }

        public void MonitorMediaEvents()
        {
            EventCode evCode;
            IntPtr param1, param2;
            Console.WriteLine("開始監聽媒體事件...");

            while (mediaEventExSecondary != null)
            {
                // 检查事件，这里使用 100 毫秒超时
                if (mediaEventExSecondary.GetEvent(out evCode, out param1, out param2, 100) >= 0)
                {
                    // Console.WriteLine(String.Format("事件被觸發: {0}", evCode));

                    // 处理事件
                    switch (evCode)
                    {
                        case EventCode.Complete:
                            Console.WriteLine("播放完成，準備加載下一首歌...");
                            // 播放结束，载入下一首歌
                            this.Invoke((MethodInvoker)delegate { 
                                if (isUserPlaylistPlaying)
                                {
                                    playingSongList.RemoveAt(0);
                                    if (!playingSongList.Any()) // 或者使用 playingSongList.Count == 0
                                    {
                                        PrimaryForm.playStates[PrimaryForm.currentSongIndexInHistory] = PlayState.Played;
                                        PlayPublicPlaylist();
                                    }
                                    else
                                    {
                                        currentSongIndex = -1;
                                        PrimaryForm.playStates[PrimaryForm.currentSongIndexInHistory] = PlayState.Played;
                                        PrimaryForm.currentSongIndexInHistory += 1;
                                        PrimaryForm.playStates[PrimaryForm.currentSongIndexInHistory] = PlayState.Playing;
                                        UpdateMarqueeTextForCurrentSong(playingSongList[0]);
                                        PlayNextSong();
                                    }
                                }
                                else
                                {
                                    currentSongIndex++;
                                    PlayNextSong();   
                                }
                            });
                            break;
                        default:
                            Console.WriteLine(String.Format("處理其他事件: {0}", evCode));
                            // 处理其他事件
                            break;
                    }

                    // 重置事件
                    mediaEventExSecondary.FreeEventParams(evCode, param1, param2);
                }
            }
        }

        public void BringOverlayToFront()
        {
            if (overlayForm != null)
            {
                if (!overlayForm.Visible)
                {
                    overlayForm.Show();
                }

                overlayForm.BringToFront();
                overlayForm.TopMost = true; // 確保窗體仍然是最頂層的
            }
        }

        public void Play()
        {
            if (mediaControlPrimary != null)
                mediaControlPrimary.Run();
            if (mediaControlSecondary != null)
                mediaControlSecondary.Run();
            isPaused = false; // 更新状态为未暂停
            OverlayForm.MainForm.HidePauseLabel(); // 隐藏暂停标签
        }

        public void Stop()
        {
            if (mediaControlPrimary != null)
                mediaControlPrimary.Stop();
            if (mediaControlSecondary != null)
                mediaControlSecondary.Stop();
        }

        public void Pause()
        {
            if (mediaControlPrimary != null)
                mediaControlPrimary.Pause();
            if (mediaControlSecondary != null)
                mediaControlSecondary.Pause();
            isPaused = true; // 更新状态为暂停
            OverlayForm.MainForm.ShowPauseLabel(); // 显示暂停标签
        }

        public void PauseOrResumeSong()
        {
            // 在这里执行按钮点击后的操作
            // 暂停歌曲操作
            if (isPaused)
            {
                Play();
                PrimaryForm.Instance.pauseButton.Visible = true;
                PrimaryForm.Instance.playButton.Visible = false;
                PrimaryForm.Instance.syncPauseButton.Visible = true;
                PrimaryForm.Instance.syncPlayButton.Visible = false;
            }
            else
            {
                Pause();
                PrimaryForm.Instance.pauseButton.Visible = false;
                PrimaryForm.Instance.playButton.Visible = true;
                PrimaryForm.Instance.syncPauseButton.Visible = false;
                PrimaryForm.Instance.syncPlayButton.Visible = true;
                OverlayForm.MainForm.ShowPauseLabel();
            }
        }

        private void UpdateSyncButtons()
        {
            if (isPaused)
            {
                PrimaryForm.Instance.syncPlayButton.Visible = true;
                PrimaryForm.Instance.syncPauseButton.Visible = false;
            }
            else
            {
                PrimaryForm.Instance.syncPlayButton.Visible = false;
                PrimaryForm.Instance.syncPauseButton.Visible = true;
            }
        }

        // 在 VideoPlayerForm 类中添加音量控制方法
        public void SetVolume(int volume)
        {
            if (audioRenderer != null)
            {
                IBasicAudio basicAudio = audioRenderer as IBasicAudio;
                if (basicAudio != null)
                {
                    basicAudio.put_Volume(volume);
                }
            }
        }
        public void SetBalance(int balance)
        {
            if (audioRenderer != null)
            {
                IBasicAudio basicAudio = audioRenderer as IBasicAudio;
                if (basicAudio != null)
                {
                    // 確保 balance 在 -10000 到 +10000 之間
                    balance = Math.Max(-10000, Math.Min(balance, 10000));
                    basicAudio.put_Balance(balance);
                    Console.WriteLine($"Balance set to {balance}");
                }
                else
                {
                    Console.WriteLine("audioRenderer does not implement IBasicAudio");
                }
            }
            else
            {
                Console.WriteLine("audioRenderer is null");
            }
        }

        // 获取当前音量的方法
        public int GetVolume()
        {
            if (audioRenderer != null)
            {
                IBasicAudio basicAudio = audioRenderer as IBasicAudio;
                if (basicAudio != null)
                {
                    int volume;
                    basicAudio.get_Volume(out volume);
                    return volume;
                }
            }
            return -10000; // 返回默认静音音量
        }

        private bool isVocalRemoved = false;



        public void ToggleVocalRemoval()
        {
            Console.WriteLine("ToggleVocalRemoval called");
            if (isVocalRemoved)
            {
                // 恢复正常平衡
                SetBalance(0); // 居中平衡，左右聲道均衡
                isVocalRemoved = false;
                Console.WriteLine("Vocal removal disabled.");
                OverlayForm.MainForm.HideOriginalSongLabel();
            }
            else
            {
                // 偏向右聲道，靜音左聲道
                SetBalance(10000); // 全右平衡，左聲道靜音
                isVocalRemoved = true;
                Console.WriteLine("Vocal removal enabled.");
                OverlayForm.MainForm.ShowOriginalSongLabel();
            }
        }



        public void SetChannelVolume(int channel, int volume)
        {
            if (audioRenderer != null)
            {
                IBasicAudio basicAudio = audioRenderer as IBasicAudio;
                if (basicAudio != null)
                {
                    // 获取当前音量
                    int currentVolume;
                    basicAudio.get_Volume(out currentVolume);

                    // 根据通道调整音量
                    if (channel == 0)
                    {
                        // 左声道
                        basicAudio.put_Volume(currentVolume + volume);
                    }
                    else if (channel == 1)
                    {
                        // 右声道
                        basicAudio.put_Volume(currentVolume - volume);
                    }
                }
            }
        }
    }
}