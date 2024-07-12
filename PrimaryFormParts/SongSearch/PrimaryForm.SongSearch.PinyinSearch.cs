using System;
using System.IO;
using System.Drawing; // Add this for Bitmap and Rectangle
using System.Linq; // Add this for LINQ methods like Concat
using System.Windows.Forms; // Add this for PictureBox, EventArgs, etc.
using IniParser;
using IniParser.Model;

namespace DualScreenDemo
{
    public partial class PrimaryForm
    {
        private PictureBox pictureBoxPinYinSongs;
        private Button[] letterButtonsForPinYinSongs;
        private Button modifyButtonPinYinSongs;
        private Button clearButtonPinYinSongs;
        private Button closeButtonPinYinSongs;
        private RichTextBox inputBoxPinYinSongs;

        private void PinyinSearchSongsButton_Click(object sender, EventArgs e)
        {
            zhuyinSearchSongButton.BackgroundImage = zhuyinSearchSongNormalBackground;
            englishSearchSongButton.BackgroundImage = englishSearchSongNormalBackground;
            pinyinSearchSongButton.BackgroundImage = pinyinSearchSongActiveBackground;
            wordCountSearchSongButton.BackgroundImage = wordCountSearchSongNormalBackground;
            handWritingSearchSongButton.BackgroundImage = handWritingSearchSongNormalBackground;
            numberSearchSongButton.BackgroundImage = numberSearchSongNormalBackground;

            // Load the configuration data
            var configData = LoadConfigData();
            string pinyinImagePath = Path.Combine(Application.StartupPath, configData["ImagePaths"]["PinYinSongs"]);

            ShowImageOnPictureBoxPinYinSongs(Path.Combine(Application.StartupPath, pinyinImagePath));

            // Now update the visibility of the PictureBox and the buttons.
            // This ordering ensures that the image is set before the PictureBox is shown.
            SetZhuYinSingersAndButtonsVisibility(false);
            SetEnglishSingersAndButtonsVisibility(false);
            SetPinYinSingersAndButtonsVisibility(false);
            SetPinYinSongsAndButtonsVisibility(true);
            pictureBoxPinYinSongs.Visible = true;
        }

        private void InitializeLetterButtonsForPinYinSongs()
        {
            var data = LoadConfigData();
            var buttonImages = LoadButtonImages(data, "PinYinLetterButtonImages", 26);
            string qwertyLayout = "QWERTYUIOPASDFGHJKLZXCVBNM";
            letterButtonsForPinYinSongs = new Button[26];

            for (int i = 0; i < 26; i++)
            {
                var coords = data["PinYinLetterButtonCoordinates"][$"button{i}"].Split(',');
                letterButtonsForPinYinSongs[i] = CreateButton(
                    $"letterButton_{qwertyLayout[i]}",
                    (int.Parse(coords[0]), int.Parse(coords[1]), int.Parse(coords[2]), int.Parse(coords[3])),
                    buttonImages[$"button{i}"].normal,
                    buttonImages[$"button{i}"].mouseDown,
                    buttonImages[$"button{i}"].mouseOver,
                    LetterButtonPinYinSongs_Click
                );
                letterButtonsForPinYinSongs[i].Tag = qwertyLayout[i];
                this.Controls.Add(letterButtonsForPinYinSongs[i]);
            }
        }

        private void LetterButtonPinYinSongs_Click(object sender, EventArgs e)
        {
            // Your event handling code here
            var button = sender as Button;
            if (button != null && button.Tag != null)
            {
                if (inputBoxPinYinSongs.Visible)
                {
                    inputBoxPinYinSongs.Text += button.Tag.ToString();
                }
            }
        }

        private void InitializeButtonsForPinYinSongs()
        {
            InitializeLetterButtonsForPinYinSongs();
            InitializeSpecialButtonsForPinYinSongs();
            InitializeInputBoxPinYinSongs();
        }

