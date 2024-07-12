using System;
using System.Linq;

namespace DualScreenDemo
{
    public partial class PrimaryForm
    {
        private void MedleyDanceButton_Click(object sender, EventArgs e)
        {
            loveDuetButton.BackgroundImage = loveDuetNormalBackground;
            talentShowButton.BackgroundImage = talentShowNormalBackground;
            medleyDanceButton.BackgroundImage = medleyDanceActiveBackground;
            ninetiesButton.BackgroundImage = ninetiesNormalBackground;
            nostalgicSongsButton.BackgroundImage = nostalgicSongsNormalBackground;
            chinaSongsButton.BackgroundImage = chinaNormalBackground;
            // vietnameseSongsButton.BackgroundImage = vietnamNormalBackground;

            medleyDanceSongs = allSongs.Where(song => song.SongGenre.Contains("C1"))
                                .OrderByDescending(song => song.Plays) // 根据点播次数降序排列
                                .ToList();
            currentPage = 0; // 重置到第一页
            currentSongList = medleyDanceSongs; // 更新当前显示的歌曲列表
            totalPages = (int)Math.Ceiling((double)medleyDanceSongs.Count / itemsPerPage);

            multiPagePanel.currentPageIndex = 0;
            multiPagePanel.LoadSongs(currentSongList);
        }
    }
}