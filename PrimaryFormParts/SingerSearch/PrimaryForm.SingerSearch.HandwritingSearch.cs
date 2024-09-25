using System;
using System.Drawing;
using System.IO;
using System.Linq;  // Add this line for LINQ
using System.Windows.Forms;
using Microsoft.Ink;

namespace DualScreenDemo
{
    public partial class PrimaryForm
    {
        private PictureBox pictureBoxHandWritingSingers;

        private Button refillButtonHandWritingSingers;
        private Button clearButtonHandWritingSingers;
        private Button closeButtonForSingers;

        private (int X, int Y, int Width, int Height) refillButtonHandWritingCoords;
        private (int X, int Y, int Width, int Height) clearButtonHandWritingCoords;
        private (int X, int Y, int Width, int Height) closeButtonHandWritingCoords;

        private void HandWritingSearchButtonForSingers_Click(object sender, EventArgs e)
        {
            this.SuspendLayout();

            zhuyinSearchButton.BackgroundImage = zhuyinSearchNormalBackground;
            englishSearchButton.BackgroundImage = englishSearchNormalBackground;
            pinyinSearchButton.BackgroundImage = pinyinSearchNormalBackground;
            wordCountSearchButton.BackgroundImage = wordCountSearchNormalBackground;
            handWritingSearchButton.BackgroundImage = handWritingSearchActiveBackground;

            // 启用双缓冲以减少闪烁
            EnableDoubleBuffering(handWritingPanelForSingers);
            EnableDoubleBuffering(handwritingInputBoxForSingers);
            EnableDoubleBuffering(candidateListBoxForSingers);
            EnableDoubleBuffering(pictureBoxHandWritingSingers);
            EnableDoubleBuffering(refillButtonHandWritingSingers);
            EnableDoubleBuffering(closeButtonForSingers);

            // Load the configuration data
            var configData = LoadConfigData();
            string handWritingImagePath = Path.Combine(Application.StartupPath, configData["ImagePaths"]["HandWritingSingers"]);

            ShowImageOnPictureBoxHandWritingSingers(Path.Combine(Application.StartupPath, handWritingImagePath));
            SetZhuYinSingersAndButtonsVisibility(false);
            SetEnglishSingersAndButtonsVisibility(false);
            SetPinYinSingersAndButtonsVisibility(false);
            SetPictureBoxArtistSearchAndButtonsVisibility(false);
            SetHandWritingForSingersAndButtonsVisibility(true);

            this.ResumeLayout();
        }
        
        private Panel handWritingPanelForSingers;
        private InkOverlay inkOverlayForSingers;
        private RichTextBox handwritingInputBoxForSingers;
        private ListBox candidateListBoxForSingers;


        private void InitializeHandWritingForSingers()
        {
            InitializeHandWritingPanelForSingers();
            InitializeInkOverlayForSingers();
            InitializeHandwritingInputBoxForSingers();
            InitializeCandidateListBoxForSingers();
            InitializeSpecialButtonsForHandWritingSingers();
        }
        
        private void InitializeHandWritingPanelForSingers()
        {
            // 初始化搜索歌手的手写面板
            handWritingPanelForSingers = new Panel
            {
                BorderStyle = BorderStyle.FixedSingle,
                Visible = false // 初始设置为不可见
            };

            // 调整和设置手写面板的位置和尺寸
            ResizeAndPositionControl(handWritingPanelForSingers, 366, 448, 650, 260);

            // 将手写面板添加到窗体的控件集合中
            this.Controls.Add(handWritingPanelForSingers);
        }

        private void InitializeInkOverlayForSingers()
        {
            try
            {
                // 初始化搜索歌手的手写面板的 InkOverlay
                inkOverlayForSingers = new InkOverlay(handWritingPanelForSingers);
                inkOverlayForSingers.Enabled = false;
                inkOverlayForSingers.Ink = new Ink();
                inkOverlayForSingers.DefaultDrawingAttributes.Color = Color.Black;
                inkOverlayForSingers.DefaultDrawingAttributes.Width = 100;
                inkOverlayForSingers.Stroke += new InkCollectorStrokeEventHandler(InkOverlayForSingers_Stroke);

                // 在Panel完全配置好之后再启用InkOverlay
                inkOverlayForSingers.Enabled = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to initialize ink overlay for singers: " + ex.Message);
            }
        }

