using System;
using System.Linq; // 為了使用 Linq 的擴展方法
using System.Collections.Generic; // 為了使用 List<T>

namespace DualScreenDemo
{
    public partial class PrimaryForm
    {        
        private void TalentShowButton_Click(object sender, EventArgs e)
        {
            loveDuetButton.BackgroundImage = loveDuetNormalBackground;
            talentShowButton.BackgroundImage = talentShowActiveBackground;
            medleyDanceButton.BackgroundImage = medleyDanceNormalBackground;
            ninetiesButton.BackgroundImage = ninetiesNormalBackground;
            nostalgicSongsButton.BackgroundImage = nostalgicSongsNormalBackground;
            chinaSongsButton.BackgroundImage = chinaNormalBackground;
            // vietnameseSongsButton.BackgroundImage = vietnamNormalBackground;

            talentShowSongs = allSongs.Where(song => song.SongGenre.Contains("B1"))
                                .OrderByDescending(song => song.Plays) // 根据点播次数降序排列
                                .ToList();
            currentPage = 0; // 重置到第一页
            currentSongList = talentShowSongs; // 更新当前显示的歌曲列表
            totalPages = (int)Math.Ceiling((double)talentShowSongs.Count / itemsPerPage);

            multiPagePanel.currentPageIndex = 0;
            multiPagePanel.LoadSongs(currentSongList);
        }
    }
}