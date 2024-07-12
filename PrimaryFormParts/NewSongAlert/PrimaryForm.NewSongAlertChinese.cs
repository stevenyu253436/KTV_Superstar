using System;
using System.Windows.Forms;
using System.Drawing;
using System.Linq; // Add this line

namespace DualScreenDemo
{
    public partial class PrimaryForm
    {
        private void GuoYuButtonNewSong_Click(object sender, EventArgs e)
        {
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
        }
    }
}