        private void InkOverlayForSingers_Stroke(object sender, InkCollectorStrokeEventArgs e)
        {
            // 处理来自歌手搜索面板的墨迹
            RecognizeInk(inkOverlayForSingers, candidateListBoxForSingers);
        }

        private void InitializeHandwritingInputBoxForSingers()
        {
            // 初始化用于歌手搜索的输入框
            handwritingInputBoxForSingers = new RichTextBox
            {
                Font = new Font("微軟正黑體", (float)26 / 900 * Screen.PrimaryScreen.Bounds.Height, FontStyle.Regular), // 选择适合的字体和大小
                Visible = false
            };
            ResizeAndPositionControl(handwritingInputBoxForSingers, 366, 373, 541, 62);
            this.Controls.Add(handwritingInputBoxForSingers);

            handwritingInputBoxForSingers.TextChanged += (sender, e) =>
            {
                string searchText = handwritingInputBoxForSingers.Text;
                // 假设 allSongs 是存储所有歌曲数据的 List<SongData>
                // 使用 Union 来合并来自 ArtistA 和 ArtistB 的搜索结果
                var searchResults = allArtists.Where(artist => artist.Name.StartsWith(searchText)).ToList();

                currentPage = 0;
                currentArtistList = searchResults;
                totalPages = (int)Math.Ceiling((double)searchResults.Count / itemsPerPage);

                multiPagePanel.currentPageIndex = 0;
                multiPagePanel.LoadSingers(currentArtistList);
            };
        }    

        private void InitializeCandidateListBoxForSingers()
        {
            // 初始化搜索歌手的候选词列表
            candidateListBoxForSingers = new ListBox
            {
                Font = new Font("微軟正黑體", (float)26 / 900 * Screen.PrimaryScreen.Bounds.Height, FontStyle.Regular),
                Visible = false
            };
            ResizeAndPositionControl(candidateListBoxForSingers, 350 + 679, 448, 115, 260);
            candidateListBoxForSingers.SelectedIndexChanged += CandidateListBoxForSingers_SelectedIndexChanged;
            this.Controls.Add(candidateListBoxForSingers);
        }

        private void CandidateListBoxForSingers_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (candidateListBoxForSingers.SelectedIndex != -1)
            {
                string selectedWord = candidateListBoxForSingers.SelectedItem.ToString();
                handwritingInputBoxForSingers.Text += selectedWord;  // 将选中的词设置为输入框的文本
                candidateListBoxForSingers.Visible = false; // 选择后隐藏列表
                
                // 清除手写板上的所有墨迹
                if (inkOverlayForSingers != null)
                {
                    inkOverlayForSingers.Ink.DeleteStrokes();
                    handWritingPanelForSingers.Invalidate(); // 请求重绘，更新界面
                }
            }
        }

        private void ShowImageOnPictureBoxHandWritingSingers(string imagePath)
        {
            // Load the original image
            Bitmap originalImage = new Bitmap(imagePath);

            // Define the display area using the coordinates from the config
            Rectangle displayArea = new Rectangle(350, 360, 810, 360);

            // Set the PictureBox's image
            pictureBoxHandWritingSingers.Image = originalImage;

            // Resize and position the PictureBox according to the loaded coordinates
            ResizeAndPositionPictureBox(pictureBoxHandWritingSingers, displayArea.X, displayArea.Y, displayArea.Width, displayArea.Height);

            pictureBoxHandWritingSingers.Visible = true;
        }

