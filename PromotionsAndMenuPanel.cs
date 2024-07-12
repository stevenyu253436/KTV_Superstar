using System;
using System.Collections.Generic;
using System.Drawing; // Image, Point
using System.Windows.Forms; // Panel, MouseEventArgs

namespace DualScreenDemo
{
    public class PromotionsAndMenuPanel : Panel
    {
        public ImagePanel prevPagePanel;
        public ImagePanel currentPagePanel;
        public ImagePanel nextPagePanel;

        private List<Image> promotionsAndMenuImages;
        private const int ImagesPerPage = 1; // 每页一张图片
        public int currentPageIndex = 0;

        private Point initialMousePosition;
        private bool isDragging = false;
        private const int DragThreshold = 50;
        private int maxPositiveDeltaX = 0;
        private int maxNegativeDeltaX = 0;
        private const int ShiftThreshold = 150;

        public PromotionsAndMenuPanel()
        {
            this.DoubleBuffered = true;
            this.AutoScroll = false;

            InitializePages();

            // 注册鼠标事件
            this.MouseDown += PromotionsAndMenuPanel_MouseDown;
            this.MouseMove += PromotionsAndMenuPanel_MouseMove;
            this.MouseUp += PromotionsAndMenuPanel_MouseUp;
        }

        private void InitializePages()
        {
            prevPagePanel = new ImagePanel();
            currentPagePanel = new ImagePanel();
            nextPagePanel = new ImagePanel();

            PrimaryForm.ResizeAndPositionControl(prevPagePanel, -1440, 0, 1440, 900);
            PrimaryForm.ResizeAndPositionControl(currentPagePanel, 0, 0, 1440, 900);
            PrimaryForm.ResizeAndPositionControl(nextPagePanel, 1440, 0, 1440, 900);

            this.Controls.Add(prevPagePanel);
            this.Controls.Add(currentPagePanel);
            this.Controls.Add(nextPagePanel);
        }

        public void LoadImages(List<Image> images)
        {
            promotionsAndMenuImages = images;
            LoadPage(currentPageIndex - 1);
            LoadPage(currentPageIndex);
            LoadPage(currentPageIndex + 1);
        }

        private void LoadPage(int pageIndex)
        {
            ImagePanel targetPanel = IdentifyTargetPanel(pageIndex);
            targetPanel.Controls.Clear();

            if (pageIndex < 0 || pageIndex * ImagesPerPage >= promotionsAndMenuImages.Count)
            {
                return;
            }

            int start = pageIndex * ImagesPerPage;
            int end = Math.Min(start + ImagesPerPage, promotionsAndMenuImages.Count);

            for (int i = start; i < end; i++)
            {
                var image = promotionsAndMenuImages[i];
                AddImagePanel(image, targetPanel);
            }
        }

        private ImagePanel IdentifyTargetPanel(int pageIndex)
        {
            if (pageIndex == currentPageIndex - 1)
                return prevPagePanel;
            else if (pageIndex == currentPageIndex + 1)
                return nextPagePanel;
            else
                return currentPagePanel;
        }

        private void AddImagePanel(Image image, Panel targetPanel)
        {
            PictureBox pictureBox = new PictureBox()
            {
                Image = image,
                SizeMode = PictureBoxSizeMode.StretchImage,
                Location = new Point(0, 0),
                Size = new Size(targetPanel.Width, targetPanel.Height)
            };

            // 注册PictureBox的鼠标事件
            pictureBox.MouseDown += PromotionsAndMenuPanel_MouseDown;
            pictureBox.MouseMove += PromotionsAndMenuPanel_MouseMove;
            pictureBox.MouseUp += PromotionsAndMenuPanel_MouseUp;

            targetPanel.Controls.Add(pictureBox);
        }

        private void PromotionsAndMenuPanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                initialMousePosition = e.Location;
                isDragging = true;
                this.Capture = true;
            }
        }

        private void PromotionsAndMenuPanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                int deltaX = e.X - initialMousePosition.X;

                if (deltaX > maxPositiveDeltaX) maxPositiveDeltaX = deltaX;
                if (deltaX < maxNegativeDeltaX) maxNegativeDeltaX = deltaX;

                ShiftPages(deltaX);
            }
        }

        private void PromotionsAndMenuPanel_MouseUp(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                int deltaX = e.X - initialMousePosition.X;
                isDragging = false;
                this.Capture = false;
                FinishShift(deltaX);
            }
        }

        public void ShiftPages(int deltaX)
        {
            int newLeft = currentPagePanel.Left + deltaX;

            if (newLeft > 1440) newLeft = 1440;
            if (newLeft < -1440) newLeft = -1440;

            prevPagePanel.Location = new Point(newLeft - 1440, 0);
            currentPagePanel.Location = new Point(newLeft, 0);
            nextPagePanel.Location = new Point(newLeft + 1440, 0);

            this.Invalidate();
            this.Update();
        }

        public void FinishShift(int deltaX)
        {
            if (maxPositiveDeltaX > ShiftThreshold)
            {
                LoadPreviousPage();
            }
            else if (maxNegativeDeltaX < -ShiftThreshold)
            {
                LoadNextPage();
            }

            prevPagePanel.Location = new Point(-1560, 0);
            currentPagePanel.Location = new Point(0, 0);
            nextPagePanel.Location = new Point(1440, 0);

            maxPositiveDeltaX = 0;
            maxNegativeDeltaX = 0;
        }

        public void LoadPreviousPage()
        {
            if (currentPageIndex > 0)
                currentPageIndex--;
            LoadPage(currentPageIndex - 1);
            LoadPage(currentPageIndex);
            LoadPage(currentPageIndex + 1);
        }

        public void LoadNextPage()
        {
            if (currentPageIndex < (promotionsAndMenuImages.Count - 1) / ImagesPerPage)
                currentPageIndex++;
            LoadPage(currentPageIndex - 1);
            LoadPage(currentPageIndex);
            LoadPage(currentPageIndex + 1);
        }
    }
}