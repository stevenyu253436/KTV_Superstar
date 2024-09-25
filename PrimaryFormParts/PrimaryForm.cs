using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using NAudio.Wave; // For NAudio components
using Microsoft.Ink; // For Ink components
using System.Text.RegularExpressions; // For Regex and Match
using WMPLib;

namespace DualScreenDemo
{
    public partial class PrimaryForm : Form
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
        public static PrimaryForm Instance { get; private set; }

        // private MediaManager mediaManager;
        private ProgressBar progressBar;

        private PictureBox pictureBox1;
        private PictureBox pictureBox2;
        private PictureBox pictureBox3;
        private PictureBox pictureBox4;
        private PictureBox pictureBox5;
        private PictureBox pictureBox6;

        private const int offsetX = 100;

        private PictureBox pictureBoxArtistSearch;

        private Button[] numberButtonsArtistSearch;
        private Button modifyButtonArtistSearch, closeButtonArtistSearch;
        private RichTextBox inputBoxArtistSearch;
        private const int offsetXArtistSearch = 100; // You may need to adjust this offset for layout purposes
        private const int offsetYArtistSearch = 100; // You may need to adjust this offset for layout purposes

        private PictureBox pictureBoxWordCount;

        private Button[] numberButtonsWordCount;
        private Button modifyButtonWordCount, closeButtonWordCount;
        private RichTextBox inputBoxWordCount;
        private const int offsetXWordCount = 100;
        private const int offsetYWordCount = 100;

        private PictureBox pictureBoxSongIDSearch;

        private Button[] numberButtonsSongIDSearch;
        private Button modifyButtonSongIDSearch, closeButtonSongIDSearch;
        private RichTextBox inputBoxSongIDSearch;
        private const int offsetXSongID = 100;
        private const int offsetYSongID = 100;
        private const int offsetXPinYin = 100;

        private Button singerSearchButton;
        private Bitmap singerSearchNormalBackground;
        private Bitmap singerSearchActiveBackground;
        private Button songSearchButton;
        private Bitmap songSearchNormalBackground;
        private Bitmap songSearchActiveBackground;
        private Button serviceBellButton;
        private Button deliciousFoodButton;
        private Bitmap deliciousFoodNormalBackground;
        private Bitmap deliciousFoodActiveBackground;
        private Button mobileSongRequestButton;
        private Button qieGeButton;
        private Button musicUpButton;
        private Button musicDownButton;
        private Button micUpButton;
        private Button micDownButton;
        private Button originalSongButton;
        private Button replayButton;
        public Button pauseButton;
        public Button playButton;
        private Button muteButton;
        private Button maleKeyButton;
        private Button femaleKeyButton;
        private Button standardKeyButton;
        private Button soundEffectButton;

        private Button pitchUpButton;
        private Button pitchDownButton;
        private Button syncScreenButton;
        private Button toggleLightButton;

        private PictureBox promotionsPictureBox;

        private List<Image> promotions;
        private List<Image> menu;

        private PictureBox VodScreenPictureBox;

        private Panel overlayPanel;

        private Button btnPreviousPage;
        private Button btnReturn;
        private Button btnNextPage;
        private Button btnApplause;
        private Button btnSimplifiedChinese;
        private Button btnTraditionalChinese;
        private Button exitButton;
        
        private static Bitmap normalStateImage;
        private static Bitmap mouseOverImage;
        private static Bitmap mouseDownImage;

        // 靜態變量儲存圖像
        private static Bitmap resizedNormalStateImage;
        private static Bitmap resizedMouseOverImage;
        private static Bitmap resizedMouseDownImage;

        private static Bitmap normalStateImageNewSongAlert;
        private static Bitmap mouseOverImageNewSongAlert;
        private static Bitmap mouseDownImageNewSongAlert;

        private static Bitmap resizedNormalStateImageForNewSongAlert;
        private static Bitmap resizedMouseOverImageForNewSongAlert;
        private static Bitmap resizedMouseDownImageForNewSongAlert;

        private static Bitmap normalStateImageArtistQuery;
        private static Bitmap mouseOverImageArtistQuery;
        private static Bitmap mouseDownImageArtistQuery;

        private static Bitmap resizedNormalStateImageForArtistQuery;
        private static Bitmap resizedMouseOverImageForArtistQuery;
        private static Bitmap resizedMouseDownImageForArtistQuery;

        private static Bitmap normalStateImageSongQuery;
        private static Bitmap mouseOverImageSongQuery;
        private static Bitmap mouseDownImageSongQuery;

        private static Bitmap resizedNormalStateImageForSongQuery;
        private static Bitmap resizedMouseOverImageForSongQuery;
        private static Bitmap resizedMouseDownImageForSongQuery;

        private static Bitmap normalStateImageLanguageQuery;
        private static Bitmap mouseOverImageLanguageQuery;
        private static Bitmap mouseDownImageLanguageQuery;

        private static Bitmap resizedNormalStateImageForLanguageQuery;
        private static Bitmap resizedMouseOverImageForLanguageQuery;
        private static Bitmap resizedMouseDownImageForLanguageQuery;

        private static Bitmap normalStateImage6_1;
        private static Bitmap mouseOverImage6_1;
        private static Bitmap mouseDownImage6_1;

        private static Bitmap resizedNormalStateImageFor6_1;
        private static Bitmap resizedMouseOverImageFor6_1;
        private static Bitmap resizedMouseDownImageFor6_1;

        private static Bitmap normalStateImageCategoryQuery;
        private static Bitmap mouseOverImageCategoryQuery;
        private static Bitmap mouseDownImageCategoryQuery;

        private static Bitmap resizedNormalStateImageForCategoryQuery;
        private static Bitmap resizedMouseOverImageForCategoryQuery;
        private static Bitmap resizedMouseDownImageForCategoryQuery;

        private static Bitmap normalStateImage7_1;
        private static Bitmap mouseOverImage7_1;
        private static Bitmap mouseDownImage7_1;

        private static Bitmap resizedNormalStateImageFor7_1;
        private static Bitmap resizedMouseOverImageFor7_1;
        private static Bitmap resizedMouseDownImageFor7_1;

        private static Bitmap normalStateImage7_1_1;
        private static Bitmap mouseOverImage7_1_1;
        private static Bitmap mouseDownImage7_1_1;

        private static Bitmap resizedNormalStateImageFor7_1_1;
        private static Bitmap resizedMouseOverImageFor7_1_1;
        private static Bitmap resizedMouseDownImageFor7_1_1;

        // For Promotions and Menu button images
        private static Bitmap normalStateImageForPromotionsAndMenu;
        private static Bitmap resizedNormalStateImageForPromotionsAndMenu;

        private static Bitmap normalStateImageForSyncScreen;
        private static Bitmap resizedNormalStateImageForSyncScreen;

        private static Bitmap normalStateImageForSceneSoundEffects;
        private static Bitmap resizedNormalStateImageForSceneSoundEffects;

        private static Bitmap normalStateImageForLightControl;
        private static Bitmap resizedNormalStateImageForLightControl;

        public VideoPlayerForm videoPlayerForm;

        // private ListView listViewSongs;
        public List<SongData> allSongs; // This should be filled with your song data
        public List<Artist> allArtists;
        public List<SongData> currentSongList;
        public List<Artist> currentArtistList;
        public List<SongData> publicSongList;
        private List<SongData> guoYuSongs;
        private List<SongData> taiYuSongs;
        private List<SongData> yueYuSongs;
        private List<SongData> yingWenSongs;
        private List<SongData> riYuSongs;
        private List<SongData> hanYuSongs;
        private List<SongData> guoYuSongs2;
        private List<SongData> taiYuSongs2;
        private List<SongData> yueYuSongs2;
        private List<SongData> yingWenSongs2;
        private List<SongData> riYuSongs2;
        private List<SongData> hanYuSongs2;
        private List<SongData> loveDuetSongs;
        private List<SongData> talentShowSongs;
        private List<SongData> medleyDanceSongs;
        private List<SongData> ninetiesSongs;
        private List<SongData> nostalgicSongs;
        private List<SongData> chinaSongs;
        private List<SongData> vietnameseSongs;
        public static List<SongData> userRequestedSongs;
        public static List<SongData> playedSongsHistory;
        public static List<PlayState> playStates;
        public static int currentSongIndexInHistory = -1;
        public MultiPagePanel multiPagePanel;
        private List<Label> songLabels = new List<Label>();
        public int currentPage = 0;
        public int totalPages; // 成員變量用於存儲總頁數
        public const int itemsPerPage = 18;
        private const int RowsPerPage = 9;
        private const int Columns = 2;

        private WaveInEvent waveIn;
        private WaveFileWriter waveWriter;
        // private string outputFilePath;


        // 常量定义，方便维护和理解
        private const int PanelStartLeft = 192;
        private const int PanelStartTop = 417;
        private const int PanelEndLeft = 991;
        private const int PanelEndTop = 681;

        private Timer lightControlTimer;
        public Timer volumeUpTimer;
        public Timer volumeDownTimer;
        private DateTime lastVolumeUpTime = DateTime.MinValue;
        private DateTime lastVolumeDownTime = DateTime.MinValue;
        public Timer micControlTimer;

        private SequenceManager sequenceManager = new SequenceManager();

        private Button buttonMiddle;
        private Button buttonTopRight;
        private Button buttonTopLeft;
        private Button buttonThanks;

        private Dictionary<Control, (Point Location, bool Visible)> initialControlStates = new Dictionary<Control, (Point Location, bool Visible)>();

        public PrimaryForm()
        {
            Instance = this;

            this.DoubleBuffered = true; // 启用双缓冲
            InitializeComponent();
            InitializeProgressBar();
            lightControlTimer = new Timer();
            lightControlTimer.Interval = 5; // 設置間隔為 100 毫秒
            lightControlTimer.Tick += LightControlTimer_Tick;
            volumeUpTimer = new Timer();
            volumeUpTimer.Interval = 100; // 設置間隔為 100 毫秒
            volumeUpTimer.Tick += VolumeUpTimer_Tick;
            volumeDownTimer = new Timer();
            volumeDownTimer.Interval = 100;
            volumeDownTimer.Tick += VolumeDownTimer_Tick;
            micControlTimer = new Timer();
            micControlTimer.Interval = 100;
            micControlTimer.Tick += MicControlTimer_Tick;
            // Initialize recording components
            InitializeRecording();
            InitializeMediaPlayer();
            LoadSongData();
            LoadImages(); // Make sure this is called before you initialize other controls that depend on these images
            InitializeFormAndControls(); // 新的初始化调用
            InitializeMultiPagePanel();
            OverlayQRCodeOnImage(HttpServer.randomFolderPath);

            InitializeHandWritingForSingers();
            InitializeHandWritingForSongs();

            // 创建按钮并设置属性
            buttonMiddle = new Button { Text = "臨", Visible = false };
            buttonTopRight = new Button { Text = "右上", Visible = false };
            buttonTopLeft = new Button { Text = "左上", Visible = false };
            buttonThanks = new Button { Text = "謝", Visible = false };

            // 定位和尺寸设置
            ResizeAndPositionButton(buttonMiddle, 1208, 423, 89, 89);
            ResizeAndPositionButton(buttonTopRight, 1394, 2, 1428 - 1394, 37 - 2);
            ResizeAndPositionButton(buttonTopLeft, 10, 2, 34, 35);
            ResizeAndPositionButton(buttonThanks, 633, 421, 91, 91);

            // 添加事件处理程序
            buttonMiddle.Click += buttonMiddle_Click;
            buttonTopRight.Click += buttonTopRight_Click;
            buttonTopLeft.Click += buttonTopLeft_Click;
            buttonThanks.Click += buttonThanks_Click;

            // 添加按钮到窗体
            this.Controls.Add(buttonMiddle);
            this.Controls.Add(buttonTopRight);
            this.Controls.Add(buttonTopLeft);
            this.Controls.Add(buttonThanks);

            // 初始化 PromotionsAndMenuPanel
            InitializePromotionsAndMenuPanel();

            // 保存初始状态
            SaveInitialControlStates(this);

            // 设置窗口最大化和无边框样式以实现全屏效果
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;

            // 訂閱 Paint 事件
            this.Paint += PrimaryForm_Paint;
        }

