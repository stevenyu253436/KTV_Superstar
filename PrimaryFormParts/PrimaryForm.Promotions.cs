using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace DualScreenDemo
{
    public partial class PrimaryForm
    {
        private Button promotionsButton;
        private Bitmap promotionsNormalBackground;
        private Bitmap promotionsActiveBackground;
        private Button previousPromotionButton;
        private Button nextPromotionButton;
        private Button closePromotionsButton;
        
        private void InitializePromotionsButton()
        {
            // 初始化上一頁按钮
            previousPromotionButton = new Button
            {
                // Text = "上一頁",
                Name = "previousPromotionButton",
                Visible = true // 初始时可见
            };
            ConfigureButton(previousPromotionButton, 1199, 815, 65, 63, resizedNormalStateImageForPromotionsAndMenu, resizedNormalStateImageForPromotionsAndMenu, resizedNormalStateImageForPromotionsAndMenu, PreviousPromotionButton_Click);

            // 初始化下一頁按钮
            nextPromotionButton = new Button
            {
                // Text = "下一頁",
                Name = "nextPromotionButton",
                Visible = true // 初始时可见
            };
            ConfigureButton(nextPromotionButton, 1353, 814, 64, 64, resizedNormalStateImageForPromotionsAndMenu, resizedNormalStateImageForPromotionsAndMenu, resizedNormalStateImageForPromotionsAndMenu, NextPromotionButton_Click);

            // 初始化优惠活动按钮
            closePromotionsButton  = new Button
            {
                // Text = "退出",
                Name = "closePromotionsButton",
                Visible = false
            };
            ConfigureButton(closePromotionsButton, 1275, 814, 65, 65, resizedNormalStateImageForPromotionsAndMenu, resizedNormalStateImageForPromotionsAndMenu, resizedNormalStateImageForPromotionsAndMenu, ClosePromotionsButton_Click);
        }

        private List<Image> LoadPromotionsImages()
        {
            List<Image> images = new List<Image>();
            string newsFolderPath = Path.Combine(Application.StartupPath, "news");

            string[] imageFiles = Directory.GetFiles(newsFolderPath, "*.jpg");

            foreach (string filePath in imageFiles)
            {
                try
                {
                    images.Add(Image.FromFile(filePath));
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error loading image: " + filePath + ". Exception: " + ex.Message);
                }
            }

            return images;
        }

        private void promotionsButton_Click(object sender, EventArgs e)
        {
            newSongAlertButton.BackgroundImage = newSongAlertNormalBackground;
            hotPlayButton.BackgroundImage = hotPlayNormalBackground;
            singerSearchButton.BackgroundImage = singerSearchNormalBackground;
            songSearchButton.BackgroundImage = songSearchNormalBackground;
            languageSearchButton.BackgroundImage = languageSearchNormalBackground;
            groupSearchButton.BackgroundImage = groupSearchNormalBackground;
            categorySearchButton.BackgroundImage = categorySearchNormalBackground;
            orderedSongsButton.BackgroundImage = orderedSongsNormalBackground;
            myFavoritesButton.BackgroundImage = myFavoritesNormalBackground;
            promotionsButton.BackgroundImage = promotionsActiveBackground;
            deliciousFoodButton.BackgroundImage = deliciousFoodNormalBackground;
            // 显示当前优惠活动的图片
            // DisplayCurrentPromotion();
            promotionsAndMenuPanel.LoadImages(promotions); // You can call different methods if needed
            promotionsAndMenuPanel.Visible = true;
            promotionsAndMenuPanel.BringToFront();

            previousPromotionButton.Visible = true;
            previousPromotionButton.BringToFront();
            nextPromotionButton.Visible = true;
            nextPromotionButton.BringToFront();
            // 显示关闭按钮
            closePromotionsButton.Visible = true;
            closePromotionsButton.BringToFront();

            // 切换pictureBoxQRCode的可见性
            if (pictureBoxQRCode != null)
            {
                pictureBoxQRCode.Visible = false;
                closeQRCodeButton.Visible = false;
            }

            SetPictureBoxToggleLightAndButtonsVisibility(false);
        }

        private void PreviousPromotionButton_Click(object sender, EventArgs e)
        {
            // // 减少当前优惠活动索引
            // currentPromotionIndex--;
            // if (currentPromotionIndex < 0)
            //     currentPromotionIndex = 0; // 防止索引越界
            // DisplayCurrentPromotion(); // 显示当前优惠活动
            promotionsAndMenuPanel.LoadPreviousPage();
        }

        private void NextPromotionButton_Click(object sender, EventArgs e)
        {
            // // 增加当前优惠活动索引
            // currentPromotionIndex++;
            // if (currentPromotionIndex >= promotionImagePaths.Count)
            //     currentPromotionIndex = promotionImagePaths.Count - 1; // 防止索引越界
            // DisplayCurrentPromotion(); // 显示当前优惠活动
            promotionsAndMenuPanel.LoadNextPage();
        }

        private void ClosePromotionsButton_Click(object sender, EventArgs e)
        {
            // promotionsPictureBox.Visible = false;
            promotionsAndMenuPanel.Visible = false;
            previousPromotionButton.Visible = false;
            nextPromotionButton.Visible = false;
            closePromotionsButton.Visible = false;

            HotPlayButton_Click(sender, e);
        }
    }
}