        private void SetHandWritingForSingersAndButtonsVisibility(bool isVisible)
        {
            // 启用双缓冲以减少闪烁
            EnableDoubleBuffering(handWritingPanelForSingers);
            EnableDoubleBuffering(handwritingInputBoxForSingers);
            EnableDoubleBuffering(candidateListBoxForSingers);
            EnableDoubleBuffering(pictureBoxHandWritingSingers);
            EnableDoubleBuffering(refillButtonHandWritingSingers);
            EnableDoubleBuffering(clearButtonHandWritingSingers);
            EnableDoubleBuffering(closeButtonForSingers);

            // 设置控件的可见性
            handWritingPanelForSingers.Visible = isVisible;
            handwritingInputBoxForSingers.Visible = isVisible;
            inkOverlayForSingers.Enabled = isVisible;
            candidateListBoxForSingers.Visible = isVisible; // 同时隐藏候选词列表
            pictureBoxHandWritingSingers.Visible = isVisible;
            refillButtonHandWritingSingers.Visible = isVisible;
            clearButtonHandWritingSingers.Visible = isVisible;
            closeButtonForSingers.Visible = isVisible;

            if (isVisible)
            {
                // Bring controls to the front if they are visible
                pictureBoxHandWritingSingers.BringToFront();
                handWritingPanelForSingers.BringToFront();
                handwritingInputBoxForSingers.BringToFront();
                candidateListBoxForSingers.BringToFront();
                refillButtonHandWritingSingers.BringToFront();
                clearButtonHandWritingSingers.BringToFront();
                closeButtonForSingers.BringToFront();
            }
        }

        private void InitializeSpecialButtonsForHandWritingSingers()
        {
            // 初始化“修改”按钮
            InitializeRefillButtonHandwritingSingers();

            // 初始化“清除”按钮
            InitializeClearButtonHandWritingSingers();

            // 初始化“关闭”按钮
            InitializeCloseButtonForSingers();
        }

        private void InitializeRefillButtonHandwritingSingers()
        {
            var data = LoadConfigData();
            refillButtonHandWritingCoords = LoadSpecialButtonCoordinates(data, "SpecialButtonCoordinates", "refillButtonHandWritingSingers");
            var buttonImages = LoadButtonImages(data, "RefillButtonImagesHandWriting");

            refillButtonHandWritingSingers = CreateSpecialButton(
                "refillButtonHandWritingSingers",
                refillButtonHandWritingCoords,
                buttonImages.normal,
                buttonImages.mouseOver,
                buttonImages.mouseDown,
                RefillButtonHandWritingSingers_Click
            );
        }

        private void RefillButtonHandWritingSingers_Click(object sender, EventArgs e)
        {
            handwritingInputBoxForSingers.Text = "";
        }

        private void InitializeClearButtonHandWritingSingers()
        {
            var data = LoadConfigData();
            clearButtonHandWritingCoords = LoadSpecialButtonCoordinates(data, "SpecialButtonCoordinates", "clearButtonHandWritingSingers");
            var buttonImages = LoadButtonImages(data, "ClearButtonImagesHandWriting");

            clearButtonHandWritingSingers = CreateSpecialButton(
                "clearButtonHandWritingSingers",
                clearButtonHandWritingCoords,
                buttonImages.normal,
                buttonImages.mouseOver,
                buttonImages.mouseDown,
                // RefillButtonHandWritingSingers_Click
                ClearButtonHandWritingSingers_Click
            );
        }

        private void ClearButtonHandWritingSingers_Click(object sender, EventArgs e)
        {
            if (this.Controls.Contains(handWritingPanelForSingers) && inkOverlayForSingers != null)
            {
                inkOverlayForSingers.Ink.DeleteStrokes();
                handWritingPanelForSingers.Invalidate(); // 请求重绘，更新界面
            }
        }

        private void InitializeCloseButtonForSingers()
        {
            var data = LoadConfigData();
            closeButtonHandWritingCoords = LoadSpecialButtonCoordinates(data, "SpecialButtonCoordinates", "closeButtonForSingers");
            var buttonImages = LoadButtonImages(data, "CloseButtonImagesHandWriting");

            closeButtonForSingers = CreateSpecialButton(
                "closeButtonForSingers",
                closeButtonHandWritingCoords,
                buttonImages.normal,
                buttonImages.mouseOver,
                buttonImages.mouseDown,
                CloseButtonForSingers_Click
            );
        }

        private void CloseButtonForSingers_Click(object sender, EventArgs e)
        {
            // 暂停布局
            this.SuspendLayout();

            SetHandWritingForSingersAndButtonsVisibility(false);

            // 恢复布局
            this.ResumeLayout();
        }
    }
}