using System;
using System.Drawing;
using System.Windows.Forms;

namespace DualScreenDemo
{
    public partial class PrimaryForm
    {
        private Button zhuyinSearchButton;
        private Bitmap zhuyinSearchNormalBackground;
        private Bitmap zhuyinSearchActiveBackground;
        private Button englishSearchButton;
        private Bitmap englishSearchNormalBackground;
        private Bitmap englishSearchActiveBackground;
        private Button pinyinSearchButton;
        private Bitmap pinyinSearchNormalBackground;
        private Bitmap pinyinSearchActiveBackground;
        // private Button strokeCountSearchButton;
        // private Bitmap strokeCountSearchNormalBackground;
        // private Bitmap strokeCountSearchActiveBackground;
        private Button wordCountSearchButton;
        private Bitmap wordCountSearchNormalBackground;
        private Bitmap wordCountSearchActiveBackground;
        private Button handWritingSearchButton;
        private Bitmap handWritingSearchNormalBackground;
        private Bitmap handWritingSearchActiveBackground;

        private void SingerSearchButton_Click(object sender, EventArgs e)
        {
            newSongAlertButton.BackgroundImage = newSongAlertNormalBackground;
            hotPlayButton.BackgroundImage = hotPlayNormalBackground;
            singerSearchButton.BackgroundImage = singerSearchActiveBackground;
            songSearchButton.BackgroundImage = songSearchNormalBackground;
            languageSearchButton.BackgroundImage = languageSearchNormalBackground;
            groupSearchButton.BackgroundImage = groupSearchNormalBackground;
            categorySearchButton.BackgroundImage = categorySearchNormalBackground;
            orderedSongsButton.BackgroundImage = orderedSongsNormalBackground;
            myFavoritesButton.BackgroundImage = myFavoritesNormalBackground;
            promotionsButton.BackgroundImage = promotionsNormalBackground;
            deliciousFoodButton.BackgroundImage = deliciousFoodNormalBackground;

            // 显示第三个图片，并可能隐藏第一个图片
            SetHotSongButtonsVisibility(false);
            SetNewSongButtonsVisibility(false);
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
            SetSingerSearchButtonsVisibility(true);

            // 切换pictureBoxQRCode的可见性
            if (pictureBoxQRCode != null)
            {
                pictureBoxQRCode.Visible = false;
                closeQRCodeButton.Visible = false;
            }
        }
        
        private void SetSingerSearchButtonsVisibility(bool isVisible)
        {
            Button[] singerSearchButtons = { zhuyinSearchButton, englishSearchButton, pinyinSearchButton, wordCountSearchButton, handWritingSearchButton };

            foreach (var button in singerSearchButtons)
            {
                button.Visible = isVisible;
                if (isVisible)
                {
                    button.BringToFront();
                }
            }
        }

        private void InitializeButtonsForSingerSearch()
        {
            // 初始化注音查詢按钮
            InitializeSearchButton(ref zhuyinSearchButton, "zhuyinSearchButton", 1214, 230, 209, 59, ref zhuyinSearchNormalBackground, ref zhuyinSearchActiveBackground, normalStateImageArtistQuery, mouseDownImageArtistQuery, ZhuyinSearchSingersButton_Click);

            // 英文查詢按钮
            InitializeSearchButton(ref englishSearchButton, "englishSearchButton", 1214, 293, 209, 58, ref englishSearchNormalBackground, ref englishSearchActiveBackground, normalStateImageArtistQuery, mouseDownImageArtistQuery, EnglishSearchSingersButton_Click);

            // 初始化拼音查詢按钮
            InitializeSearchButton(ref pinyinSearchButton, "pinyinSearchButton", 1214, 356, 209, 58, ref pinyinSearchNormalBackground, ref pinyinSearchActiveBackground, normalStateImageArtistQuery, mouseDownImageArtistQuery, PinyinSingerSearchButton_Click);

            InitializeSearchButton(ref wordCountSearchButton, "strokeCountSearchButton", 1214, 418, 209, 59, ref wordCountSearchNormalBackground, ref wordCountSearchActiveBackground, normalStateImageArtistQuery, mouseDownImageArtistQuery, WordCountSearchButton_Click);

            InitializeSearchButton(ref handWritingSearchButton, "handWritingSearchButton", 1214, 481, 209, 59, ref handWritingSearchNormalBackground, ref handWritingSearchActiveBackground, normalStateImageArtistQuery, mouseDownImageArtistQuery, HandWritingSearchButtonForSingers_Click);
        }

        private void InitializeSearchButton(ref Button button, string name, int x, int y, int width, int height, ref Bitmap normalBackground, ref Bitmap activeBackground, Bitmap normalImage, Bitmap activeImage, EventHandler clickEventHandler)
        {
            button = new Button { Text = "", Visible = true, Name = name };
            ResizeAndPositionButton(button, x, y, width, height);
            Rectangle buttonCropArea = new Rectangle(x, y, width, height);
            normalBackground = normalImage.Clone(buttonCropArea, normalImage.PixelFormat);
            activeBackground = activeImage.Clone(buttonCropArea, activeImage.PixelFormat);
            button.BackgroundImage = normalBackground;
            button.BackgroundImageLayout = ImageLayout.Stretch;
            button.FlatStyle = FlatStyle.Flat;
            button.FlatAppearance.BorderSize = 0; // Remove border
            button.Click += clickEventHandler;
            this.Controls.Add(button);
        }
    }
}