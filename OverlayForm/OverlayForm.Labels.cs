// OverlayForm/OverlayForm.Labels.cs
using System.IO;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using System;
using System.Linq;

namespace DualScreenDemo
{
    public partial class OverlayForm
    {
        public Label displayLabel;
        public Label songDisplayLabel;
        private List<Label> displayLabels;
        public Label pauseLabel;
        public Label muteLabel; // New mute label
        public Label originalSongLabel;
        public Label volumeUpLabel; // New volume up label
        public Label volumeDownLabel; // New volume down label
        private System.Windows.Forms.Timer volumeUpTimer = new System.Windows.Forms.Timer();
        private System.Windows.Forms.Timer volumeDownTimer = new System.Windows.Forms.Timer();
        private System.Windows.Forms.Timer micUpTimer = new System.Windows.Forms.Timer();
        private System.Windows.Forms.Timer micDownTimer = new System.Windows.Forms.Timer();
        public Label micUpLabel; // New microphone up label
        public Label micDownLabel; // New microphone down label
        public Label keyUpLabel; // New key up label
        public Label keyDownLabel; // New key down label
        public Label standardLabel; // New standard label
        public Label squareLabel; // New square label
        public Label professionalLabel; // New professional label
        public Label singDownLabel; // New sing down label
        public Label brightLabel; // New bright label
        public Label softLabel; // New soft label
        public Label autoLabel; // New auto label
        public Label romanticLabel; // New romantic label
        public Label dynamicLabel; // New dynamic label
        public Label tintLabel; // New tint label
        private System.Windows.Forms.Timer standardTimer = new System.Windows.Forms.Timer();
        private System.Windows.Forms.Timer squareTimer = new System.Windows.Forms.Timer();
        private System.Windows.Forms.Timer professionalTimer = new System.Windows.Forms.Timer();
        private System.Windows.Forms.Timer singDownTimer = new System.Windows.Forms.Timer();
        private System.Windows.Forms.Timer brightTimer = new System.Windows.Forms.Timer();
        private System.Windows.Forms.Timer softTimer = new System.Windows.Forms.Timer();
        private System.Windows.Forms.Timer autoTimer = new System.Windows.Forms.Timer();
        private System.Windows.Forms.Timer romanticTimer = new System.Windows.Forms.Timer();
        private System.Windows.Forms.Timer dynamicTimer = new System.Windows.Forms.Timer();
        private System.Windows.Forms.Timer tintTimer = new System.Windows.Forms.Timer();
        private System.Windows.Forms.Timer secondLineDisplayTimer = new System.Windows.Forms.Timer();
        private System.Windows.Forms.Timer qrCodeTimer = new System.Windows.Forms.Timer();

        private void InitializeLabels()
        {
            InitializeDisplayLabel();
            InitializeSongDisplayLabel();
            InitializeDisplayLabels();
            InitializePauseLabel();
            InitializeMuteLabel(); // Initialize the new mute label
            InitializeOriginalSongLabel(); // 初始化原唱标签
            InitializeVolumeUpLabel(); // Initialize the volume up label
            InitializeVolumeDownLabel(); // Initialize the volume down label
            InitializeMicUpLabel(); // Initialize the microphone up label
            InitializeMicDownLabel(); // Initialize the microphone down label
            InitializeKeyUpLabel(); // Initialize the key up label
            InitializeKeyDownLabel(); // Initialize the key down label
            InitializeStandardLabel(); // Initialize the standard label
            InitializeSquareLabel(); // Initialize the square label
            InitializeProfessionalLabel(); // Initialize the professional label
            InitializeSingDownLabel(); // Initialize the sing down label
            InitializeBrightLabel(); // Initialize the bright label
            InitializeSoftLabel(); // Initialize the soft label
            InitializeAutoLabel(); // Initialize the auto label
            InitializeRomanticLabel(); // Initialize the romantic label
            InitializeDynamicLabel(); // Initialize the dynamic label
            InitializeTintLabel(); // Initialize the tint label
            InitializeStandardKeyLabel();
            InitializeMaleKeyLabel();
            InitializeMaleKeyTimer();
            InitializeFemaleKeyLabel();
            InitializeFemaleKeyTimer();
            ConfigureKeyTimers();
        }

