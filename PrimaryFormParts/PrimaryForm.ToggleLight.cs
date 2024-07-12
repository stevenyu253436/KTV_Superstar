using System;
using System.IO;
using System.Windows.Forms; // 添加這行來引入需要的命名空間
using System.Drawing; // 添加這行來引入需要的命名空間

namespace DualScreenDemo
{
    public partial class PrimaryForm
    {
        private Button btnTurnOn;
        private Button btnTurnOff;
        private Button btnBright;
        private Button btnRomantic;
        private Button btnAuto;
        private Button btnColorTuning;
        private Button btnSoft;
        private Button btnDynamic;
        private Button btnDeskLamp;
        private Button btnStageLight;
        private Button btnShelfLight;
        private Button btnWallLight;
        private Button btnBrightnessUp1;
        private Button btnBrightnessDown1;
        private Button btnBrightnessUp2;
        private Button btnBrightnessDown2;

        private PictureBox pictureBoxToggleLight;

        private void InitializeButtonsForPictureBoxToggleLight()
        {
            btnTurnOn = new Button{ Text = "" };
            // Set the button position and size
            // ResizeAndPositionButton(btnTurnOn, 604, 410, 122, 62);
            ConfigureButton(btnTurnOn, 604, 410, 122, 62, 
                resizedNormalStateImageForLightControl, resizedNormalStateImageForLightControl, resizedNormalStateImageForLightControl, 
                null);
            btnTurnOn.Click += (sender, e) =>
            {
                // Check if the serial port is open before trying to send data
                if (SerialPortManager.mySerialPort != null && SerialPortManager.mySerialPort.IsOpen)
                {
                    // Create a byte array with the commands to send
                    byte[] commandBytes = new byte[] { 0xA2, 0xDB, 0xA4 };
                    
                    // Send the bytes to the serial port
                    SerialPortManager.mySerialPort.Write(commandBytes, 0, commandBytes.Length);
                }
                else
                {
                    MessageBox.Show("Serial port is not open. Cannot send track correction command.");
                }
            };
            btnTurnOff = new Button{ Text = "" };
            // Set the button position and size
            // ResizeAndPositionButton(btnTurnOff, 753, 411, 122, 62);
            ConfigureButton(btnTurnOff, 753, 411, 122, 62, 
                resizedNormalStateImageForLightControl, resizedNormalStateImageForLightControl, resizedNormalStateImageForLightControl, 
                null);
            btnTurnOff.Click += (sender, e) =>
            {
                SendCommandThroughSerialPort("a2 dc a4"); // Command to turn off
            };
            // btnTurnOff.Visible = true; // Initially, the
            btnBright = new Button{ Text = "" };
            // Set the button position and size
            // ResizeAndPositionButton(btnBright, 901, 411, 122, 62);
            ConfigureButton(btnBright, 901, 411, 122, 62, 
                resizedNormalStateImageForLightControl, resizedNormalStateImageForLightControl, resizedNormalStateImageForLightControl, 
                null);
            btnBright.Click += (sender, e) => 
            {
                // Check if the serial port is open before trying to send data
                if (SerialPortManager.mySerialPort != null && SerialPortManager.mySerialPort.IsOpen)
                {
                    // Create a byte array with the commands to send
                    byte[] commandBytes = new byte[] { 0xA2, 0xD5, 0xA4 };
                    
                    // Send the bytes to the serial port
                    SerialPortManager.mySerialPort.Write(commandBytes, 0, commandBytes.Length);
                }
                else
                {
                    MessageBox.Show("Serial port is not open. Cannot send track correction command.");
                }
            };
            // SendCommandThroughSerialPort("a2 d5 a4");
            btnRomantic = new Button{ Text = "" };
            // Set the button position and size
            // ResizeAndPositionButton(btnRomantic, 1049, 411, 122, 62);
            ConfigureButton(btnRomantic, 1049, 411, 122, 62, 
                resizedNormalStateImageForLightControl, resizedNormalStateImageForLightControl, resizedNormalStateImageForLightControl, 
                (sender, e) => SendCommandThroughSerialPort("a2 d7 a4"));
            // btnRomantic.Click += (sender, e) => SendCommandThroughSerialPort("a2 d7 a4");
            btnAuto = new Button{ Text = "" };
            // Set the button position and size
            // ResizeAndPositionButton(btnAuto, 1049, 494, 123, 63);
            ConfigureButton(btnAuto, 1049, 494, 123, 63, 
                resizedNormalStateImageForLightControl, resizedNormalStateImageForLightControl, resizedNormalStateImageForLightControl, 
                null);
            btnColorTuning = new Button{ Text = "" };
            // Set the button position and size
            // ResizeAndPositionButton(btnColorTuning, 1049, 579, 123, 63);
            ConfigureButton(btnColorTuning, 1049, 579, 123, 63, 
                resizedNormalStateImageForLightControl, resizedNormalStateImageForLightControl, resizedNormalStateImageForLightControl, 
                (sender, e) => SendCommandThroughSerialPort("a2 75 a4"));
            // btnColorTuning.Click += (sender, e) => SendCommandThroughSerialPort("a2 75 a4");
            btnSoft = new Button{ Text = "" };
            // Set the button position and size
            // ResizeAndPositionButton(btnSoft, 901, 495, 122, 62);
            ConfigureButton(btnSoft, 901, 495, 122, 62, 
                resizedNormalStateImageForLightControl, resizedNormalStateImageForLightControl, resizedNormalStateImageForLightControl, 
                (sender, e) => SendCommandThroughSerialPort("a2 d6 a4"));
            // btnSoft.Click += (sender, e) => SendCommandThroughSerialPort("a2 d6 a4");
            btnDynamic = new Button{ Text = "" };
            // Set the button position and size
            // ResizeAndPositionButton(btnDynamic, 901, 579, 123, 62);
            ConfigureButton(btnDynamic, 901, 579, 123, 62, 
                resizedNormalStateImageForLightControl, resizedNormalStateImageForLightControl, resizedNormalStateImageForLightControl, 
                (sender, e) => SendCommandThroughSerialPort("a2 d8 a4"));
            // btnDynamic.Click += (sender, e) => SendCommandThroughSerialPort("a2 d8 a4");
            btnDeskLamp = new Button{ Text = "" };
            // Set the button position and size
            // ResizeAndPositionButton(btnDeskLamp, 1048, 662, 124, 64);
            ConfigureButton(btnDeskLamp, 1048, 662, 124, 64, 
                resizedNormalStateImageForLightControl, resizedNormalStateImageForLightControl, resizedNormalStateImageForLightControl, 
                (sender, e) => SendCommandThroughSerialPort("a2 fb a4"));
            // btnDeskLamp.Click += (sender, e) => SendCommandThroughSerialPort("a2 fb a4");
            btnStageLight = new Button{ Text = "" };
            // Set the button position and size
            // ResizeAndPositionButton(btnStageLight, 900, 662, 124, 64);
            ConfigureButton(btnStageLight, 900, 662, 124, 64, 
                resizedNormalStateImageForLightControl, resizedNormalStateImageForLightControl, resizedNormalStateImageForLightControl, 
                (sender, e) => SendCommandThroughSerialPort("a2 fa a4"));
            // btnStageLight.Click += (sender, e) => SendCommandThroughSerialPort("a2 fa a4");
            btnShelfLight = new Button{ Text = "" };
            // Set the button position and size
            // ResizeAndPositionButton(btnShelfLight, 752, 662, 124, 64);
            ConfigureButton(btnShelfLight, 752, 662, 124, 64, 
                resizedNormalStateImageForLightControl, resizedNormalStateImageForLightControl, resizedNormalStateImageForLightControl, 
                (sender, e) => SendCommandThroughSerialPort("a2 f9 a4"));
            // btnShelfLight.Click += (sender, e) => SendCommandThroughSerialPort("a2 f9 a4");
            btnWallLight = new Button{ Text = "" };
            // Set the button position and size
            // ResizeAndPositionButton(btnWallLight, 604, 662, 124, 64);
            ConfigureButton(btnWallLight, 604, 662, 124, 64, 
                resizedNormalStateImageForLightControl, resizedNormalStateImageForLightControl, resizedNormalStateImageForLightControl, 
                (sender, e) => SendCommandThroughSerialPort("a2 f8 a4"));
            // btnWallLight.Click += (sender, e) => SendCommandThroughSerialPort("a2 f8 a4");
            btnBrightnessUp1 = new Button{ Text = "" };
            // Set the button position and size
            // ResizeAndPositionButton(btnBrightnessUp1, 603, 495, 122, 62);
            ConfigureButton(btnBrightnessUp1, 603, 495, 122, 62, 
                resizedNormalStateImageForLightControl, resizedNormalStateImageForLightControl, resizedNormalStateImageForLightControl, 
                null);
            // 設置 btnBrightnessUp1 的 MouseDown 事件
            btnBrightnessUp1.MouseDown += (sender, e) => 
            {
                lightControlTimer.Tag = "a2 d9 a4"; // 設定要發送的指令
                lightControlTimer.Start(); // 開始 Timer
            };

            // 設置 btnBrightnessUp1 的 MouseUp 事件
            btnBrightnessUp1.MouseUp += (sender, e) => 
            {
                lightControlTimer.Stop(); // 停止 Timer
            };
            btnBrightnessDown1 = new Button{ Text = "" };
            // Set the button position and size
            // ResizeAndPositionButton(btnBrightnessDown1, 605, 579, 122, 62);
            ConfigureButton(btnBrightnessDown1, 605, 579, 122, 62, 
                resizedNormalStateImageForLightControl, resizedNormalStateImageForLightControl, resizedNormalStateImageForLightControl, 
                null);
            btnBrightnessDown1.MouseDown += (sender, e) => { lightControlTimer.Tag = "a2 da a4"; lightControlTimer.Start(); };
            btnBrightnessDown1.MouseUp += (sender, e) => { lightControlTimer.Stop(); };
            btnBrightnessUp2 = new Button{ Text = "" };
            // Set the button position and size
            // ResizeAndPositionButton(btnBrightnessUp2, 753, 495, 123, 62);
            ConfigureButton(btnBrightnessUp2, 753, 495, 123, 62, 
                resizedNormalStateImageForLightControl, resizedNormalStateImageForLightControl, resizedNormalStateImageForLightControl, 
                null);
            btnBrightnessUp2.MouseDown += (sender, e) => { lightControlTimer.Tag = "a2 f6 a4"; lightControlTimer.Start(); };
            btnBrightnessUp2.MouseUp += (sender, e) => { lightControlTimer.Stop(); };
            btnBrightnessDown2 = new Button{ Text = "" };
            // Set the button position and size
            // ResizeAndPositionButton(btnBrightnessDown2, 753, 579, 123, 62);
            ConfigureButton(btnBrightnessDown2, 753, 579, 123, 62, 
                resizedNormalStateImageForLightControl, resizedNormalStateImageForLightControl, resizedNormalStateImageForLightControl, 
                null);
            btnBrightnessDown2.MouseDown += (sender, e) => { lightControlTimer.Tag = "a2 f7 a4"; lightControlTimer.Start(); };
            btnBrightnessDown2.MouseUp += (sender, e) => { lightControlTimer.Stop(); };

            // Add all buttons to PictureBoxToggleLight controls
            this.Controls.Add(btnTurnOn);
            this.Controls.Add(btnTurnOff);
            this.Controls.Add(btnBright);
            this.Controls.Add(btnRomantic);
            this.Controls.Add(btnAuto);
            this.Controls.Add(btnColorTuning);
            this.Controls.Add(btnSoft);
            this.Controls.Add(btnDynamic);
            this.Controls.Add(btnDeskLamp);
            this.Controls.Add(btnStageLight);
            this.Controls.Add(btnShelfLight);
            this.Controls.Add(btnWallLight);
            this.Controls.Add(btnBrightnessUp1);
            this.Controls.Add(btnBrightnessDown1);
            this.Controls.Add(btnBrightnessUp2);
            this.Controls.Add(btnBrightnessDown2);
        }

