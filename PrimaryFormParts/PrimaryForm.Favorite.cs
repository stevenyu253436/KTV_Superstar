using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic;

namespace DualScreenDemo
{
    public partial class PrimaryForm
    {
        private Button myFavoritesButton;
        private Bitmap myFavoritesNormalBackground;
        private Bitmap myFavoritesActiveBackground;

        private string mobileNumber = string.Empty;
        private bool isPhoneNumberValid;
        private bool showError = false;
        private PictureBox FavoritePictureBox;
        private Button[] favoriteNumberButton;
        private Button enterFavoriteButton;
        private Button newFavoriteButton;
        private Button refillFavoriteButton;
        private Button closeFavoriteButton;
        private Label errorMessageLabel;

        private void InitializeButtonsForFavoritePictureBox()
        {
            // Coordinates for the number buttons as per the provided mapping
            int[,] coords = new int[,]
            {
                {799, 508, 70, 65}, // 1
                {878, 508, 70, 65}, // 2
                {957, 508, 70, 65}, // 3
                {1036, 508, 70, 65}, // 4
                {1115, 508, 70, 65}, // 5
                {799, 580, 70, 65}, // 6
                {878, 580, 70, 65}, // 7
                {957, 580, 70, 65}, // 8
                {1036, 580, 70, 65}, // 9
                {1115, 580, 70, 65} // 0
            };

            int screenW = Screen.PrimaryScreen.Bounds.Width;
            int screenH = Screen.PrimaryScreen.Bounds.Height;

            // 假定1440是设计时的屏幕宽度，900是设计时的屏幕高度
            float widthRatio = screenW / (float)1440;
            float heightRatio = screenH / (float)900;

            favoriteNumberButton = new Button[10];

            for (int i = 0; i < favoriteNumberButton.Length; i++)
            {
                favoriteNumberButton[i] = new Button();

                ResizeAndPositionButton(favoriteNumberButton[i], coords[i, 0], coords[i, 1], coords[i, 2], coords[i, 3]);

                // 循环中使用的代码
                string fileName = (i + 2).ToString("00");  // 格式化数字为至少两位，不足前面补0
                string filePath = Path.Combine(Application.StartupPath, @"themes\superstar\我的最愛\我的最愛-" + fileName + ".jpg");
                favoriteNumberButton[i].BackgroundImage = Image.FromFile(filePath);
                favoriteNumberButton[i].BackgroundImageLayout = ImageLayout.Stretch;
                favoriteNumberButton[i].FlatStyle = FlatStyle.Flat;
                favoriteNumberButton[i].FlatAppearance.BorderSize = 0; // Remove border
                favoriteNumberButton[i].BackColor = Color.Transparent;
                favoriteNumberButton[i].FlatAppearance.MouseDownBackColor = Color.Transparent; // Mousedown background color
                favoriteNumberButton[i].FlatAppearance.MouseOverBackColor = Color.Transparent; // Mouseover background color

                // Set unique names for each number button
                favoriteNumberButton[i].Name = "favoriteNumberButton" + i;
                // Set button tag
                favoriteNumberButton[i].Tag = (i + 1).ToString();
                if (i == 9) // Adjust for 0 being the last button
                {
                    favoriteNumberButton[i].Name = "favoriteNumberButton0";
                    favoriteNumberButton[i].Tag = "0";
                }

                // Add event handler for clicks, assuming WordCountButton_Click is defined
                favoriteNumberButton[i].Click += FavoriteNumberButton_Click;

                // Add the button to the form's controls or to pictureBoxWordCount if it's being used as a container
                this.Controls.Add(favoriteNumberButton[i]);
            }

            // 进入按钮
            enterFavoriteButton = new Button()
            {
                Name = "enterFavoriteButton"
            };
            ResizeAndPositionButton(enterFavoriteButton, 842, 652, 70, 65);
            enterFavoriteButton.BackgroundImage = Image.FromFile(Path.Combine(Application.StartupPath, @"themes\superstar\我的最愛\我的最愛-12.jpg"));
            enterFavoriteButton.BackgroundImageLayout = ImageLayout.Stretch;
            enterFavoriteButton.FlatStyle = FlatStyle.Flat;
            enterFavoriteButton.FlatAppearance.BorderSize = 0; // Remove border
            enterFavoriteButton.BackColor = Color.Transparent;
            enterFavoriteButton.FlatAppearance.MouseDownBackColor = Color.Transparent; // Mousedown background color
            enterFavoriteButton.FlatAppearance.MouseOverBackColor = Color.Transparent; // Mouseover background color
            enterFavoriteButton.Click += EnterFavoriteButton_Click; // 绑定事件处理器

            // 新建按钮
            newFavoriteButton = new Button()
            {
                Name = "newFavoriteButton"
            };
            ResizeAndPositionButton(newFavoriteButton, 921, 652, 70, 65);
            newFavoriteButton.BackgroundImage = Image.FromFile(Path.Combine(Application.StartupPath, @"themes\superstar\我的最愛\我的最愛-13.jpg"));
            newFavoriteButton.BackgroundImageLayout = ImageLayout.Stretch;
            newFavoriteButton.FlatStyle = FlatStyle.Flat;
            newFavoriteButton.FlatAppearance.BorderSize = 0; // Remove border
            newFavoriteButton.BackColor = Color.Transparent;
            newFavoriteButton.FlatAppearance.MouseDownBackColor = Color.Transparent; // Mousedown background color
            newFavoriteButton.FlatAppearance.MouseOverBackColor = Color.Transparent; // Mouseover background color
            newFavoriteButton.Click += NewFavoriteButton_Click;

            // 重填按钮
            refillFavoriteButton = new Button()
            {
                Name = "refillFavoriteButton"
            };
            ResizeAndPositionButton(refillFavoriteButton, 999, 652, 70, 65);
            refillFavoriteButton.BackgroundImage = Image.FromFile(Path.Combine(Application.StartupPath, @"themes\superstar\我的最愛\我的最愛-14.jpg"));
            refillFavoriteButton.BackgroundImageLayout = ImageLayout.Stretch;
            refillFavoriteButton.FlatStyle = FlatStyle.Flat;
            refillFavoriteButton.FlatAppearance.BorderSize = 0; // Remove border
            refillFavoriteButton.BackColor = Color.Transparent;
            refillFavoriteButton.FlatAppearance.MouseDownBackColor = Color.Transparent; // Mousedown background color
            refillFavoriteButton.FlatAppearance.MouseOverBackColor = Color.Transparent; // Mouseover background color
            refillFavoriteButton.Click += RefillFavoriteButton_Click;

            // 关闭按钮
            closeFavoriteButton = new Button()
            {
                Name = "closeFavoriteButton"
            };
            ResizeAndPositionButton(closeFavoriteButton, 1078, 652, 70, 65);
            closeFavoriteButton.BackgroundImage = Image.FromFile(Path.Combine(Application.StartupPath, @"themes\superstar\我的最愛\我的最愛-15.jpg"));
            closeFavoriteButton.BackgroundImageLayout = ImageLayout.Stretch;
            closeFavoriteButton.FlatStyle = FlatStyle.Flat;
            closeFavoriteButton.FlatAppearance.BorderSize = 0; // Remove border
            closeFavoriteButton.BackColor = Color.Transparent;
            closeFavoriteButton.FlatAppearance.MouseDownBackColor = Color.Transparent; // Mousedown background color
            closeFavoriteButton.FlatAppearance.MouseOverBackColor = Color.Transparent; // Mouseover background color
            closeFavoriteButton.Click += CloseFavoriteButton_Click;

            // Initialize and set properties for the error message label
            errorMessageLabel = new Label
            {
                Text = "",
                ForeColor = Color.Black,
                Location = new Point(10, 250),  // Adjust location as needed
                AutoSize = true
            };

            // 添加按钮到控件集合
            this.Controls.Add(enterFavoriteButton);
            this.Controls.Add(newFavoriteButton);
            this.Controls.Add(refillFavoriteButton);
            this.Controls.Add(closeFavoriteButton);
            this.Controls.Add(errorMessageLabel);
        }
        
