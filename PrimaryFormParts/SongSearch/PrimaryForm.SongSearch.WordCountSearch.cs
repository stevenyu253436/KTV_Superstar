using System;
using System.Drawing; // For Point, Color, Font, FontStyle
using System.Linq; // For Where extension method
using System.Windows.Forms;
using System.IO;

namespace DualScreenDemo
{
    public partial class PrimaryForm
    {
        // 定义事件处理器
        private void WordCountSearchSong_Click(object sender, EventArgs e)
        {
            zhuyinSearchSongButton.BackgroundImage = zhuyinSearchSongNormalBackground;
            englishSearchSongButton.BackgroundImage = englishSearchSongNormalBackground;
            pinyinSearchSongButton.BackgroundImage = pinyinSearchSongNormalBackground;
            wordCountSearchSongButton.BackgroundImage = wordCountSearchSongActiveBackground;
            handWritingSearchSongButton.BackgroundImage = handWritingSearchNormalBackground;
            numberSearchSongButton.BackgroundImage = numberSearchSongNormalBackground;

            ShowImageOnPictureBoxWordCount(Path.Combine(Application.StartupPath, @"themes\superstar\6-1.png"));

            SetPictureBoxWordCountAndButtonsVisibility(true);
            pictureBoxWordCount.Visible = true;
        }

        private void ShowImageOnPictureBoxWordCount(string imagePath)
        {
            // 加载原始图像
            Bitmap originalImage = new Bitmap(imagePath);

            // 定义裁剪区域
            Rectangle cropArea = new Rectangle(593, 135, 507, 508);

            // 裁剪图像
            Bitmap croppedImage = CropImage(originalImage, cropArea);

            // 设置裁剪后的图像为 PictureBox 的图像
            pictureBoxWordCount.Image = croppedImage;
    
            // 调整 PictureBox 的大小以匹配裁剪后的图像大小并设置位置
            ResizeAndPositionPictureBox(pictureBoxWordCount, cropArea.X + offsetXWordCount, cropArea.Y + offsetXWordCount, cropArea.Width, cropArea.Height);
            
            pictureBoxWordCount.Visible = true;
        }

        private void InitializeButtonsForPictureBoxWordCount()
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

            numberButtonsWordCount = new Button[10];

            for (int i = 0; i < numberButtonsWordCount.Length; i++)
            {
                numberButtonsWordCount[i] = new Button();

                // Here, ConfigureButton() is assumed to set the button's position, size, and potentially other properties
                ConfigureButton(
                    numberButtonsWordCount[i],
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

                int newXForWordCount = (int)(((numberButtonsWordCount[i].Location.X / widthRatio) + offsetXWordCount) * widthRatio);
                int newYForWordCount = (int)(((numberButtonsWordCount[i].Location.Y / heightRatio) + offsetYWordCount) * heightRatio);
                numberButtonsWordCount[i].Location = new Point(newXForWordCount, newYForWordCount);

                // Set unique names for each number button
                numberButtonsWordCount[i].Name = "NumberButtonWordCount" + i;
                // Set button tag
                numberButtonsWordCount[i].Tag = (i + 1).ToString();
                if (i == 9) // Adjust for 0 being the last button
                {
                    numberButtonsWordCount[i].Name = "NumberButtonWordCount0";
                    numberButtonsWordCount[i].Tag = "0";
                }

                // Add event handler for clicks, assuming WordCountButton_Click is defined
                numberButtonsWordCount[i].Click += WordCountButton_Click;

                // Add the button to the form's controls or to pictureBoxWordCount if it's being used as a container
                this.Controls.Add(numberButtonsWordCount[i]);
            }

            // Modify Button
            modifyButtonWordCount = new Button {
                Name = "ModifyButtonWordCount",
                Tag = "Modify",
                Visible = false
            };
            // Coordinates for Modify button from the map: (978, 292) to (1081, 397)
            ConfigureButton(modifyButtonWordCount, 978, 292, 1081 - 978, 397 - 292, resizedNormalStateImageFor6_1, resizedMouseOverImageFor6_1, resizedMouseDownImageFor6_1, ModifyButtonWordCount_Click);
            int newX = (int)(((modifyButtonWordCount.Location.X / widthRatio) + offsetXWordCount) * widthRatio);
            int newY = (int)(((modifyButtonWordCount.Location.Y / widthRatio) + offsetYWordCount) * heightRatio);
            modifyButtonWordCount.Location = new Point(newX, newY);
            this.Controls.Add(modifyButtonWordCount);
            
            // Close Button
            closeButtonWordCount = new Button {
                Name = "CloseButtonWordCount",
                Tag = "Close",
                Visible = false
            };
            // Coordinates for Close button from the map: (982, 147) to (1082, 250)
            ConfigureButton(closeButtonWordCount, 982, 147, 1082 - 982, 250 - 147, resizedNormalStateImageFor6_1, resizedMouseOverImageFor6_1, resizedMouseDownImageFor6_1, CloseButtonWordCount_Click);
            newX = (int)(((closeButtonWordCount.Location.X / widthRatio) + offsetXWordCount) * widthRatio);
            newY = (int)(((closeButtonWordCount.Location.Y / widthRatio) + offsetYWordCount) * heightRatio);
            closeButtonWordCount.Location = new Point(newX, newY);
            this.Controls.Add(closeButtonWordCount);

            inputBoxWordCount = new RichTextBox();
            inputBoxWordCount.Name = "inputBoxWordCount";
            ResizeAndPositionControl(inputBoxWordCount, 645 + offsetXWordCount, 197 + offsetYWordCount, 986 - 645, 281 - 197);
            inputBoxWordCount.ForeColor = Color.Black;
            inputBoxWordCount.Font = new Font("細明體", (float)26 / 900 * Screen.PrimaryScreen.Bounds.Height, FontStyle.Regular);

            inputBoxWordCount.TextChanged += (sender, e) =>
            {
                string searchText = inputBoxWordCount.Text;
                int targetLength = 0;

                // 尝试将输入文本解析为整数
                if (int.TryParse(searchText, out targetLength))
                {
                    // 过滤出标题中文字数匹配输入值的歌曲
                    var searchResults = allSongs.Where(song => song.Song.Replace(" ", "").Length == targetLength).ToList();
                    currentPage = 0;
                    currentSongList = searchResults;
                    totalPages = (int)Math.Ceiling((double)searchResults.Count / itemsPerPage);

                    multiPagePanel.currentPageIndex = 0;
                    multiPagePanel.LoadSongs(currentSongList);
                }
                else
                {
                    // 如果输入不是有效整数，可能需要显示错误或清空结果
                    currentSongList.Clear();
                }
            };

            this.Controls.Add(inputBoxWordCount);
        }

        // Define the WordCountButton_Click event handler
        private void WordCountButton_Click(object sender, EventArgs e)
        {
            // Assuming 'inputBox' is the TextBox where you want to show the phonetic symbol
            // and that each button has a Tag property set to its corresponding phonetic symbol
            var button = sender as Button;
            if (button != null && button.Tag != null)
            {
                inputBoxWordCount.Text += button.Tag.ToString();
            }
        }

        private void CloseButtonWordCount_Click(object sender, EventArgs e)
        {
            // Logic for Close button
            SetPictureBoxWordCountAndButtonsVisibility(false);
        }
    }
}