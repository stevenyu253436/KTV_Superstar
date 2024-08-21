using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using ZXing;
using ZXing.QrCode;
using System.Timers;

namespace DualScreenDemo
{
    public partial class OverlayForm : Form
    {
        // private SongListControl songListControl;
        private string marqueeText = "這是跑馬燈文字示例 - 歡迎使用MediaPlayerForm!";
        private Color marqueeTextColor = Color.White; // 默认颜色
        private string marqueeTextSecondLine = ""; // 新增第二行文字
        private string marqueeTextThirdLine = "";
        private int marqueeXPos;
        private int marqueeXPosSecondLine; // 第二行的横坐标
        private int marqueeXPosThirdLine;
        private System.Windows.Forms.Timer marqueeTimer;
        private Image backgroundImage;
        private Image firstStickerImage;
        private Image secondStickerImage;
        private float firstStickerXPos;
        private float secondStickerXPos;
        private float imageYPos;
        private int screenHeight;
        private int topMargin;
        private int bottomMargin;
        public static System.Windows.Forms.Timer displayTimer = new System.Windows.Forms.Timer();
        public static System.Timers.Timer songDisplayTimer = new System.Timers.Timer();
        public static System.Timers.Timer unifiedTimer;
        private System.Windows.Forms.Timer stickerTimer1 = new System.Windows.Forms.Timer();
        private System.Windows.Forms.Timer stickerTimer2 = new System.Windows.Forms.Timer();
        private System.Windows.Forms.Timer keyUpTimer = new System.Windows.Forms.Timer();
        private System.Windows.Forms.Timer keyDownTimer = new System.Windows.Forms.Timer();
        public Label standardKeyLabel;
        private System.Windows.Forms.Timer standardKeyTimer = new System.Windows.Forms.Timer();
        public Label maleKeyLabel;
        private System.Windows.Forms.Timer maleKeyTimer;
        public Label femaleKeyLabel;
        private System.Windows.Forms.Timer femaleKeyTimer;
        private System.Windows.Forms.Timer secondLineTimer;
        private DateTime secondLineStartTime;
        private const int secondLineDuration = 20000; // 20 seconds
        private Image qrCodeImage;
        private bool showQRCode;

        public enum MarqueeStartPosition
        {
            Middle,
            Right
        }

        // 单例实例
        private static OverlayForm _mainForm;

        public static OverlayForm MainForm
        {
            get { return _mainForm; }
            private set { _mainForm = value; }
        }

