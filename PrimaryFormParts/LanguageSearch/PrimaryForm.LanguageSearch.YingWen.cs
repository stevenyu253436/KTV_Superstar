using System;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;

namespace DualScreenDemo
{
    public partial class PrimaryForm
    {
        private void InitializeYingWenButton()
        {
            Rectangle yingWenButtonCropArea = new Rectangle(1214, 418, 209, 59);
            InitializeButton(ref yingWenButton, "英文", 1214, 418, 209, 59, yingWenButtonCropArea, normalStateImageLanguageQuery, out yingWenNormalBackground, mouseDownImageLanguageQuery, out yingWenActiveBackground, YingWenButton_Click);
        }

        private void YingWenButton_Click(object sender, EventArgs e)
        {
            OnLanguageButtonClick(yingWenButton, yingWenActiveBackground, "英文");
        }
    }
}