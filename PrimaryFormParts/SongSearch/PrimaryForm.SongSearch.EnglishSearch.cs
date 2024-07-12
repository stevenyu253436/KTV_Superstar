using System;
using System.IO; // 為了使用 Path
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms; // 為了使用 Application
using System.Drawing; // 為了使用 Bitmap 和 Rectangle
using IniParser;
using IniParser.Model;
using System.Text; // Ensure this namespace is included

namespace DualScreenDemo
{
    public partial class PrimaryForm
    {
        private PictureBox pictureBoxEnglishSongs;
        
        private Button[] numberButtonsForSongs;
        private Button[] letterButtonsForEnglishSongs;
        private Button modifyButtonEnglishSongs;
        private Button clearButtonEnglishSongs;
        private Button closeButtonEnglishSongs;
        private RichTextBox inputBoxEnglishSongs;

        private void EnglishSearchSongsButton_Click(object sender, EventArgs e)
        {
            zhuyinSearchSongButton.BackgroundImage = zhuyinSearchSongNormalBackground;
            englishSearchSongButton.BackgroundImage = englishSearchSongActiveBackground;
            pinyinSearchSongButton.BackgroundImage = pinyinSearchSongNormalBackground;
            wordCountSearchSongButton.BackgroundImage = wordCountSearchSongNormalBackground;
            handWritingSearchButton.BackgroundImage = handWritingSearchSongNormalBackground;;
            numberSearchSongButton.BackgroundImage = numberSearchSongNormalBackground;

            bool shouldBeVisible = !pictureBoxEnglishSongs.Visible;

            // Load configuration data
            var configData = LoadConfigData();
            string imagePath = Path.Combine(Application.StartupPath, configData["ImagePaths"]["EnglishSongs"]);

            ShowImageOnPictureBoxEnglishSongs(Path.Combine(Application.StartupPath, imagePath));

            SetZhuYinSingersAndButtonsVisibility(false);
            SetEnglishSingersAndButtonsVisibility(false);
            SetPinYinSingersAndButtonsVisibility(false);
            SetHandWritingForSingersAndButtonsVisibility(false);
            SetZhuYinSongsAndButtonsVisibility(false);
            SetEnglishSongsAndButtonsVisibility(true);
            pictureBoxEnglishSongs.Visible = true;
        }

        // Method to initialize number buttons
        private void InitializeNumberButtonsForSongs()
        {
            var data = LoadConfigData();
            numberButtonCoords = LoadButtonCoordinates(data, "NumberButtonCoordinates", 10);
            var buttonImages = LoadButtonImages(data, "NumberButtonImages", 10);

            numberButtonsForSongs = new Button[10];
            for (int i = 0; i < 10; i++)
            {
                string normalImagePath = buttonImages[$"button{i}"].normal;
                string mouseDownImagePath = buttonImages[$"button{i}"].mouseDown;
                string mouseOverImagePath = buttonImages[$"button{i}"].mouseOver;

                // Debug output to check for null paths
                if (normalImagePath == null || mouseDownImagePath == null || mouseOverImagePath == null)
                {
                    Console.WriteLine($"Error: One or more image paths for button{i} are null.");
                    continue; // Skip this button and move to the next
                }

                // Create and add the button if all image paths are valid
                numberButtonsForSongs[i] = CreateButton(
                    $"numberButton_{i}",
                    numberButtonCoords[i],
                    normalImagePath,
                    mouseDownImagePath,
                    mouseOverImagePath,
                    NumberButtonForSongs_Click
                );
                numberButtonsForSongs[i].Tag = (i + 1) % 10;
                this.Controls.Add(numberButtonsForSongs[i]);
            }
        }

        private void NumberButtonForSongs_Click(object sender, EventArgs e)
        {
            // Your event handling code here
            var button = sender as Button;
            if (button != null && button.Tag != null)
            {
                if (inputBoxEnglishSongs.Visible)
                {
                    inputBoxEnglishSongs.Text += button.Tag.ToString();
                }
            }
        }