        private void ConfigureKeyTimers()
        {
            volumeUpTimer.Interval = 3000; // 3 seconds
            volumeUpTimer.Tick += (sender, e) => HideVolumeUpLabel();

            volumeDownTimer.Interval = 3000; // 3 seconds
            volumeDownTimer.Tick += (sender, e) => HideVolumeDownLabel();

            micUpTimer.Interval = 3000; // 3 seconds
            micUpTimer.Tick += (sender, e) => HideMicUpLabel();

            micDownTimer.Interval = 3000; // 3 seconds
            micDownTimer.Tick += (sender, e) => HideMicDownLabel();

            standardTimer.Interval = 3000; // 3 seconds
            standardTimer.Tick += (sender, e) => HideStandardLabel();

            squareTimer.Interval = 3000; // 3 seconds
            squareTimer.Tick += (sender, e) => HideSquareLabel();

            professionalTimer.Interval = 3000; // 3 seconds
            professionalTimer.Tick += (sender, e) => HideProfessionalLabel();

            singDownTimer.Interval = 3000; // 3 seconds
            singDownTimer.Tick += (sender, e) => HideSingDownLabel();

            brightTimer.Interval = 3000; // 3 seconds
            brightTimer.Tick += (sender, e) => HideBrightLabel();

            softTimer.Interval = 3000; // 3 seconds
            softTimer.Tick += (sender, e) => HideSoftLabel();

            autoTimer.Interval = 3000; // 3 seconds
            autoTimer.Tick += (sender, e) => HideAutoLabel();

            romanticTimer.Interval = 3000; // 3 seconds
            romanticTimer.Tick += (sender, e) => HideRomanticLabel();

            dynamicTimer.Interval = 3000; // 3 seconds
            dynamicTimer.Tick += (sender, e) => HideDynamicLabel();

            tintTimer.Interval = 3000; // 3 seconds
            tintTimer.Tick += (sender, e) => HideTintLabel();

            standardKeyTimer.Interval = 3000; // 3 seconds
            standardKeyTimer.Tick += (sender, e) => HideStandardKeyLabel();

            keyUpTimer.Interval = 3000; // 3 seconds
            keyUpTimer.Tick += (sender, e) => HideKeyUpLabel();
            
            keyDownTimer.Interval = 3000; // 3 seconds
            keyDownTimer.Tick += (sender, e) => HideKeyDownLabel();

            secondLineDisplayTimer.Interval = 30000; // 30秒
            secondLineDisplayTimer.Tick += SecondLineDisplayTimer_Tick;

            // Initialize Timer for hiding QR code
            qrCodeTimer.Interval = 10000; // 10 seconds
            qrCodeTimer.Tick += QrCodeTimer_Tick;
        }

        private void QrCodeTimer_Tick(object sender, EventArgs e)
        {
            showQRCode = false;
            qrCodeTimer.Stop();
            Invalidate(); // Trigger a repaint to hide the QR code
        }

        public void DisplayQRCodeOnOverlay(string randomFolderPath)
        {
            try
            {
                // Read the server address from the file
                string serverAddressFilePath = Path.Combine(Application.StartupPath, "txt", "ip.txt");
                if (!File.Exists(serverAddressFilePath))
                {
                    Console.WriteLine("Server address file not found: " + serverAddressFilePath);
                    return;
                }

                string serverAddress = File.ReadAllText(serverAddressFilePath).Trim();
                // Generate the URL content for the QR code
                string qrContent = String.Format("{0}/{1}/index.html", serverAddress, randomFolderPath);
                Console.WriteLine("QR Content: " + qrContent);

                // Generate QR code image
                string qrImagePath = Path.Combine(Application.StartupPath, "themes/superstar/_www", randomFolderPath, "qrcode.png");
                if (!File.Exists(qrImagePath))
                {
                    Console.WriteLine("QR code image not found: " + qrImagePath);
                    return;
                }

                // Attempt to open the QR code image with retries
                for (int i = 0; i < 3; i++)
                {
                    try
                    {
                        using (var fs = new FileStream(qrImagePath, FileMode.Open, FileAccess.Read))
                        {
                            qrCodeImage = Image.FromStream(fs);
                        }
                        break;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error loading QR code image: " + ex.Message);
                        System.Threading.Thread.Sleep(100); // Wait a bit before retrying
                    }
                }

                if (qrCodeImage == null)
                {
                    Console.WriteLine("Failed to load QR code image after multiple attempts.");
                    return;
                }

                showQRCode = true;
                qrCodeTimer.Start();
                Invalidate(); // Trigger a repaint to show the QR code
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in DisplayQRCodeOnOverlay: " + ex.Message);
                if (ex.InnerException != null)
                {
                    Console.WriteLine("Inner exception: " + ex.InnerException.Message);
                }
            }
        }

        private void SecondLineDisplayTimer_Tick(object sender, EventArgs e)
        {
            // 停止计时器
            secondLineDisplayTimer.Stop();

            // 清空第二行文本并重绘
            marqueeTextSecondLine = "";
            marqueeXPosSecondLine = this.Width;
            Invalidate();
        }

