using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO; // 添加這行

namespace DualScreenDemo
{
    public partial class PrimaryForm
    {
        private Button categorySearchButton;
        private Bitmap categorySearchNormalBackground; // 類別搜索按钮的正常背景图像
        private Bitmap categorySearchActiveBackground; // 類別搜索按钮的激活背景图像

        private Button loveDuetButton;
        private Bitmap loveDuetNormalBackground;
        private Bitmap loveDuetActiveBackground;
        private Button talentShowButton;
        private Bitmap talentShowNormalBackground;
        private Bitmap talentShowActiveBackground;
        private Button medleyDanceButton;
        private Bitmap medleyDanceNormalBackground;
        private Bitmap medleyDanceActiveBackground;
        private Button ninetiesButton;
        private Bitmap ninetiesNormalBackground;
        private Bitmap ninetiesActiveBackground;
        private Button nostalgicSongsButton;
        private Bitmap nostalgicSongsNormalBackground;
        private Bitmap nostalgicSongsActiveBackground;
        private Button chinaSongsButton;
        private Bitmap chinaNormalBackground;
        private Bitmap chinaActiveBackground;
        
        private void InitializeButtonsForCategorySearch()
        {
            categorySearchButton = new Button { Text = "" };
            categorySearchButton.Name = "categorySearchButton";
            ResizeAndPositionButton(categorySearchButton, 731, 97, 99, 99);
            Rectangle categorySearchCropArea = new Rectangle(731, 97, 99, 99);
            categorySearchNormalBackground = new Bitmap(Path.Combine(Application.StartupPath, "themes\\superstar\\ICON上方\\上方ICON_類別查詢-07.png"));
            categorySearchActiveBackground = mouseDownImage.Clone(categorySearchCropArea, mouseDownImage.PixelFormat);
            categorySearchButton.BackgroundImage = categorySearchNormalBackground;
            categorySearchButton.BackgroundImageLayout = ImageLayout.Stretch;
            categorySearchButton.FlatStyle = FlatStyle.Flat;
            categorySearchButton.FlatAppearance.BorderSize = 0; // Remove border
            categorySearchButton.Click += CategorySearchButton_Click;
            this.Controls.Add(categorySearchButton);
        }

