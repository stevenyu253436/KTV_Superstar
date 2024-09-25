using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace DualScreenDemo
{
    public partial class PrimaryForm
    {
        private void InitializeButtonsForPictureBoxArtistSearch()
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

            numberButtonsArtistSearch = new Button[10];

            for (int i = 0; i < numberButtonsArtistSearch.Length; i++)
            {
                numberButtonsArtistSearch[i] = new Button();

                // Here, ConfigureButton() is assumed to set the button's position, size, and potentially other properties
                ConfigureButton(
                    numberButtonsArtistSearch[i],
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

                int newXForArtistSearch = (int)(((numberButtonsArtistSearch[i].Location.X / widthRatio) + offsetXArtistSearch) * widthRatio);
                int newYForArtistSearch = (int)(((numberButtonsArtistSearch[i].Location.Y / heightRatio) + offsetXArtistSearch) * heightRatio);
                numberButtonsArtistSearch[i].Location = new Point(newXForArtistSearch, newYForArtistSearch);

                // Set unique names for each number button
                numberButtonsArtistSearch[i].Name = "NumberButtonArtistSearch" + i;
                // Set button tag
                numberButtonsArtistSearch[i].Tag = (i + 1).ToString();
                if (i == 9) // Adjust for 0 being the last button
                {
                    numberButtonsArtistSearch[i].Name = "NumberButtonArtistSearch0";
                    numberButtonsArtistSearch[i].Tag = "0";
                }

                // Add event handler for clicks, assuming WordCountButton_Click is defined
                numberButtonsArtistSearch[i].Click += ArtistButton_Click;

                // Add the button to the form's controls or to pictureBoxWordCount if it's being used as a container
                this.Controls.Add(numberButtonsArtistSearch[i]);
            }

            // Modify Button
            modifyButtonArtistSearch = new Button {
                Name = "ModifyButtonArtistSearch",
                Tag = "Modify",
                Visible = false
            };
            // Coordinates for Modify button from the map: (978, 292) to (1081, 397)
            ConfigureButton(modifyButtonArtistSearch, 978, 292, 1081 - 978, 397 - 292, resizedNormalStateImageFor6_1, resizedMouseOverImageFor6_1, resizedMouseDownImageFor6_1, ModifyButtonArtist_Click);
            int newX = (int)(((modifyButtonArtistSearch.Location.X / widthRatio) + offsetXArtistSearch) * widthRatio);
            int newY = (int)(((modifyButtonArtistSearch.Location.Y / widthRatio) + offsetYArtistSearch) * heightRatio);
            modifyButtonArtistSearch.Location = new Point(newX, newY);
            this.Controls.Add(modifyButtonArtistSearch);
            
            // Close Button
            closeButtonArtistSearch = new Button {
                Name = "CloseButtonArtistSearch",
                Tag = "Close",
                Visible = false
            };
            // Coordinates for Close button from the map: (982, 147) to (1082, 250)
            ConfigureButton(closeButtonArtistSearch, 982, 147, 1082 - 982, 250 - 147, resizedNormalStateImageFor6_1, resizedMouseOverImageFor6_1, resizedMouseDownImageFor6_1, CloseButtonArtistSearch_Click);
            newX = (int)(((closeButtonArtistSearch.Location.X / widthRatio) + offsetXArtistSearch) * widthRatio);
            newY = (int)(((closeButtonArtistSearch.Location.Y / widthRatio) + offsetYArtistSearch) * heightRatio);
            closeButtonArtistSearch.Location = new Point(newX, newY);
            this.Controls.Add(closeButtonArtistSearch);

            inputBoxArtistSearch = new RichTextBox();
            inputBoxArtistSearch.Name = "inputBoxArtistSearch";
            ResizeAndPositionControl(inputBoxArtistSearch, 645 + offsetXArtistSearch, 197 + offsetXArtistSearch, 986 - 645, 281 - 197);
            inputBoxArtistSearch.ForeColor = Color.Black;
            inputBoxArtistSearch.Font = new Font("細明體", (float)26 / 900 * Screen.PrimaryScreen.Bounds.Height, FontStyle.Regular);

            inputBoxArtistSearch.TextChanged += (sender, e) =>
            {
                string searchText = inputBoxArtistSearch.Text;
                int targetLength = 0;

                // 尝试将输入文本解析为整数
                if (int.TryParse(searchText, out targetLength))
                {
                    // 过滤出 ArtistA 或 ArtistB 名字长度匹配输入值的歌曲
                    var searchResults = allArtists.Where(artist => artist.Name.Replace(" ", "").Length == targetLength).ToList();

                    currentPage = 0;
                    currentArtistList = searchResults;
                    totalPages = (int)Math.Ceiling((double)searchResults.Count / itemsPerPage);

                    multiPagePanel.currentPageIndex = 0;
                    multiPagePanel.LoadSingers(currentArtistList);
                }
                else
                {
                    // 如果输入不是有效整数，可能需要显示错误或清空结果
                    currentArtistList.Clear();
                }
            };

            this.Controls.Add(inputBoxArtistSearch);
        }

        // Define the ArtistButton_Click event handler
        private void ArtistButton_Click(object sender, EventArgs e)
        {
            // Assuming 'inputBox' is the TextBox where you want to show the phonetic symbol
            // and that each button has a Tag property set to its corresponding phonetic symbol
            var button = sender as Button;
            if (button != null && button.Tag != null)
            {
                inputBoxArtistSearch.Text += button.Tag.ToString();
            }
        }
        
        private void WordCountSearchButton_Click(object sender, EventArgs e)
        {
            zhuyinSearchButton.BackgroundImage = zhuyinSearchNormalBackground;
            englishSearchButton.BackgroundImage = englishSearchNormalBackground;
            pinyinSearchButton.BackgroundImage = pinyinSearchNormalBackground;
            wordCountSearchButton.BackgroundImage = wordCountSearchActiveBackground;
            handWritingSearchButton.BackgroundImage = handWritingSearchNormalBackground;

            // Toggle the visibility state for the next action.
            bool shouldBeVisible = !pictureBoxArtistSearch.Visible;

            // Update PictureBox image only if it should be visible.
            if (shouldBeVisible)
            {
                ShowImageOnPictureBoxArtistSearch(Path.Combine(Application.StartupPath, @"themes\superstar\6-1.png"));
            }

            SetEnglishSingersAndButtonsVisibility(false);
            SetPinYinSingersAndButtonsVisibility(false);
            SetHandWritingForSingersAndButtonsVisibility(false);
            SetZhuYinSingersAndButtonsVisibility(false);
            SetPictureBoxArtistSearchAndButtonsVisibility(shouldBeVisible);
            pictureBoxArtistSearch.Visible = shouldBeVisible;
        }

        private void CloseButtonArtistSearch_Click(object sender, EventArgs e)
        {
            // Logic for Close button
            SetPictureBoxArtistSearchAndButtonsVisibility(false);
        }
    }
}