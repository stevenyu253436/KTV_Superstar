using System;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;

namespace DualScreenDemo
{
    public partial class PrimaryForm
    {
        private void InitializeGuoYuButton()
        {
            Rectangle guoYuButtonCropArea = new Rectangle(1214, 230, 209, 59);
            InitializeButton(ref guoYuButton, "國語", 1214, 230, 209, 59, guoYuButtonCropArea, normalStateImageLanguageQuery, out guoYuNormalBackground, mouseDownImageLanguageQuery, out guoYuActiveBackground, GuoYuButton_Click);
        }

        private void GuoYuButton_Click(object sender, EventArgs e)
        {
            OnLanguageButtonClick(guoYuButton, guoYuActiveBackground, "國語");
        }
    }
}