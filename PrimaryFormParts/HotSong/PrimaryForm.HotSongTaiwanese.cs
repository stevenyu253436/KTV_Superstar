using System;
using System.Windows.Forms;
using System.Drawing;
using System.Linq; // Add this line

namespace DualScreenDemo
{
    public partial class PrimaryForm
    {
        private void TaiYuButtonHotSong_Click(object sender, EventArgs e)
        {
            OnHotSongButtonClick(taiYuButtonHotSong, taiYuHotSongActiveBackground, "台語");
        }
    }
}