using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;
using System.Collections.Generic; // 確保引用了這個命名空間以使用 List<T>
using IniParser;
using IniParser.Model;
using System.Text; // Ensure this namespace is included

namespace DualScreenDemo
{
    public partial class PrimaryForm
    {
        private PictureBox pictureBoxZhuYinSingers;

        private Button[] phoneticButtonsForSingers;
        private Button modifyButtonZhuYinSingers;
        private Button clearButtonZhuYinSingers;
        private Button closeButtonZhuYinSingers;

        private string[] phoneticSymbols;
        private (int X, int Y, int Width, int Height)[] phoneticButtonCoords;
        private Dictionary<string, (string normal, string mouseDown, string mouseOver)> phoneticButtonImages;

        private (int X, int Y, int Width, int Height) modifyButtonZhuYinCoords;
        private (int X, int Y, int Width, int Height) clearButtonZhuYinCoords;
        private (int X, int Y, int Width, int Height) closeButtonZhuYinCoords;

        private RichTextBox inputBoxZhuYinSingers;

        private (int X, int Y, int Width, int Height) inputBoxZhuYinCoords;
        private string inputBoxFontName;
        private float inputBoxFontSize;
        private FontStyle inputBoxFontStyle;
        private Color inputBoxForeColor;

        private void ZhuyinSearchSingersButton_Click(object sender, EventArgs e)
        {
            zhuyinSearchButton.BackgroundImage = zhuyinSearchActiveBackground;
            englishSearchButton.BackgroundImage = englishSearchNormalBackground;
            pinyinSearchButton.BackgroundImage = pinyinSearchNormalBackground;
            wordCountSearchButton.BackgroundImage = wordCountSearchNormalBackground;
            handWritingSearchButton.BackgroundImage = handWritingSearchNormalBackground;

            // Load configuration data
            var configData = LoadConfigData();
            string imagePath = Path.Combine(Application.StartupPath, configData["ImagePaths"]["ZhuYinSingers"]);

            ShowImageOnPictureBoxZhuYinSingers(Path.Combine(Application.StartupPath, imagePath));
            
            // Now update the visibility of the PictureBox and the buttons.
            // This ordering ensures that the image is set before the PictureBox is shown.
            SetEnglishSingersAndButtonsVisibility(false);
            SetPinYinSingersAndButtonsVisibility(false);
            SetHandWritingForSingersAndButtonsVisibility(false);
            SetZhuYinSingersAndButtonsVisibility(true);
            SetPictureBoxArtistSearchAndButtonsVisibility(false);
            pictureBoxZhuYinSingers.Visible = true;
        }

        private void LoadPhoneticSymbolsFromConfig()
        {
            var parser = new FileIniDataParser();
            string iniFilePath = "config.ini";

            // Ensure the file is read with UTF-8 encoding
            IniData data;
            using (var reader = new StreamReader(iniFilePath, Encoding.UTF8))
            {
                data = parser.ReadData(reader);
            }

            string symbols = data["PhoneticSymbols"]["Symbols"];
            phoneticSymbols = symbols.Split(',');
        }

        private IniData LoadConfigData()
        {
            var parser = new FileIniDataParser();
            string iniFilePath = "config.ini";

            using (var reader = new StreamReader(iniFilePath, Encoding.UTF8))
            {
                return parser.ReadData(reader);
            }
        }

        private string[] LoadPhoneticSymbols(IniData data)
        {
            string symbols = data["PhoneticSymbols"]["Symbols"];
            return symbols.Split(',');
        }

        private (int X, int Y, int Width, int Height)[] LoadButtonCoordinates(IniData data, string section, int buttonCount)
        {
            var buttonList = new List<(int X, int Y, int Width, int Height)>();

            for (int i = 1; i <= buttonCount; i++)
            {
                var coordString = data[section][$"button{i}"];
                var coords = coordString.Split(',');
                buttonList.Add((int.Parse(coords[0]), int.Parse(coords[1]), int.Parse(coords[2]), int.Parse(coords[3])));
            }

            return buttonList.ToArray();
        }

