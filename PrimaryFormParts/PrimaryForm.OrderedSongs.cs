using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO; // 添加這行

namespace DualScreenDemo
{
    public partial class PrimaryForm
    {
        private Button orderedSongsButton;
        private Bitmap orderedSongsNormalBackground; // 已點歌曲按钮的正常背景图像
        private Bitmap orderedSongsActiveBackground; // 已點歌曲按钮的激活背景图像

        // This method is in your PrimaryForm class
        private void OrderedSongsButton_Click(object sender, EventArgs e)
        {
            newSongAlertButton.BackgroundImage = newSongAlertNormalBackground;
            hotPlayButton.BackgroundImage = hotPlayNormalBackground;
            singerSearchButton.BackgroundImage = singerSearchNormalBackground;
            songSearchButton.BackgroundImage = songSearchNormalBackground;
            languageSearchButton.BackgroundImage = languageSearchNormalBackground;
            groupSearchButton.BackgroundImage = groupSearchNormalBackground;
            categorySearchButton.BackgroundImage = categorySearchNormalBackground;
            orderedSongsButton.BackgroundImage = orderedSongsActiveBackground;
            myFavoritesButton.BackgroundImage = myFavoritesNormalBackground;
            promotionsButton.BackgroundImage = promotionsNormalBackground;
            deliciousFoodButton.BackgroundImage = deliciousFoodNormalBackground;

            currentPage = 0; // 重置到第一页
            currentSongList = playedSongsHistory; // 更新当前显示的歌曲列表
            totalPages = (int)Math.Ceiling((double)playedSongsHistory.Count / itemsPerPage);
            // DisplaySongsOnPageWithStatus(playedSongsHistory, 0); // Display the first page of requested songs
            multiPagePanel.currentPageIndex = 0;
            multiPagePanel.LoadPlayedSongs(currentSongList, playStates);
        
            // 切换pictureBoxQRCode的可见性
            if (pictureBoxQRCode != null)
            {
                pictureBoxQRCode.Visible = false;
                closeQRCodeButton.Visible = false;
            }

            SetPictureBoxToggleLightAndButtonsVisibility(false);
            SetPictureBoxSceneSoundEffectsAndButtonsVisibility(false);
        }
    }
}