        private void InitializeDisplayLabel()
        {
            displayLabel = new Label();
            displayLabel.Location = new Point(100, 100); // 設置顯示位置
            displayLabel.AutoSize = true;
            displayLabel.ForeColor = Color.White;
            displayLabel.Font = new Font("Microsoft JhengHei", 40, FontStyle.Bold); // 設定字體樣式
            displayLabel.BackColor = Color.Transparent;
            this.Controls.Add(displayLabel);
        }

        private void InitializeSongDisplayLabel()
        {
            songDisplayLabel = new Label();
            songDisplayLabel.Location = new Point(0, 150); // 設置顯示位置
            songDisplayLabel.AutoSize = true;
            songDisplayLabel.ForeColor = Color.White;
            songDisplayLabel.Font = new Font("Microsoft JhengHei", 40, FontStyle.Bold); // 設定字體樣式
            songDisplayLabel.BackColor = Color.Transparent;
            this.Controls.Add(songDisplayLabel);
        }

        private void InitializeDisplayLabels()
        {
            displayLabels = new List<Label>();

            for (int i = 0; i < 6; i++) // Assuming a maximum of 6 lines
            {
                Label displayLabel = new Label
                {
                    AutoSize = true,
                    ForeColor = Color.White,
                    Font = new Font("Microsoft JhengHei", 25, FontStyle.Bold), // 設定字體樣式
                    BackColor = Color.Transparent
                };
                displayLabels.Add(displayLabel);
                this.Controls.Add(displayLabel);
            }
        }

        private void InitializePauseLabel()
        {
            pauseLabel = new Label();
            pauseLabel.AutoSize = true;
            pauseLabel.ForeColor = Color.White;
            pauseLabel.Font = new Font("Microsoft JhengHei", 200, FontStyle.Bold); // 设置字体样式
            pauseLabel.BackColor = Color.Transparent;
            pauseLabel.Text = "播放暫停";
            pauseLabel.TextAlign = ContentAlignment.MiddleCenter; // 设置文本居中
            pauseLabel.Visible = false; // 初始设置为不可见
            this.Controls.Add(pauseLabel);

            // 设置标签位置到右上角
            // pauseLabel.Location = new Point(this.Width - pauseLabel.Width - 10, 100);
            // this.Resize += (s, e) => 
            // {
            //     pauseLabel.Location = new Point(this.Width - pauseLabel.Width - 10, 100);
            // };
            pauseLabel.Location = new Point(
                (this.Width - pauseLabel.Width) / 2,
                (this.Height - pauseLabel.Height) / 2
            );
            this.Resize += (s, e) =>
            {
                pauseLabel.Location = new Point(
                    (this.Width - pauseLabel.Width) / 2,
                    (this.Height - pauseLabel.Height) / 2
                );
            };
        }

        // Initialize the new mute label
        private void InitializeMuteLabel()
        {
            muteLabel = new Label();
            muteLabel.AutoSize = true;
            muteLabel.ForeColor = Color.Yellow;
            muteLabel.Font = new Font("Microsoft JhengHei", 150, FontStyle.Bold); // 设置字体样式
            muteLabel.BackColor = Color.Transparent;
            muteLabel.Text = "【全部靜音】";
            muteLabel.Visible = false; // 初始设置为不可见
            this.Controls.Add(muteLabel);

            // 设置标签位置到右上角，和暂停标签一样的位置，但下移一些
            muteLabel.Location = new Point(
                (this.Width - muteLabel.Width) / 2,
                100
            );
            this.Resize += (s, e) =>
            {
                muteLabel.Location = new Point(
                    (this.Width - muteLabel.Width) / 2,
                    100
                );
            };
        }

        private void InitializeOriginalSongLabel()
        {
            originalSongLabel = new Label
            {
                Text = "伴唱",
                Font = new Font("Microsoft JhengHei", 50, FontStyle.Bold), // 设置字体样式
                ForeColor = Color.Blue, // 设置文字颜色
                BackColor = Color.Transparent,
                AutoSize = true,
                Visible = false // 初始设置为不可见
            };

            // 设置标签位置到界面中央
            originalSongLabel.Location = new Point(
                (this.Width - originalSongLabel.Width) / 2,
                (this.Height - originalSongLabel.Height) / 2
            );

            // 确保标签在最前面
            originalSongLabel.BringToFront();

            this.Controls.Add(originalSongLabel);

            // 确保在窗口大小变化时，标签保持在中央
            this.Resize += (s, e) =>
            {
                originalSongLabel.Location = new Point(
                    (this.Width - originalSongLabel.Width) / 2,
                    (this.Height - originalSongLabel.Height) / 2
                );
            };
        }

