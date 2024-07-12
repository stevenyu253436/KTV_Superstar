using System;
using System.Linq;

namespace DualScreenDemo
{
    public partial class PrimaryForm
    {
        private void VietnameseSongsButton_Click(object sender, EventArgs e)
        {
            loveDuetButton.BackgroundImage = loveDuetNormalBackground;
            talentShowButton.BackgroundImage = talentShowNormalBackground;
            medleyDanceButton.BackgroundImage = medleyDanceNormalBackground;
            ninetiesButton.BackgroundImage = ninetiesNormalBackground;
            nostalgicSongsButton.BackgroundImage = nostalgicSongsNormalBackground;
            chinaSongsButton.BackgroundImage = chinaNormalBackground;
            // vietnameseSongsButton.BackgroundImage = vietnamActiveBackground;

            vietnameseSongs = allSongs.Where(song => song.SongGenre.Contains("G1"))
                                .OrderByDescending(song => song.Plays) // 根据点播次数降序排列
                                .ToList();
            currentPage = 0; // 重置到第一页
            currentSongList = vietnameseSongs; // 更新当前显示的歌曲列表
            totalPages = (int)Math.Ceiling((double)vietnameseSongs.Count / itemsPerPage);

            multiPagePanel.currentPageIndex = 0;
            multiPagePanel.LoadSongs(currentSongList);
        }
    }
}