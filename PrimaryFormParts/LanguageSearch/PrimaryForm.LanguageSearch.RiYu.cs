using System;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;

namespace DualScreenDemo
{
    public partial class PrimaryForm
    {
        private void InitializeRiYuButton()
        {
            Rectangle riYuButtonCropArea = new Rectangle(1214, 481, 209, 59);
            InitializeButton(ref riYuButton, "日語", 1214, 481, 209, 59, riYuButtonCropArea, normalStateImageLanguageQuery, out riYuNormalBackground, mouseDownImageLanguageQuery, out riYuActiveBackground, RiYuButton_Click);
        }

        private void RiYuButton_Click(object sender, EventArgs e)
        {
            OnLanguageButtonClick(riYuButton, riYuActiveBackground, "日語");
        }
    }
}