        private void FavoriteNumberButton_Click(object sender, EventArgs e)
        {
            Button clickedButton = sender as Button;
            if (clickedButton != null)
            {
                // Append the digit to the mobile number
                mobileNumber += clickedButton.Tag.ToString();
                Console.WriteLine("Number button clicked: " + clickedButton.Tag.ToString());
                
                // Invalidate the PictureBox to trigger a repaint
                FavoritePictureBox.Invalidate();
            }
        }
        
        private void FavoritePictureBox_Paint(object sender, PaintEventArgs e)
        {
            if (!string.IsNullOrEmpty(mobileNumber))
            {
                using (Font font = new Font("Arial", 24))
                using (Brush brush = new SolidBrush(Color.Black))
                {
                    int x = 16;
                    int y = 68;

                    if (showError)
                    {
                        string errorMessage;
                        if (!isPhoneNumberValid)
                        {
                            errorMessage = "查無此手機號碼!!!";
                        }
                        else
                        {
                            errorMessage = "手機號碼輸入錯誤!!!";
                        }
                        e.Graphics.DrawString(errorMessage, font, brush, x, y);
                    }
                    else
                    {
                        e.Graphics.DrawString(mobileNumber, font, brush, x, y);
                    }
                }
            }
        }
        
        // 进入按钮点击事件处理函数
        private void EnterFavoriteButton_Click(object sender, EventArgs e)
        {
            if (mobileNumber.StartsWith("09") && mobileNumber.Length == 10)
            {
                if (SongListManager.Instance.CheckIfPhoneNumberExists(mobileNumber))
                {
                    isPhoneNumberValid = true;
                    SongListManager.Instance.UserLogin(mobileNumber);
                    // Add logic to display favorite songs if needed
                    ToggleFavoritePictureBoxButtonsVisibility();
                }
                else
                {
                    isPhoneNumberValid = false;
                    showError = true; // Set the error flag
                    FavoritePictureBox.Invalidate();
                    FavoritePictureBox.Refresh(); // Force the control to redraw
                }
            }
            else
            {
                showError = true; // Set the error flag
                isPhoneNumberValid = true;
                FavoritePictureBox.Invalidate(); // Trigger the PictureBox to repaint
                FavoritePictureBox.Refresh(); // Force the control to redraw
            }
        }
        
