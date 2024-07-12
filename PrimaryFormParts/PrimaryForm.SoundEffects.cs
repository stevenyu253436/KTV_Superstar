using System;
using System.Drawing; // 添加這行來引入需要的命名空間
using System.IO; // 添加這行來引入需要的命名空間
using System.Windows.Forms; // 添加這行來引入需要的命名空間
using NAudio.Wave; // 添加這行來引入需要的命名空間
using WMPLib; // 添加這行來引入需要的命名空間
using System.Collections.Generic; // 添加这行来引入需要的命名空間

namespace DualScreenDemo
{
    public partial class PrimaryForm : Form
    {
        private WindowsMediaPlayer mediaPlayer;
        private IWavePlayer waveOut;
        private AudioFileReader audioFileReader;
        private PictureBox pictureBoxSceneSoundEffects;
        private Button constructionButton;
        private Button marketButton;
        private Button drivingButton;
        private Button airportButton;
        private Button officeButton;
        private Button closeButton;

        private void InitializeMediaPlayer()
        {
            mediaPlayer = new WindowsMediaPlayer();
        }

        private void InitializeSoundEffectButtons()
        {
            // 工地 button
            constructionButton = new Button
            {
                Name = "constructionButton",
            };
            ConfigureButton(constructionButton, 876, 494, 148, 64, 
                                        resizedNormalStateImageForSceneSoundEffects, resizedNormalStateImageForSceneSoundEffects, resizedNormalStateImageForSceneSoundEffects, 
                                        ConstructionButton_Click);
            this.Controls.Add(constructionButton);

            // 市場 button
            marketButton = new Button
            {
                Name = "marketButton",
            };
            ConfigureButton(marketButton, 1037, 495, 148, 63, 
                            resizedNormalStateImageForSceneSoundEffects, resizedNormalStateImageForSceneSoundEffects, resizedNormalStateImageForSceneSoundEffects, 
                            MarketButton_Click);
            this.Controls.Add(marketButton);

            // 開車 button
            drivingButton = new Button
            {
                Name = "drivingButton",
            };
            ConfigureButton(drivingButton, 876, 570, 148, 63, 
                            resizedNormalStateImageForSceneSoundEffects, resizedNormalStateImageForSceneSoundEffects, resizedNormalStateImageForSceneSoundEffects, 
                            DrivingButton_Click);
            this.Controls.Add(drivingButton);

            // 機場 button
            airportButton = new Button
            {
                Name = "airportButton",
            };
            ConfigureButton(airportButton, 1037, 570, 148, 63, 
                            resizedNormalStateImageForSceneSoundEffects, resizedNormalStateImageForSceneSoundEffects, resizedNormalStateImageForSceneSoundEffects, 
                            AirportButton_Click);
            this.Controls.Add(airportButton);

            // 辦公室 button
            officeButton = new Button
            {
                Name = "officeButton",
            };
            ConfigureButton(officeButton, 876, 646, 148, 64, 
                            resizedNormalStateImageForSceneSoundEffects, resizedNormalStateImageForSceneSoundEffects, resizedNormalStateImageForSceneSoundEffects, 
                            OfficeButton_Click);
            this.Controls.Add(officeButton);

            // 關閉 button
            closeButton = new Button
            {
                Name = "closeButton",
            };
            
            ConfigureButton(closeButton, 1036, 646, 150, 63, 
                            resizedNormalStateImageForSceneSoundEffects, resizedNormalStateImageForSceneSoundEffects, resizedNormalStateImageForSceneSoundEffects, 
                            CloseButton_Click);
            this.Controls.Add(closeButton);
        }