        // Initialize the volume up label
        private void InitializeVolumeUpLabel()
        {
            volumeUpLabel = new Label();
            volumeUpLabel.AutoSize = true;
            volumeUpLabel.ForeColor = Color.LightBlue;
            volumeUpLabel.Font = new Font("Microsoft JhengHei", 40, FontStyle.Bold); // 设置字体样式
            volumeUpLabel.BackColor = Color.Transparent;
            volumeUpLabel.Text = "音量+";
            volumeUpLabel.Visible = false; // 初始设置为不可见
            this.Controls.Add(volumeUpLabel);

            // 设置标签位置到右上角，和静音标签一样的位置，但下移一些
            volumeUpLabel.Location = new Point(this.Width - volumeUpLabel.Width - 10, 100);
            this.Resize += (s, e) => 
            {
                volumeUpLabel.Location = new Point(this.Width - volumeUpLabel.Width - 10, 100);
            };
        }

        // Initialize the volume down label
        private void InitializeVolumeDownLabel()
        {
            volumeDownLabel = new Label();
            volumeDownLabel.AutoSize = true;
            volumeDownLabel.ForeColor = Color.LightBlue;
            volumeDownLabel.Font = new Font("Microsoft JhengHei", 40, FontStyle.Bold); // 设置字体样式
            volumeDownLabel.BackColor = Color.Transparent;
            volumeDownLabel.Text = "音量-";
            volumeDownLabel.Visible = false; // 初始设置为不可见
            this.Controls.Add(volumeDownLabel);

            // 设置标签位置到右上角，和静音标签一样的位置，但下移一些
            volumeDownLabel.Location = new Point(this.Width - volumeDownLabel.Width - 10, 100);
            this.Resize += (s, e) => 
            {
                volumeDownLabel.Location = new Point(this.Width - volumeDownLabel.Width - 10, 100);
            };
        }

        // Initialize the microphone up label
        private void InitializeMicUpLabel()
        {
            micUpLabel = new Label();
            micUpLabel.AutoSize = true;
            micUpLabel.ForeColor = Color.Orange;
            micUpLabel.Font = new Font("Microsoft JhengHei", 40, FontStyle.Bold); // 设置字体样式
            micUpLabel.BackColor = Color.Transparent;
            micUpLabel.Text = "麥克風+";
            micUpLabel.Visible = false; // 初始设置为不可见
            this.Controls.Add(micUpLabel);

            // 设置标签位置到右上角，和静音标签一样的位置，但下移一些
            micUpLabel.Location = new Point(this.Width - micUpLabel.Width - 10, 100);
            this.Resize += (s, e) => 
            {
                micUpLabel.Location = new Point(this.Width - micUpLabel.Width - 10, 100);
            };
        }

        // Initialize the microphone down label
        private void InitializeMicDownLabel()
        {
            micDownLabel = new Label();
            micDownLabel.AutoSize = true;
            micDownLabel.ForeColor = Color.Orange;
            micDownLabel.Font = new Font("Microsoft JhengHei", 40, FontStyle.Bold); // 设置字体样式
            micDownLabel.BackColor = Color.Transparent;
            micDownLabel.Text = "麥克風-";
            micDownLabel.Visible = false; // 初始设置为不可见
            this.Controls.Add(micDownLabel);

            // 设置标签位置到右上角，和静音标签一样的位置，但下移一些
            micDownLabel.Location = new Point(this.Width - micDownLabel.Width - 10, 100);
            this.Resize += (s, e) => 
            {
                micDownLabel.Location = new Point(this.Width - micDownLabel.Width - 10, 100);
            };
        }

        // Initialize the key up label
        private void InitializeKeyUpLabel()
        {
            keyUpLabel = new Label();
            keyUpLabel.AutoSize = true;
            keyUpLabel.ForeColor = Color.Purple;
            keyUpLabel.Font = new Font("Microsoft JhengHei", 40, FontStyle.Bold); // 设置字体样式
            keyUpLabel.BackColor = Color.Transparent;
            keyUpLabel.Text = "升調#";
            keyUpLabel.Visible = false; // 初始设置为不可见
            this.Controls.Add(keyUpLabel);

            // 设置标签位置到右上角，和静音标签一样的位置，但下移一些
            keyUpLabel.Location = new Point(this.Width - keyUpLabel.Width - 10, 100);
            this.Resize += (s, e) => 
            {
                keyUpLabel.Location = new Point(this.Width - keyUpLabel.Width - 10, 100);
            };
        }

