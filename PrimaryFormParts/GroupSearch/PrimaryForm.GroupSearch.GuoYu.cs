using System;
using System.Windows.Forms;
using System.Drawing;
using System.Linq; // Add this line

namespace DualScreenDemo
{
    public partial class PrimaryForm
    {
        private void GroupGuoYuButton_Click(object sender, EventArgs e)
        {
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
        }
    }
}