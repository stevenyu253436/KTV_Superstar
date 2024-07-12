using System;
using System.Windows.Forms;
using System.Drawing;
using System.IO; // Add this line
using System.Linq;
using System.Collections.Generic; // For List<>

namespace DualScreenDemo
{
    public partial class PrimaryForm
    {
        private Button hotPlayButton;
        private Bitmap hotPlayNormalBackground; // 热门点播按钮的正常背景图像
        private Bitmap hotPlayActiveBackground; // 热门点播按钮的激活背景图像

        private Button guoYuButtonHotSong;
        private Bitmap guoYuHotSongNormalBackground;
        private Bitmap guoYuHotSongActiveBackground;
        private Button taiYuButtonHotSong;
        private Bitmap taiYuHotSongNormalBackground;
        private Bitmap taiYuHotSongActiveBackground;
        private Button yueYuButtonHotSong;
        private Bitmap yueYuHotSongNormalBackground;
        private Bitmap yueYuHotSongActiveBackground;
        private Button yingWenButtonHotSong;
        private Bitmap yingWenHotSongNormalBackground;
        private Bitmap yingWenHotSongActiveBackground;
        private Button riYuButtonHotSong;
        private Bitmap riYuHotSongNormalBackground;
        private Bitmap riYuHotSongActiveBackground;
        private Button hanYuButtonHotSong;
        private Bitmap hanYuHotSongNormalBackground;
        private Bitmap hanYuHotSongActiveBackground;

        private void SetHotSongButtonsVisibility(bool isVisible)
        {
            // 设置 pictureBox1 的可见性
            // pictureBox1.Visible = isVisible;

            // 定义与 pictureBox2 相关的所有按钮数组
            Button[] hotSongButtons = { guoYuButtonHotSong, taiYuButtonHotSong, yueYuButtonHotSong, yingWenButtonHotSong, riYuButtonHotSong, hanYuButtonHotSong };

            // 通过循环设置每个按钮的可见性和层级关系
            foreach (var button in hotSongButtons)
            {
                button.Visible = isVisible;

                // 仅当使按钮可见时才调用 BringToFront()
                if (isVisible)
                {
                    button.BringToFront();
                }
            }
        }

        private void HotPlayButton_Click(object sender, EventArgs e)
        {
            UpdateButtonBackgrounds(hotPlayButton, hotPlayActiveBackground);
            UpdateHotSongButtons(guoYuButtonHotSong, guoYuHotSongActiveBackground);

            int songLimit = ReadHotSongLimit(); // Get the hot song limit from the file

            guoYuSongs = GetSongsByCategory("國語", songLimit);
            UpdateSongList(guoYuSongs);

            SetButtonsVisibility();
            HideQRCode();
        }

        private void UpdateButtonBackgrounds(Button activeButton, Image activeBackground)
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
            deliciousFoodButton.BackgroundImage = deliciousFoodNormalBackground;

            activeButton.BackgroundImage = activeBackground;
        }

        private void OnHotSongButtonClick(Button activeButton, Bitmap activeBackground, string category)
        {
            UpdateHotSongButtons(activeButton, activeBackground);

            int songLimit = ReadHotSongLimit();

            var selectedSongs = allSongs.Where(song => song.Category == category)
                                        .OrderByDescending(song => song.Plays)
                                        .Take(songLimit)
                                        .ToList();

            UpdateSongList(selectedSongs);
        }

        private void UpdateHotSongButtons(Button activeButton, Image activeBackground)
        {
            guoYuButtonHotSong.BackgroundImage = guoYuHotSongNormalBackground;
            taiYuButtonHotSong.BackgroundImage = taiYuHotSongNormalBackground;
            yueYuButtonHotSong.BackgroundImage = yueYuHotSongNormalBackground;
            yingWenButtonHotSong.BackgroundImage = yingWenHotSongNormalBackground;
            riYuButtonHotSong.BackgroundImage = riYuHotSongNormalBackground;
            hanYuButtonHotSong.BackgroundImage = hanYuHotSongNormalBackground;

            activeButton.BackgroundImage = activeBackground;
        }

        private List<SongData> GetSongsByCategory(string category, int limit)
        {
            return allSongs.Where(song => song.Category == category)
                        .OrderByDescending(song => song.Plays) // 根据点播次数降序排列
                        .Take(limit) // Apply the limit from the settings file
                        .ToList();
        }