        // Initialize the key down label
        private void InitializeKeyDownLabel()
        {
            keyDownLabel = new Label();
            keyDownLabel.AutoSize = true;
            keyDownLabel.ForeColor = Color.Purple;
            keyDownLabel.Font = new Font("Microsoft JhengHei", 40, FontStyle.Bold); // 设置字体样式
            keyDownLabel.BackColor = Color.Transparent;
            keyDownLabel.Text = "降調";
            keyDownLabel.Visible = false; // 初始设置为不可见
            this.Controls.Add(keyDownLabel);

            // 设置标签位置到右上角，和静音标签一样的位置，但下移一些
            keyDownLabel.Location = new Point(this.Width - keyDownLabel.Width - 10, 100);
            this.Resize += (s, e) => 
            {
                keyDownLabel.Location = new Point(this.Width - keyDownLabel.Width - 10, 100);
            };
        }

        // Initialize the standard label
        private void InitializeStandardLabel()
        {
            standardLabel = new Label();
            standardLabel.AutoSize = true;
            standardLabel.ForeColor = Color.Green;
            standardLabel.Font = new Font("Microsoft JhengHei", 40, FontStyle.Bold); // 设置字体样式
            standardLabel.BackColor = Color.Transparent;
            standardLabel.Text = "標準";
            standardLabel.Visible = false; // 初始设置为不可见
            this.Controls.Add(standardLabel);

            standardLabel.Location = new Point(this.Width - standardLabel.Width - 10, 100);
            this.Resize += (s, e) => 
            {
                standardLabel.Location = new Point(this.Width - standardLabel.Width - 10, 100);
            };
        }

        // Initialize the square label
        private void InitializeSquareLabel()
        {
            squareLabel = new Label();
            squareLabel.AutoSize = true;
            squareLabel.ForeColor = Color.Brown;
            squareLabel.Font = new Font("Microsoft JhengHei", 40, FontStyle.Bold); // 设置字体样式
            squareLabel.BackColor = Color.Transparent;
            squareLabel.Text = "廣場";
            squareLabel.Visible = false; // 初始设置为不可见
            this.Controls.Add(squareLabel);

            squareLabel.Location = new Point(this.Width - squareLabel.Width - 10, 100);
            this.Resize += (s, e) => 
            {
                squareLabel.Location = new Point(this.Width - squareLabel.Width - 10, 100);
            };
        }

        // Initialize the professional label
        private void InitializeProfessionalLabel()
        {
            professionalLabel = new Label();
            professionalLabel.AutoSize = true;
            professionalLabel.ForeColor = Color.DarkRed;
            professionalLabel.Font = new Font("Microsoft JhengHei", 40, FontStyle.Bold); // 设置字体样式
            professionalLabel.BackColor = Color.Transparent;
            professionalLabel.Text = "專業";
            professionalLabel.Visible = false; // 初始设置为不可见
            this.Controls.Add(professionalLabel);

            professionalLabel.Location = new Point(this.Width - professionalLabel.Width - 10, 100);
            this.Resize += (s, e) => 
            {
                professionalLabel.Location = new Point(this.Width - professionalLabel.Width - 10, 100);
            };
        }

        // Initialize the sing down label
        private void InitializeSingDownLabel()
        {
            singDownLabel = new Label();
            singDownLabel.AutoSize = true;
            singDownLabel.ForeColor = Color.Magenta;
            singDownLabel.Font = new Font("Microsoft JhengHei", 40, FontStyle.Bold); // 设置字体样式
            singDownLabel.BackColor = Color.Transparent;
            singDownLabel.Text = "唱將";
            singDownLabel.Visible = false; // 初始设置为不可见
            this.Controls.Add(singDownLabel);

            singDownLabel.Location = new Point(this.Width - singDownLabel.Width - 10, 100);
            this.Resize += (s, e) => 
            {
                singDownLabel.Location = new Point(this.Width - singDownLabel.Width - 10, 100);
            };
        }

        // Initialize the bright label
        private void InitializeBrightLabel()
        {
            brightLabel = new Label();
            brightLabel.AutoSize = true;
            brightLabel.ForeColor = Color.LightYellow;
            brightLabel.Font = new Font("Microsoft JhengHei", 40, FontStyle.Bold); // 设置字体样式
            brightLabel.BackColor = Color.Transparent;
            brightLabel.Text = "明亮";
            brightLabel.Visible = false; // 初始设置为不可见
            this.Controls.Add(brightLabel);

            brightLabel.Location = new Point(this.Width - brightLabel.Width - 10, 100);
            this.Resize += (s, e) => 
            {
                brightLabel.Location = new Point(this.Width - brightLabel.Width - 10, 100);
            };
        }

