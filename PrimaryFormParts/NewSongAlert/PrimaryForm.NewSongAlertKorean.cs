using System;
using System.Windows.Forms;
using System.Drawing;
using System.Linq; // Add this line

namespace DualScreenDemo
{
    public partial class PrimaryForm
    {
        private void HanYuButtonNewSong_Click(object sender, EventArgs e)
        {
            guoYuButtonNewSong.BackgroundImage = guoYuNewSongNormalBackground;
            taiYuButtonNewSong.BackgroundImage = taiYuNewSongNormalBackground;
            yueYuButtonNewSong.BackgroundImage = yueYuNewSongNormalBackground;
            yingWenButtonNewSong.BackgroundImage = yingWenNewSongNormalBackground;
            riYuButtonNewSong.BackgroundImage = riYuNewSongNormalBackground;
            hanYuButtonNewSong.BackgroundImage = hanYuNewSongActiveBackground;

            int songLimit = ReadNewSongLimit(); // Get the new song limit from the file

            hanYuSongs2 = allSongs.Where(song => song.Category == "韓語")
                                .OrderByDescending(song => song.AddedTime) // 根据点播次数降序排列
                                .Take(songLimit) // Apply the limit from the settings file
                                .ToList();
            currentPage = 0; // 重置到第一页
            currentSongList = hanYuSongs2; // 更新当前显示的歌曲列表
            totalPages = (int)Math.Ceiling((double)hanYuSongs2.Count / itemsPerPage);

            // DisplaySongsOnPage(currentSongList, currentPage);
            multiPagePanel.currentPageIndex = 0;
            multiPagePanel.LoadSongs(currentSongList);
        }
    }
}