using System;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;

namespace DualScreenDemo
{
    public partial class PrimaryForm
    { 
        private void InitializeKeYuButton()
        {
            Rectangle keYuButtonCropArea = new Rectangle(1214, 607, 209, 58);
            InitializeButton(ref keYuButton, "客語", 1214, 607, 209, 58, keYuButtonCropArea, normalStateImageLanguageQuery, out keYuNormalBackground, mouseDownImageLanguageQuery, out keYuActiveBackground, KeYuButton_Click);
        }

        private void KeYuButton_Click(object sender, EventArgs e)
        {
            OnLanguageButtonClick(keYuButton, keYuActiveBackground, "客語");
        }
    }
}