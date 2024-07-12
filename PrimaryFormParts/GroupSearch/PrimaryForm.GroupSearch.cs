using System;
using System.Drawing;
using System.IO; // Add this line for Path
using System.Linq; // Add this line for Where
using System.Windows.Forms;

namespace DualScreenDemo
{
    public partial class PrimaryForm
    {
        private Button groupSearchButton;
        private Bitmap groupSearchNormalBackground;
        private Bitmap groupSearchActiveBackground;

        private Button groupGuoYuButton;
        private Bitmap groupGuoYuNormalBackground;
        private Bitmap groupGuoYuActiveBackground;
        private Button groupTaiYuButton;
        private Bitmap groupTaiYuNormalBackground;
        private Bitmap groupTaiYuActiveBackground;
        private Button groupYueYuButton;
        private Bitmap groupYueYuNormalBackground;
        private Bitmap groupYueYuActiveBackground;
        private Button groupYingWenButton;
        private Bitmap groupYingWenNormalBackground;
        private Bitmap groupYingWenActiveBackground;
        private Button groupRiYuButton;
        private Bitmap groupRiYuNormalBackground;
        private Bitmap groupRiYuActiveBackground;
        private Button groupHanYuButton;
        private Bitmap groupHanYuNormalBackground;
        private Bitmap groupHanYuActiveBackground;

        private void InitializeButtonsForGroupPictureBox()
        {
            // 国语团体查询按钮
            groupGuoYuButton = new Button { Text = "", Visible = false };
            ResizeAndPositionButton(groupGuoYuButton, 1214, 230, 209, 59);
            Rectangle groupGuoYuButtonCropArea = new Rectangle(1214, 230, 209, 59);
            groupGuoYuNormalBackground = normalStateImageLanguageQuery.Clone(groupGuoYuButtonCropArea, normalStateImageLanguageQuery.PixelFormat);
            groupGuoYuActiveBackground = mouseDownImageLanguageQuery.Clone(groupGuoYuButtonCropArea, mouseDownImageLanguageQuery.PixelFormat);
            groupGuoYuButton.BackgroundImage = groupGuoYuNormalBackground;
            groupGuoYuButton.BackgroundImageLayout = ImageLayout.Stretch;
            groupGuoYuButton.FlatStyle = FlatStyle.Flat;
            groupGuoYuButton.FlatAppearance.BorderSize = 0;
            groupGuoYuButton.Click += GroupGuoYuButton_Click;
            this.Controls.Add(groupGuoYuButton);

            // 台语团体查询按钮
            groupTaiYuButton = new Button { Text = "", Visible = false };
            ResizeAndPositionButton(groupTaiYuButton, 1214, 293, 209, 58);
            Rectangle groupTaiYuButtonCropArea = new Rectangle(1214, 293, 209, 58);
            groupTaiYuNormalBackground = normalStateImageLanguageQuery.Clone(groupTaiYuButtonCropArea, normalStateImageLanguageQuery.PixelFormat);
            groupTaiYuActiveBackground = mouseDownImageLanguageQuery.Clone(groupTaiYuButtonCropArea, mouseDownImageLanguageQuery.PixelFormat);
            groupTaiYuButton.BackgroundImage = groupTaiYuNormalBackground;
            groupTaiYuButton.BackgroundImageLayout = ImageLayout.Stretch;
            groupTaiYuButton.FlatStyle = FlatStyle.Flat;
            groupTaiYuButton.FlatAppearance.BorderSize = 0;
            groupTaiYuButton.Click += GroupTaiYuButton_Click;
            this.Controls.Add(groupTaiYuButton);

            // 粤语团体查询按钮
            groupYueYuButton = new Button { Text = "", Visible = false };
            ResizeAndPositionButton(groupYueYuButton, 1214, 356, 209, 58);
            Rectangle groupYueYuButtonCropArea = new Rectangle(1214, 356, 209, 58);
            groupYueYuNormalBackground = normalStateImageLanguageQuery.Clone(groupYueYuButtonCropArea, normalStateImageLanguageQuery.PixelFormat);
            groupYueYuActiveBackground = mouseDownImageLanguageQuery.Clone(groupYueYuButtonCropArea, mouseDownImageLanguageQuery.PixelFormat);
            groupYueYuButton.BackgroundImage = groupYueYuNormalBackground;
            groupYueYuButton.BackgroundImageLayout = ImageLayout.Stretch;
            groupYueYuButton.FlatStyle = FlatStyle.Flat;
            groupYueYuButton.FlatAppearance.BorderSize = 0;
            groupYueYuButton.Click += GroupYueYuButton_Click;
            this.Controls.Add(groupYueYuButton);

            // 英文团体查询按钮
            groupYingWenButton = new Button { Text = "", Visible = false };
            ResizeAndPositionButton(groupYingWenButton, 1214, 418, 209, 59);
            Rectangle groupYingWenButtonCropArea = new Rectangle(1214, 418, 209, 59);
            groupYingWenNormalBackground = normalStateImageLanguageQuery.Clone(groupYingWenButtonCropArea, normalStateImageLanguageQuery.PixelFormat);
            groupYingWenActiveBackground = mouseDownImageLanguageQuery.Clone(groupYingWenButtonCropArea, mouseDownImageLanguageQuery.PixelFormat);
            groupYingWenButton.BackgroundImage = groupYingWenNormalBackground;
            groupYingWenButton.BackgroundImageLayout = ImageLayout.Stretch;
            groupYingWenButton.FlatStyle = FlatStyle.Flat;
            groupYingWenButton.FlatAppearance.BorderSize = 0;
            groupYingWenButton.Click += GroupYingWenButton_Click;
            this.Controls.Add(groupYingWenButton);

            // 日语团体查询按钮
            groupRiYuButton = new Button { Text = "", Visible = false };
            ResizeAndPositionButton(groupRiYuButton, 1214, 481, 209, 59);
            Rectangle groupRiYuButtonCropArea = new Rectangle(1214, 481, 209, 59);
            groupRiYuNormalBackground = normalStateImageLanguageQuery.Clone(groupRiYuButtonCropArea, normalStateImageLanguageQuery.PixelFormat);
            groupRiYuActiveBackground = mouseDownImageLanguageQuery.Clone(groupRiYuButtonCropArea, mouseDownImageLanguageQuery.PixelFormat);
            groupRiYuButton.BackgroundImage = groupRiYuNormalBackground;
            groupRiYuButton.BackgroundImageLayout = ImageLayout.Stretch;
            groupRiYuButton.FlatStyle = FlatStyle.Flat;
            groupRiYuButton.FlatAppearance.BorderSize = 0;
            groupRiYuButton.Click += GroupRiYuButton_Click;
            this.Controls.Add(groupRiYuButton);

            // 韩语团体查询按钮
            groupHanYuButton = new Button { Text = "", Visible = false };
            ResizeAndPositionButton(groupHanYuButton, 1214, 544, 209, 58);
            Rectangle groupHanYuButtonCropArea = new Rectangle(1214, 544, 209, 58);
            groupHanYuNormalBackground = normalStateImageLanguageQuery.Clone(groupHanYuButtonCropArea, normalStateImageLanguageQuery.PixelFormat);
            groupHanYuActiveBackground = mouseDownImageLanguageQuery.Clone(groupHanYuButtonCropArea, mouseDownImageLanguageQuery.PixelFormat);
            groupHanYuButton.BackgroundImage = groupHanYuNormalBackground;
            groupHanYuButton.BackgroundImageLayout = ImageLayout.Stretch;
            groupHanYuButton.FlatStyle = FlatStyle.Flat;
            groupHanYuButton.FlatAppearance.BorderSize = 0;
            groupHanYuButton.Click += GroupHanYuButton_Click;
            this.Controls.Add(groupHanYuButton);
        }