        public OverlayForm()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor, true);
            MainForm = this;  // 初始化时设置单例实例
            InitializeFormSettings();
            ConfigureTimers();
            LoadBackgroundImage();
            ConfigureImageDisplay();
            InitializeLabels();
        }

        // Initialize the male key timer
        private void InitializeMaleKeyTimer()
        {
            maleKeyTimer = new System.Windows.Forms.Timer();
            maleKeyTimer.Interval = 3000; // 3秒
            maleKeyTimer.Tick += (s, e) => 
            {
                HideMaleKeyLabel();
            };
        }

        // Initialize the female key timer
        private void InitializeFemaleKeyTimer()
        {
            femaleKeyTimer = new System.Windows.Forms.Timer();
            femaleKeyTimer.Interval = 3000; // 3秒
            femaleKeyTimer.Tick += (s, e) => 
            {
                HideFemaleKeyLabel();
            };
        }

        public void UpdateDisplayLabels(string[] messages)
        {
            // Start Y position for the first message
            int startY = 100;

            // Iterate through the messages and create labels
            foreach (string message in messages)
            {
                Font labelFont = new Font("Microsoft JhengHei", 50, FontStyle.Bold);

                Label label = new Label
                {
                    Text = message,
                    AutoSize = true,
                    ForeColor = Color.White,
                    Font = labelFont,
                    BackColor = Color.Transparent,
                    Location = new Point((this.Width - TextRenderer.MeasureText(message, labelFont).Width) / 2, startY)
                };

                this.Controls.Add(label);
                label.BringToFront();

                // Update startY for the next label
                startY += 80; // Adjust based on your requirement
            }
        }

        public static void DisplayNumberAtTopLeft(string newText)
        {
            if (MainForm != null)
            {
                MainForm.Invoke(new System.Action(() =>
                {
                    // 检查和准备追加新文本
                    string currentText = MainForm.displayLabel.Text;
                    string combinedText = currentText + newText;

                    // 确保文本长度不超过6
                    if (combinedText.Length > 6)
                    {
                        // 如果结合后的文本长度超过6，则保留前6个字符
                        combinedText = combinedText.Substring(0, 6);
                    }

                    // 设置标签文本
                    MainForm.displayLabel.Text = combinedText;

                    // 重置并启动计时器
                    displayTimer.Stop();
                    displayTimer.Start();
                }));
            }
        }

        private void InitializeFormSettings()
        {
            // 初始化視窗設置
            this.FormBorderStyle = FormBorderStyle.None;
            this.TopMost = true;
            this.BackColor = Color.Magenta; // A color you don't use
            this.TransparencyKey = this.BackColor; // Makes magenta transparent
            this.Height = 50; // 根據需要設定高度

            // 假设有多个显示器，我们希望OverlayForm显示在第二个显示器上
            if (Screen.AllScreens.Length > 1)
            {
                var secondaryScreen = Screen.AllScreens[1]; // 获取第二个屏幕
                this.Width = secondaryScreen.Bounds.Width; // 设置宽度为第二个屏幕的宽度
                this.Location = new Point(secondaryScreen.Bounds.Location.X, this.Location.Y); // 调整窗体的位置，使其显示在第二个屏幕上
                screenHeight = secondaryScreen.Bounds.Height;
                topMargin = screenHeight / 3;
                bottomMargin = screenHeight * 2 / 3;
            }
            else
            {
                this.Width = Screen.PrimaryScreen.Bounds.Width; // 如果只有一个屏幕，就使用主屏幕的宽度
                this.screenHeight = Screen.PrimaryScreen.Bounds.Height;
            }

            // 初始化跑馬燈位置
            marqueeXPos = this.Width;
            marqueeXPosSecondLine = 7 * this.Width / 8;
            marqueeXPosThirdLine = this.Width;

            // 初始化 Timer
            marqueeTimer = new System.Windows.Forms.Timer();
            marqueeTimer.Interval = 20; // 更新間隔，可以根據需要調整
            marqueeTimer.Tick += MarqueeTimer_Tick;
            marqueeTimer.Start();

            secondLineTimer = new System.Windows.Forms.Timer();
            secondLineTimer.Interval = 100; // 检查间隔，可以根据需要调整
            secondLineTimer.Tick += SecondLineTimer_Tick;

            // 尝试从文件中读取跑马灯文字
            try
            {
                // 假设文件名为 "WelcomeMessage.txt"，位于应用程序的当前工作目录
                string filePath = Path.Combine(Application.StartupPath, "WelcomeMessage.txt");
                marqueeText = File.ReadAllText(filePath);
            }
            catch (Exception ex)
            {
                // 如果读取文件时发生错误，打印错误信息并使用默认文本
                Console.WriteLine("Error reading marquee text from file: " + ex.Message);
                marqueeText = "這是跑馬燈文字示例 - 歡迎使用MediaPlayerForm!";
            }

            // // 初始化图像定时器
            // imageTimer = new Timer();
            // ConfigureImageTimerInterval();
            // imageTimer.Tick += ImageTimer_Tick;

            this.DoubleBuffered = true;
        }

        // Declare the EventHandler variable
        // private EventHandler unifiedTimerEventHandler;

        private void ConfigureTimers()
        {
            // 初始化计时器，但不启动
            displayTimer.Interval = 100000; // 3秒后清除文本
            displayTimer.Tick += DisplayTimer_Tick;

            songDisplayTimer = new System.Timers.Timer(30000); // 设置间隔时间为5秒（5000毫秒）
            songDisplayTimer.Elapsed += new ElapsedEventHandler(SongDisplayTimer_Elapsed);
            songDisplayTimer.AutoReset = true;
            songDisplayTimer.Enabled = true;

//eh = new EventHandler(UnifiedTimer_Tick);
            unifiedTimer = new System.Timers.Timer(10000);
            // Declare the EventHandler variable
            unifiedTimer.Elapsed += new ElapsedEventHandler(UnifiedTimer_Elapsed);
            unifiedTimer.AutoReset = true;
            unifiedTimer.Enabled = true;

            stickerTimer1.Interval = 10000;  // 10 seconds
            stickerTimer1.Tick += (sender, e) => {
                lock (imageLock)
                {
                    firstStickerImage = null;
                    this.Invalidate();
                }
                if (secondStickerImage == null) // 如果没有第二个图片，重新加载背景图
                    LoadBackgroundImage();
                stickerTimer1.Stop();
                HideImages();  // 确保在这里调用 HideImages
            };

            stickerTimer2.Interval = 10000;  // 10 seconds
            stickerTimer2.Tick += (sender, e) => {
                lock (imageLock)
                {
                    secondStickerImage = null;
                    this.Invalidate();
                }
                if (firstStickerImage == null) // 如果没有第一个图片，重新加载背景图
                    LoadBackgroundImage();
                stickerTimer2.Stop();
                HideImages();  // 确保在这里调用 HideImages
            };
        }

        private static void DisplayTimer_Tick(object sender, EventArgs e)
        {
            if (MainForm.InvokeRequired)
            {
                MainForm.Invoke(new System.Action(() =>
                {
                    MainForm.displayLabel.Text = "";  // 清除文本
                }));
            }
            else
            {
                MainForm.displayLabel.Text = "";  // 清除文本
            }

            displayTimer.Stop(); // 停止计时器，防止重复触发
        }

        private static void SongDisplayTimer_Elapsed(object sender, EventArgs e)
        {
            if (MainForm.InvokeRequired)
            {
                Console.WriteLine("SongDisplayTimer_Tick invoked on UI thread.");

                MainForm.Invoke(new System.Action(() =>
                {
                    MainForm.songDisplayLabel.Text = "";  // 清除文本
                }));
            }
            else
            {
                Console.WriteLine("SongDisplayTimer_Tick invoked on background thread.");
                MainForm.songDisplayLabel.Text = "";  // 清除文本
            }

            songDisplayTimer.Stop(); // 停止计时器，防止重复触发
        }

        private readonly object _lockObject = new object();

        private void UnifiedTimer_Elapsed(object sender, EventArgs e)
        {
            Console.WriteLine("UnifiedTimer_Elapsed called"); // Add this line to show a message box

            if (MainForm.InvokeRequired)
            {
                MainForm.Invoke(new System.Action<object, EventArgs>(UnifiedTimer_Elapsed), new object[] { sender, e });
            }
            else
            {
                displayLabel.Text = "";

                switch (CurrentUIState)
                {
                    case UIState.SelectingLanguage:
                        // 处理选择语言后的计时行为
                        SetUIState(UIState.Initial);
                        HandleTimeout("操作超時，請重新開始。");
                        break;
                    case UIState.SelectingArtistCategory:
                        SetUIState(UIState.Initial);
                        HandleTimeout("操作超時，請重新開始。");
                        break;
                    case UIState.SelectingAction:
                        SetUIState(UIState.Initial);
                        HandleTimeout("操作超時，請重新開始。");
                        break;
                    case UIState.SelectingSong:
                        // 处理选择歌曲后的计时行为
                        SetUIState(UIState.Initial);
                        HandleTimeout("操作超時，請重新開始。");
                        break;
                    case UIState.SelectingArtist:
                        SetUIState(UIState.Initial);
                        HandleTimeout("操作超時，請重新開始。");
                        break;
                    case UIState.PlayHistory:
                        SetUIState(UIState.Initial);
                        HandleTimeout("操作超時，請重新開始。");
                        break;
                }
            }
        }

        private async Task HandleTimeout(string message)
        {
            Console.WriteLine("HandleTimeout called with message: " + message);
            SetUIState(UIState.Initial);
            DisplayMessage(message, 2000);
            await Task.Delay(2000); // Simulate some delay if needed
        }

        private void DisplayMessage(string message, int duration)
        {
            displayLabel.Text = message;
            unifiedTimer.Interval = duration;
            unifiedTimer.Start();
        }
        
        private void LoadBackgroundImage()
        {
            try
            {
                backgroundImage = Image.FromFile(Path.Combine(Application.StartupPath, "themes\\superstar\\images.jpg"));
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading background image: " + ex.Message);
                backgroundImage = null;
            }
        }

        private void ConfigureImageDisplay()
        {
            try
            {
                firstStickerImage = Image.FromFile(Path.Combine(Application.StartupPath, "superstar-pic/1-1.png"));
                firstStickerXPos = this.Width / 2;
                imageYPos = (screenHeight / 3) - firstStickerImage.Height / 6;

                // 加载背景图
                LoadBackgroundImage();
                
                // 启动第一个贴图的计时器
                stickerTimer1.Start();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading initial sticker image: " + ex.Message);
                firstStickerImage = null; // 在失败的情况下确保图像为空
            }
        }

        private void HideImages()
        {
            bool anyStickersActive = false;

            lock (imageLock)
            {
                // 隐藏并停止第一张贴图的计时器
                if (firstStickerImage != null)
                {
                    firstStickerImage = null;
                    stickerTimer1.Stop();
                    anyStickersActive = true;  // Update the flag to indicate active stickers
                }

                // 隐藏并停止第二张贴图的计时器
                if (secondStickerImage != null)
                {
                    secondStickerImage = null;
                    stickerTimer2.Stop();
                    anyStickersActive = true;  // Update the flag to indicate active stickers
                }

                // 检查是否还有贴图在显示
                if (!anyStickersActive)  // No stickers are active
                {
                    // 清除背景图片
                    backgroundImage = null;
                }

                this.Invalidate();  // Request a redraw of the form
            }
        }

        public void UpdateSongDisplayLabel(string newText)
        {
            songDisplayLabel.Text = newText;

            // 停止计时器
            songDisplayTimer.Stop();

            // 重新启动计时器
            songDisplayTimer.Start();

            // 强制窗体重绘以更新跑马灯
            this.Invalidate();
        }

        private void SecondLineTimer_Tick(object sender, EventArgs e)
        {
            if ((DateTime.Now - secondLineStartTime).TotalMilliseconds >= secondLineDuration)
            {
                marqueeTextSecondLine = "";
                secondLineTimer.Stop();
            }
        }

        public void ResetMarqueeTextToWelcomeMessage()
        {
            try
            {
                string filePath = Path.Combine(Application.StartupPath, "WelcomeMessage.txt");
                string welcomeMessage = File.ReadAllText(filePath);
                this.UpdateMarqueeText(welcomeMessage, MarqueeStartPosition.Right, Color.White);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error reading marquee text from file: " + ex.Message);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // 指定一个更大的字号来创建字体
            // 例如，将字号从 20 调整到 30
            using (Font largeFont = new Font("微軟正黑體", 40, FontStyle.Bold))
            using (Brush whiteBrush = new SolidBrush(Color.White))
            using (Brush limeGreenBrush = new SolidBrush(Color.LimeGreen))
            using (Brush marqueeBrush = new SolidBrush(marqueeTextColor)) // 新增：使用动态颜色
            {
                // 獲取文字的大小
                SizeF textSize = e.Graphics.MeasureString(marqueeText, largeFont);

                // 計算 Y 座標以垂直居中文字
                // float yPosition = (this.Height - textSize.Height) / 2;
                float yPosition1 = 10;   // 第一行 Y 坐标
                float yPosition2 = 55;   // 第二行 Y 坐标
                float yPosition3 = 100;  // 第三行 Y 坐标

                // 繪製跑馬燈文字
                e.Graphics.DrawString(marqueeText, largeFont, marqueeBrush, new PointF(marqueeXPos, yPosition1));

                // 设置剪辑区域，使第二行文字只在 3/4 到 1/4 范围内显示
                Rectangle clipRect = new Rectangle((int)(this.Width / 8), (int)yPosition2, (int)(3 * this.Width / 4), (int)textSize.Height);
                // Rectangle clipRect = new Rectangle(0, (int)yPosition2, (int)(3 * this.Width / 4), (int)textSize.Height);
                Region originalClip = e.Graphics.Clip;
                e.Graphics.SetClip(clipRect);

                // 計算第二行文字的寬度
                SizeF textSizeSecondLine = e.Graphics.MeasureString(marqueeTextSecondLine, largeFont);

                // 計算文本的起始 X 座標，讓它置中
                float centeredXPos = (this.Width - textSizeSecondLine.Width) / 2;

                // 繪製文字，使用計算出來的 centeredXPos 作為 X 座標
                e.Graphics.DrawString(marqueeTextSecondLine, largeFont, limeGreenBrush, new PointF(centeredXPos, yPosition2));

                // 恢复原始剪辑区域
                e.Graphics.Clip = originalClip;

                if (marqueeTextSecondLine == "")
                {
                    e.Graphics.DrawString(marqueeTextThirdLine, largeFont, whiteBrush, new PointF(marqueeXPosThirdLine, yPosition2));
                }
                else
                {
                    e.Graphics.DrawString(marqueeTextThirdLine, largeFont, whiteBrush, new PointF(marqueeXPosThirdLine, yPosition3));
                }
            }

            // 检查 image 是否不为 null
            lock (imageLock)
            {
                if (backgroundImage != null)
                {
                    e.Graphics.DrawImage(backgroundImage, new Rectangle(25, 100, this.Width - 50, (int)(this.Height * 2 / 3) - 100));
                }
                if (firstStickerImage != null)
                {
                    e.Graphics.DrawImage(firstStickerImage, firstStickerXPos, imageYPos);
                }
                if (secondStickerImage != null)
                {
                    e.Graphics.DrawImage(secondStickerImage, secondStickerXPos, imageYPos);
                }
                if (showQRCode && qrCodeImage != null)
                {
                    // Set the position where you want to overlay the QR code
                    Rectangle qrCodeRect = new Rectangle(32, topMargin, screenHeight / 3, screenHeight / 3);
                    e.Graphics.DrawImage(qrCodeImage, qrCodeRect);
                }
            }
        }

        // 保证 DisplayBarrage 方法是公共的
        public void DisplayBarrage(string text)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new System.Action(() => DisplayBarrage(text)));  // 使用 System.Action 来解决命名空间冲突
                return;
            }

            // Random number generator for colors and positions
            Random rnd = new Random();

            // Generate labels with various colors and sizes
            for (int i = 0; i < 30; i++) // Adjust the number of labels as needed
            {
                Label lblBarrage = new Label
                {
                    Text = text, // Use the same text for all labels
                    AutoSize = true, // Auto-size label to fit text
                    ForeColor = Color.FromArgb(rnd.Next(256), rnd.Next(256), rnd.Next(256)), // Random color for the text
                    Font = new Font("Arial", rnd.Next(10, 50)), // Random font size between 10 and 50
                    Location = new Point(rnd.Next(0, this.Width), rnd.Next(0, this.Height)) // Random position within the form
                };

                // Add the label to the form's controls
                this.Controls.Add(lblBarrage);
                lblBarrage.BringToFront(); // Bring the label to the front

                // Timer for moving the label to the left
                System.Windows.Forms.Timer moveTimer = new System.Windows.Forms.Timer { Interval = 50 }; // Smaller interval for smoother movement
                moveTimer.Tick += (sender, e) =>
                {
                    lblBarrage.Left -= 5; // Move the label to the left

                    if (lblBarrage.Right < 0) // Remove label if it's completely out of the form's bounds
                    {
                        lblBarrage.Dispose();
                        moveTimer.Dispose();
                    }
                };
                moveTimer.Start();

                // Timer for label disappearance
                int duration = rnd.Next(3000, 7000); // Randomize the time label stays visible between 3 to 7 seconds
                System.Windows.Forms.Timer durationTimer = new System.Windows.Forms.Timer { Interval = duration };
                durationTimer.Tick += (sender, e) =>
                {
                    if (moveTimer.Enabled)
                    {
                        moveTimer.Stop(); // Stop moving the label
                        moveTimer.Dispose();
                    }

                    this.Controls.Remove(lblBarrage);
                    lblBarrage.Dispose();
                    durationTimer.Stop(); // Make sure to stop the duration timer as well
                    durationTimer.Dispose();
                };
                durationTimer.Start();
            }
        }

        public void DisplaySticker(string stickerId)
        {
            Console.WriteLine("Attempting to display sticker.");
            this.Invoke((MethodInvoker)delegate {
                // 打印窗体的宽度和高度
                Console.WriteLine("Form Width: " + this.Width);
                Console.WriteLine("Form Height: " + this.Height);

                // 图片文件路径应该基于 stickerId 动态确定
                string imagePath = String.Format("{0}\\superstar-pic\\{1}.png", Application.StartupPath, stickerId);
                Console.WriteLine("Image path: " + imagePath);
                try
                {
                    Image newSticker = Image.FromFile(imagePath);
                    lock (imageLock)
                    {
                        if (firstStickerImage == null)
                        {
                            firstStickerImage = newSticker;
                            firstStickerXPos = this.Width / 2 - firstStickerImage.Width / 2;
                            LoadBackgroundImage();  // 確保加載背景圖片
                            stickerTimer1.Start();  // 启动第一个贴图的计时器
                        }
                        else if (secondStickerImage == null)
                        {
                            firstStickerXPos = this.Width * 3 / 10f - firstStickerImage.Width / 8;
                            secondStickerImage = newSticker;
                            secondStickerXPos = this.Width * 7 / 10f - secondStickerImage.Width / 8;
                            // 清除背景图片
                            backgroundImage = null;
                            stickerTimer2.Start();  // 启动第二个贴图的计时器
                        }
                        else
                        {
                            // 已有两个图片，替换第一个并移动到新位置
                            firstStickerImage = secondStickerImage;
                            firstStickerXPos = this.Width * 3 / 10f - firstStickerImage.Width / 8;
                            
                            secondStickerImage = newSticker;
                            secondStickerXPos = this.Width * 7 / 10f - secondStickerImage.Width / 8;
                            stickerTimer2.Start();  // 重启第二个贴图的计时器
                        }
                    }
                    this.Invalidate(); // 请求重绘以更新显示
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error loading sticker image: " + ex.Message);
                }
            });
        }

        // 读取歌曲编号的方法
        public static string ReadSongNumber()
        {
            // 从参数中获取歌曲编号的文本
            string songNumber = MainForm.displayLabel.Text; // 读取输入的文本并去除首尾空白
            return songNumber; // 返回读取到的歌曲编号
        }

        private List<SongData> LanguageSongList;

        // 定义一个枚举来表示不同的用户界面状态
        public enum UIState
        {
            Initial,
            SelectingLanguage,
            SelectingSong,
            SelectingAction, // 新增状态
            PlayingSong,
            SelectingArtistCategory,   // 新增状态
            SelectingStrokeCount, // 新增状态
            SelectingArtist,
            PlayHistory // 新增状态
        }

        public enum Category
        {
            NewSongs, // 新歌
            HotSongs, // 热门歌曲
            Artists   // 歌星
            // 可以添加更多类别
        }

        // 在类中定义当前状态变量
        public static UIState CurrentUIState = UIState.Initial;
        private string currentLanguage = "";  // 用于存储当前选定的语种
        // 存储当前选择的类别
        public static Category CurrentCategory { get; set; }
        private SongData selectedSong;  // 在类级别声明一个 SongData 类型的变量

        public static void SetUIState(UIState newState)
        {
            CurrentUIState = newState;
            // 停止当前运行的计时器以确保清除任何现有的计时设置
            displayTimer.Stop();

            // 根据状态改变配置计时器
            switch (newState)
            {
                case UIState.Initial:
                    // Clear existing controls
                    foreach (var control in MainForm.Controls.OfType<Control>().ToArray())
                    {
                        if (control != MainForm.displayLabel &&
                            control != MainForm.pauseLabel &&
                            control != MainForm.muteLabel &&
                            control != MainForm.volumeUpLabel &&
                            control != MainForm.volumeDownLabel &&
                            control != MainForm.micUpLabel &&
                            control != MainForm.micDownLabel &&
                            control != MainForm.standardKeyLabel &&
                            control != MainForm.keyUpLabel &&
                            control != MainForm.keyDownLabel &&
                            control != MainForm.maleKeyLabel &&
                            control != MainForm.femaleKeyLabel &&
                            control != MainForm.squareLabel &&
                            control != MainForm.professionalLabel &&
                            control != MainForm.standardLabel &&
                            control != MainForm.singDownLabel &&
                            control != MainForm.brightLabel &&
                            control != MainForm.softLabel &&
                            control != MainForm.autoLabel &&
                            control != MainForm.romanticLabel &&
                            control != MainForm.dynamicLabel &&
                            control != MainForm.tintLabel) // Keep the specified controls
                        {
                            MainForm.Controls.Remove(control);
                            control.Dispose();
                        }
                    }

                    MainForm.displayLabel.Text = ""; // 将文本设置为空字符串
                    CommandHandler.readyForSongListInput = false;
                    unifiedTimer.Stop();
                    break;
                case UIState.SelectingLanguage:
                    unifiedTimer.Interval = 10000;
                    unifiedTimer.Enabled = true;
                    unifiedTimer.Start();
                    break;
                case UIState.SelectingSong:
                    unifiedTimer.Interval = 10000;
                    unifiedTimer.Enabled = true;
                    unifiedTimer.Start();
                    break;
                case UIState.SelectingArtistCategory:
                    unifiedTimer.Interval = 10000;
                    unifiedTimer.Enabled = true;
                    unifiedTimer.Start();
                    break;
                case UIState.SelectingStrokeCount:
                    unifiedTimer.Interval = 10000;
                    unifiedTimer.Enabled = true;
                    unifiedTimer.Start();
                    break;
                case UIState.SelectingArtist:
                    unifiedTimer.Interval = 10000;
                    unifiedTimer.Enabled = true;
                    unifiedTimer.Start();
                    break;
                case UIState.PlayHistory:
                    unifiedTimer.Interval = 10000;
                    unifiedTimer.Enabled = true;
                    unifiedTimer.Start();
                    break;
                default:
                    break;
            }
        }

        int number;
        int inputNumber;

        public void OnUserInput(string input)
        {
            bool isNumber = int.TryParse(input, out number); // 先解析输入，再進行判断

            if (isNumber)
            {
                if (CurrentCategory == Category.NewSongs)
                {
                    switch (CurrentUIState)
                    {
                        case UIState.SelectingLanguage:
                            HandleLanguageSelection(number);
                            break;

                        case UIState.SelectingSong:
                            HandleSongSelection(number);
                            break;

                        default:
                            displayLabel.Text = "無效的狀態";
                            displayLabel.BringToFront();
                            break;
                    }
                }
                else if (CurrentCategory == Category.HotSongs)
                {
                    switch (CurrentUIState)
                    {
                        case UIState.SelectingLanguage:
                            HandleLanguageSelection(number);
                            break;

                        case UIState.SelectingSong:
                            HandleSongSelection(number);
                            break;

                        default:
                            displayLabel.Text = "無效的狀態";
                            displayLabel.BringToFront();
                            break;
                    }
                }
                else if (CurrentCategory == Category.Artists)
                {
                    switch (CurrentUIState)
                    {
                        case UIState.SelectingArtistCategory:
                            ProcessArtistCategorySelection(number);
                            break;

                        case UIState.SelectingStrokeCount:
                            ProcessStrokeCountSelection(number);
                            break;

                        case UIState.SelectingArtist:
                            HandleArtistSelection(number);
                            break;

                        case UIState.SelectingSong:
                            HandleSongSelection(number);
                            break;

                        default:
                            displayLabel.Text = "無效的狀態";
                            displayLabel.BringToFront();
                            break;
                    }
                }
            }
            else if (input == "a")
            {
                try
                {
                    if (CurrentUIState == UIState.SelectingSong)
                    {
                        Console.WriteLine("Current State is SelectingSong, ready to process song selection.");
                        Console.WriteLine("Number: " + inputNumber);
                        int songIndex = (currentPage - 1) * songsPerPage + (inputNumber - 1);
                        Console.WriteLine("Calculated Song Index: " + songIndex + ", Total Songs: " + totalSongs);

                        if (songIndex >= 0 && songIndex < totalSongs)
                        {
                            selectedSong = LanguageSongList[songIndex];  // 将选择的歌曲赋值给 selectedSong
                            Console.WriteLine("Adding song to playlist: " + LanguageSongList[songIndex].Song);

                            // 调用 DisplaySongsWithArrows 并传入当前索引
                            DisplayActionWithSong(currentPage, songIndex, "點播");
                            // songListControl.UpdateSongs(LanguageSongList, songIndex, "點播", currentPage);
                            // songListControl.BringToFront();
                            AddSongToPlaylist(selectedSong);
                        }
                        else
                        {
                            Console.WriteLine("Song index out of range.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    // 捕获异常并记录错误日志
                    Console.WriteLine("An error occurred while processing input 'a': " + ex.Message);
                    Console.WriteLine(ex.StackTrace);
                }
            }
            else if (input == "b")
            {
                if (CurrentUIState == UIState.SelectingSong)
                {
                    Console.WriteLine("Current State is SelectingSong, ready to process song selection.");
                    int songIndex = (currentPage - 1) * songsPerPage + (inputNumber - 1);
                    Console.WriteLine("Calculated Song Index: " + songIndex + ", Total Songs: " + totalSongs);

                    if (songIndex < totalSongs)
                    {
                        selectedSong = LanguageSongList[songIndex];  // 将选择的歌曲赋值给 selectedSong
                        Console.WriteLine("Adding song to playlist: " + LanguageSongList[songIndex].Song);

                        // 调用 DisplaySongsWithArrows 并传入当前索引
                        DisplayActionWithSong(currentPage, songIndex, "插播");
                        // songListControl.UpdateSongs(LanguageSongList, songIndex, "插播", currentPage);
                        // songListControl.BringToFront();
                        InsertSongToPlaylist(selectedSong);
                    }
                    else
                    {
                        Console.WriteLine("Song index out of range.");
                    }
                }
            }
        }

        private void HandleLanguageSelection(int number)
        {
            switch (number)
            {
                case 1:
                    currentLanguage = "國語";
                    break;
                case 2:
                    currentLanguage = "台語";
                    break;
                case 3:
                    currentLanguage = "粵語";
                    break;
                case 4:
                    currentLanguage = "英文";
                    break;
                case 5:
                    currentLanguage = "日語";
                    break;
                case 6:
                    currentLanguage = "韓語";
                    break;
                default:
                    displayLabel.Text = "輸入錯誤!!!";
                    displayLabel.BringToFront();
                    return;
            }
            Console.WriteLine("Language selected: " + currentLanguage);
            DisplaySongsInLanguage(currentLanguage, CurrentCategory);
            CurrentUIState = UIState.SelectingSong;
            Console.WriteLine("State changed to SelectingSong");
        }

        private Artist selectedArtist;

        private void HandleArtistSelection(int number)
        {
            int artistIndex = (currentPage - 1) * artistsPerPage + (number - 1);
            inputNumber = number;
            if (artistIndex < totalArtists)
            {
                selectedArtist = currentArtistList[artistIndex];
                currentLanguage = selectedArtist.Name; // 将当前类别设置为所选艺术家的名称
                SetUIState(UIState.SelectingSong); // 设置状态为选择歌曲
                LanguageSongList = SongListManager.Instance.GetSongsByArtist(selectedArtist.Name);
                currentPage = 1;
                totalSongs = LanguageSongList.Count;
                DisplaySongs(currentPage);
            }
            else
            {
                Console.WriteLine("Song index out of range.");
            }
        }

        private void HandleSongSelection(int number)
        {
            Console.WriteLine("Current State is SelectingSong, ready to process song selection.");
            int songIndex = (currentPage - 1) * songsPerPage + (number - 1);
            inputNumber = number;
            Console.WriteLine("Calculated Song Index: " + songIndex + ", Total Songs: " + totalSongs);

            if (songIndex < totalSongs)
            {
                selectedSong = LanguageSongList[songIndex];  // 将选择的歌曲赋值给 selectedSong
                Console.WriteLine("Adding song to playlist: " + LanguageSongList[songIndex].Song);

                // 调用 DisplaySongsWithArrows 并传入当前索引
                DisplaySongsWithArrows(currentPage, songIndex);
                // songListControl.UpdateSongs(LanguageSongList, songIndex, "", currentPage);
                // songListControl.BringToFront();
            }
            else
            {
                Console.WriteLine("Song index out of range.");
            }
        }

        private string currentArtistCategory;

        private void ProcessArtistCategorySelection(int number)
        {
            switch (number)
            {
                case 1:
                    currentArtistCategory = "男";
                    break;
                case 2:
                    currentArtistCategory = "女";
                    break;
                case 3:
                    currentArtistCategory = "團";
                    break;
                case 4:
                    currentArtistCategory = "外";
                    break;
                case 5:
                    currentArtistCategory = "全部";
                    break;
                default:
                    Console.WriteLine("Invalid selection");
                    return;
            }

            ClearDisplay();
            DisplayStrokeCountOptions();
        }

        private void ClearDisplay()
        {
            // Clear the display logic goes here
            // This could involve removing or hiding current buttons, labels, etc.
            foreach (var control in this.Controls.OfType<Control>().ToArray())
            {
                if (control != displayLabel &&
                    control != pauseLabel &&
                    control != muteLabel &&
                    control != volumeUpLabel &&
                    control != volumeDownLabel &&
                    control != micUpLabel &&
                    control != micDownLabel &&
                    control != standardKeyLabel &&
                    control != keyUpLabel &&
                    control != keyDownLabel &&
                    control != maleKeyLabel &&
                    control != femaleKeyLabel &&
                    control != squareLabel &&
                    control != professionalLabel &&
                    control != standardLabel &&
                    control != singDownLabel &&
                    control != brightLabel &&
                    control != softLabel &&
                    control != autoLabel &&
                    control != romanticLabel &&
                    control != dynamicLabel &&
                    control != tintLabel) // Keep the specified controls
                {
                    this.Controls.Remove(control);
                    control.Dispose();
                }
            }
        }

        private string strokeRange; // 全局变量声明
        private int totalArtists = 0;
        private const int artistsPerPage = 10;
        private List<Artist> currentArtistList = new List<Artist>();

        private void ProcessStrokeCountSelection(int number)
        {
            List<Artist> selectedArtists = null;
            switch (number)
            {
                case 1:
                    selectedArtists = ArtistManager.Instance.GetArtistsByCategoryAndStrokeCountRange(currentArtistCategory ,0, 3);
                    strokeRange = "00~03";
                    break;
                case 2:
                    selectedArtists = ArtistManager.Instance.GetArtistsByCategoryAndStrokeCountRange(currentArtistCategory, 4, 7);
                    strokeRange = "04~07";
                    break;
                case 3:
                    selectedArtists = ArtistManager.Instance.GetArtistsByCategoryAndStrokeCountRange(currentArtistCategory, 8, 11);
                    strokeRange = "08~11";
                    break;
                case 4:
                    selectedArtists = ArtistManager.Instance.GetArtistsByCategoryAndStrokeCountRange(currentArtistCategory, 12, 15);
                    strokeRange = "12~15";
                    break;
                case 5:
                    selectedArtists = ArtistManager.Instance.GetArtistsByCategoryAndStrokeCountRange(currentArtistCategory, 16, int.MaxValue);
                    strokeRange = "16以上";
                    break;
                default:
                    Console.WriteLine("Invalid selection");
                    return;
            }

            if (selectedArtists != null && selectedArtists.Count > 0)
            {
                SetUIState(OverlayForm.UIState.SelectingArtist); // 设置状态为选择艺术家
                DisplayArtists(selectedArtists, currentPage);
            }
            else
            {
                Console.WriteLine("No artists found for the selected stroke count range.");
            }
        }

        private void DisplayArtists(List<Artist> artists, int page)
        {
            currentArtistList = artists;
            totalArtists = artists.Count;

            if (artists == null || artists.Count == 0)
            {
                Console.WriteLine("Artist list is null or empty.");
                return;
            }

            // Calculate start and end indices for artists on the current page
            int startIndex = (page - 1) * artistsPerPage;
            int endIndex = Math.Min(startIndex + artistsPerPage, artists.Count);

            // Clear existing controls
            foreach (var control in this.Controls.OfType<Control>().ToArray())
            {
                if (control != displayLabel &&
                    control != pauseLabel &&
                    control != muteLabel &&
                    control != volumeUpLabel &&
                    control != volumeDownLabel &&
                    control != micUpLabel &&
                    control != micDownLabel &&
                    control != standardKeyLabel &&
                    control != keyUpLabel &&
                    control != keyDownLabel &&
                    control != maleKeyLabel &&
                    control != femaleKeyLabel &&
                    control != squareLabel &&
                    control != professionalLabel &&
                    control != standardLabel &&
                    control != singDownLabel &&
                    control != brightLabel &&
                    control != softLabel &&
                    control != autoLabel &&
                    control != romanticLabel &&
                    control != dynamicLabel &&
                    control != tintLabel) // Keep the specified controls
                {
                    this.Controls.Remove(control);
                    control.Dispose();
                }
            }

            // Calculate total pages
            int totalPages = (int)Math.Ceiling((double)artists.Count / artistsPerPage);

            string categoryDisplayText;
            switch (currentArtistCategory)
            {
                case "男":
                    categoryDisplayText = "男歌星";
                    break;
                case "女":
                    categoryDisplayText = "女歌星";
                    break;
                case "團":
                    categoryDisplayText = "團體";
                    break;
                case "外":
                    categoryDisplayText = "外語";
                    break;
                case "全部":
                    categoryDisplayText = "全部";
                    break;
                default:
                    categoryDisplayText = currentArtistCategory; // 默认情况下保持原样
                    break;
            }

            // Display first line with stroke range, category, and page info
            string headerText = String.Format("{0} -- {1} ({2} / {3})", categoryDisplayText, strokeRange, page, totalPages);
            Label headerLabel = new Label
            {
                Text = headerText,
                AutoSize = true,
                ForeColor = Color.White,
                Font = new Font("Microsoft JhengHei", 60, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };

            // Center the header label horizontally
            int headerLabelWidth = TextRenderer.MeasureText(headerLabel.Text, headerLabel.Font).Width;
            int headerLabelX = (this.Width - headerLabelWidth) / 2;
            headerLabel.Location = new Point(headerLabelX, 100); // Position it at the top with some padding
            this.Controls.Add(headerLabel);
            headerLabel.BringToFront();

            // Create and display new labels for artists
            int startY = 200; // 起始Y坐标
            int verticalSpacing = 100; // Vertical space between artist labels
            int startX = 100; // 起始X坐标
            int middleX = this.Width / 2; // 中间位置的X坐标

            for (int i = startIndex; i < endIndex; i++)
            {
                int artistNumber = i - startIndex + 1; // 1-based artist number
                string artistText = artists[i].Name;

                // Create artist label
                Label artistLabel = new Label
                {
                    Text = String.Format("{0}. {1}", artistNumber, artistText),
                    AutoSize = true,
                    ForeColor = Color.White,
                    Font = new Font("Microsoft JhengHei", 60, FontStyle.Bold),
                    BackColor = Color.Transparent,
                    Location = new Point(i < startIndex + 5 ? startX : middleX + startX, startY)
                };

                this.Controls.Add(artistLabel);
                artistLabel.BringToFront();

                startY += verticalSpacing; // 更新Y坐标

                // 如果超过5个艺术家，则切换到下一列并重置Y坐标
                if ((i + 1) % 5 == 0)
                {
                    startY = 200; // 重置Y坐标
                }
            }

            // Ensure displayLabel is not used for artist display anymore
            displayLabel.Text = string.Empty;
        }

        private void DisplayStrokeCountOptions()
        {
            string categoryDisplayText;
            switch (currentArtistCategory)
            {
                case "男":
                    categoryDisplayText = "男歌星";
                    break;
                case "女":
                    categoryDisplayText = "女歌星";
                    break;
                case "團":
                    categoryDisplayText = "團體";
                    break;
                case "外":
                    categoryDisplayText = "外語";
                    break;
                case "全部":
                    categoryDisplayText = "全部";
                    break;
                default:
                    categoryDisplayText = currentArtistCategory; // 默认情况下保持原样
                    break;
            }

            // Construct the messages to display
            string[] messages = new string[]
            {
                categoryDisplayText,
                "1. 0~3  4. 12~15",
                "2. 4~7  5. 16以上",
                "3. 8~11"
            };

            // Set UI state to selecting stroke count
            SetUIState(OverlayForm.UIState.SelectingStrokeCount);

            // Update the display labels with the messages
            UpdateDisplayLabels(messages);
        }

        private void DisplaySongsInLanguage(string language, Category category)
        {
            Dictionary<string, List<SongData>> selectedSongList;

            // 选择使用哪个歌曲列表
            if (category == Category.NewSongs)
            {
                selectedSongList = SongListManager.NewSongLists;
            }
            else if (category == Category.HotSongs)
            {
                selectedSongList = SongListManager.HotSongLists;
            }
            else
            {
                displayLabel.Text = "無效的類別";
                return;
            }

            // 尝试从歌曲列表中获取指定语言的歌曲列表
            List<SongData> songsInLanguage;
            if (!selectedSongList.TryGetValue(language, out songsInLanguage))
            {
                displayLabel.Text = "未找到指定語言的歌曲";
                return;
            }

            LanguageSongList = songsInLanguage; // 更新当前语言的歌曲列表
            totalSongs = songsInLanguage.Count; // 更新总歌曲数
            currentPage = 1; // 重置为第一页

            DisplaySongs(currentPage); // 调用显示歌曲的方法来显示第一页
        }

        public void AddSongToPlaylist(SongData songData)
        {
            try
            {
                // Assuming songData has two file paths for two different hosts
                var filePath1 = songData.SongFilePathHost1;
                var filePath2 = songData.SongFilePathHost2;

                // Check if both song file paths do not exist
                if (!File.Exists(filePath1) && !File.Exists(filePath2))
                {
                    // MessageBox.Show(String.Format("File does not exist on both hosts: {0} and {1}", filePath1, filePath2));

                    // Write to log file
                    PrimaryForm.WriteLog(String.Format("File not found on both hosts: {0} and {1}", filePath1, filePath2));
                }
                else
                {
                    try
                    {
                        // Handle the case where at least one file exists
                        // For example, you can choose to play the file that exists
                        var pathToPlay = File.Exists(filePath1) ? filePath1 : filePath2;
                        
                        // 添加调试语句：输出选择的文件路径
                        Console.WriteLine("Path to play: " + pathToPlay);

                        // Add the logic to play the song using 'pathToPlay'
                        // and handle the userRequestedSongs list as before
                        // 检查播放列表是否为空，在添加歌曲之前
                        bool wasEmpty = PrimaryForm.userRequestedSongs.Count == 0;
                        
                        // 添加调试语句：输出播放列表是否为空
                        Console.WriteLine("Was user requested songs list empty: " + wasEmpty);

                        if (wasEmpty)
                        {
                            PrimaryForm.userRequestedSongs.Add(songData);
                            VideoPlayerForm.Instance.SetPlayingSongList(PrimaryForm.userRequestedSongs);
                            // 添加歌曲到播放历史
                            PrimaryForm.playedSongsHistory.Add(songData);
                            PrimaryForm.playStates.Add(PlayState.Playing);
                            PrimaryForm.currentSongIndexInHistory += 1;
                        }
                        else if (PrimaryForm.userRequestedSongs.Count == 1)
                        {
                            PrimaryForm.userRequestedSongs.Add(songData);
                            VideoPlayerForm.UpdateMarqueeTextForNextSong(songData);
                            // 添加歌曲到播放历史
                            PrimaryForm.playedSongsHistory.Add(songData);
                            PrimaryForm.playStates.Add(PlayState.NotPlayed);
                        }
                        else
                        {
                            PrimaryForm.userRequestedSongs.Add(songData);
                            // 添加歌曲到播放历史
                            PrimaryForm.playedSongsHistory.Add(songData);
                            PrimaryForm.playStates.Add(PlayState.NotPlayed);
                        }

                        PrimaryForm.PrintPlayingSongList();
                    }
                    catch (Exception ex)
                    {
                        // 打印异常信息到控制台或日志文件
                        Console.WriteLine("Error occurred in handling the case where at least one file exists: " + ex.Message);
                        // 或者您也可以使用 MessageBox 来显示异常信息
                        MessageBox.Show("Error occurred in handling the case where at least one file exists: " + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exception
                Console.WriteLine("Error occurred: " + ex.Message);
            }
        }

        public void InsertSongToPlaylist(SongData songData)
        {
            try
            {
                // Assuming songData has two file paths for two different hosts
                var filePath1 = songData.SongFilePathHost1;
                var filePath2 = songData.SongFilePathHost2;

                // Check if both song file paths do not exist
                if (!File.Exists(filePath1) && !File.Exists(filePath2))
                {
                    // MessageBox.Show(String.Format("File does not exist on both hosts: {0} and {1}", filePath1, filePath2));

                    // Write to log file
                    PrimaryForm.WriteLog(String.Format("File not found on both hosts: {0} and {1}", filePath1, filePath2));
                }
                else
                {
                    try
                    {
                        // Handle the case where at least one file exists
                        // For example, you can choose to play the file that exists
                        var pathToPlay = File.Exists(filePath1) ? filePath1 : filePath2;
                        
                        // 添加调试语句：输出选择的文件路径
                        Console.WriteLine("Path to play: " + pathToPlay);

                        // Add the logic to play the song using 'pathToPlay'
                        // and handle the userRequestedSongs list as before
                        // 检查播放列表是否为空，在添加歌曲之前
                        bool wasEmpty = PrimaryForm.userRequestedSongs.Count == 0;
                        
                        // 添加调试语句：输出播放列表是否为空
                        Console.WriteLine("Was user requested songs list empty: " + wasEmpty);

                        if (wasEmpty)
                        {
                            PrimaryForm.userRequestedSongs.Add(songData);
                            VideoPlayerForm.Instance.SetPlayingSongList(PrimaryForm.userRequestedSongs);
                            // 添加歌曲到播放历史
                            PrimaryForm.playedSongsHistory.Add(songData);
                            PrimaryForm.playStates.Add(PlayState.Playing);
                            PrimaryForm.currentSongIndexInHistory += 1;
                        }
                        else if (PrimaryForm.userRequestedSongs.Count == 1)
                        {
                            PrimaryForm.userRequestedSongs.Insert(1, songData);
                            VideoPlayerForm.UpdateMarqueeTextForNextSong(songData);
                            // 添加歌曲到播放历史
                            PrimaryForm.playedSongsHistory.Insert(PrimaryForm.currentSongIndexInHistory + 1, songData);
                            PrimaryForm.playStates.Insert(PrimaryForm.currentSongIndexInHistory + 1, PlayState.NotPlayed);
                        }
                        else
                        {
                            PrimaryForm.userRequestedSongs.Insert(1, songData);
                            // 添加歌曲到播放历史
                            PrimaryForm.playedSongsHistory.Insert(PrimaryForm.currentSongIndexInHistory + 1, songData);
                            PrimaryForm.playStates.Insert(PrimaryForm.currentSongIndexInHistory + 1, PlayState.NotPlayed);
                        }

                        PrimaryForm.PrintPlayingSongList();
                    }
                    catch (Exception ex)
                    {
                        // 打印异常信息到控制台或日志文件
                        Console.WriteLine("Error occurred in handling the case where at least one file exists: " + ex.Message);
                        // 或者您也可以使用 MessageBox 来显示异常信息
                        MessageBox.Show("Error occurred in handling the case where at least one file exists: " + ex.Message);
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exception
                Console.WriteLine("Error occurred: " + ex.Message);
            }
        }

        public int currentPage = 1;
        public int songsPerPage = 5;
        public int totalSongs = 0;  // 此值应该在歌曲加载时计算

        public void DisplaySongs(int page)
        {
            if (LanguageSongList == null || LanguageSongList.Count == 0)
            {
                Console.WriteLine("LanguageSongList is null or empty.");
                return;
            }

            // Calculate start and end indices for songs on the current page
            int startIndex = (page - 1) * songsPerPage;
            int endIndex = Math.Min(startIndex + songsPerPage, LanguageSongList.Count);

            // Clear existing controls
            foreach (var control in this.Controls.OfType<Control>().ToArray())
            {
                if (control != displayLabel &&
                    control != pauseLabel &&
                    control != muteLabel &&
                    control != volumeUpLabel &&
                    control != volumeDownLabel &&
                    control != micUpLabel &&
                    control != micDownLabel &&
                    control != standardKeyLabel &&
                    control != keyUpLabel &&
                    control != keyDownLabel &&
                    control != maleKeyLabel &&
                    control != femaleKeyLabel &&
                    control != squareLabel &&
                    control != professionalLabel &&
                    control != standardLabel &&
                    control != singDownLabel &&
                    control != brightLabel &&
                    control != softLabel &&
                    control != autoLabel &&
                    control != romanticLabel &&
                    control != dynamicLabel &&
                    control != tintLabel) // Keep the specified controls
                {
                    this.Controls.Remove(control);
                    control.Dispose();
                }
            }

            // Determine the category text
            string categoryText;
            if (OverlayForm.CurrentCategory == OverlayForm.Category.NewSongs)
            {
                categoryText = "新歌";
            }
            else if (OverlayForm.CurrentCategory == OverlayForm.Category.HotSongs)
            {
                categoryText = "熱門";
            }
            else
            {
                categoryText = "";
            }

            // Calculate total pages
            int totalPages = (int)Math.Ceiling((double)LanguageSongList.Count / songsPerPage);

            // Display first line with language, category, and page info
            string headerText = String.Format("{0} - {1} ({2} / {3})", currentLanguage, categoryText, page, totalPages);
            Label headerLabel = new Label
            {
                Text = headerText,
                AutoSize = true,
                ForeColor = Color.White,
                Font = new Font("Microsoft JhengHei", 60, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                BackColor = Color.Transparent
            };

            // Center the header label horizontally
            int headerLabelWidth = TextRenderer.MeasureText(headerLabel.Text, headerLabel.Font).Width;
            int headerLabelX = (this.Width - headerLabelWidth) / 2;
            headerLabel.Location = new Point(headerLabelX, 100); // Position it at the top with some padding
            this.Controls.Add(headerLabel);
            headerLabel.BringToFront();

            // Create and display new labels for songs
            int startY = 200; // 起始Y坐标
            int verticalSpacing = 100; // Vertical space between artist labels
            int startX = 100; // 起始X坐标
            int middleX = this.Width / 2; // 中间位置的X坐标

            // Create and display new labels for songs
            for (int i = startIndex; i < endIndex; i++)
            {
                int songNumber = i - startIndex + 1; // 1-based song number
                string songText = LanguageSongList[i].Song;
                string artistText;
                if (!string.IsNullOrWhiteSpace(LanguageSongList[i].ArtistB))
                {
                    artistText = String.Format("{0} - {1}", LanguageSongList[i].ArtistA, LanguageSongList[i].ArtistB);
                }
                else
                {
                    artistText = LanguageSongList[i].ArtistA;
                }

                Label songLabel = new Label
                {
                    Text = String.Format("{0}. {1}", songNumber, songText),
                    // Size = new Size(this.Width * 2 / 3 - 100, 80),
                    // Size = new Size(this.Width * 2 / 3 - 100, verticalSpacing),
                    AutoSize = true,
                    ForeColor = Color.White,
                    Font = new Font("Microsoft JhengHei", 60, FontStyle.Bold),
                    TextAlign = ContentAlignment.TopLeft,
                    Anchor = AnchorStyles.Left,
                    Location = new Point(startX, startY) // Set the position 100 pixels away from the left edge
                };

                Label artistLabel = new Label
                {
                    Text = artistText,
                    // Size = new Size(this.Width / 3, 80),
                    // Size = new Size(this.Width / 3, verticalSpacing),
                    AutoSize = true,
                    ForeColor = Color.White,
                    Font = new Font("Microsoft JhengHei", 60, FontStyle.Bold),
                    TextAlign = ContentAlignment.TopLeft,
                    Anchor = AnchorStyles.Right,
                    Location = new Point(this.Width * 2 / 3, startY)
                };

                this.Controls.Add(songLabel);
                this.Controls.Add(artistLabel);

                startY += verticalSpacing; // 更新Y坐标
            }

            // Ensure displayLabel is not used for song display anymore
            displayLabel.Text = string.Empty;
        }

        public void DisplaySongsWithArrows(int page, int highlightIndex)
        {
            try
            {
                if (LanguageSongList == null || LanguageSongList.Count == 0)
                {
                    Console.WriteLine("Error: LanguageSongList is null or empty.");
                    return;
                }

                // Determine the category text
                string categoryText;
                if (OverlayForm.CurrentCategory == OverlayForm.Category.NewSongs)
                {
                    categoryText = "新歌";
                }
                else if (OverlayForm.CurrentCategory == OverlayForm.Category.HotSongs)
                {
                    categoryText = "熱門";
                }
                else
                {
                    categoryText = "";
                }

                // Use String.Format to log parameter values
                Console.WriteLine(String.Format("DisplaySongsWithArrows called with page: {0}, highlightIndex: {1}", page, highlightIndex));

                int startIndex = (page - 1) * songsPerPage;
                int endIndex = Math.Min(startIndex + songsPerPage, LanguageSongList.Count);

                // Clear existing controls
                foreach (var control in this.Controls.OfType<Control>().ToArray())
                {
                    if (control != displayLabel &&
                        control != pauseLabel &&
                        control != muteLabel &&
                        control != volumeUpLabel &&
                        control != volumeDownLabel &&
                        control != micUpLabel &&
                        control != micDownLabel &&
                        control != standardKeyLabel &&
                        control != keyUpLabel &&
                        control != keyDownLabel &&
                        control != maleKeyLabel &&
                        control != femaleKeyLabel &&
                        control != squareLabel &&
                        control != professionalLabel &&
                        control != standardLabel &&
                        control != singDownLabel &&
                        control != brightLabel &&
                        control != softLabel &&
                        control != autoLabel &&
                        control != romanticLabel &&
                        control != dynamicLabel &&
                        control != tintLabel) // Keep the specified controls
                    {
                        this.Controls.Remove(control);
                        control.Dispose();
                    }
                }

                // Calculate total pages
                int totalPages = (int)Math.Ceiling((double)LanguageSongList.Count / songsPerPage);

                // Display first line with language, category, and page info
                string headerText = String.Format("{0} - {1} ({2} / {3})", currentLanguage, categoryText, page, totalPages);
                Label headerLabel = new Label
                {
                    Text = headerText,
                    AutoSize = true,
                    ForeColor = Color.White,
                    Font = new Font("Microsoft JhengHei", 60, FontStyle.Bold),
                    TextAlign = ContentAlignment.MiddleCenter,
                    BackColor = Color.Transparent,
                };

                // Center the header label horizontally
                int headerLabelWidth = TextRenderer.MeasureText(headerLabel.Text, headerLabel.Font).Width;
                int headerLabelX = (this.Width - headerLabelWidth) / 2;
                headerLabel.Location = new Point(headerLabelX, 100); // Position it at the top with some padding
                this.Controls.Add(headerLabel);
                headerLabel.BringToFront();

                // Create and display new labels for songs
                int startY = 200; // 起始Y坐标
                int verticalSpacing = 100; // Vertical space between artist labels
                int startX = 100; // 起始X坐标
                int middleX = this.Width / 2; // 中间位置的X坐标

                // Create and display new labels for songs
                for (int i = startIndex; i < endIndex; i++)
                {
                    int songNumber = i - startIndex + 1; // 1-based song number
                    string songText = LanguageSongList[i].Song;
                    string artistText;
                    if (!string.IsNullOrWhiteSpace(LanguageSongList[i].ArtistB))
                    {
                        artistText = String.Format("{0} - {1}", LanguageSongList[i].ArtistA, LanguageSongList[i].ArtistB);
                    }
                    else
                    {
                        artistText = LanguageSongList[i].ArtistA;
                    }

                    Label songLabel = new Label
                    {
                        Text = String.Format("{0}. {1}", songNumber, songText),
                        // Size = new Size(this.Width * 2 / 3 - 100, 80), // Set a fixed size for the label
                        AutoSize = true,
                        ForeColor = (i == highlightIndex) ? Color.LimeGreen : Color.White, // Highlight color for selected song
                        Font = new Font("Microsoft JhengHei", 60, FontStyle.Bold),
                        TextAlign = ContentAlignment.TopLeft,
                        Anchor = AnchorStyles.Left,
                        Location = new Point(startX, startY) // Set the position 100 pixels away from the left edge
                    };

                    Label artistLabel = new Label
                    {
                        Text = artistText,
                        // Size = new Size(this.Width / 3, 80), // Set a fixed size for the label
                        AutoSize = true,
                        ForeColor = (i == highlightIndex) ? Color.LimeGreen : Color.White, // Highlight color for selected song
                        Font = new Font("Microsoft JhengHei", 60, FontStyle.Bold),
                        TextAlign = ContentAlignment.TopLeft,
                        Anchor = AnchorStyles.Right,
                        Location = new Point(this.Width * 2 / 3, startY)
                    };

                    this.Controls.Add(songLabel);
                    this.Controls.Add(artistLabel);

                    startY += verticalSpacing;
                }

                // Ensure displayLabel is not used for song display anymore
                displayLabel.Text = string.Empty;
            }
            catch (Exception ex)
            {
                // Capture exception and log the error
                Console.WriteLine(String.Format("Error in DisplaySongsWithArrows: {0}", ex.Message));
                Console.WriteLine(ex.StackTrace);
            }
        }

        public void DisplayActionWithSong(int page, int songIndex, string actionType)
        {
            try
            {
                if (LanguageSongList == null || LanguageSongList.Count == 0)
                {
                    Console.WriteLine("Error: LanguageSongList is null or empty.");
                    return;
                }

                SongData song = LanguageSongList[songIndex];

                int startIndex = (page - 1) * songsPerPage;
                int endIndex = Math.Min(startIndex + songsPerPage, LanguageSongList.Count);

                // Use String.Format to log parameter values
                Console.WriteLine(String.Format("DisplayActionWithSong called with page: {0}, songIndex: {1}, actionType: {2}", page, songIndex, actionType));

                // Clear existing controls
                foreach (var control in this.Controls.OfType<Control>().ToArray())
                {
                    if (control != displayLabel &&
                        control != pauseLabel &&
                        control != muteLabel &&
                        control != volumeUpLabel &&
                        control != volumeDownLabel &&
                        control != micUpLabel &&
                        control != micDownLabel &&
                        control != standardKeyLabel &&
                        control != keyUpLabel &&
                        control != keyDownLabel &&
                        control != maleKeyLabel &&
                        control != femaleKeyLabel &&
                        control != squareLabel &&
                        control != professionalLabel &&
                        control != standardLabel &&
                        control != singDownLabel &&
                        control != brightLabel &&
                        control != softLabel &&
                        control != autoLabel &&
                        control != romanticLabel &&
                        control != dynamicLabel &&
                        control != tintLabel) // Keep the specified controls
                    {
                        this.Controls.Remove(control);
                        control.Dispose();
                    }
                }

                // Calculate total pages
                int totalPages = (int)Math.Ceiling((double)LanguageSongList.Count / songsPerPage);

                // Display first line with language, category, and page info
                string headerText = String.Format("{0}: {1} - {2}", actionType, song.ArtistA, song.Song);
                Label headerLabel = new Label
                {
                    Text = headerText,
                    AutoSize = true,
                    ForeColor = actionType == "點播" ? Color.LimeGreen : Color.Yellow,
                    Font = new Font("Microsoft JhengHei", 60, FontStyle.Bold),
                    TextAlign = ContentAlignment.MiddleLeft,
                    BackColor = Color.Transparent
                };

                // Align the header label to the left
                int headerLabelX = 10; // Position it with a left margin
                headerLabel.Location = new Point(headerLabelX, 100);
                this.Controls.Add(headerLabel);
                headerLabel.BringToFront();

                // Create and display new labels for songs
                int startY = 200; // 起始Y坐标
                int verticalSpacing = 100; // Vertical space between artist labels
                int startX = 100; // 起始X坐标
                int middleX = this.Width / 2; // 中间位置的X坐标

                // Create and display new labels for songs
                for (int i = startIndex; i < endIndex; i++)
                {
                    int songNumber = i - startIndex + 1; // 1-based song number
                    string songText = LanguageSongList[i].Song;
                    string artistText;
                    if (!string.IsNullOrWhiteSpace(LanguageSongList[i].ArtistB))
                    {
                        artistText = String.Format("{0} - {1}", LanguageSongList[i].ArtistA, LanguageSongList[i].ArtistB);
                    }
                    else
                    {
                        artistText = LanguageSongList[i].ArtistA;
                    }

                    Label songLabel = new Label
                    {
                        Text = String.Format("{0}. {1}", songNumber, songText),
                        AutoSize = true,
                        ForeColor = Color.White,
                        Font = new Font("Microsoft JhengHei", 60, FontStyle.Bold),
                        TextAlign = ContentAlignment.TopLeft,
                        Anchor = AnchorStyles.Left,
                        Location = new Point(startX, startY) // Set the position 100 pixels away from the left edge
                    };

                    Label artistLabel = new Label
                    {
                        Text = artistText,
                        AutoSize = true,
                        ForeColor = Color.White,
                        Font = new Font("Microsoft JhengHei", 60, FontStyle.Bold),
                        TextAlign = ContentAlignment.TopLeft,
                        Anchor = AnchorStyles.Right,
                        Location = new Point(this.Width * 2 / 3, startY)
                    };

                    // Event handler for custom painting
                    // songLabel.Paint += (sender, e) =>
                    // {
                    //     ControlPaint.DrawBorder(e.Graphics, songLabel.ClientRectangle, Color.Black, ButtonBorderStyle.Solid);
                    //     e.Graphics.DrawString(songLabel.Text, songLabel.Font, Brushes.White, new PointF(0, 0));
                    // };

                    // artistLabel.Paint += (sender, e) =>
                    // {
                    //     ControlPaint.DrawBorder(e.Graphics, artistLabel.ClientRectangle, Color.Black, ButtonBorderStyle.Solid);
                    //     e.Graphics.DrawString(artistLabel.Text, artistLabel.Font, Brushes.White, new PointF(0, 0));
                    // };

                    this.Controls.Add(songLabel);
                    this.Controls.Add(artistLabel);
                    
                    startY += verticalSpacing;
                }

                // Ensure displayLabel is not used for song display anymore
                displayLabel.Text = string.Empty;
            }
            catch (Exception ex)
            {
                // Capture exception and log the error
                Console.WriteLine(String.Format("Error in DisplayActionWithSong: {0}", ex.Message));
                Console.WriteLine(ex.StackTrace);
            }
        }

        public void NextPage()
        {
            // 先停止计时器
            unifiedTimer.Stop();

            if (CurrentUIState == UIState.SelectingArtist)
            {
                if (currentPage * artistsPerPage < totalArtists)
                {
                    currentPage++;
                    DisplayArtists(currentArtistList, currentPage);
                }
            }
            else
            {                    
                if (currentPage * songsPerPage < totalSongs)
                {
                    currentPage++;
                    DisplaySongs(currentPage);
                }
            }

            // 重新启动计时器
            unifiedTimer.Start();
        }

        public void PreviousPage()
        {
            // 先停止计时器
            unifiedTimer.Stop();

            if (CurrentUIState == UIState.SelectingArtist)
            {
                if (currentPage > 1)
                {
                    currentPage--;
                    DisplayArtists(currentArtistList, currentPage);
                }
            }
            else
            {   
                if (currentPage > 1)
                {
                    currentPage--;
                    DisplaySongs(currentPage);
                }
            }

            // 重新启动计时器
            unifiedTimer.Start();
        }
    }
}