        private void InitializeSpecialButtonsForPinYinSongs()
        {
            // 初始化“修改”按钮
            InitializeModifyButtonPinYinSongs();

            // 初始化“清除”按钮
            InitializeClearButtonPinYinSongs();

            // 初始化“关闭”按钮
            InitializeCloseButtonPinYinSongs();
        }

        private void InitializeModifyButtonPinYinSongs()
        {
            var data = LoadConfigData();
            modifyButtonPinYinCoords = LoadSpecialButtonCoordinates(data, "SpecialButtonCoordinates", "modifyButtonPinYinSongs");
            var buttonImages = LoadButtonImages(data, "ModifyButtonImagesPinYin");

            modifyButtonPinYinSongs = CreateSpecialButton(
                "btnModifyPinYinSongs",
                modifyButtonPinYinCoords,
                buttonImages.normal,
                buttonImages.mouseOver,
                buttonImages.mouseDown,
                ModifyButtonPinYinSongs_Click
            );
        }

        private void ModifyButtonPinYinSongs_Click(object sender, EventArgs e)
        {
            // 检查注音输入框是否存在
            if (this.Controls.Contains(inputBoxPinYinSongs) && inputBoxPinYinSongs.Text.Length > 0)
            {
                inputBoxPinYinSongs.Text = inputBoxPinYinSongs.Text.Substring(0, inputBoxPinYinSongs.Text.Length - 1);
            }
        }

        private void InitializeClearButtonPinYinSongs()
        {
            var data = LoadConfigData();
            clearButtonPinYinCoords = LoadSpecialButtonCoordinates(data, "SpecialButtonCoordinates", "clearButtonPinYinSongs");
            var buttonImages = LoadButtonImages(data, "ClearButtonImagesPinYin");

            clearButtonPinYinSongs = CreateSpecialButton(
                "btnClearPinYinSongs",
                clearButtonPinYinCoords,
                buttonImages.normal,
                buttonImages.mouseOver,
                buttonImages.mouseDown,
                ClearButtonPinYinSongs_Click
            );
        }

        private void ClearButtonPinYinSongs_Click(object sender, EventArgs e)
        {            
            if (this.Controls.Contains(inputBoxPinYinSongs) && inputBoxPinYinSongs.Text.Length > 0)
            {
                inputBoxPinYinSongs.Text = "";
            }
        }

        private void InitializeCloseButtonPinYinSongs()
        {
            var data = LoadConfigData();
            closeButtonPinYinCoords = LoadSpecialButtonCoordinates(data, "SpecialButtonCoordinates", "closeButtonPinYinSongs");
            var buttonImages = LoadButtonImages(data, "CloseButtonImagesPinYin");

            closeButtonPinYinSongs = CreateSpecialButton(
                "btnClosePinYinSongs",
                closeButtonPinYinCoords,
                buttonImages.normal,
                buttonImages.mouseOver,
                buttonImages.mouseDown,
                CloseButtonPinYinSongs_Click
            );
        }

        private void CloseButtonPinYinSongs_Click(object sender, EventArgs e)
        {
            pictureBoxPinYinSongs.Visible = false;
            SetPinYinSongsAndButtonsVisibility(false);
        }

        private void InitializeInputBoxPinYinSongs()
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

                int x = int.Parse(data["InputBoxPinYinSongs"]["X"]);
                int y = int.Parse(data["InputBoxPinYinSongs"]["Y"]);
                int width = int.Parse(data["InputBoxPinYinSongs"]["Width"]);
                int height = int.Parse(data["InputBoxPinYinSongs"]["Height"]);
                string fontName = data["InputBoxPinYinSongs"]["FontName"];
                float fontSize = float.Parse(data["InputBoxPinYinSongs"]["FontSize"]);
                FontStyle fontStyle = (FontStyle)Enum.Parse(typeof(FontStyle), data["InputBoxPinYinSongs"]["FontStyle"]);
                Color foreColor = Color.FromName(data["InputBoxPinYinSongs"]["ForeColor"]);

                inputBoxPinYinSongs = new RichTextBox
                {
                    Visible = false,
                    Name = "inputBoxPinYinSongs",
                    ForeColor = foreColor,
                    Font = new Font(fontName, fontSize / 900 * Screen.PrimaryScreen.Bounds.Height, fontStyle)
                };

