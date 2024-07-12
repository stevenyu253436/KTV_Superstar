using System;
using System.Windows.Forms;
using System.Drawing;
using System.IO; // Add this line
using System.Linq;

namespace DualScreenDemo
{
    public partial class PrimaryForm
    {
        private Button newSongAlertButton;
        // 图像变量
        private Bitmap newSongAlertNormalBackground; // 新歌快报按钮的正常背景图像
        private Bitmap newSongAlertActiveBackground; // 新歌快报按钮的激活背景图像

        private Button guoYuButtonNewSong;
        private Bitmap guoYuNewSongNormalBackground; // 新歌快报按钮的正常背景图像
        private Bitmap guoYuNewSongActiveBackground;
        private Button taiYuButtonNewSong;
        private Bitmap taiYuNewSongNormalBackground; // 新歌快报按钮的正常背景图像
        private Bitmap taiYuNewSongActiveBackground;
        private Button yueYuButtonNewSong;
        private Bitmap yueYuNewSongNormalBackground; // 新歌快报按钮的正常背景图像
        private Bitmap yueYuNewSongActiveBackground;
        private Button yingWenButtonNewSong;
        private Bitmap yingWenNewSongNormalBackground; // 新歌快报按钮的正常背景图像
        private Bitmap yingWenNewSongActiveBackground;
        private Button riYuButtonNewSong;
        private Bitmap riYuNewSongNormalBackground; // 新歌快报按钮的正常背景图像
        private Bitmap riYuNewSongActiveBackground;
        private Button hanYuButtonNewSong;
        private Bitmap hanYuNewSongNormalBackground; // 新歌快报按钮的正常背景图像
        private Bitmap hanYuNewSongActiveBackground;

        private void ToggleNewSongButtonsVisibility()
        {
            // 选取pictureBoxNewSongButtons数组中的任意一个按钮来检测可见状态
            bool areButtonsVisible = guoYuButtonNewSong.Visible; // 或其他相关按钮

            // 设置相反的可见性状态
            SetNewSongButtonsVisibility(!areButtonsVisible);
        }

        private void SetNewSongButtonsVisibility(bool isVisible)
        {
            // 设置 pictureBox2 的可见性
            // pictureBox2.Visible = isVisible;

            // 定义与 pictureBox2 相关的所有按钮数组
            Button[] pictureBox2Buttons = { guoYuButtonNewSong, taiYuButtonNewSong, yueYuButtonNewSong, yingWenButtonNewSong, riYuButtonNewSong, hanYuButtonNewSong };

            // 通过循环设置每个按钮的可见性和层级关系
            foreach (var button in pictureBox2Buttons)
            {
                button.Visible = isVisible;

                // 仅当使按钮可见时才调用 BringToFront()
                if (isVisible)
                {
                    button.BringToFront();
                }
            }
        }

        private void NewSongAlertButton_Click(object sender, EventArgs e)
        {
            // 更改新歌快报按钮背景为激活状态
            newSongAlertButton.BackgroundImage = newSongAlertActiveBackground;
            hotPlayButton.BackgroundImage = hotPlayNormalBackground;
            singerSearchButton.BackgroundImage = singerSearchNormalBackground;
            songSearchButton.BackgroundImage = songSearchNormalBackground;
            languageSearchButton.BackgroundImage = languageSearchNormalBackground;
            groupSearchButton.BackgroundImage = groupSearchNormalBackground;
            categorySearchButton.BackgroundImage = categorySearchNormalBackground;
            orderedSongsButton.BackgroundImage = orderedSongsNormalBackground;
            myFavoritesButton.BackgroundImage = myFavoritesNormalBackground;
            promotionsButton.BackgroundImage = promotionsNormalBackground;
            deliciousFoodButton.BackgroundImage = deliciousFoodNormalBackground;

            guoYuButtonNewSong.BackgroundImage = guoYuNewSongActiveBackground;
            taiYuButtonNewSong.BackgroundImage = taiYuNewSongNormalBackground;
            yueYuButtonNewSong.BackgroundImage = yueYuNewSongNormalBackground;
            yingWenButtonNewSong.BackgroundImage = yingWenNewSongNormalBackground;
            riYuButtonNewSong.BackgroundImage = riYuNewSongNormalBackground;
            hanYuButtonNewSong.BackgroundImage = hanYuNewSongNormalBackground;

            int songLimit = ReadNewSongLimit(); // Get the new song limit from the file

            guoYuSongs2 = allSongs.Where(song => song.Category == "國語")
                                .OrderByDescending(song => song.AddedTime) // 根据点播次数降序排列
                                .Take(songLimit) // Apply the limit from the settings file
                                .ToList();
            currentPage = 0; // 重置到第一页
            currentSongList = guoYuSongs2; // 更新当前显示的歌曲列表
            totalPages = (int)Math.Ceiling((double)guoYuSongs2.Count / itemsPerPage);

            // DisplaySongsOnPage(currentSongList, currentPage);
            multiPagePanel.currentPageIndex = 0;
            multiPagePanel.LoadSongs(currentSongList);

            // 显示第二个图片，并可能隐藏第一个图片
            SetHotSongButtonsVisibility(false);
            SetSingerSearchButtonsVisibility(false);
            SetSongSearchButtonsVisibility(false);
            SetPictureBoxLanguageButtonsVisibility(false);
            SetGroupButtonsVisibility(false);
            SetPictureBoxCategoryAndButtonsVisibility(false);
            SetZhuYinSingersAndButtonsVisibility(false);
            SetZhuYinSongsAndButtonsVisibility(false);
            SetEnglishSingersAndButtonsVisibility(false);
            SetEnglishSongsAndButtonsVisibility(false);
            SetPinYinSingersAndButtonsVisibility(false);
            SetPinYinSongsAndButtonsVisibility(false);
            SetPictureBoxToggleLightAndButtonsVisibility(false);
            SetPictureBoxSceneSoundEffectsAndButtonsVisibility(false);
            ToggleNewSongButtonsVisibility();

            // 切换pictureBoxQRCode的可见性
            if (pictureBoxQRCode != null)
            {
                pictureBoxQRCode.Visible = false;
                closeQRCodeButton.Visible = false;
            }
        }

