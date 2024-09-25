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
        private PictureBox pictureBoxEnglishSingers;
        
        private Button[] numberButtonsForSingers;
        private Button[] letterButtonsForEnglishSingers;
        private Button modifyButtonEnglishSingers;
        private Button clearButtonEnglishSingers;
        private Button closeButtonEnglishSingers;

        private (int X, int Y, int Width, int Height) modifyButtonEnglishCoords;
        private (int X, int Y, int Width, int Height) clearButtonEnglishCoords;
        private (int X, int Y, int Width, int Height) closeButtonEnglishCoords;

        private RichTextBox inputBoxEnglishSingers;

        private void EnglishSearchSingersButton_Click(object sender, EventArgs e)
        {
            zhuyinSearchButton.BackgroundImage = zhuyinSearchNormalBackground;
            englishSearchButton.BackgroundImage = englishSearchActiveBackground;
            pinyinSearchButton.BackgroundImage = pinyinSearchNormalBackground;
            wordCountSearchButton.BackgroundImage = wordCountSearchNormalBackground;
            handWritingSearchButton.BackgroundImage = handWritingSearchNormalBackground;

            bool shouldBeVisible = !pictureBoxEnglishSingers.Visible;

            // Load configuration data
            var configData = LoadConfigData();
            string imagePath = Path.Combine(Application.StartupPath, configData["ImagePaths"]["EnglishSingers"]);

            ShowImageOnPictureBoxEnglishSingers(Path.Combine(Application.StartupPath, imagePath));

            SetZhuYinSingersAndButtonsVisibility(false);
            SetPinYinSingersAndButtonsVisibility(false);
            SetHandWritingForSingersAndButtonsVisibility(false);
            SetEnglishSingersAndButtonsVisibility(true);
            SetPictureBoxArtistSearchAndButtonsVisibility(false);
            pictureBoxEnglishSingers.Visible = true;
        }

        private (int X, int Y, int Width, int Height)[] numberButtonCoords;

        private void LoadNumberButtonCoordsFromConfig()
        {
            var parser = new FileIniDataParser();
            IniData data = parser.ReadFile("config.ini");

            var buttonList = new List<(int X, int Y, int Width, int Height)>();

            for (int i = 1; i <= 10; i++)
            {
                var coordString = data["NumberButtonCoordinates"][$"button{i}"];
                var coords = coordString.Split(',');
                buttonList.Add((int.Parse(coords[0]), int.Parse(coords[1]), int.Parse(coords[2]), int.Parse(coords[3])));
            }

            numberButtonCoords = buttonList.ToArray();
        }

        private Button CreateButton(string name, (int X, int Y, int Width, int Height) coords, string normalImagePath, string mouseDownImagePath, string mouseOverImagePath, EventHandler clickEventHandler)
        {
            var button = new Button
            {
                Name = name,
                FlatStyle = FlatStyle.Flat,
                FlatAppearance = { BorderSize = 0, MouseDownBackColor = Color.Transparent, MouseOverBackColor = Color.Transparent },
                BackgroundImageLayout = ImageLayout.Stretch,
                BackgroundImage = Image.FromFile(Path.Combine(Application.StartupPath, normalImagePath))
            };

            ResizeAndPositionButton(button, coords.X, coords.Y, coords.Width, coords.Height);

            button.MouseEnter += (sender, e) => button.BackgroundImage = Image.FromFile(Path.Combine(Application.StartupPath, mouseOverImagePath));
            button.MouseLeave += (sender, e) => button.BackgroundImage = Image.FromFile(Path.Combine(Application.StartupPath, normalImagePath));
            button.MouseDown += (sender, e) => button.BackgroundImage = Image.FromFile(Path.Combine(Application.StartupPath, mouseDownImagePath));
            button.MouseUp += (sender, e) => button.BackgroundImage = Image.FromFile(Path.Combine(Application.StartupPath, normalImagePath));

            button.Click += clickEventHandler;

            return button;
        }

        // Method to initialize number buttons
        private void InitializeNumberButtonsForSingers()
        {
            var data = LoadConfigData();
            numberButtonCoords = LoadButtonCoordinates(data, "NumberButtonCoordinates", 10);
            var buttonImages = LoadButtonImages(data, "NumberButtonImages", 10);

            numberButtonsForSingers = new Button[10];
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
                numberButtonsForSingers[i] = CreateButton(
                    $"numberButton_{i}",
                    numberButtonCoords[i],
                    normalImagePath,
                    mouseDownImagePath,
                    mouseOverImagePath,
                    NumberButtonForSingers_Click
                );
                numberButtonsForSingers[i].Tag = (i + 1) % 10;
                this.Controls.Add(numberButtonsForSingers[i]);
            }
        }

        private void NumberButtonForSingers_Click(object sender, EventArgs e)
        {
            // Your event handling code here
            var button = sender as Button;
            if (button != null && button.Tag != null)
            {
                if (inputBoxEnglishSingers.Visible)
                {
                    inputBoxEnglishSingers.Text += button.Tag.ToString();
                }
            }
        }

        private void InitializeLetterButtonsForEnglishSingers()
        {
            var data = LoadConfigData();
            var buttonImages = LoadButtonImages(data, "EnglishLetterButtonImages", 26);
            string qwertyLayout = "QWERTYUIOPASDFGHJKLZXCVBNM";
            letterButtonsForEnglishSingers = new Button[26];

            for (int i = 0; i < 26; i++)
            {
                var coords = data["EnglishLetterButtonCoordinates"][$"button{i}"].Split(',');
                letterButtonsForEnglishSingers[i] = CreateButton(
                    $"letterButton_{qwertyLayout[i]}",
                    (int.Parse(coords[0]), int.Parse(coords[1]), int.Parse(coords[2]), int.Parse(coords[3])),
                    buttonImages[$"button{i}"].normal,
                    buttonImages[$"button{i}"].mouseDown,
                    buttonImages[$"button{i}"].mouseOver,
                    LetterButtonEnglishSingers_Click
                );
                letterButtonsForEnglishSingers[i].Tag = qwertyLayout[i];
                this.Controls.Add(letterButtonsForEnglishSingers[i]);
            }
        }

        private void LetterButtonEnglishSingers_Click(object sender, EventArgs e)
        {
            // Your event handling code here
            var button = sender as Button;
            if (button != null && button.Tag != null)
            {
                if (inputBoxEnglishSingers.Visible)
                {
                    inputBoxEnglishSingers.Text += button.Tag.ToString();
                }
            }
        }

        private void InitializeButtonsForEnglishSingers()
        {
            InitializeNumberButtonsForSingers();
            InitializeLetterButtonsForEnglishSingers();

            // 初始化“修改”按钮
            InitializeModifyButtonEnglishSingers();

            // 初始化“清除”按钮
            InitializeClearButtonEnglishSingers();

            // 初始化“关闭”按钮
            InitializeCloseButtonEnglishSingers();

            InitializeInputBoxEnglishSingers();
        }

        private void InitializeModifyButtonEnglishSingers()
        {
            var data = LoadConfigData();
            modifyButtonEnglishCoords = LoadSpecialButtonCoordinates(data, "SpecialButtonCoordinates", "modifyButtonEnglishSingers");
            var buttonImages = LoadButtonImages(data, "ModifyButtonImagesEnglish");

            modifyButtonEnglishSingers = CreateSpecialButton(
                "btnModifyEnglishSingers",
                modifyButtonEnglishCoords,
                buttonImages.normal,
                buttonImages.mouseOver,
                buttonImages.mouseDown,
                ModifyButtonEnglishSingers_Click
            );

            this.Controls.Add(modifyButtonEnglishSingers);
        }

        private void ModifyButtonEnglishSingers_Click(object sender, EventArgs e)
        {
            // 检查English输入框是否存在
            if (this.Controls.Contains(inputBoxEnglishSingers) && inputBoxEnglishSingers.Text.Length > 0)
            {
                inputBoxEnglishSingers.Text = inputBoxEnglishSingers.Text.Substring(0, inputBoxEnglishSingers.Text.Length - 1);
            }
        }

        private void InitializeClearButtonEnglishSingers()
        {
            var data = LoadConfigData();
            clearButtonEnglishCoords = LoadSpecialButtonCoordinates(data, "SpecialButtonCoordinates", "clearButtonEnglishSingers");
            var buttonImages = LoadButtonImages(data, "ClearButtonImagesEnglish");

            clearButtonEnglishSingers = CreateSpecialButton(
                "btnClearEnglishSingers",
                clearButtonEnglishCoords,
                buttonImages.normal,
                buttonImages.mouseOver,
                buttonImages.mouseDown,
                ClearButtonEnglishSingers_Click
            );

            this.Controls.Add(clearButtonEnglishSingers);
        }

        private void ClearButtonEnglishSingers_Click(object sender, EventArgs e)
        {
            if (this.Controls.Contains(inputBoxEnglishSingers) && inputBoxEnglishSingers.Text.Length > 0)
            {
                inputBoxEnglishSingers.Text = "";
            }
        }

        private void InitializeCloseButtonEnglishSingers()
        {
            var data = LoadConfigData();
            closeButtonEnglishCoords = LoadSpecialButtonCoordinates(data, "SpecialButtonCoordinates", "closeButtonEnglishSingers");
            var buttonImages = LoadButtonImages(data, "CloseButtonImagesEnglish");

            closeButtonEnglishSingers = CreateSpecialButton(
                "btnCloseEnglishSingers",
                closeButtonEnglishCoords,
                buttonImages.normal,
                buttonImages.mouseOver,
                buttonImages.mouseDown,
                CloseButtonEnglishSingers_Click
            );

            this.Controls.Add(closeButtonEnglishSingers);
        }

        private void CloseButtonEnglishSingers_Click(object sender, EventArgs e)
        {
            // Hide the PictureBox controls
            pictureBoxEnglishSingers.Visible = false;
            SetEnglishSingersAndButtonsVisibility(false);
        }

        private void InitializeInputBoxEnglishSingers()
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

                int x = int.Parse(data["InputBoxEnglishSingers"]["X"]);
                int y = int.Parse(data["InputBoxEnglishSingers"]["Y"]);
                int width = int.Parse(data["InputBoxEnglishSingers"]["Width"]);
                int height = int.Parse(data["InputBoxEnglishSingers"]["Height"]);
                string fontName = data["InputBoxEnglishSingers"]["FontName"];
                float fontSize = float.Parse(data["InputBoxEnglishSingers"]["FontSize"]);
                FontStyle fontStyle = (FontStyle)Enum.Parse(typeof(FontStyle), data["InputBoxEnglishSingers"]["FontStyle"]);
                Color foreColor = Color.FromName(data["InputBoxEnglishSingers"]["ForeColor"]);

                inputBoxEnglishSingers = new RichTextBox
                {
                    Visible = false,
                    Name = "inputBoxEnglishSingers",
                    ForeColor = foreColor,
                    Font = new Font(fontName, fontSize / 900 * Screen.PrimaryScreen.Bounds.Height, fontStyle)
                };

                ResizeAndPositionControl(inputBoxEnglishSingers, x, y, width, height);

                inputBoxEnglishSingers.TextChanged += (sender, e) =>
                {
                    string searchText = inputBoxEnglishSingers.Text;
                    var searchResults = allArtists.Where(artist => artist.Name.Replace(" ", "").StartsWith(searchText)).ToList();
                    
                    currentPage = 0;
                    currentArtistList = searchResults;
                    totalPages = (int)Math.Ceiling((double)searchResults.Count / itemsPerPage);

                    multiPagePanel.currentPageIndex = 0;
                    multiPagePanel.LoadSingers(currentArtistList);
                };

                this.Controls.Add(inputBoxEnglishSingers);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        private void ShowImageOnPictureBoxEnglishSingers(string imagePath)
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

                int x = int.Parse(data["PictureBoxEnglishSingers"]["X"]);
                int y = int.Parse(data["PictureBoxEnglishSingers"]["Y"]);
                int width = int.Parse(data["PictureBoxEnglishSingers"]["Width"]);
                int height = int.Parse(data["PictureBoxEnglishSingers"]["Height"]);

                // 加载原始图像
                Bitmap originalImage = new Bitmap(imagePath);

                // 设置裁剪后的图像为 PictureBox 的图像
                pictureBoxEnglishSingers.Image = originalImage;
            
                // 调整 PictureBox 的大小以匹配裁剪后的图像大小并设置位置
                ResizeAndPositionPictureBox(pictureBoxEnglishSingers, x, y, width, height);
                    
                pictureBoxEnglishSingers.Visible = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        private void SetEnglishSingersAndButtonsVisibility(bool isVisible)
        {
            System.Action action = () =>
            {
                SuspendLayout();

                pictureBoxEnglishSingers.Visible = isVisible;
                if (isVisible) pictureBoxEnglishSingers.BringToFront();

                foreach (var button in numberButtonsForSingers)
                {
                    button.Visible = isVisible;
                    if (isVisible) button.BringToFront();
                }

                foreach (var button in letterButtonsForEnglishSingers)
                {
                    button.Visible = isVisible;
                    if (isVisible) button.BringToFront();
                }

                // 注释掉的部分，因为它们在这个上下文中似乎不相关或已被移除
                if (modifyButtonEnglishSingers != null)
                {
                    modifyButtonEnglishSingers.Visible = isVisible;
                    if (isVisible) modifyButtonEnglishSingers.BringToFront();
                }

                if (clearButtonEnglishSingers != null)
                {
                    clearButtonEnglishSingers.Visible = isVisible;
                    if (isVisible) clearButtonEnglishSingers.BringToFront();
                }

                closeButtonEnglishSingers.Visible = isVisible;
                if (isVisible) closeButtonEnglishSingers.BringToFront();

                inputBoxEnglishSingers.Visible = isVisible;
                if (isVisible) inputBoxEnglishSingers.BringToFront();

                ResumeLayout();
                PerformLayout(); // Optional: Forces immediate layout update

                // Optional: Force a refresh on all updated controls
                pictureBoxEnglishSingers.Refresh();
                foreach (var button in numberButtonsForSingers.Concat(letterButtonsForEnglishSingers))
                {
                    button.Refresh();
                }

                // 同样，因为它们似乎在这个上下文中不相关或已被移除，所以不包括 modifyButton2 和 clearButton 的 Refresh 调用
                modifyButtonEnglishSingers.Refresh();
                clearButtonEnglishSingers.Refresh();
                closeButtonEnglishSingers.Refresh();
                inputBoxEnglishSingers.Refresh();
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