        // 新建按钮点击事件处理函数
        private void NewFavoriteButton_Click(object sender, EventArgs e)
        {
            if (mobileNumber.StartsWith("09") && mobileNumber.Length == 10)
            {
                if (SongListManager.Instance.CheckIfPhoneNumberExists(mobileNumber))
                {
                    isPhoneNumberValid = true;
                    SongListManager.Instance.UserLogin(mobileNumber);
                    // Add logic to display favorite songs if needed
                    ToggleFavoritePictureBoxButtonsVisibility();
                }
                else
                {
                    isPhoneNumberValid = true;
                    SongListManager.Instance.AddNewUser(mobileNumber);
                    SongListManager.Instance.UserLogin(mobileNumber);
                    // Add logic to display favorite songs if needed
                    // Create a list with a single empty SongData element
                    List<SongData> emptySongList = new List<SongData> { new SongData("", "", "歡迎光臨 " + "(" + mobileNumber + ")", 0, "", "", "", "", DateTime.Now, "", "", "", "", "", "", "", "", "", "", "", "", 1) };
                    multiPagePanel.currentPageIndex = 0;
                    multiPagePanel.LoadSongs(emptySongList);
                    ToggleFavoritePictureBoxButtonsVisibility();
                }
            }
            else
            {
                showError = true; // Set the error flag
                isPhoneNumberValid = true;
                FavoritePictureBox.Invalidate(); // Trigger the PictureBox to repaint
                FavoritePictureBox.Refresh();
            }
        }
        
        // 重填按钮点击事件处理函数
        private void RefillFavoriteButton_Click(object sender, EventArgs e)
        {
            // 清空 mobileNumber 字符串
            mobileNumber = string.Empty;

            // 重置 showError 标志
            showError = false;

            // 使 FavoritePictureBox 无效并重绘
            FavoritePictureBox.Invalidate();
            FavoritePictureBox.Refresh();

            SongListManager.Instance.IsUserLoggedIn = false;
            SongListManager.Instance.UserPhoneNumber = string.Empty;
        }
        
        // 关闭按钮点击事件处理函数
        private void CloseFavoriteButton_Click(object sender, EventArgs e)
        {
            // 执行关闭我的最爱界面的逻辑
            ToggleFavoritePictureBoxButtonsVisibility();
        }
        
