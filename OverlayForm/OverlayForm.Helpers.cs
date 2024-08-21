// OverlayForm/OverlayForm.Helpers.cs
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace DualScreenDemo
{
    public partial class OverlayForm
    {
        private readonly object imageLock = new object();        

        private void AdjustLabelPositions()
        {
            int labelHeight = displayLabels.First().Height;
            int totalHeight = displayLabels.Count * labelHeight;
            int startY = 100;

            for (int i = 0; i < displayLabels.Count; i++)
            {
                Label label = displayLabels[i];
                int centerX = (this.Width - label.Width) / 2;
                int centerY = startY + i * labelHeight;
                label.Location = new Point(centerX, centerY);
            }

            if (pauseLabel != null)
            {
                // Adjust the pauseLabel position to the top-right corner
                pauseLabel.Location = new Point(this.Width - pauseLabel.Width - 10, 100);
            }

            if (muteLabel != null)
            {
                // Adjust the muteLabel position to the top-right corner below the pauseLabel
                muteLabel.Location = new Point(this.Width - muteLabel.Width - 10, 140);
            }
        }

        public void UpdateMarqueeText(string newText, MarqueeStartPosition startPosition, Color textColor)
        {
            this.marqueeText = newText;
            this.marqueeTextColor = textColor; // 新增：保存文本颜色

            // 根据开始位置参数设置文本的X位置
            switch (startPosition)
            {
                case MarqueeStartPosition.Middle:
                    // 确保从屏幕中间开始
                    Size textSize = TextRenderer.MeasureText(marqueeText, this.Font);
                    int textWidth = textSize.Width;
                    this.marqueeXPos = (this.Width / 2) - (textWidth / 2) - 100;
                    break;
                case MarqueeStartPosition.Right:
                    this.marqueeXPos = this.Width;
                    break;
            }

            // 强制窗体重绘以更新跑马灯
            this.Invalidate();
        }

        public void UpdateMarqueeTextSecondLine(string newText)
        {
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(() => UpdateMarqueeTextSecondLine(newText)));
                return;
            }
            marqueeTextSecondLine = newText;
            
            // 使用 Graphics 计算文本宽度
            using (Graphics graphics = this.CreateGraphics())
            {
                float textWidth = MeasureDisplayStringWidth(graphics, marqueeTextSecondLine, new Font("微軟正黑體", 40, FontStyle.Bold));
                marqueeXPosSecondLine = (int)((this.Width - textWidth) / 2); // 居中显示，并将 float 转换为 int
            }

            // 启动计时器
            secondLineStartTime = DateTime.Now;
            secondLineTimer.Start();

            // 启动显示定时器，30秒后隐藏文本
            secondLineDisplayTimer.Start();
        }

        public void UpdateMarqueeTextThirdLine(string newText)
        {
            // 打印消息，查看方法是否被调用以及传入的新文本是什么
            Console.WriteLine("UpdateMarqueeTextThirdLine called with text: " + newText);

            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(() => UpdateMarqueeTextThirdLine(newText)));
                return;
            }
            marqueeTextThirdLine = newText;
            marqueeXPosThirdLine = this.Width; // 重置第二行的位置

            // 打印更新后的文本位置
            Console.WriteLine("Marquee text position reset to: " + marqueeXPosThirdLine);

            // 请求重绘界面以显示更新的跑马灯
            Invalidate();
        }

        private void MarqueeTimer_Tick(object sender, EventArgs e)
        {
            // 更新跑馬燈位置
            marqueeXPos -= 2; // 每次移動 5 像素，速度可以調整
            // marqueeXPosSecondLine -= 2; // 第二行也同样移动
            marqueeXPosThirdLine -= 2;

            if (marqueeXPos < -MeasureDisplayStringWidth(this.CreateGraphics(), marqueeText, this.Font))
                marqueeXPos = this.Width;
            
            // if (!string.IsNullOrEmpty(marqueeTextSecondLine))
            // {
            //     marqueeXPosSecondLine -= 2; // 第二行文本也移动

            //     // 使用 Graphics 计算文本宽度
            //     using (Graphics graphics = this.CreateGraphics())
            //     {
            //         float textWidth = MeasureDisplayStringWidth(graphics, marqueeTextSecondLine, new Font("微軟正黑體", 24, FontStyle.Bold));
                    
            //         // 检查第二行是否到达屏幕的1/4位置
            //         if (marqueeXPosSecondLine < (int)(this.Width / 4 - textWidth))
            //         {
            //             marqueeXPosSecondLine = this.Width * 3 / 4;
            //         }
            //     }
            // }

            if (!string.IsNullOrEmpty(marqueeTextThirdLine))
            {
                marqueeXPosThirdLine -= 2; // 第二行文本也移动

                if (marqueeXPosThirdLine < -MeasureDisplayStringWidth(this.CreateGraphics(), marqueeTextThirdLine, this.Font))
                {
                    marqueeXPosThirdLine = this.Width; // 如果您想让它只显示一次然后停止，这里应改为隐藏或清空文本
                    marqueeTextThirdLine = ""; // 清空第二行文本
                    // 如果需要，可以在这里停止关于第二行的定时器或进一步的更新逻辑
                    // 比如，停止定时器或设置标志避免再次进入这个逻辑块
                }
            }

            // 要求重繪視窗以更新文字位置
            this.Invalidate();
        }

        private float MeasureDisplayStringWidth(Graphics graphics, string text, Font font)
        {
            // 測量字符串的寬度
            StringFormat format = new StringFormat();
            RectangleF rect = new RectangleF(0, 0, 1000, 1000);
            CharacterRange[] ranges = { new CharacterRange(0, text.Length) };
            Region[] regions = new Region[1];

            format.SetMeasurableCharacterRanges(ranges);
            regions = graphics.MeasureCharacterRanges(text, font, rect, format);
            rect = regions[0].GetBounds(graphics);

            return rect.Width;
        }
    }
}