        private void InitializeButtonsForNewSong()
        {
            // 初始化“国语”按钮
            // Initialize the "国语" button with custom images and event handlers
            guoYuButtonNewSong = new Button{ Text = "", Visible = false };
            // Set the button position and size
            ResizeAndPositionButton(guoYuButtonNewSong, 1214, 230, 209, 59);
            Rectangle guoYuNewSongButtonCropArea = new Rectangle(1214, 230, 209, 59); // 根据需要调整这些值
            guoYuNewSongNormalBackground = normalStateImageNewSongAlert.Clone(guoYuNewSongButtonCropArea, normalStateImageNewSongAlert.PixelFormat);
            guoYuNewSongActiveBackground = mouseDownImageNewSongAlert.Clone(guoYuNewSongButtonCropArea, mouseDownImageNewSongAlert.PixelFormat);
            guoYuButtonNewSong.BackgroundImage = guoYuNewSongNormalBackground;
            guoYuButtonNewSong.BackgroundImageLayout = ImageLayout.Stretch;
            guoYuButtonNewSong.FlatStyle = FlatStyle.Flat;
            guoYuButtonNewSong.FlatAppearance.BorderSize = 0; // Remove border
            // Event handler for the button click event
            guoYuButtonNewSong.Click += GuoYuButtonNewSong_Click;
            // Add the button to the form
            this.Controls.Add(guoYuButtonNewSong);

            // 台语按钮
            taiYuButtonNewSong = new Button { Text = "", Visible = false };
            // Set the button position and size
            ResizeAndPositionButton(taiYuButtonNewSong, 1214, 293, 209, 58);
            Rectangle taiYuNewSongButtonCropArea = new Rectangle(1214, 293, 209, 58); // 根据需要调整这些值
            taiYuNewSongNormalBackground = normalStateImageNewSongAlert.Clone(taiYuNewSongButtonCropArea, normalStateImageNewSongAlert.PixelFormat);
            taiYuNewSongActiveBackground = mouseDownImageNewSongAlert.Clone(taiYuNewSongButtonCropArea, mouseDownImageNewSongAlert.PixelFormat);
            taiYuButtonNewSong.BackgroundImage = taiYuNewSongNormalBackground;
            taiYuButtonNewSong.BackgroundImageLayout = ImageLayout.Stretch;
            taiYuButtonNewSong.FlatStyle = FlatStyle.Flat;
            taiYuButtonNewSong.FlatAppearance.BorderSize = 0; // Remove border
            // Event handler for the button click event
            taiYuButtonNewSong.Click += TaiYuButtonNewSong_Click;
            // Add the button to the form
            this.Controls.Add(taiYuButtonNewSong);

            // 粵語按钮
            yueYuButtonNewSong = new Button { Text = "", Visible = false };
            // Set the button position and size
            ResizeAndPositionButton(yueYuButtonNewSong, 1214, 356, 209, 58);
            Rectangle yueYuNewSongButtonCropArea = new Rectangle(1214, 356, 209, 58); // 根据需要调整这些值
            yueYuNewSongNormalBackground = normalStateImageNewSongAlert.Clone(yueYuNewSongButtonCropArea, normalStateImageNewSongAlert.PixelFormat);
            yueYuNewSongActiveBackground = mouseDownImageNewSongAlert.Clone(yueYuNewSongButtonCropArea, mouseDownImageNewSongAlert.PixelFormat);
            yueYuButtonNewSong.BackgroundImage = yueYuNewSongNormalBackground;
            yueYuButtonNewSong.BackgroundImageLayout = ImageLayout.Stretch;
            yueYuButtonNewSong.FlatStyle = FlatStyle.Flat;
            yueYuButtonNewSong.FlatAppearance.BorderSize = 0; // Remove border
            // Event handler for the button click event
            yueYuButtonNewSong.Click += YueYuButtonNewSong_Click;
            // Add the button to the form
            this.Controls.Add(yueYuButtonNewSong);

            // 英文按钮
            yingWenButtonNewSong = new Button { Text = "英文2", Visible = false };
            // Set the button position and size
            ResizeAndPositionButton(yingWenButtonNewSong, 1214, 418, 209, 59);
            Rectangle yingWenNewSongButtonCropArea = new Rectangle(1214, 418, 209, 59); // 根据需要调整这些值
            yingWenNewSongNormalBackground = normalStateImageNewSongAlert.Clone(yingWenNewSongButtonCropArea, normalStateImageNewSongAlert.PixelFormat);
            yingWenNewSongActiveBackground = mouseDownImageNewSongAlert.Clone(yingWenNewSongButtonCropArea, mouseDownImageNewSongAlert.PixelFormat);
            yingWenButtonNewSong.BackgroundImage = yingWenNewSongNormalBackground;
            yingWenButtonNewSong.BackgroundImageLayout = ImageLayout.Stretch;
            yingWenButtonNewSong.FlatStyle = FlatStyle.Flat;
            yingWenButtonNewSong.FlatAppearance.BorderSize = 0; // Remove border
            // Event handler for the button click event
            yingWenButtonNewSong.Click += YingWenButtonNewSong_Click;
            // Add the button to the form
            this.Controls.Add(yingWenButtonNewSong);

            // 日語按钮
            riYuButtonNewSong = new Button { Text = "日語2", Visible = false };
            // Set the button position and size
            ResizeAndPositionButton(riYuButtonNewSong, 1214, 481, 209, 59);
            Rectangle riYuNewSongButtonCropArea = new Rectangle(1214, 481, 209, 59); // 根据需要调整这些值
            riYuNewSongNormalBackground = normalStateImageNewSongAlert.Clone(riYuNewSongButtonCropArea, normalStateImageNewSongAlert.PixelFormat);
            riYuNewSongActiveBackground = mouseDownImageNewSongAlert.Clone(riYuNewSongButtonCropArea, mouseDownImageNewSongAlert.PixelFormat);
            riYuButtonNewSong.BackgroundImage = riYuNewSongNormalBackground;
            riYuButtonNewSong.BackgroundImageLayout = ImageLayout.Stretch;
            riYuButtonNewSong.FlatStyle = FlatStyle.Flat;
            riYuButtonNewSong.FlatAppearance.BorderSize = 0; // Remove border
            // Event handler for the button click event
            riYuButtonNewSong.Click += RiYuButtonNewSong_Click;
            // Add the button to the form
            this.Controls.Add(riYuButtonNewSong);

            // 韓語按钮
            hanYuButtonNewSong = new Button { Text = "韓語2", Visible = false };
            // Set the button position and size
            ResizeAndPositionButton(hanYuButtonNewSong, 1214, 544, 209, 58);
            Rectangle hanYuNewSongButtonCropArea = new Rectangle(1214, 544, 209, 58); // 根据需要调整这些值
            hanYuNewSongNormalBackground = normalStateImageNewSongAlert.Clone(hanYuNewSongButtonCropArea, normalStateImageNewSongAlert.PixelFormat);
            hanYuNewSongActiveBackground = mouseDownImageNewSongAlert.Clone(hanYuNewSongButtonCropArea, mouseDownImageNewSongAlert.PixelFormat);
            hanYuButtonNewSong.BackgroundImage = hanYuNewSongNormalBackground;
            hanYuButtonNewSong.BackgroundImageLayout = ImageLayout.Stretch;
            hanYuButtonNewSong.FlatStyle = FlatStyle.Flat;
            hanYuButtonNewSong.FlatAppearance.BorderSize = 0; // Remove border
            // Event handler for the button click event
            hanYuButtonNewSong.Click += HanYuButtonNewSong_Click;
            // Add the button to the form
            this.Controls.Add(hanYuButtonNewSong);
        }

        public static int ReadNewSongLimit()
        {
            string filePath = Path.Combine(Application.StartupPath, "SongLimitsSettings.txt"); // Path to your settings file
            try
            {
                // Read all lines from the file
                var lines = File.ReadAllLines(filePath);
                foreach (var line in lines)
                {
                    // Check if the line contains the setting for NewSongLimit
                    if (line.StartsWith("NewSongLimit:"))
                    {
                        string valuePart = line.Split(':')[1].Trim(); // Extract the number part
                        int limit; // Declare limit outside the tryParse
                        if (int.TryParse(valuePart, out limit))
                        {
                            return limit; // Return the parsed limit
                        }
                        break; // Stop processing once we find the NewSongLimit
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to read song limits from file: " + ex.Message);
                return 100; // Return default value in case of error
            }

            return 100; // Return default value if not found in file
        }
    }
}