        private void UpdateSongList(List<SongData> songs)
        {
            currentPage = 0; // 重置到第一页
            currentSongList = songs; // 更新当前显示的歌曲列表
            totalPages = (int)Math.Ceiling((double)songs.Count / itemsPerPage);

            // DisplaySongsOnPage(currentSongList, currentPage);
            multiPagePanel.currentPageIndex = 0;
            multiPagePanel.LoadSongs(currentSongList);
        }

        private void SetButtonsVisibility()
        {
            SetNewSongButtonsVisibility(false);
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
            SetHotSongButtonsVisibility(true);
        }

        private void HideQRCode()
        {
            if (pictureBoxQRCode != null)
            {
                pictureBoxQRCode.Visible = false;
                closeQRCodeButton.Visible = false;
            }
        }

        private void InitializeButtonsForHotSong()
        {
            InitializeHotSongButton(ref guoYuButtonHotSong, "國語", 1214, 230, 209, 59, 
                                    normalStateImageNewSongAlert, out guoYuHotSongNormalBackground, 
                                    mouseDownImageNewSongAlert, out guoYuHotSongActiveBackground, 
                                    GuoYuButtonHotSong_Click);

            InitializeHotSongButton(ref taiYuButtonHotSong, "台語", 1214, 293, 209, 58, 
                                    normalStateImageNewSongAlert, out taiYuHotSongNormalBackground, 
                                    mouseDownImageNewSongAlert, out taiYuHotSongActiveBackground, 
                                    TaiYuButtonHotSong_Click);

            InitializeHotSongButton(ref yueYuButtonHotSong, "粵語", 1214, 356, 209, 58, 
                                    normalStateImageNewSongAlert, out yueYuHotSongNormalBackground, 
                                    mouseDownImageNewSongAlert, out yueYuHotSongActiveBackground, 
                                    YueYuButtonHotSong_Click);

            InitializeHotSongButton(ref yingWenButtonHotSong, "英文", 1214, 418, 209, 59, 
                                    normalStateImageNewSongAlert, out yingWenHotSongNormalBackground, 
                                    mouseDownImageNewSongAlert, out yingWenHotSongActiveBackground, 
                                    YingWenButtonHotSong_Click);

            InitializeHotSongButton(ref riYuButtonHotSong, "日語", 1214, 481, 209, 59, 
                                    normalStateImageNewSongAlert, out riYuHotSongNormalBackground, 
                                    mouseDownImageNewSongAlert, out riYuHotSongActiveBackground, 
                                    RiYuButtonHotSong_Click);

            InitializeHotSongButton(ref hanYuButtonHotSong, "韓語", 1214, 544, 209, 58, 
                                    normalStateImageNewSongAlert, out hanYuHotSongNormalBackground, 
                                    mouseDownImageNewSongAlert, out hanYuHotSongActiveBackground, 
                                    HanYuButtonHotSong_Click);
        }

        private void InitializeHotSongButton(ref Button button, string buttonText, int x, int y, int width, int height, 
                                            Image normalBackground, out Bitmap normalBackgroundOut, 
                                            Image activeBackground, out Bitmap activeBackgroundOut, 
                                            EventHandler clickEventHandler)
        {
            button = new Button { Text = buttonText, Visible = false };
            ResizeAndPositionButton(button, x, y, width, height);
            normalBackgroundOut = new Bitmap(normalBackground).Clone(new Rectangle(x, y, width, height), normalBackground.PixelFormat);
            activeBackgroundOut = new Bitmap(activeBackground).Clone(new Rectangle(x, y, width, height), activeBackground.PixelFormat);
            button.BackgroundImage = normalBackgroundOut;
            button.BackgroundImageLayout = ImageLayout.Stretch;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0;
            button.Click += clickEventHandler;
            this.Controls.Add(button);
        }

        public static int ReadHotSongLimit()
        {
            string filePath = Path.Combine(Application.StartupPath, "SongLimitsSettings.txt"); // Path to your settings file
            try
            {
                // Read all lines from the file
                var lines = File.ReadAllLines(filePath);
                foreach (var line in lines)
                {
                    // Check if the line contains the setting for NewSongLimit
                    if (line.StartsWith("HotSongLimit:"))
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