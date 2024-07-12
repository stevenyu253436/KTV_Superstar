using System;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;

namespace DualScreenDemo
{
    public partial class PrimaryForm
    {
        private void InitializeTaiYuButton()
        {
            Rectangle taiYuButtonCropArea = new Rectangle(1214, 293, 209, 58);
            InitializeButton(ref taiYuButton, "台語", 1214, 293, 209, 58, taiYuButtonCropArea, normalStateImageLanguageQuery, out taiYuNormalBackground, mouseDownImageLanguageQuery, out taiYuActiveBackground, TaiYuButton_Click);
        }

        private void TaiYuButton_Click(object sender, EventArgs e)
        {
            OnLanguageButtonClick(taiYuButton, taiYuActiveBackground, "台語");
        }
    }
}