        private void InitializeCategorySearchButtons()
        {
            // Initialize the "男女情歌" button with custom images and event handlers
            loveDuetButton = new Button { Text = "", Visible = false };
            ResizeAndPositionButton(loveDuetButton, 1214, 230, 209, 59);
            Rectangle loveDuetButtonCropArea = new Rectangle(1214, 230, 209, 59);
            loveDuetNormalBackground = normalStateImageCategoryQuery.Clone(loveDuetButtonCropArea, normalStateImageCategoryQuery.PixelFormat);
            loveDuetActiveBackground = mouseDownImageCategoryQuery.Clone(loveDuetButtonCropArea, mouseDownImageCategoryQuery.PixelFormat);
            loveDuetButton.BackgroundImage = loveDuetNormalBackground;
            loveDuetButton.BackgroundImageLayout = ImageLayout.Stretch;
            loveDuetButton.FlatStyle = FlatStyle.Flat;
            loveDuetButton.FlatAppearance.BorderSize = 0;
            loveDuetButton.Click += LoveDuetButton_Click;  // You need to define this method
            this.Controls.Add(loveDuetButton);

            // Initialize the "選秀節目" button with custom images and event handlers
            talentShowButton = new Button { Text = "", Visible = false };
            ResizeAndPositionButton(talentShowButton, 1214, 293, 209, 58);
            Rectangle talentShowButtonCropArea = new Rectangle(1214, 293, 209, 58);
            talentShowNormalBackground = normalStateImageCategoryQuery.Clone(talentShowButtonCropArea, normalStateImageCategoryQuery.PixelFormat);
            talentShowActiveBackground = mouseDownImageCategoryQuery.Clone(talentShowButtonCropArea, mouseDownImageCategoryQuery.PixelFormat);
            talentShowButton.BackgroundImage = talentShowNormalBackground;
            talentShowButton.BackgroundImageLayout = ImageLayout.Stretch;
            talentShowButton.FlatStyle = FlatStyle.Flat;
            talentShowButton.FlatAppearance.BorderSize = 0;
            talentShowButton.Click += TalentShowButton_Click;  // You need to define this method
            this.Controls.Add(talentShowButton);

            // Initialize the "串燒舞曲" button with custom images and event handlers
            medleyDanceButton = new Button { Text = "", Visible = false };
            ResizeAndPositionButton(medleyDanceButton, 1214, 356, 209, 58);
            Rectangle medleyDanceButtonCropArea = new Rectangle(1214, 356, 209, 58);
            medleyDanceNormalBackground = normalStateImageCategoryQuery.Clone(medleyDanceButtonCropArea, normalStateImageCategoryQuery.PixelFormat);
            medleyDanceActiveBackground = mouseDownImageCategoryQuery.Clone(medleyDanceButtonCropArea, mouseDownImageCategoryQuery.PixelFormat);
            medleyDanceButton.BackgroundImage = medleyDanceNormalBackground;
            medleyDanceButton.BackgroundImageLayout = ImageLayout.Stretch;
            medleyDanceButton.FlatStyle = FlatStyle.Flat;
            medleyDanceButton.FlatAppearance.BorderSize = 0;
            medleyDanceButton.Click += MedleyDanceButton_Click;  // You need to define this method
            this.Controls.Add(medleyDanceButton);

            // Initialize the "90年代" button with custom images and event handlers
            ninetiesButton = new Button { Text = "", Visible = false };
            ResizeAndPositionButton(ninetiesButton, 1214, 418, 209, 59);
            Rectangle ninetiesButtonCropArea = new Rectangle(1214, 418, 209, 59);
            ninetiesNormalBackground = normalStateImageCategoryQuery.Clone(ninetiesButtonCropArea, normalStateImageCategoryQuery.PixelFormat);
            ninetiesActiveBackground = mouseDownImageCategoryQuery.Clone(ninetiesButtonCropArea, mouseDownImageCategoryQuery.PixelFormat);
            ninetiesButton.BackgroundImage = ninetiesNormalBackground;
            ninetiesButton.BackgroundImageLayout = ImageLayout.Stretch;
            ninetiesButton.FlatStyle = FlatStyle.Flat;
            ninetiesButton.FlatAppearance.BorderSize = 0;
            ninetiesButton.Click += NinetiesButton_Click;  // You need to define this method
            this.Controls.Add(ninetiesButton);

            // Initialize the "懷舊老歌" button with custom images and event handlers
            nostalgicSongsButton = new Button { Text = "", Visible = false };
            ResizeAndPositionButton(nostalgicSongsButton, 1214, 481, 209, 59);
            Rectangle nostalgicSongsButtonCropArea = new Rectangle(1214, 481, 209, 59);
            nostalgicSongsNormalBackground = normalStateImageCategoryQuery.Clone(nostalgicSongsButtonCropArea, normalStateImageCategoryQuery.PixelFormat);
            nostalgicSongsActiveBackground = mouseDownImageCategoryQuery.Clone(nostalgicSongsButtonCropArea, mouseDownImageCategoryQuery.PixelFormat);
            nostalgicSongsButton.BackgroundImage = nostalgicSongsNormalBackground;
            nostalgicSongsButton.BackgroundImageLayout = ImageLayout.Stretch;
            nostalgicSongsButton.FlatStyle = FlatStyle.Flat;
            nostalgicSongsButton.FlatAppearance.BorderSize = 0;
            nostalgicSongsButton.Click += NostalgicSongsButton_Click;  // You need to define this method
            this.Controls.Add(nostalgicSongsButton);

            // 中國大陸按钮
            chinaSongsButton = new Button { Text = "", Visible = false };
            ResizeAndPositionButton(chinaSongsButton, 1214, 544, 209, 58);
            Rectangle chinaCropArea = new Rectangle(1214, 544, 209, 58); // 假设裁切区域起始点和大小
            // 设置按钮背景图
            chinaNormalBackground = normalStateImageCategoryQuery.Clone(chinaCropArea, normalStateImageCategoryQuery.PixelFormat);
            chinaActiveBackground = mouseDownImageCategoryQuery.Clone(chinaCropArea, mouseDownImageCategoryQuery.PixelFormat);
            chinaSongsButton.BackgroundImage = chinaNormalBackground;
            chinaSongsButton.BackgroundImageLayout = ImageLayout.Stretch;
            chinaSongsButton.FlatStyle = FlatStyle.Flat;
            chinaSongsButton.FlatAppearance.BorderSize = 0; // 移除边框
            chinaSongsButton.Click += ChinaSongsButton_Click; // 事件处理器
            this.Controls.Add(chinaSongsButton);

            // // 越南歌曲按钮
            // vietnameseSongsButton = new Button { Text = "", Visible = false };
            // ResizeAndPositionButton(vietnameseSongsButton, 1214, 607, 209, 58);
            // Rectangle vietnamCropArea = new Rectangle(1214, 607, 209, 58); // 同上，根据实际图片调整
            // // 设置按钮背景图
            // vietnamNormalBackground = normalStateImageCategoryQuery.Clone(vietnamCropArea, normalStateImageCategoryQuery.PixelFormat);
            // vietnamActiveBackground = mouseDownImageCategoryQuery.Clone(vietnamCropArea, mouseDownImageCategoryQuery.PixelFormat);
            // vietnameseSongsButton.BackgroundImage = vietnamNormalBackground;
            // vietnameseSongsButton.BackgroundImageLayout = ImageLayout.Stretch;
            // vietnameseSongsButton.FlatStyle = FlatStyle.Flat;
            // vietnameseSongsButton.FlatAppearance.BorderSize = 0; // 移除边框
            // vietnameseSongsButton.Click += VietnameseSongsButton_Click; // 事件处理器
            // this.Controls.Add(vietnameseSongsButton);
        }