        private Dictionary<string, (string normal, string mouseDown, string mouseOver)> LoadButtonImages(IniData data, string section, int buttonCount)
        {
            var buttonImages = new Dictionary<string, (string normal, string mouseDown, string mouseOver)>();

            for (int i = 0; i < 35; i++)
            {
                buttonImages[$"button{i}"] = (
                    data[section][$"button{i}_normal"],
                    data[section][$"button{i}_mouseDown"],
                    data[section][$"button{i}_mouseOver"]
                );
            }

            return buttonImages;
        }

        private (int X, int Y, int Width, int Height) LoadSpecialButtonCoordinates(IniData data, string section, string buttonKey)
        {
            var coords = data[section][buttonKey].Split(',');
            return (int.Parse(coords[0]), int.Parse(coords[1]), int.Parse(coords[2]), int.Parse(coords[3]));
        }

        private (string normal, string mouseDown, string mouseOver) LoadButtonImages(IniData data, string section)
        {
            return (
                data[section]["normal"],
                data[section]["mouseDown"],
                data[section]["mouseOver"]
            );
        }

        private void InitializePhoneticButtons()
        {
            var data = LoadConfigData();

            phoneticSymbols = LoadPhoneticSymbols(data);
            phoneticButtonCoords = LoadButtonCoordinates(data, "PhoneticButtonCoordinates", 35);
            phoneticButtonImages = LoadButtonImages(data, "PhoneticButtonImages", 35);

            phoneticButtonsForSingers = new Button[35];
            for (int i = 0; i < 35; i++)
            {
                var buttonImages = phoneticButtonImages[$"button{i}"];
                CreatePhoneticButton(i, buttonImages.normal, buttonImages.mouseDown, buttonImages.mouseOver);
            }
        }

        private void CreatePhoneticButton(int index, string normalImagePath, string mouseDownImagePath, string mouseOverImagePath)
        {
            try
            {
                // Console.WriteLine($"Creating button at index {index} with normal image path: {normalImagePath}");

                phoneticButtonsForSingers[index] = new Button
                {
                    Name = $"phoneticButton_{phoneticSymbols[index]}",
                    BackgroundImage = Image.FromFile(Path.Combine(Application.StartupPath, normalImagePath)),
                    BackgroundImageLayout = ImageLayout.Stretch,
                    FlatStyle = FlatStyle.Flat,
                    FlatAppearance = { BorderSize = 0 }
                };

                ResizeAndPositionButton(phoneticButtonsForSingers[index], phoneticButtonCoords[index].X, phoneticButtonCoords[index].Y, 
                                        phoneticButtonCoords[index].Width, phoneticButtonCoords[index].Height);

                Image normalImage = Image.FromFile(Path.Combine(Application.StartupPath, normalImagePath));
                Image mouseDownImage = Image.FromFile(Path.Combine(Application.StartupPath, mouseDownImagePath));
                Image mouseOverImage = Image.FromFile(Path.Combine(Application.StartupPath, mouseOverImagePath));

                phoneticButtonsForSingers[index].MouseDown += (s, e) => phoneticButtonsForSingers[index].BackgroundImage = mouseDownImage;
                phoneticButtonsForSingers[index].MouseUp += (s, e) => phoneticButtonsForSingers[index].BackgroundImage = normalImage;
                phoneticButtonsForSingers[index].MouseEnter += (s, e) => phoneticButtonsForSingers[index].BackgroundImage = mouseOverImage;
                phoneticButtonsForSingers[index].MouseLeave += (s, e) => phoneticButtonsForSingers[index].BackgroundImage = normalImage;
                phoneticButtonsForSingers[index].Click += PhoneticButton_Click;
                phoneticButtonsForSingers[index].Tag = phoneticSymbols[index];

                this.Controls.Add(phoneticButtonsForSingers[index]);

                // Console.WriteLine($"Button {index} created successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating button at index {index}: {ex.Message}");
            }
        }

        private void InitializeButtonsForZhuYinSingers()
        {
            LoadPhoneticSymbolsFromConfig();
            InitializePhoneticButtons();
            InitializeSpecialButtonsForZhuYinSingers();
            InitializeInputBoxZhuYinSingers();
        }