        private void InitializeLetterButtonsForEnglishSongs()
        {
            var data = LoadConfigData();
            var buttonImages = LoadButtonImages(data, "EnglishLetterButtonImages", 26);
            string qwertyLayout = "QWERTYUIOPASDFGHJKLZXCVBNM";
            letterButtonsForEnglishSongs = new Button[26];

            for (int i = 0; i < 26; i++)
            {
                var coords = data["EnglishLetterButtonCoordinates"][$"button{i}"].Split(',');
                letterButtonsForEnglishSongs[i] = CreateButton(
                    $"letterButton_{qwertyLayout[i]}",
                    (int.Parse(coords[0]), int.Parse(coords[1]), int.Parse(coords[2]), int.Parse(coords[3])),
                    buttonImages[$"button{i}"].normal,
                    buttonImages[$"button{i}"].mouseDown,
                    buttonImages[$"button{i}"].mouseOver,
                    LetterButtonEnglishSongs_Click
                );
                letterButtonsForEnglishSongs[i].Tag = qwertyLayout[i];
                this.Controls.Add(letterButtonsForEnglishSongs[i]);
            }
        }

        private void LetterButtonEnglishSongs_Click(object sender, EventArgs e)
        {
            // Your event handling code here
            var button = sender as Button;
            if (button != null && button.Tag != null)
            {
                if (inputBoxEnglishSongs.Visible)
                {
                    inputBoxEnglishSongs.Text += button.Tag.ToString();
                }
            }
        }

        private void InitializeButtonsForEnglishSongs()
        {
            InitializeNumberButtonsForSongs();
            InitializeLetterButtonsForEnglishSongs();

            // 初始化“修改”按钮
            InitializeModifyButtonEnglishSongs();

            // 初始化“清除”按钮
            InitializeClearButtonEnglishSongs();

            // 初始化“关闭”按钮
            InitializeCloseButtonEnglishSongs();

            InitializeInputBoxEnglishSongs();
        }

        private void InitializeModifyButtonEnglishSongs()
        {
            var data = LoadConfigData();
            modifyButtonEnglishCoords = LoadSpecialButtonCoordinates(data, "SpecialButtonCoordinates", "modifyButtonEnglishSongs");
            var buttonImages = LoadButtonImages(data, "ModifyButtonImagesEnglish");

            modifyButtonEnglishSongs = CreateSpecialButton(
                "btnModifyEnglishSongs",
                modifyButtonEnglishCoords,
                buttonImages.normal,
                buttonImages.mouseOver,
                buttonImages.mouseDown,
                ModifyButtonEnglishSongs_Click
            );

            this.Controls.Add(modifyButtonEnglishSongs);
        }

        private void ModifyButtonEnglishSongs_Click(object sender, EventArgs e)
        {
            // 检查English输入框是否存在
            if (this.Controls.Contains(inputBoxEnglishSongs) && inputBoxEnglishSongs.Text.Length > 0)
            {
                inputBoxEnglishSongs.Text = inputBoxEnglishSongs.Text.Substring(0, inputBoxEnglishSongs.Text.Length - 1);
            }
        }

        private void InitializeClearButtonEnglishSongs()
        {
            var data = LoadConfigData();
            clearButtonEnglishCoords = LoadSpecialButtonCoordinates(data, "SpecialButtonCoordinates", "clearButtonEnglishSongs");
            var buttonImages = LoadButtonImages(data, "ClearButtonImagesEnglish");

            clearButtonEnglishSongs = CreateSpecialButton(
                "btnClearEnglishSongs",
                clearButtonEnglishCoords,
                buttonImages.normal,
                buttonImages.mouseOver,
                buttonImages.mouseDown,
                ClearButtonEnglishSongs_Click
            );

            this.Controls.Add(clearButtonEnglishSongs);
        }

