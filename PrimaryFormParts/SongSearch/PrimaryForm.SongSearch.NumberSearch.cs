using System;
using System.Drawing; // Bitmap 和 Rectangle
using System.IO; // Path
using System.Linq; // Linq
using System.Windows.Forms; // Application

namespace DualScreenDemo
{
    public partial class PrimaryForm
    {
        private void NumberSearchButton2_Click(object sender, EventArgs e)
        {
            zhuyinSearchSongButton.BackgroundImage = zhuyinSearchSongNormalBackground;
            englishSearchSongButton.BackgroundImage = englishSearchSongNormalBackground;
            pinyinSearchSongButton.BackgroundImage = pinyinSearchSongNormalBackground;
            wordCountSearchSongButton.BackgroundImage = wordCountSearchSongNormalBackground;
            handWritingSearchSongButton.BackgroundImage = handWritingSearchNormalBackground;
            numberSearchSongButton.BackgroundImage = numberSearchSongActiveBackground;

            ShowImageOnPictureBoxSongIDSearch(Path.Combine(Application.StartupPath, @"themes\superstar\6-1.png"));

            SetPictureBoxSongIDSearchAndButtonsVisibility(true);
            pictureBoxSongIDSearch.Visible = true;
        }

        private void ShowImageOnPictureBoxSongIDSearch(string imagePath)
        {
            // 加载原始图像
            Bitmap originalImage = new Bitmap(imagePath);

            // 定义裁剪区域
            Rectangle cropArea = new Rectangle(593, 135, 507, 508);

            // 裁剪图像
            Bitmap croppedImage = CropImage(originalImage, cropArea);

            // 设置裁剪后的图像为 PictureBox 的图像
            pictureBoxSongIDSearch.Image = croppedImage;
    
            // 调整 PictureBox 的大小以匹配裁剪后的图像大小并设置位置
            ResizeAndPositionPictureBox(pictureBoxSongIDSearch, cropArea.X + offsetXSongID, cropArea.Y + offsetYSongID, cropArea.Width, cropArea.Height);
            
            pictureBoxSongIDSearch.Visible = true;
        }

        private void ModifyButtonSongIDSearch_Click(object sender, EventArgs e)
        {
            // 检查注音输入框是否存在
            if (inputBoxSongIDSearch.Text.Length > 0)
            {
                // Logic for Modify button
                inputBoxSongIDSearch.Text = inputBoxSongIDSearch.Text.Substring(0, inputBoxSongIDSearch.Text.Length - 1);
            }
        }

        private void CloseButtonSongIDSearch_Click(object sender, EventArgs e)
        {
            // Logic for Close button
            SetPictureBoxSongIDSearchAndButtonsVisibility(false);
        }

        private void SetPictureBoxSongIDSearchAndButtonsVisibility(bool isVisible)
        {
            System.Action action = () =>
            {
                SuspendLayout();

                // Set the visibility of PictureBoxWordCount
                pictureBoxSongIDSearch.Visible = isVisible;

                // Ensure pictureBoxSongIDSearch is brought to the front if it is made visible
                if (isVisible) pictureBoxSongIDSearch.BringToFront();

                // Set visibility for modify and close buttons
                modifyButtonSongIDSearch.Visible = isVisible;
                closeButtonSongIDSearch.Visible = isVisible;

                // If visible, make sure these buttons are also brought to the front
                if (isVisible)
                {
                    modifyButtonSongIDSearch.BringToFront();
                    closeButtonSongIDSearch.BringToFront();
                }

                // Iterate through all number buttons and set their visibility
                foreach (Button button in numberButtonsSongIDSearch)
                {
                    button.Visible = isVisible;
                    // Optionally, ensure that the button is brought to the front if it becomes visible.
                    if (isVisible)
                        button.BringToFront();
                }

                inputBoxSongIDSearch.Visible = isVisible;
                if (isVisible) inputBoxSongIDSearch.BringToFront();

                ResumeLayout();
            };

            if (this.InvokeRequired)
            {
                this.Invoke(action);
            }
            else
            {
                action();
            }
        }
        