        private void CategorySearchButton_Click(object sender, EventArgs e)
        {
            newSongAlertButton.BackgroundImage = newSongAlertNormalBackground;
            hotPlayButton.BackgroundImage = hotPlayNormalBackground;
            singerSearchButton.BackgroundImage = singerSearchNormalBackground;
            songSearchButton.BackgroundImage = songSearchNormalBackground;
            languageSearchButton.BackgroundImage = languageSearchNormalBackground;
            groupSearchButton.BackgroundImage = groupSearchNormalBackground;
            categorySearchButton.BackgroundImage = categorySearchActiveBackground;
            orderedSongsButton.BackgroundImage = orderedSongsNormalBackground;
            myFavoritesButton.BackgroundImage = myFavoritesNormalBackground;
            promotionsButton.BackgroundImage = promotionsNormalBackground;
            deliciousFoodButton.BackgroundImage = deliciousFoodNormalBackground;

            loveDuetButton.BackgroundImage = loveDuetActiveBackground;
            talentShowButton.BackgroundImage = talentShowNormalBackground;
            medleyDanceButton.BackgroundImage = medleyDanceNormalBackground;
            ninetiesButton.BackgroundImage = ninetiesNormalBackground;
            nostalgicSongsButton.BackgroundImage = nostalgicSongsNormalBackground;
            chinaSongsButton.BackgroundImage = chinaNormalBackground;
            // vietnameseSongsButton.BackgroundImage = vietnamNormalBackground;

            loveDuetSongs = allSongs.Where(song => song.SongGenre.Contains("A1"))
                                .OrderByDescending(song => song.Plays) // 根据点播次数降序排列
                                .ToList();
            currentPage = 0; // 重置到第一页
            currentSongList = loveDuetSongs; // 更新当前显示的歌曲列表
            totalPages = (int)Math.Ceiling((double)loveDuetSongs.Count / itemsPerPage);

            // DisplaySongsOnPage(currentSongList, currentPage);
            multiPagePanel.currentPageIndex = 0;
            multiPagePanel.LoadSongs(currentSongList);

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
            SetPinYinSingersAndButtonsVisibility(false);
            SetPinYinSongsAndButtonsVisibility(false);
            SetPictureBoxToggleLightAndButtonsVisibility(false);
            SetPictureBoxSceneSoundEffectsAndButtonsVisibility(false);
            SetPictureBoxCategoryAndButtonsVisibility(true);

            // 切换pictureBoxQRCode的可见性
            if (pictureBoxQRCode != null)
            {
                pictureBoxQRCode.Visible = false;
                closeQRCodeButton.Visible = false;
            }
        }
        
        private void TogglePictureBoxCategoryButtonsVisibility()
        {
            // 选取pictureBox2Buttons数组中的任意一个按钮来检测可见状态
            bool areButtonsVisible = loveDuetButton.Visible; // 或其他相关按钮

            // 设置相反的可见性状态
            SetPictureBoxCategoryAndButtonsVisibility(!areButtonsVisible);
        }
        
        private void SetPictureBoxCategoryAndButtonsVisibility(bool isVisible)
        {   
            loveDuetButton.Visible = isVisible;
            loveDuetButton.BringToFront();
            
            talentShowButton.Visible = isVisible;
            talentShowButton.BringToFront();
            
            medleyDanceButton.Visible = isVisible;
            medleyDanceButton.BringToFront();
            
            ninetiesButton.Visible = isVisible;
            ninetiesButton.BringToFront();
            
            nostalgicSongsButton.Visible = isVisible;
            nostalgicSongsButton.BringToFront();
            
            chinaSongsButton.Visible = isVisible;
            chinaSongsButton.BringToFront();

            // vietnameseSongsButton.Visible = isVisible;
            // vietnameseSongsButton.BringToFront();
        }
    }
}