        // Event handler for the "暫停|播放" button click
        private void ToggleLightButton_Click(object sender, EventArgs e)
        {
            // 首先设置所有相关PictureBox和按钮的可见性为false
            SetHotSongButtonsVisibility(false);
            SetNewSongButtonsVisibility(false);
            SetSingerSearchButtonsVisibility(false);
            SetSongSearchButtonsVisibility(false);

            // 检查PictureBoxToggleLight是否已经可见
            if (!pictureBoxToggleLight.Visible)
            {
                // 如果不可见，显示图片并设置相关按钮和PictureBox为可见
                ShowImageOnPictureBoxToggleLight(Path.Combine(Application.StartupPath, @"themes\superstar\選單內介面_燈光控制.jpg"));
                SetPictureBoxToggleLightAndButtonsVisibility(true);
            }
            else
            {
                // 如果已经可见，则只需要切换其可见性（在这个例子里可能隐藏它）
                TogglePictureBoxToggleLightButtonsVisibility();
            }
            
            // 切换pictureBoxQRCode的可见性
            if (pictureBoxQRCode != null)
            {
                pictureBoxQRCode.Visible = false;
                closeQRCodeButton.Visible = false;
            }
        }

        private void ShowImageOnPictureBoxToggleLight(string imagePath)
        {
            // 加载原始图像
            Bitmap originalImage = new Bitmap(imagePath);

            // 定义裁剪区域
            Rectangle cropArea = new Rectangle(570, 359, 630, 379);

            // 裁剪图像
            Bitmap croppedImage = CropImage(originalImage, cropArea);

            // 设置裁剪后的图像为 PictureBox 的图像
            pictureBoxToggleLight.Image = croppedImage;
    
            // 调整 PictureBox 的大小以匹配裁剪后的图像大小并设置位置
            ResizeAndPositionPictureBox(pictureBoxToggleLight, cropArea.X, cropArea.Y, cropArea.Width, cropArea.Height);
            
            pictureBoxToggleLight.Visible = true;
        }