        private void SaveInitialControlStates(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                initialControlStates[control] = (control.Location, control.Visible);

                if (control.HasChildren)
                {
                    SaveInitialControlStates(control);
                }
            }
        }

        private void PrimaryForm_Paint(object sender, PaintEventArgs e)
        {
            // 獲取主機名稱
            string hostName = System.Net.Dns.GetHostName();

            // 取主機名稱末三碼，前面加個"包廂"
            string displayName = "包廂" + hostName.Substring(Math.Max(0, hostName.Length - 3));

            // 設置字體和顏色
            Font font = new Font("微軟正黑體", 24, FontStyle.Bold);
            Brush brush = new SolidBrush(Color.Red);

            // 設置顯示位置
            PointF point = new PointF(400, 30);

            // 繪製顯示名稱
            e.Graphics.DrawString(displayName, font, brush, point);
        }

        // Event handlers
        private void buttonMiddle_Click(object sender, EventArgs e)
        {
            sequenceManager.ProcessClick("中間");
        }

        private void buttonTopRight_Click(object sender, EventArgs e)
        {
            sequenceManager.ProcessClick("右上");
        }

        private void buttonTopLeft_Click(object sender, EventArgs e)
        {
            sequenceManager.ProcessClick("左上");
        }

        private void buttonThanks_Click(object sender, EventArgs e)
        {
            sequenceManager.ProcessClick("謝謝");
        }

        public void ShowSendOffScreen()
        {
            // 获取主机名
            string hostName = System.Environment.MachineName;
            // 从主机名中提取末尾的三个字符
            string boxNumber = hostName.Length >= 3 ? hostName.Substring(hostName.Length - 3) : hostName;

            // 隐藏所有按钮
            HideAllButtons();

            // 隐藏所有 PictureBox
            SetHotSongButtonsVisibility(false);
            SetNewSongButtonsVisibility(false);
            SetSingerSearchButtonsVisibility(false);
            SetSongSearchButtonsVisibility(false);
            SetPictureBoxLanguageButtonsVisibility(false);
            SetGroupButtonsVisibility(false);
            SetZhuYinSingersAndButtonsVisibility(false);
            SetZhuYinSongsAndButtonsVisibility(false);
            SetEnglishSingersAndButtonsVisibility(false);
            SetEnglishSongsAndButtonsVisibility(false);
            SetPictureBoxWordCountAndButtonsVisibility(false);
            SetPinYinSingersAndButtonsVisibility(false);
            SetPinYinSongsAndButtonsVisibility(false);
            SetPictureBoxToggleLightAndButtonsVisibility(false);
            pictureBoxQRCode.Visible = false;
            inputBoxZhuYinSingers.Text = "";
            inputBoxZhuYinSongs.Text = "";
            inputBoxEnglishSingers.Text = "";
            inputBoxEnglishSongs.Text = "";
            inputBoxPinYinSingers.Text = "";
            inputBoxPinYinSongs.Text = "";
            inputBoxWordCount.Text = "";

            multiPagePanel.Visible = false;

            // 使特定的按钮可见并置于前面
            buttonMiddle.Visible = true;
            buttonTopRight.Visible = true;
            buttonTopLeft.Visible = true;
            buttonThanks.Visible = true;

            buttonMiddle.BringToFront();
            buttonTopRight.BringToFront();
            buttonTopLeft.BringToFront();
            buttonThanks.BringToFront();

            // 加载送客画面背景图
            string filePath = Path.Combine(Application.StartupPath, "VOD_送客畫面.jpg");
            Image backgroundImage = Image.FromFile(filePath);

            // 创建Graphics对象进行绘图
            using (Graphics g = Graphics.FromImage(backgroundImage))
            {
                // 设置文本格式
                StringFormat sf = new StringFormat();
                sf.Alignment = StringAlignment.Center;
                sf.LineAlignment = StringAlignment.Center;

                // 定义文本区域
                Rectangle rect = new Rectangle(633, 300, 180, 60);

                // 定义字体
                using (Font font = new Font("Arial", 50, FontStyle.Bold, GraphicsUnit.Pixel))
                {
                    // 绘制文本
                    g.DrawString(boxNumber, font, Brushes.Black, rect, sf);
                }
            }

            // 将绘制了文本的图片设为背景
            this.BackgroundImage = backgroundImage;
            // 更多UI更新代码
        }

        public void HideAllButtons()
        {
            HideControlsRecursively(this); // 传入窗体自身作为起点
        }

        private void HideControlsRecursively(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                if (control is Button) // 检查控件是否为按钮
                {
                    control.Visible = false; // 隐藏按钮
                }
                else if (control.HasChildren) // 检查控件是否包含子控件
                {
                    HideControlsRecursively(control); // 递归隐藏子控件中的按钮
                }
            }
        }

        public void HideSendOffScreen()
        {
            this.DoubleBuffered = true; // 启用双缓冲
            InitializeComponent();
            InitializeProgressBar();
            lightControlTimer = new Timer();
            lightControlTimer.Interval = 5; // 設置間隔為 100 毫秒
            lightControlTimer.Tick += LightControlTimer_Tick;
            volumeUpTimer = new Timer();
            volumeUpTimer.Interval = 100; // 設置間隔為 100 毫秒
            volumeUpTimer.Tick += VolumeUpTimer_Tick;
            volumeDownTimer = new Timer();
            volumeDownTimer.Interval = 100;
            volumeDownTimer.Tick += VolumeDownTimer_Tick;
            micControlTimer = new Timer();
            micControlTimer.Interval = 100;
            micControlTimer.Tick += MicControlTimer_Tick;
            // Initialize recording components
            InitializeRecording();
            InitializeMediaPlayer();
            LoadSongData();
            LoadImages(); // Make sure this is called before you initialize other controls that depend on these images
            InitializeFormAndControls(); // 新的初始化调用
            InitializeMultiPagePanel();
            OverlayQRCodeOnImage(HttpServer.randomFolderPath);

            InitializeHandWritingForSingers();
            InitializeHandWritingForSongs();

            // 创建按钮并设置属性
            buttonMiddle = new Button { Text = "臨", Visible = false };
            buttonTopRight = new Button { Text = "右上", Visible = false };
            buttonTopLeft = new Button { Text = "左上", Visible = false };
            buttonThanks = new Button { Text = "謝", Visible = false };

            // 定位和尺寸设置
            ResizeAndPositionButton(buttonMiddle, 1208, 423, 89, 89);
            ResizeAndPositionButton(buttonTopRight, 1394, 2, 1428 - 1394, 37 - 2);
            ResizeAndPositionButton(buttonTopLeft, 10, 2, 34, 35);
            ResizeAndPositionButton(buttonThanks, 633, 421, 91, 91);

            // 添加事件处理程序
            buttonMiddle.Click += buttonMiddle_Click;
            buttonTopRight.Click += buttonTopRight_Click;
            buttonTopLeft.Click += buttonTopLeft_Click;
            buttonThanks.Click += buttonThanks_Click;

            // 添加按钮到窗体
            this.Controls.Add(buttonMiddle);
            this.Controls.Add(buttonTopRight);
            this.Controls.Add(buttonTopLeft);
            this.Controls.Add(buttonThanks);

            // 初始化 PromotionsAndMenuPanel
            InitializePromotionsAndMenuPanel();

            // 设置窗口最大化和无边框样式以实现全屏效果
            this.WindowState = FormWindowState.Maximized;
            this.FormBorderStyle = FormBorderStyle.None;
            
            // 确保所有控件和资源已完全加载
            this.Shown += (sender, e) =>
            {
                RestoreInitialControlStates(this);
            };
        }

        private void RestoreInitialControlStates(Control parent)
        {
            foreach (Control control in parent.Controls)
            {
                if (initialControlStates.TryGetValue(control, out var state))
                {
                    control.Location = state.Location;
                    control.Visible = state.Visible;
                }

                if (control.HasChildren)
                {
                    RestoreInitialControlStates(control);
                }
            }
        }

        private void UpdateProgress(TimeSpan currentPosition)
        {
            // 更新進度條或播放資訊
            if (progressBar.InvokeRequired) {
                progressBar.Invoke(new System.Action(() => {
                    progressBar.Value = (int)currentPosition.TotalSeconds;
                }));
            } else {
                progressBar.Value = (int)currentPosition.TotalSeconds;
            }
        }

        private void InitializeComponent()
        {

        }

        private void InitializeProgressBar()
        {
            // 創建進度條實例
            progressBar = new ProgressBar();

            // 設置進度條位置和大小
            progressBar.Location = new System.Drawing.Point(10, 10); // 這裡的數值根據需要調整
            progressBar.Size = new System.Drawing.Size(300, 30); // 這裡的數值根據需要調整

            // 設置進度條的其他屬性
            progressBar.Minimum = 0;
            progressBar.Maximum = 100;
            progressBar.Value = 0;

            // 將進度條添加到表單控件集中
            this.Controls.Add(progressBar);
        }

        private void EnableDoubleBuffering(Control control)
        {
            if (control != null)
            {
                var doubleBufferedProperty = control.GetType().GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
                if (doubleBufferedProperty != null)
                {
                    doubleBufferedProperty.SetValue(control, true, null);
                }
            }
        }

        private void InitializeRecording()
        {
            // 列出所有可用的錄音設備
            for (int n = 0; n < WaveIn.DeviceCount; n++)
            {
                var caps = WaveIn.GetCapabilities(n);
                Console.WriteLine(String.Format("{0}: {1}", n, caps.ProductName));
            }

            waveIn = new WaveInEvent();
            waveIn.DataAvailable += OnDataAvailable;
            waveIn.RecordingStopped += OnRecordingStopped; // 訂閱停止錄音的事件
            waveIn.WaveFormat = new WaveFormat(44100, 1); // CD quality mono
            // string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            // outputFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), String.Format("recording_{0}.wav", timestamp));
        }

        private void OnDataAvailable(object sender, WaveInEventArgs e)
        {
            if (waveWriter != null)
            {
                waveWriter.Write(e.Buffer, 0, e.BytesRecorded);
                waveWriter.Flush();
            }
        }

        private void OnRecordingStopped(object sender, StoppedEventArgs e)
        {
            if (waveIn != null)
            {
                waveIn.Dispose();
                waveIn = null;
            }

            if (waveWriter != null)
            {
                waveWriter.Dispose();
                waveWriter = null;
            }
        }

        private void InitializeFormAndControls()
        {
            this.SuspendLayout();
            // 初始化并调整 PictureBox
            InitializePictureBox();

            // 直接在Form上初始化并添加按钮
            InitializeButtonsForHotSong(); // 修改此处
            InitializeButtonsForNewSong(); // 初始化 pictureBox2 上的按钮
            InitializeButtonsForSingerSearch();
            InitializeButtonsForSongSearch();
            InitializeButtonsForPictureBoxLanguageQuery();
            InitializeButtonsForGroupPictureBox();
            InitializeCategorySearchButtons();
            InitializeButtonsForZhuYinSingers();
            InitializeButtonsForZhuYinSongs();
            InitializeButtonsForEnglishSingers();
            InitializeButtonsForEnglishSongs();
            InitializeButtonsForPictureBoxArtistSearch();
            InitializeButtonsForPictureBoxWordCount();
            InitializeButtonsForPinYinSingers();
            InitializeButtonsForPinYinSongs();
            InitializeButtonsForPictureBoxSongIDSearch();
            InitializeButtonsForFavoritePictureBox();
            InitializePromotionsButton();
            InitializeButtonsForPictureBoxToggleLight();
            InitializeButtonsForVodScreenPictureBox();
            InitializeSoundEffectButtons();
            InitializeSyncScreen();

            // 添加其他控件（如按钮）并调整它们的位置
            InitializeOtherControls(); // 假设这是添加和设置其他控件的方法

            // 从配置文件读取主题路径
            string selectedTheme = ReadSelectedThemePath();
            if (!string.IsNullOrEmpty(selectedTheme))
            {
                // 假设主题路径下有一个命名为 "background.png" 的背景图片文件
                string backgroundImagePath = Path.Combine(Application.StartupPath, "themes\\superstar\\555009.jpg");
                
                try
                {
                    this.BackgroundImage = Image.FromFile(backgroundImagePath);
                    this.BackgroundImageLayout = ImageLayout.Stretch;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(String.Format("加载背景图片时发生错误: {0}", ex.Message));
                    // 可以设置一个默认背景图片或采取其他措施
                }
            }
            // 设置 Form 的背景图片
            // this.BackgroundImage = Image.FromFile(@"themes\black\0.png");
            // this.BackgroundImageLayout = ImageLayout.Stretch;

            // 确保 PictureBox 在背景图片之上
            pictureBox1.BringToFront();
            pictureBox2.BringToFront();
            pictureBox3.BringToFront();
            pictureBox4.BringToFront();
            pictureBoxQRCode.BringToFront();
            pictureBoxZhuYinSingers.BringToFront();
            pictureBoxZhuYinSongs.BringToFront();
            pictureBoxEnglishSingers.BringToFront();
            pictureBoxEnglishSongs.BringToFront();
            pictureBoxWordCount.BringToFront();
            FavoritePictureBox.BringToFront();
            promotionsPictureBox.BringToFront();
            pictureBoxToggleLight.BringToFront();
            overlayPanel.BringToFront();
            VodScreenPictureBox.BringToFront();

            newSongAlertButton.BringToFront();
            hotPlayButton.BringToFront();
            singerSearchButton.BringToFront();
            songSearchButton.BringToFront();
            languageSearchButton.BringToFront();
            groupSearchButton.BringToFront();
            categorySearchButton.BringToFront();
            serviceBellButton.BringToFront();
            orderedSongsButton.BringToFront();
            myFavoritesButton.BringToFront();
            deliciousFoodButton.BringToFront();
            promotionsButton.BringToFront();
            mobileSongRequestButton.BringToFront();
            qieGeButton.BringToFront();
            musicUpButton.BringToFront();
            musicDownButton.BringToFront();
            micUpButton.BringToFront();
            micDownButton.BringToFront();
            originalSongButton.BringToFront();
            replayButton.BringToFront();
            pauseButton.BringToFront();
            playButton.BringToFront();
            muteButton.BringToFront();
            maleKeyButton.BringToFront();
            femaleKeyButton.BringToFront();
            standardKeyButton.BringToFront();
            soundEffectButton.BringToFront();
            pitchUpButton.BringToFront();
            pitchDownButton.BringToFront();
            syncScreenButton.BringToFront();
            toggleLightButton.BringToFront();
            btnPreviousPage.BringToFront();
            btnReturn.BringToFront();
            btnNextPage.BringToFront();
            btnApplause.BringToFront();
            btnSimplifiedChinese.BringToFront();
            btnTraditionalChinese.BringToFront();
            exitButton.BringToFront();

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private string ReadSelectedThemePath()
        {
            // 指定配置文件路径
            string configFilePath = Path.Combine(Application.StartupPath, "theme_description.txt");
            try
            {
                // 读取所有行到一个字符串数组中
                string[] lines = File.ReadAllLines(configFilePath);

                // 遍历每行，寻找 "Selected Theme:" 开头的行
                foreach (string line in lines)
                {
                    if (line.StartsWith("Selected Theme: "))
                    {
                        // 提取主题路径
                        string themePath = line.Substring("Selected Theme: ".Length).Trim();
                        return themePath;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(String.Format("读取文件时发生错误: {0}", ex.Message));
            }

            return string.Empty; // 如果未找到或发生错误，返回空字符串
        }

        private void InitializePictureBox()
        {
            pictureBox1 = new PictureBox
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                SizeMode = PictureBoxSizeMode.StretchImage
            };

            // Initialize and configure pictureBox2
            pictureBox2 = new PictureBox
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                SizeMode = PictureBoxSizeMode.StretchImage,
                Visible = false // Initially invisible
            };

            // Initialize and configure pictureBox3
            pictureBox3 = new PictureBox
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                SizeMode = PictureBoxSizeMode.StretchImage,
                Visible = false // Initially invisible
            };

            // Initialize and configure pictureBox4
            pictureBox4 = new PictureBox
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                SizeMode = PictureBoxSizeMode.StretchImage,
                Visible = false // Initially invisible
            };

            // Initialize and configure pictureBox5
            pictureBox5 = new PictureBox
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                SizeMode = PictureBoxSizeMode.StretchImage,
                Visible = false // Initially invisible
            };

            // Initialize and configure pictureBox6
            pictureBox6 = new PictureBox
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                SizeMode = PictureBoxSizeMode.StretchImage,
                Visible = false // Initially invisible
            };
            
            pictureBoxZhuYinSingers = new PictureBox
            {
                Name = "pictureBoxZhuYinSingers", // 设置 PictureBox 的名称
                BackColor = Color.Transparent,
                SizeMode = PictureBoxSizeMode.StretchImage,
                Visible = false // Initially invisible
            };

            pictureBoxZhuYinSongs = new PictureBox
            {
                Name = "pictureBoxZhuYinSongs", // 设置 PictureBox 的名称
                BackColor = Color.Transparent,
                SizeMode = PictureBoxSizeMode.StretchImage,
                Visible = false // Initially invisible
            };

            pictureBoxEnglishSingers = new PictureBox
            {
                Name = "pictureBoxEnglishSingers",
                BackColor = Color.Transparent,
                SizeMode = PictureBoxSizeMode.StretchImage,
                Visible = false // 初始不可见
            };

            pictureBoxEnglishSongs = new PictureBox
            {
                Name = "pictureBoxEnglishSongs",
                BackColor = Color.Transparent,
                SizeMode = PictureBoxSizeMode.StretchImage,
                Visible = false // 初始不可见
            };

            pictureBoxArtistSearch = new PictureBox
            {
                Name = "pictureBoxArtistSearch",
                BackColor = Color.Transparent,
                SizeMode = PictureBoxSizeMode.StretchImage,
                Visible = false // 初始不可见
            };

            pictureBoxWordCount = new PictureBox
            {
                Name = "pictureBoxWordCount",
                BackColor = Color.Transparent,
                SizeMode = PictureBoxSizeMode.StretchImage,
                Visible = false // 初始不可见
            };

            pictureBoxPinYinSingers = new PictureBox
            {
                Name = "pictureBoxPinYinSingers",
                BackColor = Color.Transparent,
                SizeMode = PictureBoxSizeMode.StretchImage,
                Visible = false // 初始不可见
            };

            pictureBoxPinYinSongs = new PictureBox
            {
                Name = "pictureBoxPinYinSongs",
                BackColor = Color.Transparent,
                SizeMode = PictureBoxSizeMode.StretchImage,
                Visible = false // 初始不可见
            };

            pictureBoxSongIDSearch = new PictureBox
            {
                Name = "pictureBoxSongIDSearch",
                BackColor = Color.Transparent,
                SizeMode = PictureBoxSizeMode.StretchImage,
                Visible = false // 初始不可见
            };

            pictureBoxHandWritingSingers = new PictureBox
            {
                Name = "pictureBoxHandWritingSingers",
                BackColor = Color.Transparent,
                SizeMode = PictureBoxSizeMode.StretchImage,
                Visible = false // 初始不可见
            };

            pictureBoxHandWritingSongs = new PictureBox
            {
                Name = "pictureBoxHandWritingSongs",
                BackColor = Color.Transparent,
                SizeMode = PictureBoxSizeMode.StretchImage,
                Visible = false // 初始不可见
            };

            FavoritePictureBox = new PictureBox
            {
                Name = "FavoritePictureBox",
                BackColor = Color.Transparent,
                SizeMode = PictureBoxSizeMode.StretchImage,
                Visible = false
            };
            FavoritePictureBox.Paint += new PaintEventHandler(FavoritePictureBox_Paint);

            promotionsPictureBox = new PictureBox
            {
                Name = "promotionsPictureBox",
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                SizeMode = PictureBoxSizeMode.StretchImage,
                Visible = false // 初始不可见
            };

            pictureBoxToggleLight = new PictureBox
            {
                Name = "pictureBoxToggleLight",
                BackColor = Color.Transparent,
                SizeMode = PictureBoxSizeMode.StretchImage,
                Visible = false // 初始不可见
            };

            VodScreenPictureBox = new PictureBox
            {
                Name = "VodScreenPictureBox",
                BackColor = Color.FromArgb(128, 0, 0, 0),
                SizeMode = PictureBoxSizeMode.StretchImage,
                Visible = false
            };

            overlayPanel = new Panel
            {
                Name = "overlayPanel",
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(128, 0, 0, 0),
                // SizeMode = PictureBoxSizeMode.StretchImage,
                Visible = false
            };

            pictureBoxQRCode = new PictureBox
            {
                Name = "pictureBoxQRCode",
                // Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                SizeMode = PictureBoxSizeMode.StretchImage,
                Visible = false // Initially invisible
            };

            pictureBoxSceneSoundEffects = new PictureBox
            {
                Name = "pictureBoxSceneSoundEffects",
                BackColor = Color.Transparent,
                SizeMode = PictureBoxSizeMode.StretchImage,
                Visible = false // Initially invisible
            };

            this.Controls.Add(pictureBox1);
            this.Controls.Add(pictureBox2);
            this.Controls.Add(pictureBox3);
            this.Controls.Add(pictureBox4);
            this.Controls.Add(pictureBox5);
            this.Controls.Add(pictureBox6);
            this.Controls.Add(pictureBoxQRCode); // Add the QR Code PictureBox to the form
            this.Controls.Add(pictureBoxZhuYinSingers);
            this.Controls.Add(pictureBoxZhuYinSongs);
            this.Controls.Add(pictureBoxEnglishSingers);
            this.Controls.Add(pictureBoxEnglishSongs);
            this.Controls.Add(pictureBoxArtistSearch);
            this.Controls.Add(pictureBoxWordCount);
            this.Controls.Add(pictureBoxPinYinSingers);
            this.Controls.Add(pictureBoxPinYinSongs);
            this.Controls.Add(pictureBoxHandWritingSingers);
            this.Controls.Add(pictureBoxHandWritingSongs);
            this.Controls.Add(pictureBoxSongIDSearch);
            this.Controls.Add(FavoritePictureBox);
            this.Controls.Add(promotionsPictureBox);
            this.Controls.Add(pictureBoxToggleLight);
            this.Controls.Add(VodScreenPictureBox);
            this.Controls.Add(overlayPanel);
            this.Controls.Add(pictureBoxSceneSoundEffects); // Add the scene sound effects PictureBox to the form
        }

        // Define the PhoneticButton_Click event handler
        private void PhoneticButton_Click(object sender, EventArgs e)
        {
            var button = sender as Button;
            if (button != null && button.Tag != null)
            {
                // 根据当前可见的文本框，将注音符号添加到相应的文本框
                if (inputBoxZhuYinSingers.Visible)
                {
                    inputBoxZhuYinSingers.Text += button.Tag.ToString();
                }
                else if (inputBoxZhuYinSongs.Visible)
                {
                    inputBoxZhuYinSongs.Text += button.Tag.ToString();
                }
            }
        }

        private void ModifyButtonArtist_Click(object sender, EventArgs e)
        {
            // 检查注音输入框是否存在
            if (inputBoxArtistSearch.Text.Length > 0)
            {
                // Logic for Modify button
                inputBoxArtistSearch.Text = inputBoxArtistSearch.Text.Substring(0, inputBoxArtistSearch.Text.Length - 1);
            }
        }

        private void ModifyButtonWordCount_Click(object sender, EventArgs e)
        {
            // 检查注音输入框是否存在
            if (inputBoxWordCount.Text.Length > 0)
            {
                // Logic for Modify button
                inputBoxWordCount.Text = inputBoxWordCount.Text.Substring(0, inputBoxWordCount.Text.Length - 1);
            }
        }

        private void BtnBrightnessUp1_MouseDown(object sender, MouseEventArgs e)
        {
            // 設置 Timer 用於發送的指令
            lightControlTimer.Tag = "a2 d9 a4";
            lightControlTimer.Start();
        }

        private void BtnBrightnessUp1_MouseUp(object sender, MouseEventArgs e)
        {
            lightControlTimer.Stop();
        }

        private void LightControlTimer_Tick(object sender, EventArgs e)
        {
            if(lightControlTimer.Tag != null)
            {
                SendCommandThroughSerialPort(lightControlTimer.Tag.ToString());
            }
        }

        private void VolumeUpTimer_Tick(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                try
                {
                    // 替换为实际的耗时操作
                    SendCommandThroughSerialPort("a2 b3 a4");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to send command: " + ex.Message);
                }
            });
        }

        private void VolumeDownTimer_Tick(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                try
                {
                    // 替换为实际的耗时操作
                    SendCommandThroughSerialPort("a2 b4 a4");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to send command: " + ex.Message);
                }
            });
        }

        private void MicControlTimer_Tick(object sender, EventArgs e)
        {
            Task.Run(() =>
            {
                try
                {
                    if(micControlTimer.Tag != null)
                    {
                        SendCommandThroughSerialPort(micControlTimer.Tag.ToString());
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to send command: " + ex.Message);
                }
            });
        }

        public static void SendCommandThroughSerialPort(string command)
        {
            if (SerialPortManager.mySerialPort != null && SerialPortManager.mySerialPort.IsOpen)
            {
                try
                {
                    // Convert the command to byte array if necessary. 
                    // Example assumes command is a hex string to be sent as bytes
                    byte[] commandBytes = HexStringToByteArray(command);
                    SerialPortManager.mySerialPort.Write(commandBytes, 0, commandBytes.Length);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to send command: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Serial port is not open.");
            }
        }

        // Utility method to convert hex string to byte array
        public static byte[] HexStringToByteArray(string hex)
        {
            hex = hex.Replace(" ", ""); // Remove any spaces from the string
            byte[] bytes = new byte[hex.Length / 2];
            for (int i = 0; i < hex.Length; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }
            return bytes;
        }

        private void OptionButton_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            MessageBox.Show(String.Format("Clicked on option: {0}", clickedButton.Text));
            // 这里可以添加更多的逻辑，例如基于按钮文本执行搜索操作等
        }

        private void RecognizeInk(InkOverlay inkOverlay, ListBox candidateListBox)
        {
            if (inkOverlay.Ink.Strokes.Count > 0)
            {
                using (RecognizerContext context = new RecognizerContext())
                {
                    context.Strokes = inkOverlay.Ink.Strokes;
                    RecognitionStatus status;
                    RecognitionResult result = context.Recognize(out status);

                    if (status == RecognitionStatus.NoError)
                    {
                        // 输出识别结果到控制台
                        // Console.WriteLine(result.TopString);

                        // 获取候选字词列表并显示
                        List<string> candidates = new List<string>();
                        foreach (RecognitionAlternate alternate in result.GetAlternatesFromSelection())
                        {
                            candidates.Add(alternate.ToString());
                        }
                        ShowCandidates(candidates, candidateListBox);
                    }
                    else
                    {
                        // Console.WriteLine("无法识别手写内容。");
                        candidateListBox.Visible = false; // 如果识别失败，隐藏候选列表
                    }
                }
            }
            else
            {
                // Console.WriteLine("请先在面板上写点什么！");
                candidateListBox.Visible = false; // 如果没有墨迹，隐藏候选列表
            }
        }

        private void ShowCandidates(List<string> candidates, ListBox candidateListBox)
        {
            candidateListBox.Items.Clear();
            foreach (var candidate in candidates)
            {
                candidateListBox.Items.Add(candidate);
            }
            candidateListBox.Visible = true;
        }

        private void BtnShowAll_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Show All button clicked!");
            // 这里添加按钮点击后想执行的代码
        }

        private void SetPictureBoxArtistSearchAndButtonsVisibility(bool isVisible)
        {
            System.Action action = () =>
            {
                SuspendLayout();

                // Set the visibility of PictureBoxWordCount
                pictureBoxArtistSearch.Visible = isVisible;

                // Ensure PictureBoxWordCount is brought to the front if it is made visible
                if (isVisible) pictureBoxArtistSearch.BringToFront();

                // Set visibility for modify and close buttons
                modifyButtonArtistSearch.Visible = isVisible;
                closeButtonArtistSearch.Visible = isVisible;

                // If visible, make sure these buttons are also brought to the front
                if (isVisible)
                {
                    modifyButtonArtistSearch.BringToFront();
                    closeButtonArtistSearch.BringToFront();
                }

                // Iterate through all number buttons and set their visibility
                foreach (Button button in numberButtonsArtistSearch)
                {
                    button.Visible = isVisible;
                    // Optionally, ensure that the button is brought to the front if it becomes visible.
                    if (isVisible)
                        button.BringToFront();
                }

                inputBoxArtistSearch.Visible = isVisible;
                if (isVisible) inputBoxArtistSearch.BringToFront();

                ResumeLayout();
            };

            if (this.InvokeRequired)
            {
                this.Invoke(action);
            }
            else
            {
                action();
            }
        }

        private void SetPictureBoxWordCountAndButtonsVisibility(bool isVisible)
        {
            System.Action action = () =>
            {
                SuspendLayout();

                // Set the visibility of PictureBoxWordCount
                pictureBoxWordCount.Visible = isVisible;

                // Ensure PictureBoxWordCount is brought to the front if it is made visible
                if (isVisible) pictureBoxWordCount.BringToFront();

                // Set visibility for modify and close buttons
                modifyButtonWordCount.Visible = isVisible;
                closeButtonWordCount.Visible = isVisible;

                // If visible, make sure these buttons are also brought to the front
                if (isVisible)
                {
                    modifyButtonWordCount.BringToFront();
                    closeButtonWordCount.BringToFront();
                }

                // Iterate through all number buttons and set their visibility
                foreach (Button button in numberButtonsWordCount)
                {
                    button.Visible = isVisible;
                    // Optionally, ensure that the button is brought to the front if it becomes visible.
                    if (isVisible)
                        button.BringToFront();
                }

                inputBoxWordCount.Visible = isVisible;
                if (isVisible) inputBoxWordCount.BringToFront();

                ResumeLayout();
            };

            if (this.InvokeRequired)
            {
                this.Invoke(action);
            }
            else
            {
                action();
            }
        }

        private void ConfigureButton(Button button, int posX, int posY, int width, int height, 
                             Bitmap normalStateImage, Bitmap mouseOverImage, Bitmap mouseDownImage, 
                             EventHandler clickEventHandler)
        {
            // Set the button position and size
            ResizeAndPositionButton(button, posX, posY, width, height);

            // Specify a unique crop area for the button
            Rectangle cropArea = new Rectangle(button.Location.X, button.Location.Y, button.Size.Width, button.Size.Height);

            // Create cropped images specifically for the button states
            button.BackgroundImage = normalStateImage.Clone(cropArea, normalStateImage.PixelFormat);
            button.BackgroundImageLayout = ImageLayout.Stretch;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0; // Remove border
            button.FlatAppearance.MouseDownBackColor = Color.Transparent; // Mousedown background color
            button.FlatAppearance.MouseOverBackColor = Color.Transparent; // Mouseover background color

            // Mouse events to change the background image
            button.MouseEnter += (sender, e) => button.BackgroundImage = mouseOverImage.Clone(cropArea, mouseOverImage.PixelFormat);
            button.MouseLeave += (sender, e) => button.BackgroundImage = normalStateImage.Clone(cropArea, normalStateImage.PixelFormat);
            button.MouseDown += (sender, e) => button.BackgroundImage = mouseDownImage.Clone(cropArea, mouseDownImage.PixelFormat);
            button.MouseUp += (sender, e) => button.BackgroundImage = normalStateImage.Clone(cropArea, normalStateImage.PixelFormat);

            // Event handler for the button click event
            if (clickEventHandler != null)
            {
                button.Click += clickEventHandler;
            }

            // Add the button to the form
            this.Controls.Add(button);
        }

        private void InitializeOtherControls()
        {
            InitializeButton(ref newSongAlertButton, ref newSongAlertNormalBackground, ref newSongAlertActiveBackground, "newSongAlertButton", 25, 97, 99, 99, "themes\\superstar\\ICON上方\\上方ICON_新歌快報X.png", NewSongAlertButton_Click);

            InitializeButton(ref hotPlayButton, ref hotPlayNormalBackground, ref hotPlayActiveBackground, "hotPlayButton", 143, 97, 99, 99, "themes\\superstar\\ICON上方\\上方ICON_熱門排行-02.png", HotPlayButton_Click);

            InitializeButton(ref singerSearchButton, ref singerSearchNormalBackground, ref singerSearchActiveBackground, "singerSearchButton", 261, 97, 99, 99, "themes\\superstar\\ICON上方\\上方ICON_歌星查詢-03.png", SingerSearchButton_Click);

            InitializeButton(ref songSearchButton, ref songSearchNormalBackground, ref songSearchActiveBackground, "songSearchButton", 378, 97, 100, 99, "themes\\superstar\\ICON上方\\上方ICON_歌名查詢-04.png", SongSearchButton_Click);

            InitializeButton(ref languageSearchButton, ref languageSearchNormalBackground, ref languageSearchActiveBackground, "languageSearchButton", 496, 97, 99, 99, "themes\\superstar\\ICON上方\\上方ICON_語別查詢-05.png", LanguageSongSelectionButton_Click);
            
            InitializeButton(ref groupSearchButton, ref groupSearchNormalBackground, ref groupSearchActiveBackground, "groupSearchButton", 614, 97, 99, 100, "themes\\superstar\\ICON上方\\上方ICON_合唱查詢-06.png", GroupSongSelectionButton_Click);

            InitializeButton(ref categorySearchButton, ref categorySearchNormalBackground, ref categorySearchActiveBackground, "categorySearchButton", 731, 97, 99, 99, "themes\\superstar\\ICON上方\\上方ICON_類別查詢-07.png", CategorySearchButton_Click);

            // 服务铃按钮
            serviceBellButton = new Button { Text = "" };
            serviceBellButton.Name = "serviceBellButton";
            ConfigureButton(serviceBellButton, 848, 96, 101, 102, 
                resizedNormalStateImage, resizedMouseOverImage, resizedMouseDownImage, 
                (sender, e) => SendCommandThroughSerialPort("a2 53 a4"));

            InitializeButton(ref orderedSongsButton, ref orderedSongsNormalBackground, ref orderedSongsActiveBackground, "orderedSongsButton", 966, 97, 100, 99, "themes\\superstar\\ICON上方\\上方ICON_已點歌曲-09.png", OrderedSongsButton_Click);

            InitializeButton(ref myFavoritesButton, ref myFavoritesNormalBackground, ref myFavoritesActiveBackground, "myFavoritesButton", 1084, 97, 99, 99, "themes\\superstar\\ICON上方\\上方ICON_我的最愛-10.png", MyFavoritesButton_Click);

            InitializeButton(ref promotionsButton, ref promotionsNormalBackground, ref promotionsActiveBackground, "promotionsButton", 1202, 97, 99, 99, "themes\\superstar\\ICON上方\\上方ICON_優惠活動-11.png", promotionsButton_Click);

            InitializeButton(ref deliciousFoodButton, ref deliciousFoodNormalBackground, ref deliciousFoodActiveBackground, "deliciousFoodButton", 1320, 97, 98, 99, "themes\\superstar\\ICON上方\\上方ICON_美味菜單-12.png", DeliciousFoodButton_Click);

            mobileSongRequestButton = new Button { Text = "" };
            mobileSongRequestButton.Name = "mobileSongRequestButton";
            ConfigureButton(mobileSongRequestButton, 1211, 669, 210, 70, 
                resizedNormalStateImage, resizedMouseOverImage, resizedMouseDownImage, 
                MobileSongRequestButton_Click);

            qieGeButton = new Button{ Text = "" };
            qieGeButton.Name = "qieGeButton";
            ConfigureButton(qieGeButton, 28, 755, 92, 132, 
                resizedNormalStateImage, resizedNormalStateImage, resizedNormalStateImage, 
                (sender, e) => videoPlayerForm.SkipToNextSong());
            this.Controls.Add(qieGeButton);

            // Create the "音樂+" button
            musicUpButton = new Button{ Text = "" };
            musicUpButton.Name = "musicUpButton";
            ResizeAndPositionButton(musicUpButton, 136, 754, 92, 58);
            Rectangle musicUpButtonCropArea = new Rectangle(136, 754, 92, 58);
            musicUpButton.BackgroundImage = normalStateImage.Clone(musicUpButtonCropArea, normalStateImage.PixelFormat);
            musicUpButton.BackgroundImageLayout = ImageLayout.Stretch;
            musicUpButton.FlatStyle = FlatStyle.Flat;
            musicUpButton.FlatAppearance.BorderSize = 0; // Remove border
            musicUpButton.MouseDown += (sender, e) => { OverlayForm.MainForm.ShowVolumeUpLabel(); volumeUpTimer.Start(); };
            musicUpButton.MouseUp += (sender, e) => { OverlayForm.MainForm.HideVolumeUpLabel(); volumeUpTimer.Stop(); };
            this.Controls.Add(musicUpButton);

            // Create the "音樂-" button
            musicDownButton = new Button{ Text = "" };
            musicDownButton.Name = "musicDownButton";
            ResizeAndPositionButton(musicDownButton, 136, 827, 92, 57);
            Rectangle musicDownButtonCropArea = new Rectangle(136, 827, 92, 57);
            musicDownButton.BackgroundImage = normalStateImage.Clone(musicDownButtonCropArea, normalStateImage.PixelFormat);
            musicDownButton.BackgroundImageLayout = ImageLayout.Stretch;
            musicDownButton.FlatStyle = FlatStyle.Flat;
            musicDownButton.FlatAppearance.BorderSize = 0; // Remove border
            musicDownButton.MouseDown += (sender, e) => { OverlayForm.MainForm.ShowVolumeDownLabel(); volumeDownTimer.Start(); };
            musicDownButton.MouseUp += (sender, e) => { OverlayForm.MainForm.HideVolumeDownLabel(); volumeDownTimer.Stop(); };
            this.Controls.Add(musicDownButton);

            // Create the "麥克風+" button
            micUpButton = new Button{ Text = "" };
            micUpButton.Name = "micUpButton";
            ResizeAndPositionButton(micUpButton, 244, 754, 92, 57);
            Rectangle micUpButtonCropArea = new Rectangle(244, 754, 92, 57);
            micUpButton.BackgroundImage = normalStateImage.Clone(micUpButtonCropArea, normalStateImage.PixelFormat);
            micUpButton.BackgroundImageLayout = ImageLayout.Stretch;
            micUpButton.FlatStyle = FlatStyle.Flat;
            micUpButton.FlatAppearance.BorderSize = 0; // Remove border
            micUpButton.MouseDown += (sender, e) => { OverlayForm.MainForm.ShowMicUpLabel(); micControlTimer.Tag = "a2 b5 a4"; micControlTimer.Start(); };
            micUpButton.MouseUp += (sender, e) => { OverlayForm.MainForm.HideMicUpLabel(); micControlTimer.Stop(); };
            this.Controls.Add(micUpButton);

            // Create the "麥克風-" button
            micDownButton = new Button{ Text = "" };
            micDownButton.Name = "micDownButton";
            ResizeAndPositionButton(micDownButton, 244, 827, 92, 57);
            Rectangle micDownButtonCropArea = new Rectangle(244, 827, 92, 57);
            micDownButton.BackgroundImage = normalStateImage.Clone(micDownButtonCropArea, normalStateImage.PixelFormat);
            micDownButton.BackgroundImageLayout = ImageLayout.Stretch;
            micDownButton.FlatStyle = FlatStyle.Flat;
            micDownButton.FlatAppearance.BorderSize = 0; // Remove border
            micDownButton.MouseDown += (sender, e) => { OverlayForm.MainForm.ShowMicDownLabel(); micControlTimer.Tag = "a2 b6 a4"; micControlTimer.Start(); };
            micDownButton.MouseUp += (sender, e) => { OverlayForm.MainForm.HideMicDownLabel(); micControlTimer.Stop(); };
            this.Controls.Add(micDownButton);

            // Create the "原唱" button
            originalSongButton = new Button { Text = "" };
            originalSongButton.Name = "originalSongButton";
            ResizeAndPositionButton(originalSongButton, 353, 756, 91, 55);
            Rectangle originalSongButtonCropArea = new Rectangle(353, 756, 91, 55);
            originalSongButton.BackgroundImage = normalStateImage.Clone(originalSongButtonCropArea, normalStateImage.PixelFormat);
            originalSongButton.BackgroundImageLayout = ImageLayout.Stretch;
            originalSongButton.FlatStyle = FlatStyle.Flat;
            originalSongButton.FlatAppearance.BorderSize = 0; // Remove border
            originalSongButton.Click += OriginalSongButton_Click;
            this.Controls.Add(originalSongButton);

            // Create the "重唱" button
            replayButton = new Button{ Text = "" };
            replayButton.Name = "replayButton";
            ResizeAndPositionButton(replayButton, 353, 828, 91, 55);
            Rectangle replayButtonCropArea = new Rectangle(353, 828, 91, 55);
            replayButton.BackgroundImage = normalStateImage.Clone(replayButtonCropArea, normalStateImage.PixelFormat);
            replayButton.BackgroundImageLayout = ImageLayout.Stretch;
            replayButton.FlatStyle = FlatStyle.Flat;
            replayButton.FlatAppearance.BorderSize = 0; // Remove border
            replayButton.Click += ReplayButton_Click; // 关联事件处理器
            this.Controls.Add(replayButton);

            // Create the "暫停" button
            pauseButton = new Button {
                Text = "",
                Name = "pauseButton"
            };
            ResizeAndPositionButton(pauseButton, 461, 755, 91, 56);
            Rectangle pauseButtonCropArea = new Rectangle(461, 755, 91, 56);
            pauseButton.BackgroundImage = normalStateImage.Clone(pauseButtonCropArea, normalStateImage.PixelFormat);
            pauseButton.BackgroundImageLayout = ImageLayout.Stretch;
            pauseButton.FlatStyle = FlatStyle.Flat;
            pauseButton.FlatAppearance.BorderSize = 0; // Remove border
            pauseButton.Click += PauseButton_Click; // 添加事件处理器
            this.Controls.Add(pauseButton);

            // Create the "播放" button
            playButton = new Button {
                Text = "",
                Name = "playButton",
                Visible = false  // 初始状态不可见
            };
            ResizeAndPositionButton(playButton, 461, 755, 91, 56);
            Rectangle playButtonCropArea = new Rectangle(461, 755, 91, 56);
            playButton.BackgroundImage = normalStateImage.Clone(playButtonCropArea, normalStateImage.PixelFormat);
            playButton.BackgroundImageLayout = ImageLayout.Stretch;
            playButton.FlatStyle = FlatStyle.Flat;
            playButton.FlatAppearance.BorderSize = 0; // Remove border
            playButton.Click += PlayButton_Click; // 添加事件处理器
            this.Controls.Add(playButton);

            // Create the "靜音" button
            muteButton = new Button{ Text = "" };
            muteButton.Name = "muteButton";
            ResizeAndPositionButton(muteButton, 461, 828, 91, 55);
            Rectangle muteButtonCropArea = new Rectangle(461, 828, 91, 55);
            muteButton.BackgroundImage = normalStateImage.Clone(muteButtonCropArea, normalStateImage.PixelFormat);
            muteButton.BackgroundImageLayout = ImageLayout.Stretch;
            muteButton.FlatStyle = FlatStyle.Flat;
            muteButton.FlatAppearance.BorderSize = 0; // Remove border
            muteButton.Click += MuteUnmuteButton_Click;
            this.Controls.Add(muteButton);

            // Create the "男调" button
            maleKeyButton = new Button{ Text = "" };
            maleKeyButton.Name = "maleKeyButton";
            ResizeAndPositionButton(maleKeyButton, 569, 755, 91, 56);
            Rectangle maleKeyButtonCropArea = new Rectangle(569, 755, 91, 56);
            maleKeyButton.BackgroundImage = normalStateImage.Clone(maleKeyButtonCropArea, normalStateImage.PixelFormat);
            maleKeyButton.BackgroundImageLayout = ImageLayout.Stretch;
            maleKeyButton.FlatStyle = FlatStyle.Flat;
            maleKeyButton.Click += MaleKeyButton_Click;
            maleKeyButton.FlatAppearance.BorderSize = 0; // Remove border

            this.Controls.Add(maleKeyButton);

            // Create the "女调" button
            femaleKeyButton = new Button{ Text = "" };
            femaleKeyButton.Name = "femaleKeyButton";
            ResizeAndPositionButton(femaleKeyButton, 570, 828, 90, 55);
            Rectangle femaleKeyButtonCropArea = new Rectangle(570, 828, 90, 55);
            femaleKeyButton.BackgroundImage = normalStateImage.Clone(femaleKeyButtonCropArea, normalStateImage.PixelFormat);
            femaleKeyButton.BackgroundImageLayout = ImageLayout.Stretch;
            femaleKeyButton.FlatStyle = FlatStyle.Flat;
            femaleKeyButton.FlatAppearance.BorderSize = 0; // Remove border
            femaleKeyButton.Click += FemaleKeyButton_Click;
            this.Controls.Add(femaleKeyButton);

            // Create the "标准调" button
            standardKeyButton = new Button { Text = "" };
            standardKeyButton.Name = "standardKeyButton";
            ResizeAndPositionButton(standardKeyButton, 677, 757, 91, 56);
            Rectangle standardKeyButtonCropArea = new Rectangle(677, 757, 91, 56);
            standardKeyButton.BackgroundImage = normalStateImage.Clone(standardKeyButtonCropArea, normalStateImage.PixelFormat);
            standardKeyButton.BackgroundImageLayout = ImageLayout.Stretch;
            standardKeyButton.FlatStyle = FlatStyle.Flat;
            standardKeyButton.FlatAppearance.BorderSize = 0; // Remove border
            standardKeyButton.Click += StandardKeyButton_Click;
            this.Controls.Add(standardKeyButton);

            // Create the "音效" button
            soundEffectButton = new Button { Text = "" };
            soundEffectButton.Name = "soundEffectButton";
            ResizeAndPositionButton(soundEffectButton, 677, 827, 91, 56);
            Rectangle soundEffectButtonCropArea = new Rectangle(677, 827, 91, 56);
            soundEffectButton.BackgroundImage = normalStateImage.Clone(soundEffectButtonCropArea, normalStateImage.PixelFormat);
            soundEffectButton.BackgroundImageLayout = ImageLayout.Stretch;
            soundEffectButton.FlatStyle = FlatStyle.Flat;
            soundEffectButton.FlatAppearance.BorderSize = 0; // Remove border
            soundEffectButton.Click += SoundEffectButton_Click;
            this.Controls.Add(soundEffectButton);

            // Create the "升調" button
            pitchUpButton = new Button{ Text = "" };
            pitchUpButton.Name = "pitchUpButton";
            ResizeAndPositionButton(pitchUpButton, 786, 755, 90, 56);
            Rectangle pitchUpButtonCropArea = new Rectangle(786, 755, 90, 56);
            pitchUpButton.BackgroundImage = normalStateImage.Clone(pitchUpButtonCropArea, normalStateImage.PixelFormat);
            pitchUpButton.BackgroundImageLayout = ImageLayout.Stretch;
            pitchUpButton.FlatStyle = FlatStyle.Flat;
            pitchUpButton.FlatAppearance.BorderSize = 0; // Remove border
            pitchUpButton.Click += PitchUpButton_Click;
            this.Controls.Add(pitchUpButton);

            // Create the "降調" button
            pitchDownButton = new Button{ Text = "" };
            pitchDownButton.Name = "pitchDownButton";
            ResizeAndPositionButton(pitchDownButton, 786, 828, 90, 55);
            Rectangle pitchDownButtonCropArea = new Rectangle(786, 828, 90, 55);
            pitchDownButton.BackgroundImage = normalStateImage.Clone(pitchDownButtonCropArea, normalStateImage.PixelFormat);
            pitchDownButton.BackgroundImageLayout = ImageLayout.Stretch;
            pitchDownButton.FlatStyle = FlatStyle.Flat;
            pitchDownButton.FlatAppearance.BorderSize = 0; // Remove border
            pitchDownButton.Click += PitchDownButton_Click;
            this.Controls.Add(pitchDownButton);

            // Create the "同步画面" button for the same row as the "升调" button
            syncScreenButton = new Button { Text = "" };
            syncScreenButton.Name = "syncScreenButton";
            ResizeAndPositionButton(syncScreenButton, 893, 754, 93, 133);
            Rectangle syncScreenButtonCropArea = new Rectangle(893, 754, 93, 133);
            syncScreenButton.BackgroundImage = normalStateImage.Clone(syncScreenButtonCropArea, normalStateImage.PixelFormat);
            syncScreenButton.BackgroundImageLayout = ImageLayout.Stretch;
            syncScreenButton.FlatStyle = FlatStyle.Flat;
            syncScreenButton.FlatAppearance.BorderSize = 0; // Remove border
            syncScreenButton.Click += SyncScreenButton_Click;  // 添加事件处理器
            this.Controls.Add(syncScreenButton);

            toggleLightButton = new Button{ Text = "" };
            toggleLightButton.Name = "toggleLightButton";
            // Set the button position and size
            ResizeAndPositionButton(toggleLightButton, 1002, 756, 91, 130);
            Rectangle toggleLightButtonCropArea = new Rectangle(1002, 756, 91, 130);
            toggleLightButton.BackgroundImage = normalStateImage.Clone(toggleLightButtonCropArea, normalStateImage.PixelFormat);
            toggleLightButton.BackgroundImageLayout = ImageLayout.Stretch;
            toggleLightButton.FlatStyle = FlatStyle.Flat;
            toggleLightButton.FlatAppearance.BorderSize = 0; // Remove border
            // Event handler for the button click event
            toggleLightButton.Click += ToggleLightButton_Click;
            // Add the button to the form
            this.Controls.Add(toggleLightButton);

            // Create the "上一頁" button
            Rectangle previousPageButtonCropArea = new Rectangle(1109, 754, 93, 58);
            InitializeButton(
                ref btnPreviousPage, 
                "btnPreviousPage", 
                1109, 754, 93, 58, 
                previousPageButtonCropArea, 
                normalStateImage, 
                PreviousPageButton_Click
            );

            // Create the "退出" button
            btnReturn = new Button{ Text = "" };
            btnReturn.Name = "btnReturn";
            // Set the button position and size
            ResizeAndPositionButton(btnReturn, 1218, 755, 92, 57);
            Rectangle returnButtonCropArea = new Rectangle(1218, 755, 92, 57);
            btnReturn.BackgroundImage = normalStateImage.Clone(returnButtonCropArea, normalStateImage.PixelFormat);
            btnReturn.BackgroundImageLayout = ImageLayout.Stretch;
            btnReturn.FlatStyle = FlatStyle.Flat;
            btnReturn.FlatAppearance.BorderSize = 0; // Remove border
            // Event handler for the button click event
            btnReturn.Click += ShouYeButton_Click;
            // Add the button to the form
            this.Controls.Add(btnReturn);

            // Create the "下一頁" button
            Rectangle nextPageButtonCropArea = new Rectangle(1326, 754, 92, 58);
            InitializeButton(
                ref btnNextPage, 
                "btnNextPage", 
                1326, 754, 92, 58, 
                nextPageButtonCropArea, 
                normalStateImage, 
                NextPageButton_Click
            );

            // Create the "掌声" button
            Rectangle applauseButtonCropArea = new Rectangle(1110, 828, 91, 55);
            InitializeButton(
                ref btnApplause, 
                "btnApplause", 
                1110, 828, 91, 55, 
                applauseButtonCropArea, 
                normalStateImage, 
                ApplauseButton_Click
            );

            // Create the "简体" button
            Rectangle simplifiedChineseButtonCropArea = new Rectangle(1327, 828, 90, 55);
            InitializeButton(
                ref btnSimplifiedChinese, 
                "btnSimplifiedChinese", 
                1327, 828, 90, 55, 
                simplifiedChineseButtonCropArea, 
                normalStateImage, 
                SimplifiedChineseButton_Click
            );

            // Create the "繁体" button
            Rectangle traditionalChineseButtonCropArea = new Rectangle(1219, 828, 90, 55);
            InitializeButton(
                ref btnTraditionalChinese, 
                "btnTraditionalChinese", 
                1219, 828, 90, 55, 
                traditionalChineseButtonCropArea, 
                normalStateImage, 
                TraditionalChineseButton_Click
            );

            // Create the "Exit" button
            exitButton = new Button{};
            exitButton.Name = "exitButton";
            ConfigureButton(exitButton, 1394, 2, 1428 - 1394, 37 - 2, 
                resizedNormalStateImage, resizedMouseOverImage, resizedMouseDownImage, 
                (sender, e) => Application.Exit());
        }

        private void InitializeButton(ref Button button, ref Bitmap normalBackground, ref Bitmap activeBackground, string buttonName, int x, int y, int width, int height, string imagePath, EventHandler clickEventHandler)
        {
            button = new Button { Text = "", Name = buttonName };
            ResizeAndPositionButton(button, x, y, width, height);
            Rectangle buttonCropArea = new Rectangle(x, y, width, height);
            normalBackground = new Bitmap(Path.Combine(Application.StartupPath, imagePath));
            activeBackground = mouseDownImage.Clone(buttonCropArea, mouseDownImage.PixelFormat);
            button.BackgroundImage = normalBackground;
            button.BackgroundImageLayout = ImageLayout.Stretch;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            if (clickEventHandler != null)
            {
                button.Click += clickEventHandler;
            }
            this.Controls.Add(button);
        }

        private void InitializeButton(
            ref Button button, 
            string buttonName, 
            int x, int y, 
            int width, int height, 
            Rectangle cropArea, 
            Bitmap normalStateImage, 
            EventHandler clickEventHandler)
        {
            button = new Button { Text = "", Name = buttonName };
            ResizeAndPositionButton(button, x, y, width, height);
            button.BackgroundImage = normalStateImage.Clone(cropArea, normalStateImage.PixelFormat);
            button.BackgroundImageLayout = ImageLayout.Stretch;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            if (clickEventHandler != null)
            {
                button.Click += clickEventHandler;
            }
            this.Controls.Add(button);
        }

        private void InitializeMultiPagePanel()
        {
            multiPagePanel = new MultiPagePanel();
            ResizeAndPositionControl(multiPagePanel, 25, 227, 1150, 512);
            this.Controls.Add(multiPagePanel);

            multiPagePanel.BringToFront();
        }

        private void PrintControlZOrder(Panel panel)
        {
            Console.WriteLine(String.Format("Printing Z-Order for controls in {0}:", panel.Name));
            int index = 0;
            foreach (Control control in panel.Controls)
            {
                // 输出控件类型、索引、位置和大小
                Console.WriteLine(String.Format("Control Index: {0}, Type: {1}, Location: {2}, Size: {3}, Text: {4}",
                    index,
                    control.GetType().Name,
                    control.Location,
                    control.Size,
                    control.Text));
                index++;
            }
        }

        private SongData currentSelectedSong;

        public void Label_Click(object sender, EventArgs e)
        {
            var label = sender as Label;
            if (label != null && label.Tag is SongData)
            {
                // 获取 SongData
                currentSelectedSong = label.Tag as SongData;

                this.DoubleBuffered = true;
                this.SuspendLayout();
                // 可以更新 UI，提示用户歌曲已选中，例如更新 PictureBox 显示
                DrawTextOnVodScreenPictureBox(Path.Combine(Application.StartupPath, @"themes\superstar\點播介面\點播介面_有按鈕.png"), currentSelectedSong);
                SetVodScreenPictureBoxAndButtonsVisibility(true);
                this.ResumeLayout(true);
            }
        }

        public static void WriteLog(string message)
        {
            // 指定日志文件的路径
            string logFilePath = "logfile.txt"; // 请替换为实际路径

            // 使用StreamWriter来写入日志信息
            using (StreamWriter sw = new StreamWriter(logFilePath, true)) // true表示追加文本而不是覆写
            {
                // 写入当前的日期和时间，以及错误信息
                sw.WriteLine(String.Format("{0}: {1}", DateTime.Now, message));
            }
        }

        public static void PrintPlayingSongList()
        {
            Console.WriteLine("當前播放列表:");
            foreach (var song in userRequestedSongs)
            {
                // 检查是否存在 ArtistB，并相应地调整输出格式
                string outputText = !string.IsNullOrWhiteSpace(song.ArtistB)
                                    ? String.Format("{0} - {1} - {2}", song.ArtistA, song.ArtistB, song.Song)
                                    : String.Format("{0} - {1}", song.ArtistA, song.Song);
                
                Console.WriteLine(outputText);
            }
        }

        private void NextPageButton_Click(object sender, EventArgs e)
        {
            multiPagePanel.LoadNextPage();
        }

        private void SimplifiedChineseButton_Click(object sender, EventArgs e)
        {
            multiPagePanel.IsSimplified = true;
        }

        private void TraditionalChineseButton_Click(object sender, EventArgs e)
        {
            multiPagePanel.IsSimplified = false;
        }

        private void PreviousPageButton_Click(object sender, EventArgs e)
        {
            multiPagePanel.LoadPreviousPage();
        }
        
        private void LoadSongData()
        {
            // 初始化播放列表
            // allSongs = new List<SongData>();
            userRequestedSongs = new List<SongData>();
            publicSongList = new List<SongData>();

            // 初始化播放历史
            playedSongsHistory = new List<SongData>();
            playStates = new List<PlayState>();

            try
            {
                string videoDirectory = @"C:\video\";
                string[] videoFiles = Directory.GetFiles(videoDirectory, "*.mpg");

                // Define the regex pattern to parse the filename
                string pattern = @"^(?<songNumber>\d+)-.*?-(?<songName>[^-]+)-";

                foreach (var songPath in videoFiles)
                {
                    string fileName = Path.GetFileNameWithoutExtension(songPath);
                    Match match = Regex.Match(fileName, pattern);

                    if (match.Success)
                    {
                        string songNumber = match.Groups["songNumber"].Value;
                        string songName = match.Groups["songName"].Value;

                        SongData song = new SongData(songNumber, "", songName, 0, "", "", "", "", DateTime.Now, songPath, "", "", "", "", "", "", "", "", "", "", "", 1);
                        publicSongList.Add(song);
                    }
                    else
                    {
                        SongData song = new SongData("", "", "", 0, "", "", "", "", DateTime.Now, songPath, "", "", "", "", "", "", "", "", "", "", "", 1);
                        publicSongList.Add(song);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("Failed to read Excel file: {0}", ex.Message));
            }
        }

        private Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

                using (var wrapMode = new System.Drawing.Imaging.ImageAttributes())
                {
                    wrapMode.SetWrapMode(System.Drawing.Drawing2D.WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }

        private void LoadImages()
        {
            // 假设您已经有了一个方法 ReadSelectedThemePath() 来获取当前选择的主题路径
            string selectedThemePath = ReadSelectedThemePath(); // 或者直接使用变量，如果已经在其他地方读取
            // 使用 selectedThemePath 作为图片资源的基本路径
            string basePath = Path.Combine(Application.StartupPath, selectedThemePath);
            int targetWidth = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
            int targetHeight = System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;

            normalStateImage = new Bitmap(Path.Combine(Application.StartupPath, "themes\\superstar\\555009.jpg"));
            // Resize the image
            resizedNormalStateImage = ResizeImage(normalStateImage, targetWidth, targetHeight);

            mouseOverImage = new Bitmap(Path.Combine(Application.StartupPath, "themes\\superstar\\主選單_已按.jpg"));
            // Resize the image
            resizedMouseOverImage = ResizeImage(mouseOverImage, targetWidth, targetHeight);

            mouseDownImage = new Bitmap(Path.Combine(Application.StartupPath, "themes\\superstar\\主選單_已按.jpg"));
            // Resize the image
            resizedMouseDownImage = ResizeImage(mouseDownImage, targetWidth, targetHeight);

            normalStateImageNewSongAlert = new Bitmap(Path.Combine(Application.StartupPath, "themes\\superstar\\新歌快報_未按.jpg"));
            // Resize the image
            resizedNormalStateImageForNewSongAlert = ResizeImage(normalStateImageNewSongAlert, targetWidth, targetHeight);

            mouseOverImageNewSongAlert = new Bitmap(Path.Combine(Application.StartupPath, "themes\\superstar\\新歌快報_已按.jpg"));
            // Resize the image
            resizedMouseOverImageForNewSongAlert = ResizeImage(mouseOverImageNewSongAlert, targetWidth, targetHeight);

            mouseDownImageNewSongAlert = new Bitmap(Path.Combine(Application.StartupPath, "themes\\superstar\\新歌快報_已按.jpg"));
            // Resize the image
            resizedMouseDownImageForNewSongAlert = ResizeImage(mouseDownImageNewSongAlert, targetWidth, targetHeight);

            normalStateImageArtistQuery = new Bitmap(Path.Combine(Application.StartupPath, "themes\\superstar\\3.歌星查詢_未按.jpg"));
            // Resize the image
            resizedNormalStateImageForArtistQuery = ResizeImage(normalStateImageArtistQuery, targetWidth, targetHeight);

            mouseOverImageArtistQuery = new Bitmap(Path.Combine(Application.StartupPath, "themes\\superstar\\3.歌星查詢_已按.jpg"));
            // Resize the image
            resizedMouseOverImageForArtistQuery = ResizeImage(mouseOverImageArtistQuery, targetWidth, targetHeight);

            mouseDownImageArtistQuery = new Bitmap(Path.Combine(Application.StartupPath, "themes\\superstar\\3.歌星查詢_已按.jpg"));
            // Resize the image
            resizedMouseDownImageForArtistQuery = ResizeImage(mouseDownImageArtistQuery, targetWidth, targetHeight);

            normalStateImageSongQuery = new Bitmap(Path.Combine(Application.StartupPath, "themes\\superstar\\4.歌名查詢_未按.jpg"));
            // Resize the image
            resizedNormalStateImageForSongQuery = ResizeImage(normalStateImageSongQuery, targetWidth, targetHeight);

            mouseOverImageSongQuery = new Bitmap(Path.Combine(Application.StartupPath, "themes\\superstar\\4.歌名查詢_已按.jpg"));
            // Resize the image
            resizedMouseOverImageForSongQuery = ResizeImage(mouseOverImageSongQuery, targetWidth, targetHeight);

            mouseDownImageSongQuery = new Bitmap(Path.Combine(Application.StartupPath, "themes\\superstar\\4.歌名查詢_已按.jpg"));
            // Resize the image
            resizedMouseDownImageForSongQuery = ResizeImage(mouseDownImageSongQuery, targetWidth, targetHeight);

            // 为5-1.png及其变化状态加载和调整大小
            normalStateImageLanguageQuery = new Bitmap(Path.Combine(Application.StartupPath, "themes\\superstar\\語別查詢_未按.jpg"));
            resizedNormalStateImageForLanguageQuery = ResizeImage(normalStateImageLanguageQuery, targetWidth, targetHeight);

            mouseOverImageLanguageQuery = new Bitmap(Path.Combine(Application.StartupPath, "themes\\superstar\\語別查詢_已按.jpg"));
            resizedMouseOverImageForLanguageQuery = ResizeImage(mouseOverImageLanguageQuery, targetWidth, targetHeight);

            mouseDownImageLanguageQuery = new Bitmap(Path.Combine(Application.StartupPath, "themes\\superstar\\語別查詢_已按.jpg"));
            resizedMouseDownImageForLanguageQuery = ResizeImage(mouseDownImageLanguageQuery, targetWidth, targetHeight);

            // 为6-1.png及其变化状态加载和调整大小
            normalStateImage6_1 = new Bitmap(Path.Combine(Application.StartupPath, "themes\\superstar\\6-1.png"));
            resizedNormalStateImageFor6_1 = ResizeImage(normalStateImage6_1, targetWidth, targetHeight);

            mouseOverImage6_1 = new Bitmap(Path.Combine(Application.StartupPath, "themes\\superstar\\6-1.mouseover.png"));
            resizedMouseOverImageFor6_1 = ResizeImage(mouseOverImage6_1, targetWidth, targetHeight);

            mouseDownImage6_1 = new Bitmap(Path.Combine(Application.StartupPath, "themes\\superstar\\6-1.mousedown.png"));
            resizedMouseDownImageFor6_1 = ResizeImage(mouseDownImage6_1, targetWidth, targetHeight);

            normalStateImageCategoryQuery = new Bitmap(Path.Combine(Application.StartupPath, "themes\\superstar\\類別查詢_未按.jpg"));
            resizedNormalStateImageForCategoryQuery = ResizeImage(normalStateImageCategoryQuery, targetWidth, targetHeight);

            mouseOverImageCategoryQuery = new Bitmap(Path.Combine(Application.StartupPath, "themes\\superstar\\類別查詢_已按.jpg"));
            resizedMouseOverImageForCategoryQuery = ResizeImage(mouseOverImageCategoryQuery, targetWidth, targetHeight);

            mouseDownImageCategoryQuery = new Bitmap(Path.Combine(Application.StartupPath, "themes\\superstar\\類別查詢_已按.jpg"));
            resizedMouseDownImageForCategoryQuery = ResizeImage(mouseDownImageCategoryQuery, targetWidth, targetHeight);

            // 为7-1.png及其变化状态加载和调整大小
            normalStateImage7_1 = new Bitmap(Path.Combine(Application.StartupPath, "themes\\superstar\\7-1.png"));
            resizedNormalStateImageFor7_1 = ResizeImage(normalStateImage7_1, targetWidth, targetHeight);

            mouseOverImage7_1 = new Bitmap(Path.Combine(Application.StartupPath, "themes\\superstar\\7-1.mouseover.png"));
            resizedMouseOverImageFor7_1 = ResizeImage(mouseOverImage7_1, targetWidth, targetHeight);

            mouseDownImage7_1 = new Bitmap(Path.Combine(Application.StartupPath, "themes\\superstar\\7-1.mousedown.png"));
            resizedMouseDownImageFor7_1 = ResizeImage(mouseDownImage7_1, targetWidth, targetHeight);

            // 为7-1-1.png及其变化状态加载和调整大小
            normalStateImage7_1_1 = new Bitmap(Path.Combine(Application.StartupPath, "themes\\superstar\\7-1-1.png"));
            resizedNormalStateImageFor7_1_1 = ResizeImage(normalStateImage7_1_1, targetWidth, targetHeight);

            mouseOverImage7_1_1 = new Bitmap(Path.Combine(Application.StartupPath, "themes\\superstar\\7-1-1.mouseover.png"));
            resizedMouseOverImageFor7_1_1 = ResizeImage(mouseOverImage7_1_1, targetWidth, targetHeight);

            mouseDownImage7_1_1 = new Bitmap(Path.Combine(Application.StartupPath, "themes\\superstar\\7-1-1.mousedown.png"));
            resizedMouseDownImageFor7_1_1 = ResizeImage(mouseDownImage7_1_1, targetWidth, targetHeight);

            normalStateImageForPromotionsAndMenu = new Bitmap(Path.Combine(Application.StartupPath, "themes\\superstar\\555021.jpg"));
            resizedNormalStateImageForPromotionsAndMenu = ResizeImage(normalStateImageForPromotionsAndMenu, targetWidth, targetHeight);

            try
            {
                string imagePath = Path.Combine(Application.StartupPath, "themes\\superstar\\555019.jpg");

                if (File.Exists(imagePath))
                {
                    normalStateImageForSyncScreen = new Bitmap(imagePath);
                    resizedNormalStateImageForSyncScreen = ResizeImage(normalStateImageForSyncScreen, targetWidth, targetHeight);
                    Console.WriteLine("Image loaded successfully.");
                }
                else
                {
                    Console.WriteLine("Image file does not exist: " + imagePath);
                    // Handle the case where the image file does not exist
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to load image: " + ex.Message);
                // Handle the exception, e.g., show a default image or an error message
            }

            normalStateImageForSceneSoundEffects = new Bitmap(Path.Combine(Application.StartupPath, "themes\\superstar\\555022.jpg"));
            resizedNormalStateImageForSceneSoundEffects = ResizeImage(normalStateImageForSceneSoundEffects, targetWidth, targetHeight);

            normalStateImageForLightControl = new Bitmap(Path.Combine(Application.StartupPath, "themes\\superstar\\選單內介面_燈光控制.jpg"));
            resizedNormalStateImageForLightControl = ResizeImage(normalStateImageForLightControl, targetWidth, targetHeight);
        }

        public Bitmap MakeTransparentImage(string imagePath, Color maskColor)
        {
            Bitmap bmp = new Bitmap(imagePath);
            // Console.WriteLine(String.Format("Loaded image: {0}", imagePath));
            bmp.MakeTransparent(maskColor); // 使指定的颜色变为透明
            // Console.WriteLine(String.Format("Applied transparency using mask color: {0}", maskColor));
            return bmp;
        }

        // Method to update PictureBox for artist search
        private void ShowImageOnPictureBoxArtistSearch(string imagePath)
        {
            Bitmap originalImage = new Bitmap(imagePath);

            // 定义裁剪区域
            Rectangle cropArea = new Rectangle(593, 135, 507, 508);

            // 裁剪图像
            Bitmap croppedImage = CropImage(originalImage, cropArea);

            // 设置裁剪后的图像为 PictureBox 的图像
            pictureBoxArtistSearch.Image = croppedImage;
    
            // 调整 PictureBox 的大小以匹配裁剪后的图像大小并设置位置
            ResizeAndPositionPictureBox(pictureBoxArtistSearch, cropArea.X + offsetXWordCount, cropArea.Y + offsetXWordCount, cropArea.Width, cropArea.Height);
            
            pictureBoxArtistSearch.Visible = true;
        }

        private Bitmap CropImage(Bitmap source, Rectangle section)
        {
            // 创建裁剪后的图像的容器
            Bitmap bmp = new Bitmap(section.Width, section.Height);

            using (Graphics g = Graphics.FromImage(bmp))
            {
                // 绘制选定区域的图像
                g.DrawImage(source, 0, 0, section, GraphicsUnit.Pixel);
            }

            return bmp;
        }

        private void DrawTextOnVodScreenPictureBox(string imagePath, SongData songData)
        {
            // 加载原始图像
            Bitmap originalImage = new Bitmap(imagePath);

            // 设置裁剪后的图像为 PictureBox 的图像
            VodScreenPictureBox.Image = originalImage;

            int screenWidth = 1440;
            int screenHeight = 900;
            int pictureBoxWidth = 938;
            int pictureBoxHeight = 209;

            int xPosition = (screenWidth - pictureBoxWidth) / 2;
            int yPosition = (screenHeight - pictureBoxHeight) / 2;

            // 创建Graphics对象进行绘制
            using (Graphics g = Graphics.FromImage(originalImage))
            {
                float dpiX = g.DpiX;
                float points = 25;  // 点单位的字体大小
                float pixels = points * (dpiX / 72);  // 将点转换为像素

                // 设置文字样式
                Font font = new Font("微軟正黑體", points, FontStyle.Bold);
                Brush textBrush = Brushes.Black;

                // 获取歌曲信息字符串
                string songInfo = songData.Song ?? "未提供歌曲信息";

                // 绘制文本
                g.DrawString(songInfo, font, textBrush, new PointF(201, 29)); // 调整位置和文字

                // 可以在此添加更多绘图代码，如绘制更多的文本或图形
            }
    
            // 调整 PictureBox 的大小以匹配裁剪后的图像大小并设置位置
            ResizeAndPositionPictureBox(VodScreenPictureBox, xPosition, yPosition, pictureBoxWidth, pictureBoxHeight);
            
            VodScreenPictureBox.Visible = true;
        }

        public static void ResizeAndPositionButton(Button button, int originalX, int originalY, int originalWidth, int originalHeight)
        {
            int screenW = Screen.PrimaryScreen.Bounds.Width;
            int screenH = Screen.PrimaryScreen.Bounds.Height;

            // 以下是调整按钮尺寸和位置的代码，根据需要进行修改
            float widthRatio = screenW / (float)1440; // 假定1440是设计时的屏幕宽度
            float heightRatio = screenH / (float)900; // 假定900是设计时的屏幕高度

            // 根据比例调整控件的位置和大小
            button.Location = new Point(
                (int)(originalX * widthRatio),
                (int)(originalY * heightRatio)
            );
            button.Size = new Size(
                (int)(originalWidth * widthRatio),
                (int)(originalHeight * heightRatio)
            );
        }

        private static void ResizeAndPositionLabel(Label label, int originalX, int originalY, int originalWidth, int originalHeight)
        {
            int screenW = Screen.PrimaryScreen.Bounds.Width;
            int screenH = Screen.PrimaryScreen.Bounds.Height;

            float widthRatio = screenW / (float)1440; // 假定1440是设计时的屏幕宽度
            float heightRatio = screenH / (float)900; // 假定900是设计时的屏幕高度

            // 根据比例调整控件的位置和大小
            label.Location = new Point(
                (int)(originalX * widthRatio),
                (int)(originalY * heightRatio)
            );
            label.Size = new Size(
                (int)(originalWidth * widthRatio),
                (int)(originalHeight * heightRatio)
            );
        }

        private static void ResizeAndPositionPictureBox(PictureBox pictureBox, int originalX, int originalY, int originalWidth, int originalHeight)
        {
            int screenW = Screen.PrimaryScreen.Bounds.Width;
            int screenH = Screen.PrimaryScreen.Bounds.Height;

            float widthRatio = screenW / (float)1440; // 假定1440是设计时的屏幕宽度
            float heightRatio = screenH / (float)900; // 假定900是设计时的屏幕高度

            // 根据比例调整控件的位置和大小
            pictureBox.Location = new Point(
                (int)(originalX * widthRatio),
                (int)(originalY * heightRatio)
            );
            pictureBox.Size = new Size(
                (int)(originalWidth * widthRatio),
                (int)(originalHeight * heightRatio)
            );
        }

        public static void ResizeAndPositionControl(Control control, int originalX, int originalY, int originalWidth, int originalHeight)
        {
            int screenW = Screen.PrimaryScreen.Bounds.Width;
            int screenH = Screen.PrimaryScreen.Bounds.Height;

            // Assume 1440 is the design width and 900 is the design height of the screen
            float widthRatio = screenW / (float)1440;
            float heightRatio = screenH / (float)900;

            // Adjust the control's location and size based on screen ratio
            control.Location = new Point(
                (int)(originalX * widthRatio),
                (int)(originalY * heightRatio)
            );
            control.Size = new Size(
                (int)(originalWidth * widthRatio),
                (int)(originalHeight * heightRatio)
            );
        }

        private void DeliciousFoodButton_Click(object sender, EventArgs e)
        {
            newSongAlertButton.BackgroundImage = newSongAlertNormalBackground;
            hotPlayButton.BackgroundImage = hotPlayNormalBackground;
            singerSearchButton.BackgroundImage = singerSearchNormalBackground;
            songSearchButton.BackgroundImage = songSearchNormalBackground;
            languageSearchButton.BackgroundImage = languageSearchNormalBackground;
            groupSearchButton.BackgroundImage = groupSearchNormalBackground;
            categorySearchButton.BackgroundImage = categorySearchNormalBackground;
            orderedSongsButton.BackgroundImage = orderedSongsNormalBackground;
            myFavoritesButton.BackgroundImage = myFavoritesNormalBackground;
            promotionsButton.BackgroundImage = promotionsNormalBackground;
            deliciousFoodButton.BackgroundImage = deliciousFoodActiveBackground;

            SetHotSongButtonsVisibility(false);
            SetNewSongButtonsVisibility(false);
            SetSingerSearchButtonsVisibility(false);
            SetSongSearchButtonsVisibility(false);
            SetPictureBoxLanguageButtonsVisibility(false);
            SetGroupButtonsVisibility(false);
            SetZhuYinSingersAndButtonsVisibility(false);
            SetZhuYinSongsAndButtonsVisibility(false);
            SetEnglishSingersAndButtonsVisibility(false);
            SetEnglishSongsAndButtonsVisibility(false);
            SetPinYinSingersAndButtonsVisibility(false);
            SetPinYinSongsAndButtonsVisibility(false);
            SetPictureBoxCategoryAndButtonsVisibility(false);

            // Calculate the total pages based on the count of images
            // totalPages = (int)Math.Ceiling((double)imagePaths.Count / imagesPerPage);

            // Set the current page to zero
            // currentPage = 0;
            promotionsAndMenuPanel.currentPageIndex = 0;

            // Display the first page of food images
            // DisplayFoodsOnPage(currentPage); // 显示第一页的美食图片
            promotionsAndMenuPanel.LoadImages(menu); // You can call different methods if needed
            promotionsAndMenuPanel.Visible = true;
            promotionsAndMenuPanel.BringToFront();

            previousPromotionButton.Visible = true;
            previousPromotionButton.BringToFront();
            nextPromotionButton.Visible = true;
            nextPromotionButton.BringToFront();
            // 显示关闭按钮
            closePromotionsButton.Visible = true;
            closePromotionsButton.BringToFront();

            // 切换pictureBoxQRCode的可见性
            if (pictureBoxQRCode != null)
            {
                pictureBoxQRCode.Visible = false;
                closeQRCodeButton.Visible = false;
            }
        }

        private void MobileSongRequestButton_Click(object sender, EventArgs e)
        {
            // 显示第三个图片，并可能隐藏第一个图片
            SetHotSongButtonsVisibility(false);
            SetNewSongButtonsVisibility(false);
            SetSingerSearchButtonsVisibility(false);
            SetSongSearchButtonsVisibility(false);
            SetPictureBoxLanguageButtonsVisibility(false);
            SetZhuYinSingersAndButtonsVisibility(false);
            SetZhuYinSongsAndButtonsVisibility(false);
            SetEnglishSingersAndButtonsVisibility(false);
            SetEnglishSongsAndButtonsVisibility(false);
            SetPinYinSingersAndButtonsVisibility(false);
            SetPinYinSongsAndButtonsVisibility(false);
            SetGroupButtonsVisibility(false);

            // 切换pictureBoxQRCode的可见性
            if (pictureBoxQRCode != null)
            {
                pictureBoxQRCode.Visible = true;
                pictureBoxQRCode.BringToFront(); // 确保QR码图片在最前面显示

                closeQRCodeButton.Visible = true;
                closeQRCodeButton.BringToFront();
            }
            else
            {
                Console.WriteLine("pictureBoxQRCode is not initialized!");
            }
        }

        public void OriginalSongButton_Click(object sender, EventArgs e)
        {
            Console.WriteLine("OriginalSongButton_Click triggered"); // 日誌輸出，便於調試

            // 切換標籤的可見性
            OverlayForm.MainForm.ToggleOriginalSongLabel();

            // 可選：根據標籤的狀態，切換音訊處理
            videoPlayerForm.ToggleVocalRemoval();
        }

        private void ReplayButton_Click(object sender, EventArgs e)
        {
            videoPlayerForm.ReplayCurrentSong();
        }

        private void PauseButton_Click(object sender, EventArgs e)
        {
            videoPlayerForm.Pause();

            // 切换按钮的可见性
            pauseButton.Visible = false;
            playButton.Visible = true;
        }

        private void PlayButton_Click(object sender, EventArgs e)
        {
            videoPlayerForm.Play();

            // 切换按钮的可见性
            playButton.Visible = false;
            pauseButton.Visible = true;
        }

        private void ShouYeButton_Click(object sender, EventArgs e)
        {
            // 显示第三个图片，并可能隐藏第一个图片
            SetHotSongButtonsVisibility(false);
            SetNewSongButtonsVisibility(false);
            SetSingerSearchButtonsVisibility(false);
            SetSongSearchButtonsVisibility(false);
            SetPictureBoxLanguageButtonsVisibility(false);
            SetGroupButtonsVisibility(false);
            SetZhuYinSingersAndButtonsVisibility(false);
            SetZhuYinSongsAndButtonsVisibility(false);
            SetEnglishSingersAndButtonsVisibility(false);
            SetEnglishSongsAndButtonsVisibility(false);
            SetPictureBoxWordCountAndButtonsVisibility(false);
            SetPinYinSingersAndButtonsVisibility(false);
            SetPinYinSongsAndButtonsVisibility(false);
            SetPictureBoxToggleLightAndButtonsVisibility(false);
            inputBoxZhuYinSingers.Text = "";
            inputBoxZhuYinSongs.Text = "";
            inputBoxEnglishSingers.Text = "";
            inputBoxEnglishSongs.Text = "";
            inputBoxPinYinSingers.Text = "";
            inputBoxPinYinSongs.Text = "";
            inputBoxWordCount.Text = "";
            
            // 先清除既有的 Label 控件
            foreach (var label in songLabels)
            {
                this.Controls.Remove(label);
                label.Dispose();
            }
            songLabels.Clear(); // 清除列表中的引用
        }

        // Event handler for the "靜音|恢復" button click
        private void MuteUnmuteButton_Click(object sender, EventArgs e)
        {
            // 显示第三个图片，并可能隐藏第一个图片
            SetHotSongButtonsVisibility(false);
            SetNewSongButtonsVisibility(false);
            SetSingerSearchButtonsVisibility(false);
            SetSongSearchButtonsVisibility(false);
            SetPictureBoxToggleLightAndButtonsVisibility(false);

            if (videoPlayerForm.isMuted)
            {
                // 取消静音，恢复之前的音量
                videoPlayerForm.SetVolume(videoPlayerForm.previousVolume);
                // muteButton.Text = "Mute";
                videoPlayerForm.isMuted = false;
                OverlayForm.MainForm.HideMuteLabel();
            }
            else
            {
                // 静音，将音量设置为-10000
                videoPlayerForm.previousVolume = videoPlayerForm.GetVolume();
                videoPlayerForm.SetVolume(-10000);
                // muteButton.Text = "Unmute";
                videoPlayerForm.isMuted = true;
                OverlayForm.MainForm.ShowMuteLabel();
            }
        }

        private string GetRoomNumber()
        {
            string roomNumberFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "RoomNumber.txt");
            try
            {
                // 假设文件中的第一行就是包厢号
                string roomNumber = File.ReadLines(roomNumberFilePath).First();
                return roomNumber;
            }
            catch (Exception ex)
            {
                MessageBox.Show(String.Format("讀取包廂號文件出錯: {0}", ex.Message));
                return null; // 或者返回一个默认的包厢号，例如 "Unknown"
            }
        }

        // 下面添加事件處理程序的實現
        private void MaleKeyButton_Click(object sender, EventArgs e)
        {
            OverlayForm.MainForm.ShowMaleKeyLabel();
        }
        
        private void FemaleKeyButton_Click(object sender, EventArgs e)
        {
            OverlayForm.MainForm.ShowFemaleKeyLabel();
        }

        private void StandardKeyButton_Click(object sender, EventArgs e)
        {
            OverlayForm.MainForm.ShowStandardKeyLabel();
        }

        // 下面添加事件處理程序的實現
        private void PitchUpButton_Click(object sender, EventArgs e)
        {
            OverlayForm.MainForm.ShowKeyUpLabel();

            // MessageBox.Show("升調功能啟動");
            if (SerialPortManager.mySerialPort != null && SerialPortManager.mySerialPort.IsOpen)
            {
                // 假設 0xA2, 0xC1, 0xA4 是升調的指令
                byte[] commandBytesIncreasePitch = new byte[] { 0xA2, 0xB1, 0xA4 };
                SerialPortManager.mySerialPort.Write(commandBytesIncreasePitch, 0, commandBytesIncreasePitch.Length);
                // MessageBox.Show("升調指令已發送。");
            }
            else
            {
                MessageBox.Show("串口未開啟，無法發送升調指令。");
            }
        }

        private void PitchDownButton_Click(object sender, EventArgs e)
        {
            OverlayForm.MainForm.ShowKeyDownLabel();

            // MessageBox.Show("降調功能啟動");
            if (SerialPortManager.mySerialPort != null && SerialPortManager.mySerialPort.IsOpen)
            {
                // 假設 0xA2, 0xC2, 0xA4 是降調的指令
                byte[] commandBytesDecreasePitch = new byte[] { 0xA2, 0xB2, 0xA4 };
                SerialPortManager.mySerialPort.Write(commandBytesDecreasePitch, 0, commandBytesDecreasePitch.Length);
                // MessageBox.Show("降調指令已發送。");
            }
            else
            {
                MessageBox.Show("串口未開啟，無法發送降調指令。");
            }
        }

        // Event handler for the "硬是消音" button click
        private void HardMuteButton_Click(object sender, EventArgs e)
        {
            // Your logic to handle hard mute action
            MessageBox.Show("硬是消音 功能開發中...");
        }

        // Event handler for the "音軌修正" button click
        private void TrackCorrectionButton_Click(object sender, EventArgs e)
        {
            // Check if the serial port is open before trying to send data
            if (SerialPortManager.mySerialPort != null && SerialPortManager.mySerialPort.IsOpen)
            {
                // Create a byte array with the commands to send
                byte[] commandBytes = new byte[] { 0xA2, 0xD5, 0xA4 };
                
                // Send the bytes to the serial port
                SerialPortManager.mySerialPort.Write(commandBytes, 0, commandBytes.Length);

                MessageBox.Show("音軌修正指令已發送.");
            }
            else
            {
                MessageBox.Show("Serial port is not open. Cannot send track correction command.");
            }
        }

        // Event handler for the "鼓掌" button click
        private void ApplauseButton_Click(object sender, EventArgs e)
        {
            PlayApplauseSound();
        }

        // Event handler for the "歡呼" button click
        private void CheerButton_Click(object sender, EventArgs e)
        {
            // Your logic to handle cheer action, such as playing a cheering sound effect
            MessageBox.Show("歡呼 功能開發中...");
        }

        // Event handler for the "嘲笑" button click
        private void MockButton_Click(object sender, EventArgs e)
        {
            // Your logic to handle mock action, such as playing a mocking sound effect
            MessageBox.Show("嘲笑 功能開發中...");
        }

        // Event handler for the "噓聲" button click
        private void BooButton_Click(object sender, EventArgs e)
        {
            // Your logic to handle boo action, such as playing a boo sound effect
            MessageBox.Show("噓聲 功能開發中...");
        }
    }
}