        private void MyFavoritesButton_Click(object sender, EventArgs e)
        {
            newSongAlertButton.BackgroundImage = newSongAlertNormalBackground;
            hotPlayButton.BackgroundImage = hotPlayNormalBackground;
            singerSearchButton.BackgroundImage = singerSearchNormalBackground;
            songSearchButton.BackgroundImage = songSearchNormalBackground;
            languageSearchButton.BackgroundImage = languageSearchNormalBackground;
            groupSearchButton.BackgroundImage = groupSearchNormalBackground;
            categorySearchButton.BackgroundImage = categorySearchNormalBackground;
            orderedSongsButton.BackgroundImage = orderedSongsNormalBackground;
            myFavoritesButton.BackgroundImage = myFavoritesActiveBackground;
            promotionsButton.BackgroundImage = promotionsNormalBackground;
            deliciousFoodButton.BackgroundImage = deliciousFoodNormalBackground;

            // 检查PictureBoxToggleLight是否已经可见
            if (!FavoritePictureBox.Visible)
            {
                // 如果不可见，显示图片并设置相关按钮和PictureBox为可见
                ShowImageOnFavoritePictureBox(Path.Combine(Application.StartupPath, @"themes\superstar\其他介面\其他_我的最愛.jpg"));
                SetFavoritePictureBoxAndButtonsVisibility(true);
            }
            else
            {
                // 如果已经可见，则只需要切换其可见性（在这个例子里可能隐藏它）
                ToggleFavoritePictureBoxButtonsVisibility();
            }

            // 切换pictureBoxQRCode的可见性
            if (pictureBoxQRCode != null)
            {
                pictureBoxQRCode.Visible = false;
                closeQRCodeButton.Visible = false;
            }

            SetPictureBoxToggleLightAndButtonsVisibility(false);
            SetPictureBoxSceneSoundEffectsAndButtonsVisibility(false);
        }
        
        private void ShowImageOnFavoritePictureBox(string imagePath)
        {
            // 加载原始图像
            Bitmap originalImage = new Bitmap(imagePath);

            // 打印原始图像的尺寸
            Console.WriteLine(String.Format("Original Image Size: {0}x{1}", originalImage.Width, originalImage.Height));

            // 定义裁剪区域
            Rectangle cropArea = new Rectangle(784, 393, 555, 442);

            // 裁剪图像
            Bitmap croppedImage = CropImage(originalImage, cropArea);

            // 设置裁剪后的图像为 PictureBox 的图像
            FavoritePictureBox.Image = croppedImage;
    
            // 调整 PictureBox 的大小以匹配裁剪后的图像大小并设置位置
            ResizeAndPositionPictureBox(FavoritePictureBox, cropArea.X, cropArea.Y, 416, 323);
            
            FavoritePictureBox.Visible = true;
        }

        private void ToggleFavoritePictureBoxButtonsVisibility()
        {
            // 选取pictureBox2Buttons数组中的任意一个按钮来检测可见状态
            bool areButtonsVisible = FavoritePictureBox.Visible; // 或其他相关按钮

            // 设置相反的可见性状态
            SetFavoritePictureBoxAndButtonsVisibility(!areButtonsVisible);
        }

        private void SetFavoritePictureBoxAndButtonsVisibility(bool isVisible)
        {
            System.Action action = () =>
            {
                SuspendLayout();

                // Set the visibility of FavoritePictureBox
                FavoritePictureBox.Visible = isVisible;

                // Ensure FavoritePictureBox is brought to the front if it is made visible
                if (isVisible) FavoritePictureBox.BringToFront();

                // Set visibility for modify and close buttons
                enterFavoriteButton.Visible = isVisible;
                newFavoriteButton.Visible = isVisible;
                refillFavoriteButton.Visible = isVisible;
                closeFavoriteButton.Visible = isVisible;

                // If visible, make sure these buttons are also brought to the front
                if (isVisible)
                {
                    enterFavoriteButton.BringToFront();
                    newFavoriteButton.BringToFront();
                    refillFavoriteButton.BringToFront();
                    closeFavoriteButton.BringToFront();
                }

                // Iterate through all number buttons and set their visibility
                foreach (Button button in favoriteNumberButton)
                {
                    button.Visible = isVisible;
                    // Optionally, ensure that the button is brought to the front if it becomes visible.
                    if (isVisible)
                        button.BringToFront();
                }

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
    }
}