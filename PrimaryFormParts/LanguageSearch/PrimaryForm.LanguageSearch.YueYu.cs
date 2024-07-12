using System;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;

namespace DualScreenDemo
{
    public partial class PrimaryForm
    {
        private void InitializeYueYuButton()
        {
            Rectangle yueYuButtonCropArea = new Rectangle(1214, 356, 209, 58);
            InitializeButton(ref yueYuButton, "粵語", 1214, 356, 209, 58, yueYuButtonCropArea, normalStateImageLanguageQuery, out yueYuNormalBackground, mouseDownImageLanguageQuery, out yueYuActiveBackground, YueYuButton_Click);
        }

        private void YueYuButton_Click(object sender, EventArgs e)
        {
            OnLanguageButtonClick(yueYuButton, yueYuActiveBackground, "粵語");
        }
    }
}