        private void ClearButtonEnglishSongs_Click(object sender, EventArgs e)
        {
            if (this.Controls.Contains(inputBoxEnglishSongs) && inputBoxEnglishSongs.Text.Length > 0)
            {
                inputBoxEnglishSongs.Text = "";
            }
        }

        private void InitializeCloseButtonEnglishSongs()
        {
            var data = LoadConfigData();
            closeButtonEnglishCoords = LoadSpecialButtonCoordinates(data, "SpecialButtonCoordinates", "closeButtonEnglishSongs");
            var buttonImages = LoadButtonImages(data, "CloseButtonImagesEnglish");

            closeButtonEnglishSongs = CreateSpecialButton(
                "btnCloseEnglishSongs",
                closeButtonEnglishCoords,
                buttonImages.normal,
                buttonImages.mouseOver,
                buttonImages.mouseDown,
                CloseButtonEnglishSongs_Click
            );

            this.Controls.Add(closeButtonEnglishSongs);
        }

        private void CloseButtonEnglishSongs_Click(object sender, EventArgs e)
        {
            // Hide the PictureBox controls
            pictureBoxEnglishSongs.Visible = false;
            SetEnglishSongsAndButtonsVisibility(false);
        }

        private void InitializeInputBoxEnglishSongs()
        {
            try
            {
                var parser = new FileIniDataParser();
                parser.Parser.Configuration.AssigmentSpacer = "";
                parser.Parser.Configuration.CommentString = "#";
                parser.Parser.Configuration.CaseInsensitive = true;

                // Read INI file with UTF-8 encoding
                IniData data;
                using (var reader = new StreamReader("config.ini", System.Text.Encoding.UTF8))
                {
                    data = parser.ReadData(reader);
                }

                int x = int.Parse(data["InputBoxEnglishSongs"]["X"]);
                int y = int.Parse(data["InputBoxEnglishSongs"]["Y"]);
                int width = int.Parse(data["InputBoxEnglishSongs"]["Width"]);
                int height = int.Parse(data["InputBoxEnglishSongs"]["Height"]);
                string fontName = data["InputBoxEnglishSongs"]["FontName"];
                float fontSize = float.Parse(data["InputBoxEnglishSongs"]["FontSize"]);
                FontStyle fontStyle = (FontStyle)Enum.Parse(typeof(FontStyle), data["InputBoxEnglishSongs"]["FontStyle"]);
                Color foreColor = Color.FromName(data["InputBoxEnglishSongs"]["ForeColor"]);

                inputBoxEnglishSongs = new RichTextBox
                {
                    Visible = false,
                    Name = "inputBoxEnglishSongs",
                    ForeColor = foreColor,
                    Font = new Font(fontName, fontSize / 900 * Screen.PrimaryScreen.Bounds.Height, fontStyle)
                };

                ResizeAndPositionControl(inputBoxEnglishSongs, x, y, width, height);

                inputBoxEnglishSongs.TextChanged += (sender, e) =>
                {
                    string searchText = inputBoxEnglishSongs.Text;
                    var searchResults = allSongs.Where(song => song.Song.StartsWith(searchText)).ToList();
                    currentPage = 0;
                    currentSongList = searchResults;
                    totalPages = (int)Math.Ceiling((double)searchResults.Count / itemsPerPage);

                    multiPagePanel.currentPageIndex = 0;
                    multiPagePanel.LoadSongs(currentSongList);
                };

                this.Controls.Add(inputBoxEnglishSongs);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        private void ShowImageOnPictureBoxEnglishSongs(string imagePath)
        {
            try
            {
                var parser = new FileIniDataParser();
                parser.Parser.Configuration.AssigmentSpacer = "";
                parser.Parser.Configuration.CommentString = "#";
                parser.Parser.Configuration.CaseInsensitive = true;

                // Read INI file with UTF-8 encoding
                IniData data;
                using (var reader = new StreamReader("config.ini", System.Text.Encoding.UTF8))
                {
                    data = parser.ReadData(reader);
                }

                int x = int.Parse(data["PictureBoxEnglishSongs"]["X"]);
                int y = int.Parse(data["PictureBoxEnglishSongs"]["Y"]);
                int width = int.Parse(data["PictureBoxEnglishSongs"]["Width"]);
                int height = int.Parse(data["PictureBoxEnglishSongs"]["Height"]);

                // 加载原始图像
                Bitmap originalImage = new Bitmap(imagePath);

                // 设置裁剪后的图像为 PictureBox 的图像
                pictureBoxEnglishSongs.Image = originalImage;
            
                // 调整 PictureBox 的大小以匹配裁剪后的图像大小并设置位置
                ResizeAndPositionPictureBox(pictureBoxEnglishSongs, x, y, width, height);
                    
                pictureBoxEnglishSongs.Visible = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        private void SetEnglishSongsAndButtonsVisibility(bool isVisible)
        {
            System.Action action = () =>
            {
                SuspendLayout();

                if (pictureBoxEnglishSongs == null)
                {
                    Console.WriteLine("pictureBoxEnglishSongs is null");
                }
                else
                {
                    pictureBoxEnglishSongs.Visible = isVisible;
                    if (isVisible) pictureBoxEnglishSongs.BringToFront();
                    pictureBoxEnglishSongs.Refresh();
                }

                if (numberButtonsForSongs == null)
                {
                    Console.WriteLine("numberButtonsForSongs is null");
                }
                else
                {
                    foreach (var button in numberButtonsForSongs)
                    {
                        if (button == null)
                        {
                            Console.WriteLine("A button in numberButtonsForSongs is null");
                        }
                        else
                        {
                            button.Visible = isVisible;
                            if (isVisible) button.BringToFront();
                            button.Refresh();
                        }
                    }
                }

                if (letterButtonsForEnglishSongs == null)
                {
                    Console.WriteLine("letterButtonsForEnglishSongs is null");
                }
                else
                {
                    foreach (var button in letterButtonsForEnglishSongs)
                    {
                        if (button == null)
                        {
                            Console.WriteLine("A button in letterButtonsForEnglishSongs is null");
                        }
                        else
                        {
                            button.Visible = isVisible;
                            if (isVisible) button.BringToFront();
                            button.Refresh();
                        }
                    }
                }

                if (modifyButtonEnglishSongs == null)
                {
                    Console.WriteLine("modifyButtonEnglishSongs is null");
                }
                else
                {
                    modifyButtonEnglishSongs.Visible = isVisible;
                    if (isVisible) modifyButtonEnglishSongs.BringToFront();
                    modifyButtonEnglishSongs.Refresh();
                }

                if (clearButtonEnglishSongs == null)
                {
                    Console.WriteLine("clearButtonEnglishSongs is null");
                }
                else
                {
                    clearButtonEnglishSongs.Visible = isVisible;
                    if (isVisible) clearButtonEnglishSongs.BringToFront();
                    clearButtonEnglishSongs.Refresh();
                }

                if (closeButtonEnglishSongs == null)
                {
                    Console.WriteLine("closeButtonEnglishSongs is null");
                }
                else
                {
                    closeButtonEnglishSongs.Visible = isVisible;
                    if (isVisible) closeButtonEnglishSongs.BringToFront();
                    closeButtonEnglishSongs.Refresh();
                }

                if (inputBoxEnglishSongs == null)
                {
                    Console.WriteLine("inputBoxEnglishSongs is null");
                }
                else
                {
                    inputBoxEnglishSongs.Visible = isVisible;
                    if (isVisible) inputBoxEnglishSongs.BringToFront();
                    inputBoxEnglishSongs.Refresh();
                }

                ResumeLayout();
                PerformLayout(); // Optional: Forces immediate layout update
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
    }
}