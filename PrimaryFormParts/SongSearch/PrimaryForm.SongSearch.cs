using System;
using System.Drawing;
using System.Windows.Forms;

namespace DualScreenDemo
{
    public partial class PrimaryForm
    {
        private Button zhuyinSearchSongButton;
        private Bitmap zhuyinSearchSongNormalBackground;
        private Bitmap zhuyinSearchSongActiveBackground;
        private Button englishSearchSongButton;
        private Bitmap englishSearchSongNormalBackground;
        private Bitmap englishSearchSongActiveBackground;
        private Button wordCountSearchSongButton;
        private Bitmap wordCountSearchSongNormalBackground;
        private Bitmap wordCountSearchSongActiveBackground;
        private Button pinyinSearchSongButton;
        private Bitmap pinyinSearchSongNormalBackground;
        private Bitmap pinyinSearchSongActiveBackground;
        private Button handWritingSearchSongButton;
        private Bitmap handWritingSearchSongNormalBackground;
        private Bitmap handWritingSearchSongActiveBackground;
        private Button numberSearchSongButton;
        private Bitmap numberSearchSongNormalBackground;
        private Bitmap numberSearchSongActiveBackground;

        private void SongSearchButton_Click(object sender, EventArgs e)
        {
            newSongAlertButton.BackgroundImage = newSongAlertNormalBackground;
            hotPlayButton.BackgroundImage = hotPlayNormalBackground;
            singerSearchButton.BackgroundImage = singerSearchNormalBackground;
            songSearchButton.BackgroundImage = songSearchActiveBackground;
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
            SetSingerSearchButtonsVisibility(false);
            SetPictureBoxLanguageButtonsVisibility(false);
            SetGroupButtonsVisibility(false);
            SetPictureBoxCategoryAndButtonsVisibility(false);
            SetZhuYinSingersAndButtonsVisibility(false);
            SetZhuYinSongsAndButtonsVisibility(false);
            SetEnglishSingersAndButtonsVisibility(false);
            // SetEnglishSongsAndButtonsVisibility(false);
            SetPinYinSingersAndButtonsVisibility(false);
            SetPinYinSongsAndButtonsVisibility(false);
            SetPictureBoxToggleLightAndButtonsVisibility(false);
            SetPictureBoxSceneSoundEffectsAndButtonsVisibility(false);
            SetSongSearchButtonsVisibility(true);

            // 切换pictureBoxQRCode的可见性
            if (pictureBoxQRCode != null)
            {
                pictureBoxQRCode.Visible = false;
                closeQRCodeButton.Visible = false;
            }
        }
        
        private void SetSongSearchButtonsVisibility(bool isVisible)
        {
            pictureBox4.Visible = isVisible;

            Button[] songSearchButtons = { zhuyinSearchSongButton, englishSearchSongButton, wordCountSearchSongButton, pinyinSearchSongButton, handWritingSearchSongButton, numberSearchSongButton };

            foreach (var button in songSearchButtons)
            {
                button.Visible = isVisible;
                if (isVisible)
                {
                    button.BringToFront();
                }
            }
        }

        private void InitializeButtonsForSongSearch()
        {
            // 初始化注音查詢按钮
            InitializeSearchButton(ref zhuyinSearchSongButton, "zhuyinSearchSongButton", 1214, 230, 209, 59, ref zhuyinSearchSongNormalBackground, ref zhuyinSearchSongActiveBackground, normalStateImageSongQuery, mouseDownImageSongQuery, ZhuyinSearchSongsButton_Click);

            // 英文查詢按钮
            InitializeSearchButton(ref englishSearchSongButton, "englishSearchSongButton", 1214, 293, 209, 58, ref englishSearchSongNormalBackground, ref englishSearchSongActiveBackground, normalStateImageSongQuery, mouseDownImageSongQuery, EnglishSearchSongsButton_Click);

            // 初始化拼音查詢按钮
            InitializeSearchButton(ref pinyinSearchSongButton, "pinyinSearchSongButton", 1214, 356, 209, 58, ref pinyinSearchSongNormalBackground, ref pinyinSearchSongActiveBackground, normalStateImageSongQuery, mouseDownImageSongQuery, PinyinSearchSongsButton_Click);

            // 初始化字數查詢按钮
            InitializeSearchButton(ref wordCountSearchSongButton, "wordCountSearchSongButton", 1214, 418, 209, 59, ref wordCountSearchSongNormalBackground, ref wordCountSearchSongActiveBackground, normalStateImageSongQuery, mouseDownImageSongQuery, WordCountSearchSong_Click);

            // 初始化手寫查詢按钮
            InitializeSearchButton(ref handWritingSearchSongButton, "handWritingSearchSongButton", 1214, 481, 209, 59, ref handWritingSearchSongNormalBackground, ref handWritingSearchSongActiveBackground, normalStateImageSongQuery, mouseDownImageSongQuery, HandWritingSearchButtonForSongs_Click);

            // 初始化編號查詢按钮
            InitializeSearchButton(ref numberSearchSongButton, "numberSearchSongButton", 1214, 544, 209, 58, ref numberSearchSongNormalBackground, ref numberSearchSongActiveBackground, normalStateImageSongQuery, mouseDownImageSongQuery, NumberSearchButton2_Click);
        }
    }
}