        private void TogglePictureBoxToggleLightButtonsVisibility()
        {
            // 选取pictureBox2Buttons数组中的任意一个按钮来检测可见状态
            bool areButtonsVisible = pictureBoxToggleLight.Visible; // 或其他相关按钮

            // 设置相反的可见性状态
            SetPictureBoxToggleLightAndButtonsVisibility(!areButtonsVisible);
        }

        private void SetPictureBoxToggleLightAndButtonsVisibility(bool isVisible)
        {
            // Set the visibility of PictureBoxToggleLight itself
            pictureBoxToggleLight.Visible = isVisible;

            // Set visibility of each control directly added to the form that is related to the PictureBoxToggleLight
            btnTurnOn.Visible = isVisible;
            btnTurnOff.Visible = isVisible;
            btnBright.Visible = isVisible;
            btnRomantic.Visible = isVisible;
            btnAuto.Visible = isVisible;
            btnColorTuning.Visible = isVisible;
            btnSoft.Visible = isVisible;
            btnDynamic.Visible = isVisible;
            btnDeskLamp.Visible = isVisible;
            btnStageLight.Visible = isVisible;
            btnShelfLight.Visible = isVisible;
            btnWallLight.Visible = isVisible;
            btnBrightnessUp1.Visible = isVisible;
            btnBrightnessDown1.Visible = isVisible;
            btnBrightnessUp2.Visible = isVisible;
            btnBrightnessDown2.Visible = isVisible;

            if (isVisible)
            {
                // Bring the PictureBox to the front
                pictureBoxToggleLight.BringToFront();

                // Also bring each button to the front if visible
                btnTurnOn.BringToFront();
                btnTurnOff.BringToFront();
                btnBright.BringToFront();
                btnRomantic.BringToFront();
                btnAuto.BringToFront();
                btnColorTuning.BringToFront();
                btnSoft.BringToFront();
                btnDynamic.BringToFront();
                btnDeskLamp.BringToFront();
                btnStageLight.BringToFront();
                btnShelfLight.BringToFront();
                btnWallLight.BringToFront();
                btnBrightnessUp1.BringToFront();
                btnBrightnessDown1.BringToFront();
                btnBrightnessUp2.BringToFront();
                btnBrightnessDown2.BringToFront();
            }
        }
    }
}