        private Image RemoveWhiteBorder(string imagePath)
        {
            Bitmap bmp = new Bitmap(imagePath);

            // 使用 LockBits 和 UnlockBits 提高性能
            Rectangle rect = new Rectangle(0, 0, bmp.Width, bmp.Height);
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, bmp.PixelFormat);

            IntPtr ptr = bmpData.Scan0;
            int bytes = Math.Abs(bmpData.Stride) * bmp.Height;
            byte[] rgbValues = new byte[bytes];
            System.Runtime.InteropServices.Marshal.Copy(ptr, rgbValues, 0, bytes);

            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    int position = (y * bmpData.Stride) + (x * 4);
                    byte b = rgbValues[position];
                    byte g = rgbValues[position + 1];
                    byte r = rgbValues[position + 2];
                    byte a = rgbValues[position + 3];

                    // 如果是邊框的白色像素，則將其設置為透明
                    if ((x < 5 || x > bmp.Width - 5 || y < 5 || y > bmp.Height - 5) && r == 255 && g == 255 && b == 255)
                    {
                        rgbValues[position] = 255;
                        rgbValues[position + 1] = 255;
                        rgbValues[position + 2] = 255;
                        rgbValues[position + 3] = 0; // 將白色像素設置為完全透明
                    }
                }
            }

            System.Runtime.InteropServices.Marshal.Copy(rgbValues, 0, ptr, bytes);
            bmp.UnlockBits(bmpData);

            return bmp;
        }

        private void InitializeSpecialButtonsForZhuYinSingers()
        {
            // 初始化“修改”按钮
            InitializeModifyButtonZhuYinSingers();

            // 初始化“清除”按钮
            InitializeClearButtonZhuYinSingers();

            // 初始化“关闭”按钮
            InitializeCloseButtonZhuYinSingers();
        }

        private void InitializeModifyButtonZhuYinSingers()
        {
            var data = LoadConfigData();
            modifyButtonZhuYinCoords = LoadSpecialButtonCoordinates(data, "SpecialButtonCoordinates", "modifyButtonZhuYinSingers");
            var buttonImages = LoadButtonImages(data, "ModifyButtonImagesZhuYin");

            modifyButtonZhuYinSingers = CreateSpecialButton(
                "btnModifyZhuYinSingers",
                modifyButtonZhuYinCoords,
                buttonImages.normal,
                buttonImages.mouseOver,
                buttonImages.mouseDown,
                ModifyButtonZhuYinSingers_Click
            );
        }

        private void ModifyButtonZhuYinSingers_Click(object sender, EventArgs e)
        {
            // 检查注音输入框是否存在
            if (this.Controls.Contains(inputBoxZhuYinSingers) && inputBoxZhuYinSingers.Text.Length > 0)
            {
                inputBoxZhuYinSingers.Text = inputBoxZhuYinSingers.Text.Substring(0, inputBoxZhuYinSingers.Text.Length - 1);
            }
        }

        private void InitializeClearButtonZhuYinSingers()
        {
            var data = LoadConfigData();
            clearButtonZhuYinCoords = LoadSpecialButtonCoordinates(data, "SpecialButtonCoordinates", "clearButtonZhuYinSingers");
            var buttonImages = LoadButtonImages(data, "ClearButtonImagesZhuYin");

            clearButtonZhuYinSingers = CreateSpecialButton(
                "btnClearZhuYinSingers",
                clearButtonZhuYinCoords,
                buttonImages.normal,
                buttonImages.mouseOver,
                buttonImages.mouseDown,
                ClearButtonZhuYinSingers_Click
            );
        }

        private void ClearButtonZhuYinSingers_Click(object sender, EventArgs e)
        {            
            if (this.Controls.Contains(inputBoxZhuYinSingers) && inputBoxZhuYinSingers.Text.Length > 0)
            {
                inputBoxZhuYinSingers.Text = "";
            }
        }

        private void InitializeCloseButtonZhuYinSingers()
        {
            var data = LoadConfigData();
            closeButtonZhuYinCoords = LoadSpecialButtonCoordinates(data, "SpecialButtonCoordinates", "closeButtonZhuYinSingers");
            var buttonImages = LoadButtonImages(data, "CloseButtonImagesZhuYin");

            closeButtonZhuYinSingers = CreateSpecialButton(
                "btnCloseZhuYinSingers",
                closeButtonZhuYinCoords,
                buttonImages.normal,
                buttonImages.mouseOver,
                buttonImages.mouseDown,
                CloseButtonZhuYinSingers_Click
            );
        }

        private void CloseButtonZhuYinSingers_Click(object sender, EventArgs e)
        {
            pictureBoxZhuYinSingers.Visible = false;
            SetZhuYinSingersAndButtonsVisibility(false);
        }

        private Button CreateSpecialButton(string name, (int X, int Y, int Width, int Height) coords, string normalImagePath, string mouseOverImagePath, string mouseDownImagePath, EventHandler clickEventHandler)
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
            this.Controls.Add(button);

            return button;
        }

        private void InitializeInputBoxZhuYinSingers()
        {
            try
            {
                LoadInputBoxConfig();

                inputBoxZhuYinSingers = new RichTextBox
                {
                    Name = "inputBoxZhuYinSingers",
                    ForeColor = inputBoxForeColor,
                    Font = new Font(inputBoxFontName, inputBoxFontSize, inputBoxFontStyle),
                    ScrollBars = RichTextBoxScrollBars.None // Add this line to remove the scroll bars
                };

                ResizeAndPositionControl(inputBoxZhuYinSingers, inputBoxZhuYinCoords.X, inputBoxZhuYinCoords.Y, inputBoxZhuYinCoords.Width, inputBoxZhuYinCoords.Height);

                inputBoxZhuYinSingers.TextChanged += (sender, e) =>
                {
                    string searchText = inputBoxZhuYinSingers.Text;
                    var searchResults = allArtists.Where(artist => artist.Phonetic.StartsWith(searchText)).ToList();  // 使用 ToList() 進行轉換

                    currentPage = 0;
                    currentArtistList = searchResults;
                    totalPages = (int)Math.Ceiling((double)searchResults.Count / itemsPerPage);

                    multiPagePanel.currentPageIndex = 0;
                    multiPagePanel.LoadSingers(currentArtistList);
                };

                this.Controls.Add(inputBoxZhuYinSingers);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error initializing inputBoxZhuYinSingers: " + ex.Message);
            }
        }

        private void LoadInputBoxConfig()
        {
            try
            {
                var parser = new FileIniDataParser();
                string iniFilePath = "config.ini";

                IniData data;
                using (var reader = new StreamReader(iniFilePath, Encoding.UTF8))
                {
                    data = parser.ReadData(reader);
                }

                inputBoxZhuYinCoords = (
                    int.Parse(data["InputBoxZhuYinSingers"]["X"]),
                    int.Parse(data["InputBoxZhuYinSingers"]["Y"]),
                    int.Parse(data["InputBoxZhuYinSingers"]["Width"]),
                    int.Parse(data["InputBoxZhuYinSingers"]["Height"])
                );

                inputBoxFontName = data["InputBoxZhuYinSingers"]["FontName"];
                inputBoxFontSize = float.Parse(data["InputBoxZhuYinSingers"]["FontSize"]);
                inputBoxFontStyle = (FontStyle)Enum.Parse(typeof(FontStyle), data["InputBoxZhuYinSingers"]["FontStyle"]);
                inputBoxForeColor = Color.FromName(data["InputBoxZhuYinSingers"]["ForeColor"]);

                // Console.WriteLine("Loaded inputBox configuration successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error loading inputBox configuration: " + ex.Message);
            }
        }

        private (int X, int Y, int Width, int Height) pictureBoxZhuYinSingerCoords;

        private void LoadPictureBoxZhuYinSingerCoordsFromConfig()
        {
            var parser = new FileIniDataParser();
            IniData data = parser.ReadFile("config.ini");

            var coords = data["PictureBoxZhuYinSingers"];
            pictureBoxZhuYinSingerCoords = (
                int.Parse(coords["X"]),
                int.Parse(coords["Y"]),
                int.Parse(coords["Width"]),
                int.Parse(coords["Height"])
            );
        }

        private void ShowImageOnPictureBoxZhuYinSingers(string imagePath)
        {
            // Load coordinates from config
            LoadPictureBoxZhuYinSingerCoordsFromConfig();

            // Load the original image
            Bitmap originalImage = new Bitmap(imagePath);

            // Define the display area using the coordinates from the config
            Rectangle displayArea = new Rectangle(pictureBoxZhuYinSingerCoords.X, pictureBoxZhuYinSingerCoords.Y, pictureBoxZhuYinSingerCoords.Width, pictureBoxZhuYinSingerCoords.Height);

            // Set the PictureBox's image
            pictureBoxZhuYinSingers.Image = originalImage;

            // Resize and position the PictureBox according to the loaded coordinates
            ResizeAndPositionPictureBox(pictureBoxZhuYinSingers, displayArea.X, displayArea.Y, displayArea.Width, displayArea.Height);

            pictureBoxZhuYinSingers.Visible = true;
        }

        private void SetZhuYinSingersAndButtonsVisibility(bool isVisible)
        {
            System.Action action = () =>
            {
                try
                {
                    SuspendLayout();

                    if (pictureBoxZhuYinSingers == null)
                    {
                        Console.WriteLine("pictureBoxZhuYinSingers is null");
                    }
                    else
                    {
                        pictureBoxZhuYinSingers.Visible = isVisible;
                        if (isVisible) pictureBoxZhuYinSingers.BringToFront();
                    }

                    if (phoneticButtonsForSingers == null)
                    {
                        Console.WriteLine("phoneticButtonsForSingers is null");
                    }
                    else
                    {
                        foreach (var button in phoneticButtonsForSingers)
                        {
                            if (button == null)
                            {
                                Console.WriteLine("One of the phoneticButtonsForSingers is null");
                            }
                            else
                            {
                                button.Visible = isVisible;
                                if (isVisible) button.BringToFront();
                            }
                        }
                    }

                    if (modifyButtonZhuYinSingers == null)
                    {
                        Console.WriteLine("modifyButtonZhuYinSingers is null");
                    }
                    else
                    {
                        modifyButtonZhuYinSingers.Visible = isVisible;
                        if (isVisible) modifyButtonZhuYinSingers.BringToFront();
                    }

                    if (clearButtonZhuYinSingers == null)
                    {
                        Console.WriteLine("clearButtonZhuYinSingers is null");
                    }
                    else
                    {
                        clearButtonZhuYinSingers.Visible = isVisible;
                        if (isVisible) clearButtonZhuYinSingers.BringToFront();
                    }

                    if (closeButtonZhuYinSingers == null)
                    {
                        Console.WriteLine("closeButtonZhuYinSingers is null");
                    }
                    else
                    {
                        closeButtonZhuYinSingers.Visible = isVisible;
                        if (isVisible) closeButtonZhuYinSingers.BringToFront();
                    }

                    if (inputBoxZhuYinSingers == null)
                    {
                        Console.WriteLine("inputBoxZhuYinSingers is null");
                    }
                    else
                    {
                        inputBoxZhuYinSingers.Visible = isVisible;
                        if (isVisible) inputBoxZhuYinSingers.BringToFront();
                    }

                    ResumeLayout();
                    PerformLayout(); // Optional: Forces immediate layout update

                    // Optional: Force a refresh on all updated controls
                    pictureBoxZhuYinSingers?.Refresh();
                    if (phoneticButtonsForSingers != null)
                    {
                        foreach (var button in phoneticButtonsForSingers)
                        {
                            button?.Refresh();
                        }
                    }
                    modifyButtonZhuYinSingers?.Refresh();
                    clearButtonZhuYinSingers?.Refresh();
                    closeButtonZhuYinSingers?.Refresh();
                    inputBoxZhuYinSingers?.Refresh();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error in SetZhuYinSingersAndButtonsVisibility: " + ex.Message);
                }
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