        // Initialize the soft label
        private void InitializeSoftLabel()
        {
            softLabel = new Label();
            softLabel.AutoSize = true;
            softLabel.ForeColor = Color.LightGreen;
            softLabel.Font = new Font("Microsoft JhengHei", 40, FontStyle.Bold); // 设置字体样式
            softLabel.BackColor = Color.Transparent;
            softLabel.Text = "柔和";
            softLabel.Visible = false; // 初始设置为不可见
            this.Controls.Add(softLabel);

            softLabel.Location = new Point(this.Width - softLabel.Width - 10, 100);
            this.Resize += (s, e) => 
            {
                softLabel.Location = new Point(this.Width - softLabel.Width - 10, 100);
            };
        }

        // Initialize the auto label
        private void InitializeAutoLabel()
        {
            autoLabel = new Label();
            autoLabel.AutoSize = true;
            autoLabel.ForeColor = Color.Cyan;
            autoLabel.Font = new Font("Microsoft JhengHei", 40, FontStyle.Bold); // 设置字体样式
            autoLabel.BackColor = Color.Transparent;
            autoLabel.Text = "自動";
            autoLabel.Visible = false; // 初始设置为不可见
            this.Controls.Add(autoLabel);

            autoLabel.Location = new Point(this.Width - autoLabel.Width - 10, 100);
            this.Resize += (s, e) => 
            {
                autoLabel.Location = new Point(this.Width - autoLabel.Width - 10, 100);
            };
        }

        // Initialize the romantic label
        private void InitializeRomanticLabel()
        {
            romanticLabel = new Label();
            romanticLabel.AutoSize = true;
            romanticLabel.ForeColor = Color.Pink;
            romanticLabel.Font = new Font("Microsoft JhengHei", 40, FontStyle.Bold); // 设置字体样式
            romanticLabel.BackColor = Color.Transparent;
            romanticLabel.Text = "浪漫";
            romanticLabel.Visible = false; // 初始设置为不可见
            this.Controls.Add(romanticLabel);

            romanticLabel.Location = new Point(this.Width - romanticLabel.Width - 10, 100);
            this.Resize += (s, e) => 
            {
                romanticLabel.Location = new Point(this.Width - romanticLabel.Width - 10, 100);
            };
        }

        // Initialize the dynamic label
        private void InitializeDynamicLabel()
        {
            dynamicLabel = new Label();
            dynamicLabel.AutoSize = true;
            dynamicLabel.ForeColor = Color.Red;
            dynamicLabel.Font = new Font("Microsoft JhengHei", 40, FontStyle.Bold); // 设置字体样式
            dynamicLabel.BackColor = Color.Transparent;
            dynamicLabel.Text = "動感";
            dynamicLabel.Visible = false; // 初始设置为不可见
            this.Controls.Add(dynamicLabel);

            dynamicLabel.Location = new Point(this.Width - dynamicLabel.Width - 10, 100);
            this.Resize += (s, e) => 
            {
                dynamicLabel.Location = new Point(this.Width - dynamicLabel.Width - 10, 100);
            };
        }

        // Initialize the tint label
        private void InitializeTintLabel()
        {
            tintLabel = new Label();
            tintLabel.AutoSize = true;
            tintLabel.ForeColor = Color.Blue;
            tintLabel.Font = new Font("Microsoft JhengHei", 40, FontStyle.Bold); // 设置字体样式
            tintLabel.BackColor = Color.Transparent;
            tintLabel.Text = "調色";
            tintLabel.Visible = false; // 初始设置为不可见
            this.Controls.Add(tintLabel);

            tintLabel.Location = new Point(this.Width - tintLabel.Width - 10, 100);
            this.Resize += (s, e) => 
            {
                tintLabel.Location = new Point(this.Width - tintLabel.Width - 10, 100);
            };
        }

        // 显示标准标签
        public void ShowStandardLabel()
        {
            standardLabel.Visible = true;
            standardTimer.Start();
        }

        // 隐藏标准标签
        public void HideStandardLabel()
        {
            standardLabel.Visible = false;
            standardTimer.Stop();
        }

        // 显示广场标签
        public void ShowSquareLabel()
        {
            squareLabel.Visible = true;
            squareTimer.Start();
        }

        // 隐藏广场标签
        public void HideSquareLabel()
        {
            squareLabel.Visible = false;
            squareTimer.Stop();
        }

        // 显示专业标签
        public void ShowProfessionalLabel()
        {
            professionalLabel.Visible = true;
            professionalTimer.Start();
        }

        // 隐藏专业标签
        public void HideProfessionalLabel()
        {
            professionalLabel.Visible = false;
            professionalTimer.Stop();
        }

        // 显示唱降标签
        public void ShowSingDownLabel()
        {
            singDownLabel.Visible = true;
            singDownTimer.Start();
        }