        private void InitializeButtonsForPictureBoxSongIDSearch()
        {
            // Coordinates for the number buttons as per the provided mapping
            int[,] coords = new int[,]
            {
                {651, 292, 752, 400}, // 1
                {760, 292, 861, 400}, // 2
                {869, 292, 972, 399}, // 3
                {652, 401, 752, 502}, // 4
                {760, 401, 861, 504}, // 5
                {869, 398, 972, 502}, // 6
                {651, 502, 753, 607}, // 7
                {759, 504, 863, 607}, // 8
                {869, 503, 973, 608}, // 9
                {981, 501, 1083, 609} // 0
            };

            int screenW = Screen.PrimaryScreen.Bounds.Width;
            int screenH = Screen.PrimaryScreen.Bounds.Height;

            // 假定1440是设计时的屏幕宽度，900是设计时的屏幕高度
            float widthRatio = screenW / (float)1440;
            float heightRatio = screenH / (float)900;

            numberButtonsSongIDSearch = new Button[10];

            for (int i = 0; i < numberButtonsSongIDSearch.Length; i++)
            {
                numberButtonsSongIDSearch[i] = new Button();

                // Here, ConfigureButton() is assumed to set the button's position, size, and potentially other properties
                ConfigureButton(
                    numberButtonsSongIDSearch[i],
                    coords[i, 0], // x1
                    coords[i, 1], // y1
                    coords[i, 2] - coords[i, 0], // width
                    coords[i, 3] - coords[i, 1], // height
                    /* Other parameters, such as images for different states, if used in ConfigureButton */
                    resizedNormalStateImageFor6_1,
                    resizedMouseOverImageFor6_1,
                    resizedMouseDownImageFor6_1,
                    null
                );

                int newXForSongID = (int)(((numberButtonsSongIDSearch[i].Location.X / widthRatio) + offsetXSongID) * widthRatio);
                int newYForSongID = (int)(((numberButtonsSongIDSearch[i].Location.Y / heightRatio) + offsetYSongID) * heightRatio);
                numberButtonsSongIDSearch[i].Location = new Point(newXForSongID, newYForSongID);

                // Set unique names for each number button
                numberButtonsSongIDSearch[i].Name = "NumberButtonSongIDSearch" + i;
                // Set button tag
                numberButtonsSongIDSearch[i].Tag = (i + 1).ToString();
                if (i == 9) // Adjust for 0 being the last button
                {
                    numberButtonsSongIDSearch[i].Name = "NumberButtonSongIDSearch0";
                    numberButtonsSongIDSearch[i].Tag = "0";
                }

                // Add event handler for clicks, assuming WordCountButton_Click is defined
                numberButtonsSongIDSearch[i].Click += SongIDSearchButton_Click;

                // Add the button to the form's controls or to pictureBoxWordCount if it's being used as a container
                this.Controls.Add(numberButtonsSongIDSearch[i]);
            }

            // Modify Button
            modifyButtonSongIDSearch = new Button {
                Name = "ModifyButtonSongIDSearch",
                Tag = "Modify",
                Visible = false
            };
            // Coordinates for Modify button from the map: (978, 292) to (1081, 397)
            ConfigureButton(modifyButtonSongIDSearch, 978, 292, 1081 - 978, 397 - 292, resizedNormalStateImageFor6_1, resizedMouseOverImageFor6_1, resizedMouseDownImageFor6_1, ModifyButtonSongIDSearch_Click);
            int newX = (int)(((modifyButtonSongIDSearch.Location.X / widthRatio) + offsetXSongID) * widthRatio);
            int newY = (int)(((modifyButtonSongIDSearch.Location.Y / widthRatio) + offsetYSongID) * heightRatio);
            modifyButtonSongIDSearch.Location = new Point(newX, newY);
            this.Controls.Add(modifyButtonSongIDSearch);
            
            // Close Button
            closeButtonSongIDSearch = new Button {
                Name = "CloseButtonSongIDSearch",
                Tag = "Close",
                Visible = false
            };
            // Coordinates for Close button from the map: (982, 147) to (1082, 250)
            ConfigureButton(closeButtonSongIDSearch, 982, 147, 1082 - 982, 250 - 147, resizedNormalStateImageFor6_1, resizedMouseOverImageFor6_1, resizedMouseDownImageFor6_1, CloseButtonSongIDSearch_Click);
            newX = (int)(((closeButtonSongIDSearch.Location.X / widthRatio) + offsetXSongID) * widthRatio);
            newY = (int)(((closeButtonSongIDSearch.Location.Y / widthRatio) + offsetYSongID) * heightRatio);
            closeButtonSongIDSearch.Location = new Point(newX, newY);
            this.Controls.Add(closeButtonSongIDSearch);

            inputBoxSongIDSearch = new RichTextBox();
            inputBoxSongIDSearch.Name = "inputBoxSongIDSearch";
            ResizeAndPositionControl(inputBoxSongIDSearch, 645 + offsetXSongID, 197 + offsetXSongID, 986 - 645, 281 - 197);
            inputBoxSongIDSearch.ForeColor = Color.Black;
            inputBoxSongIDSearch.Font = new Font("細明體", (float)26 / 900 * Screen.PrimaryScreen.Bounds.Height, FontStyle.Regular);

            inputBoxSongIDSearch.TextChanged += (sender, e) =>
            {
                string searchText = inputBoxSongIDSearch.Text;
                // 假设 allSongs 是存储所有歌曲数据的 List<SongData>
                var searchResults = allSongs.Where(song => song.SongNumber.StartsWith(searchText)).ToList();
                currentPage = 0;
                currentSongList = searchResults;
                totalPages = (int)Math.Ceiling((double)searchResults.Count / itemsPerPage);
                
                multiPagePanel.currentPageIndex = 0;
                multiPagePanel.LoadSongs(currentSongList);
            };

            this.Controls.Add(inputBoxSongIDSearch);
        }

        // Define the SongIDSearchButton_Click event handler
        private void SongIDSearchButton_Click(object sender, EventArgs e)
        {
            // Assuming 'inputBox' is the TextBox where you want to show the phonetic symbol
            // and that each button has a Tag property set to its corresponding phonetic symbol
            var button = sender as Button;
            if (button != null && button.Tag != null)
            {
                inputBoxSongIDSearch.Text += button.Tag.ToString();
            }
        }
    }
}