                ResizeAndPositionControl(inputBoxPinYinSongs, x, y, width, height);

                inputBoxPinYinSongs.TextChanged += (sender, e) =>
                {
                    string searchText = inputBoxPinYinSongs.Text;
                    var searchResults = allSongs.Where(song => song.PinyinNotation.StartsWith(searchText)).ToList();
                    currentPage = 0;
                    currentSongList = searchResults;
                    totalPages = (int)Math.Ceiling((double)searchResults.Count / itemsPerPage);

                    multiPagePanel.currentPageIndex = 0;
                    multiPagePanel.LoadSongs(currentSongList);
                };

                this.Controls.Add(inputBoxPinYinSongs);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        private (int X, int Y, int Width, int Height) pictureBoxPinYinSongCoords;

        private void LoadPictureBoxPinYinSongCoordsFromConfig()
        {
            var parser = new FileIniDataParser();
            IniData data = parser.ReadFile("config.ini");

            var coords = data["PictureBoxPinYinSongs"];
            pictureBoxPinYinSongCoords = (
                int.Parse(coords["X"]),
                int.Parse(coords["Y"]),
                int.Parse(coords["Width"]),
                int.Parse(coords["Height"])
            );
        }

        private void ShowImageOnPictureBoxPinYinSongs(string imagePath)
        {
            // Load coordinates from config
            LoadPictureBoxPinYinSongCoordsFromConfig();

            // 加载原始图像
            Bitmap originalImage = new Bitmap(imagePath);

            // 定义裁剪区域
            Rectangle displayArea = new Rectangle(pictureBoxPinYinSongCoords.X, pictureBoxPinYinSongCoords.Y, pictureBoxPinYinSongCoords.Width, pictureBoxPinYinSongCoords.Height);

            // 设置裁剪后的图像为 PictureBox 的图像
            pictureBoxPinYinSongs.Image = originalImage;

            // 调整 PictureBox 的大小以匹配裁剪后的图像大小并设置位置
            ResizeAndPositionPictureBox(pictureBoxPinYinSongs, displayArea.X, displayArea.Y, displayArea.Width, displayArea.Height);
            
            pictureBoxPinYinSongs.Visible = true;
        }

        private void SetPinYinSongsAndButtonsVisibility(bool isVisible)
        {
            System.Action action = () =>
            {
                SuspendLayout();

                pictureBoxPinYinSongs.Visible = isVisible;
                if (isVisible) pictureBoxPinYinSongs.BringToFront();

                foreach (var button in letterButtonsForPinYinSongs)
                {
                    button.Visible = isVisible;
                    if (isVisible) button.BringToFront();
                }

                if (modifyButtonPinYinSongs != null)
                {
                    modifyButtonPinYinSongs.Visible = isVisible;
                    if (isVisible) modifyButtonPinYinSongs.BringToFront();
                }

                if (clearButtonPinYinSongs != null)
                {
                    clearButtonPinYinSongs.Visible = isVisible;
                    if (isVisible) clearButtonPinYinSongs.BringToFront();
                }

                closeButtonPinYinSongs.Visible = isVisible;
                if (isVisible) closeButtonPinYinSongs.BringToFront();

                inputBoxPinYinSongs.Visible = isVisible;
                if (isVisible) inputBoxPinYinSongs.BringToFront();

                ResumeLayout();
                PerformLayout(); // Optional: Forces immediate layout update

                // Optional: Force a refresh on all updated controls
                pictureBoxPinYinSongs.Refresh();
                foreach (var button in letterButtonsForPinYinSongs)
                {
                    button.Refresh();
                }

                // 同样，因为它们似乎在这个上下文中不相关或已被移除，所以不包括 modifyButton2 和 clearButton 的 Refresh 调用
                modifyButtonPinYinSongs.Refresh();
                clearButtonPinYinSongs.Refresh();
                closeButtonPinYinSongs.Refresh();
                inputBoxPinYinSongs.Refresh();
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