        // 隐藏唱降标签
        public void HideSingDownLabel()
        {
            singDownLabel.Visible = false;
            singDownTimer.Stop();
        }

        // 显示明亮标签
        public void ShowBrightLabel()
        {
            brightLabel.Visible = true;
            brightTimer.Start();
        }

        // 隐藏明亮标签
        public void HideBrightLabel()
        {
            brightLabel.Visible = false;
            brightTimer.Stop();
        }

        // 显示柔和标签
        public void ShowSoftLabel()
        {
            softLabel.Visible = true;
            softTimer.Start();
        }

        // 隐藏柔和标签
        public void HideSoftLabel()
        {
            softLabel.Visible = false;
            softTimer.Stop();
        }

        // 显示自動标签
        public void ShowAutoLabel()
        {
            autoLabel.Visible = true;
            autoTimer.Start();
        }

        // 隐藏自動标签
        public void HideAutoLabel()
        {
            autoLabel.Visible = false;
            autoTimer.Stop();
        }

        // 显示浪漫标签
        public void ShowRomanticLabel()
        {
            romanticLabel.Visible = true;
            romanticTimer.Start();
        }

        // 隐藏浪漫标签
        public void HideRomanticLabel()
        {
            romanticLabel.Visible = false;
            romanticTimer.Stop();
        }

        // 显示動感标签
        public void ShowDynamicLabel()
        {
            dynamicLabel.Visible = true;
            dynamicTimer.Start();
        }

        // 隐藏動感标签
        public void HideDynamicLabel()
        {
            dynamicLabel.Visible = false;
            dynamicTimer.Stop();
        }

        // 显示調色标签
        public void ShowTintLabel()
        {
            tintLabel.Visible = true;
            tintTimer.Start();
        }

        // 隐藏調色标签
        public void HideTintLabel()
        {
            tintLabel.Visible = false;
            tintTimer.Stop();
        }

        // Initialize the standard key label
        private void InitializeStandardKeyLabel()
        {
            standardKeyLabel = new Label();
            standardKeyLabel.AutoSize = true;
            standardKeyLabel.ForeColor = Color.Purple;
            standardKeyLabel.Font = new Font("Microsoft JhengHei", 40, FontStyle.Bold); // 设置字体样式
            standardKeyLabel.BackColor = Color.Transparent;
            standardKeyLabel.Text = "標準調";
            standardKeyLabel.Visible = false; // 初始设置为不可见
            this.Controls.Add(standardKeyLabel);

            // 设置标签位置到右上角，和静音标签一样的位置，但下移一些
            standardKeyLabel.Location = new Point(this.Width - standardKeyLabel.Width - 10, 100);
            this.Resize += (s, e) => 
            {
                standardKeyLabel.Location = new Point(this.Width - standardKeyLabel.Width - 10, 100);
            };
        }

        // Initialize the male key label
        private void InitializeMaleKeyLabel()
        {
            maleKeyLabel = new Label();
            maleKeyLabel.AutoSize = true;
            maleKeyLabel.ForeColor = Color.Blue;
            maleKeyLabel.Font = new Font("Microsoft JhengHei", 40, FontStyle.Bold); // 设置字体样式
            maleKeyLabel.BackColor = Color.Transparent;
            maleKeyLabel.Text = "男調";
            maleKeyLabel.Visible = false; // 初始设置为不可见
            this.Controls.Add(maleKeyLabel);

            // 设置标签位置到右上角，和静音标签一样的位置，但下移一些
            maleKeyLabel.Location = new Point(this.Width - maleKeyLabel.Width - 10, 100);
            this.Resize += (s, e) => 
            {
                maleKeyLabel.Location = new Point(this.Width - maleKeyLabel.Width - 10, 100);
            };
        }

        // 显示男调标签
        public void ShowMaleKeyLabel()
        {
            maleKeyLabel.Visible = true;
            maleKeyTimer.Start();
        }

        // 隐藏男调标签
        public void HideMaleKeyLabel()
        {
            maleKeyLabel.Visible = false;
            maleKeyTimer.Stop();
        }

        // Initialize the female key label
        private void InitializeFemaleKeyLabel()
        {
            femaleKeyLabel = new Label();
            femaleKeyLabel.AutoSize = true;
            femaleKeyLabel.ForeColor = Color.Pink;
            femaleKeyLabel.Font = new Font("Microsoft JhengHei", 40, FontStyle.Bold); // 设置字体样式
            femaleKeyLabel.BackColor = Color.Transparent;
            femaleKeyLabel.Text = "女調";
            femaleKeyLabel.Visible = false; // 初始设置为不可见
            this.Controls.Add(femaleKeyLabel);

            // 设置标签位置到右上角，和静音标签一样的位置，但下移一些
            femaleKeyLabel.Location = new Point(this.Width - femaleKeyLabel.Width - 10, 100);
            this.Resize += (s, e) => 
            {
                femaleKeyLabel.Location = new Point(this.Width - femaleKeyLabel.Width - 10, 100);
            };
        }