        private void SoundEffectButton_Click(object sender, EventArgs e)
        {
            // 首先设置所有相关PictureBox和按钮的可见性为false
            SetHotSongButtonsVisibility(false);
            SetNewSongButtonsVisibility(false);
            SetSingerSearchButtonsVisibility(false);
            SetSongSearchButtonsVisibility(false);

            if (!pictureBoxSceneSoundEffects.Visible)
            {
                ShowImageOnPictureBoxSceneSoundEffects(Path.Combine(Application.StartupPath, @"themes\superstar\555022.jpg"));
                SetPictureBoxSceneSoundEffectsAndButtonsVisibility(true);
            }
            else
            {
                TogglePictureBoxSceneSoundEffectsButtonsVisibility();
            }
        }
        
        private void ConstructionButton_Click(object sender, EventArgs e) => PlaySound(@"sounds\1857.mp3");
        private void MarketButton_Click(object sender, EventArgs e) => PlaySound(@"sounds\13472_Audio Trimmer.mp3");
        private void DrivingButton_Click(object sender, EventArgs e) => PlaySound(@"sounds\kc1.mp3");
        private void AirportButton_Click(object sender, EventArgs e) => PlayMediaSound(@"sounds\xm2401.m4a");
        private void OfficeButton_Click(object sender, EventArgs e) => PlayMediaSound(@"sounds\y1640.m4a");
        private void CloseButton_Click(object sender, EventArgs e) => TogglePictureBoxSceneSoundEffectsButtonsVisibility();

        private void PlaySound(string filePath)
        {
            waveOut?.Dispose();
            audioFileReader?.Dispose();

            waveOut = new WaveOutEvent();
            audioFileReader = new AudioFileReader(Path.Combine(Application.StartupPath, filePath));
            waveOut.Init(audioFileReader);
            waveOut.Play();
        }

        private void PlayMediaSound(string filePath)
        {
            mediaPlayer.URL = Path.Combine(Application.StartupPath, filePath);
            mediaPlayer.controls.play();
        }

        public void PlayApplauseSound()
        {
            mediaPlayer.URL = Path.Combine(Application.StartupPath, "zs.m4a");
            mediaPlayer.controls.play();
        }

        private void ShowImageOnPictureBoxSceneSoundEffects(string imagePath)
        {
            // 加载原始图像
            Bitmap originalImage = new Bitmap(imagePath);

            // 定义裁剪区域
            Rectangle cropArea = new Rectangle(859, 427, 342, 295);

            // 裁剪图像
            Bitmap croppedImage = CropImage(originalImage, cropArea);

            // 设置裁剪后的图像为 PictureBox 的图像
            pictureBoxSceneSoundEffects.Image = croppedImage;
    
            // 调整 PictureBox 的大小以匹配裁剪后的图像大小并设置位置
            ResizeAndPositionPictureBox(pictureBoxSceneSoundEffects, cropArea.X, cropArea.Y, cropArea.Width, cropArea.Height);
            
            pictureBoxSceneSoundEffects.Visible = true;
        }

        private void TogglePictureBoxSceneSoundEffectsButtonsVisibility()
        {
            // 选取pictureBox2Buttons数组中的任意一个按钮来检测可见状态
            bool areButtonsVisible = pictureBoxSceneSoundEffects.Visible; // 或其他相关按钮

            // 设置相反的可见性状态
            SetPictureBoxSceneSoundEffectsAndButtonsVisibility(!areButtonsVisible);
        }

        // Adjust visibility method
        private void SetPictureBoxSceneSoundEffectsAndButtonsVisibility(bool isVisible)
        {
            // Set the visibility of PictureBoxSceneSoundEffects itself
            pictureBoxSceneSoundEffects.Visible = isVisible;

            if (isVisible)
            {
                // Bring the PictureBox to the front
                pictureBoxSceneSoundEffects.BringToFront();
            }

            // List of buttons to control visibility
            List<Button> soundEffectButtons = new List<Button>
            {
                constructionButton,
                marketButton,
                drivingButton,
                airportButton,
                officeButton,
                closeButton
            };

            // Set visibility for each button and bring to front if visible
            foreach (Button button in soundEffectButtons)
            {
                button.Visible = isVisible;
                if (isVisible)
                {
                    button.BringToFront();
                }
            }
        }
    }
}