        private void GroupSongSelectionButton_Click(object sender, EventArgs e)
        {
            newSongAlertButton.BackgroundImage = newSongAlertNormalBackground;
            hotPlayButton.BackgroundImage = hotPlayNormalBackground;
            singerSearchButton.BackgroundImage = singerSearchNormalBackground;
            songSearchButton.BackgroundImage = songSearchNormalBackground;
            languageSearchButton.BackgroundImage = languageSearchNormalBackground;
            groupSearchButton.BackgroundImage = groupSearchActiveBackground;
            categorySearchButton.BackgroundImage = categorySearchNormalBackground;
            orderedSongsButton.BackgroundImage = orderedSongsNormalBackground;
            myFavoritesButton.BackgroundImage = myFavoritesNormalBackground;
            promotionsButton.BackgroundImage = promotionsNormalBackground;
            deliciousFoodButton.BackgroundImage = deliciousFoodNormalBackground;

            groupGuoYuButton.BackgroundImage = groupGuoYuActiveBackground;
            groupTaiYuButton.BackgroundImage = groupTaiYuNormalBackground;
            groupYueYuButton.BackgroundImage = groupYueYuNormalBackground;
            groupYingWenButton.BackgroundImage = groupYingWenNormalBackground;
            groupRiYuButton.BackgroundImage = groupRiYuNormalBackground;
            groupHanYuButton.BackgroundImage = groupHanYuNormalBackground;

            guoYuSongs = allSongs.Where(song => song.Category == "國語" && (song.ArtistACategory == "團" || song.ArtistBCategory == "團"))
                                .OrderByDescending(song => song.Plays) // 根据点播次数降序排列
                                .ToList();
            currentPage = 0; // 重置到第一页
            currentSongList = guoYuSongs; // 更新当前显示的歌曲列表
            totalPages = (int)Math.Ceiling((double)guoYuSongs.Count / itemsPerPage);

            // DisplaySongsOnPage(currentSongList, currentPage);
            multiPagePanel.currentPageIndex = 0;
            multiPagePanel.LoadSongs(currentSongList);

            // 显示第三个图片，并可能隐藏第一个图片
            SetHotSongButtonsVisibility(false);
            SetNewSongButtonsVisibility(false);
            SetSingerSearchButtonsVisibility(false);
            SetSongSearchButtonsVisibility(false);
            SetPictureBoxLanguageButtonsVisibility(false);
            SetPictureBoxCategoryAndButtonsVisibility(false);
            SetZhuYinSingersAndButtonsVisibility(false);
            SetZhuYinSongsAndButtonsVisibility(false);
            SetEnglishSingersAndButtonsVisibility(false);
            SetEnglishSongsAndButtonsVisibility(false);
            SetPinYinSingersAndButtonsVisibility(false);
            SetPinYinSongsAndButtonsVisibility(false);
            SetPictureBoxToggleLightAndButtonsVisibility(false);
            SetPictureBoxSceneSoundEffectsAndButtonsVisibility(false);
            SetGroupButtonsVisibility(true);

            // 切换pictureBoxQRCode的可见性
            if (pictureBoxQRCode != null)
            {
                pictureBoxQRCode.Visible = false;
                closeQRCodeButton.Visible = false;
            }
        }
        
        private void SetGroupButtonsVisibility(bool isVisible)
        {
            Button[] pictureBox6Buttons = { groupGuoYuButton, groupTaiYuButton, groupYueYuButton, groupYingWenButton, groupRiYuButton, groupHanYuButton };

            foreach (var button in pictureBox6Buttons)
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