        // 显示女调标签
        public void ShowFemaleKeyLabel()
        {
            femaleKeyLabel.Visible = true;
            femaleKeyTimer.Start();
        }

        // 隐藏女调标签
        public void HideFemaleKeyLabel()
        {
            femaleKeyLabel.Visible = false;
            femaleKeyTimer.Stop();
        }

        // 显示暂停标签
        public void ShowPauseLabel()
        {
            pauseLabel.Visible = true;
        }

        // 隐藏暂停标签
        public void HidePauseLabel()
        {
            pauseLabel.Visible = false;
        }

        // 显示静音标签
        public void ShowMuteLabel()
        {
            muteLabel.Visible = true;
        }

        // 隐藏静音标签
        public void HideMuteLabel()
        {
            muteLabel.Visible = false;
        }
        public void ShowOriginalSongLabel()
        {
            originalSongLabel.Visible = true;
        }

        public void HideOriginalSongLabel()
        {
            originalSongLabel.Visible = false;
        }

        public void ToggleOriginalSongLabel()
        {
            originalSongLabel.Visible = !originalSongLabel.Visible;
        }

        // 显示音量+标签
        public void ShowVolumeUpLabel()
        {
            volumeUpLabel.Visible = true;
            volumeUpTimer.Start();
        }

        // 隐藏音量+标签
        public void HideVolumeUpLabel()
        {
            volumeUpLabel.Visible = false;
        }

        // 显示音量-标签
        public void ShowVolumeDownLabel()
        {
            volumeDownLabel.Visible = true;
            volumeDownTimer.Start();
        }

        // 隐藏音量-标签
        public void HideVolumeDownLabel()
        {
            volumeDownLabel.Visible = false;
        }

        // 显示麥克風+标签
        public void ShowMicUpLabel()
        {
            micUpLabel.Visible = true;
            micUpTimer.Start();
        }

        // 隐藏麥克風+标签
        public void HideMicUpLabel()
        {
            micUpLabel.Visible = false;
        }

        // 显示麥克風-标签
        public void ShowMicDownLabel()
        {
            micDownLabel.Visible = true;
            micDownTimer.Start();
        }

        // 隐藏麥克風-标签
        public void HideMicDownLabel()
        {
            micDownLabel.Visible = false;
        }

        // 显示標準調标签
        public void ShowStandardKeyLabel()
        {
            standardKeyLabel.Visible = true;
            standardKeyTimer.Start();
        }

        // 隐藏標準調标签
        public void HideStandardKeyLabel()
        {
            standardKeyLabel.Visible = false;
            standardKeyTimer.Stop();
        }

        // 显示升調#标签
        public void ShowKeyUpLabel()
        {
            keyUpLabel.Visible = true;
            keyUpTimer.Start();
        }

        // 隐藏升調#标签
        public void HideKeyUpLabel()
        {
            keyUpLabel.Visible = false;
            keyUpTimer.Stop();
        }

        // 显示降調标签
        public void ShowKeyDownLabel()
        {
            keyDownLabel.Visible = true;
            keyDownTimer.Start();
        }

        // 隐藏降調标签
        public void HideKeyDownLabel()
        {
            keyDownLabel.Visible = false;
            keyDownTimer.Stop();
        }

        // 隐藏所有标签
        public void HideAllLabels()
        {
            // 隐藏女调标签
            femaleKeyLabel.Visible = false;
            femaleKeyTimer.Stop();

            // 隐藏暂停标签
            pauseLabel.Visible = false;

            // 隐藏静音标签
            muteLabel.Visible = false;

            // 隐藏音量+标签
            volumeUpLabel.Visible = false;
            volumeUpTimer.Stop();

            // 隐藏音量-标签
            volumeDownLabel.Visible = false;
            volumeDownTimer.Stop();

            // 隐藏麦克风+标签
            micUpLabel.Visible = false;

            // 隐藏麦克风-标签
            micDownLabel.Visible = false;

            // 隐藏标准调标签
            standardKeyLabel.Visible = false;
            standardKeyTimer.Stop();

            // 隐藏升调#标签
            keyUpLabel.Visible = false;
            keyUpTimer.Stop();

            // 隐藏降调标签
            keyDownLabel.Visible = false;
            keyDownTimer.Stop();
        }
    }
}