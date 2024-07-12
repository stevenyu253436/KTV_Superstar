using System;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;

namespace DualScreenDemo
{
    public partial class PrimaryForm
    { 
        private void InitializeHanYuButton()
        {
            Rectangle hanYuButtonCropArea = new Rectangle(1214, 544, 209, 58);
            InitializeButton(ref hanYuButton, "韓語", 1214, 544, 209, 58, hanYuButtonCropArea, normalStateImageLanguageQuery, out hanYuNormalBackground, mouseDownImageLanguageQuery, out hanYuActiveBackground, HanYuButton_Click);
        }

        private void HanYuButton_Click(object sender, EventArgs e)
        {
            OnLanguageButtonClick(hanYuButton, hanYuActiveBackground, "韓語");
        }
    }
}