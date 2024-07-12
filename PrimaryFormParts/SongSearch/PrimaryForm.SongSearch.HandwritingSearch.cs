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
        private PictureBox pictureBoxHandWritingSongs;

        private Button refillButtonHandWritingSongs;
        private Button clearButtonHandWritingSongs;
        private Button closeButtonForSongs;

        private void HandWritingSearchButtonForSongs_Click(object sender, EventArgs e)
        {
            this.SuspendLayout();

            zhuyinSearchSongButton.BackgroundImage = zhuyinSearchSongNormalBackground;
            englishSearchSongButton.BackgroundImage = englishSearchSongNormalBackground;
            pinyinSearchSongButton.BackgroundImage = pinyinSearchSongNormalBackground;
            wordCountSearchSongButton.BackgroundImage = wordCountSearchSongNormalBackground;
            handWritingSearchSongButton.BackgroundImage = handWritingSearchSongActiveBackground;
            numberSearchSongButton.BackgroundImage = numberSearchSongNormalBackground;

            // 启用双缓冲以减少闪烁
            EnableDoubleBuffering(handWritingPanelForSongs);
            EnableDoubleBuffering(handwritingInputBoxForSongs);
            EnableDoubleBuffering(candidateListBoxForSongs);
            EnableDoubleBuffering(pictureBoxHandWritingSongs);
            EnableDoubleBuffering(refillButtonHandWritingSongs);
            EnableDoubleBuffering(closeButtonForSongs);

            // Load the configuration data
            var configData = LoadConfigData();
            string handWritingImagePath = Path.Combine(Application.StartupPath, configData["ImagePaths"]["HandWritingSongs"]);

            ShowImageOnPictureBoxHandWritingSongs(Path.Combine(Application.StartupPath, handWritingImagePath));
            
            SetZhuYinSingersAndButtonsVisibility(false);
            SetEnglishSingersAndButtonsVisibility(false);
            SetPinYinSingersAndButtonsVisibility(false);
            SetHandWritingForSingersAndButtonsVisibility(false);
            SetZhuYinSongsAndButtonsVisibility(false);
            SetEnglishSongsAndButtonsVisibility(false);
            SetPinYinSongsAndButtonsVisibility(false);
            SetHandWritingForSongsAndButtonsVisibility(true);

            this.ResumeLayout();
        }
        
        private Panel handWritingPanelForSongs;
        private InkOverlay inkOverlayForSongs;
        private RichTextBox handwritingInputBoxForSongs;
        private ListBox candidateListBoxForSongs;


        private void InitializeHandWritingForSongs()
        {
            InitializeHandWritingPanelForSongs();
            InitializeInkOverlayForSongs();
            InitializeHandwritingInputBoxForSongs();
            InitializeCandidateListBoxForSongs();
            InitializeSpecialButtonsForHandWritingSongs();
        }
        
        private void InitializeHandWritingPanelForSongs()
        {
            // 初始化搜索歌手的手写面板
            handWritingPanelForSongs = new Panel
            {
                BorderStyle = BorderStyle.FixedSingle,
                Visible = false // 初始设置为不可见
            };

            // 调整和设置手写面板的位置和尺寸
            ResizeAndPositionControl(handWritingPanelForSongs, 366, 448, 650, 260);

            // 将手写面板添加到窗体的控件集合中
            this.Controls.Add(handWritingPanelForSongs);
        }

        private void InitializeInkOverlayForSongs()
        {
            try
            {
                // 初始化搜索歌手的手写面板的 InkOverlay
                inkOverlayForSongs = new InkOverlay(handWritingPanelForSongs);
                inkOverlayForSongs.Enabled = false;
                inkOverlayForSongs.Ink = new Ink();
                inkOverlayForSongs.DefaultDrawingAttributes.Color = Color.Black;
                inkOverlayForSongs.DefaultDrawingAttributes.Width = 100;
                inkOverlayForSongs.Stroke += new InkCollectorStrokeEventHandler(InkOverlayForSongs_Stroke);

                // 在Panel完全配置好之后再启用InkOverlay
                inkOverlayForSongs.Enabled = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to initialize ink overlay for singers: " + ex.Message);
            }
        }

        private void InkOverlayForSongs_Stroke(object sender, InkCollectorStrokeEventArgs e)
        {
            // 处理来自歌曲搜索面板的墨迹
            RecognizeInk(inkOverlayForSongs, candidateListBoxForSongs);
        }

        private void InitializeHandwritingInputBoxForSongs()
        {
            // 初始化用于歌手搜索的输入框
            handwritingInputBoxForSongs = new RichTextBox
            {
                Font = new Font("微軟正黑體", (float)26 / 900 * Screen.PrimaryScreen.Bounds.Height, FontStyle.Regular), // 选择适合的字体和大小
                Visible = false
            };
            ResizeAndPositionControl(handwritingInputBoxForSongs, 366, 373, 541, 62);
            this.Controls.Add(handwritingInputBoxForSongs);

            handwritingInputBoxForSongs.TextChanged += (sender, e) =>
            {
                string searchText = handwritingInputBoxForSongs.Text;
                // 假设 allSongs 是存储所有歌曲数据的 List<SongData>
                // 使用 Union 来合并来自 ArtistA 和 ArtistB 的搜索结果
                var searchResults = allSongs.Where(song => song.Song.StartsWith(searchText)).ToList();
                currentPage = 0;
                currentSongList = searchResults;
                totalPages = (int)Math.Ceiling((double)searchResults.Count / itemsPerPage);

                multiPagePanel.currentPageIndex = 0;
                multiPagePanel.LoadSongs(currentSongList);
            };
        }    

        private void InitializeCandidateListBoxForSongs()
        {
            // 初始化搜索歌手的候选词列表
            candidateListBoxForSongs = new ListBox
            {
                Font = new Font("微軟正黑體", (float)26 / 900 * Screen.PrimaryScreen.Bounds.Height, FontStyle.Regular),
                Visible = false
            };
            ResizeAndPositionControl(candidateListBoxForSongs, 350 + 679, 448, 115, 260);
            candidateListBoxForSongs.SelectedIndexChanged += CandidateListBoxForSongs_SelectedIndexChanged;
            this.Controls.Add(candidateListBoxForSongs);
        }

        private void CandidateListBoxForSongs_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (candidateListBoxForSongs.SelectedIndex != -1)
            {
                string selectedWord = candidateListBoxForSongs.SelectedItem.ToString();
                handwritingInputBoxForSongs.Text += selectedWord;  // 将选中的词设置为输入框的文本
                candidateListBoxForSongs.Visible = false; // 选择后隐藏列表
                
                // 清除手写板上的所有墨迹
                if (inkOverlayForSongs != null)
                {
                    inkOverlayForSongs.Ink.DeleteStrokes();
                    handWritingPanelForSongs.Invalidate(); // 请求重绘，更新界面
                }
            }
        }

        private void ShowImageOnPictureBoxHandWritingSongs(string imagePath)
        {
            // Load the original image
            Bitmap originalImage = new Bitmap(imagePath);

            // Define the display area using the coordinates from the config
            Rectangle displayArea = new Rectangle(350, 360, 810, 360);

            // Set the PictureBox's image
            pictureBoxHandWritingSongs.Image = originalImage;

            // Resize and position the PictureBox according to the loaded coordinates
            ResizeAndPositionPictureBox(pictureBoxHandWritingSongs, displayArea.X, displayArea.Y, displayArea.Width, displayArea.Height);

            pictureBoxHandWritingSongs.Visible = true;
        }

        private void SetHandWritingForSongsAndButtonsVisibility(bool isVisible)
        {
            // 启用双缓冲以减少闪烁
            EnableDoubleBuffering(handWritingPanelForSongs);
            EnableDoubleBuffering(handwritingInputBoxForSongs);
            EnableDoubleBuffering(candidateListBoxForSongs);
            EnableDoubleBuffering(pictureBoxHandWritingSongs);
            EnableDoubleBuffering(refillButtonHandWritingSongs);
            EnableDoubleBuffering(clearButtonHandWritingSongs);
            EnableDoubleBuffering(closeButtonForSongs);

            // 设置控件的可见性
            handWritingPanelForSongs.Visible = isVisible;
            handwritingInputBoxForSongs.Visible = isVisible;
            inkOverlayForSongs.Enabled = isVisible;
            candidateListBoxForSongs.Visible = isVisible; // 同时隐藏候选词列表
            pictureBoxHandWritingSongs.Visible = isVisible;
            refillButtonHandWritingSongs.Visible = isVisible;
            clearButtonHandWritingSongs.Visible = isVisible;
            closeButtonForSongs.Visible = isVisible;

            if (isVisible)
            {
                // Bring controls to the front if they are visible
                pictureBoxHandWritingSongs.BringToFront();
                handWritingPanelForSongs.BringToFront();
                handwritingInputBoxForSongs.BringToFront();
                candidateListBoxForSongs.BringToFront();
                refillButtonHandWritingSongs.BringToFront();
                clearButtonHandWritingSongs.BringToFront();
                closeButtonForSongs.BringToFront();
            }
        }

        private void InitializeSpecialButtonsForHandWritingSongs()
        {
            // 初始化“修改”按钮
            InitializeRefillButtonHandwritingSongs();

            // 初始化“清除”按钮
            InitializeClearButtonHandWritingSongs();

            // 初始化“关闭”按钮
            InitializeCloseButtonForSongs();
        }

        private void InitializeRefillButtonHandwritingSongs()
        {
            var data = LoadConfigData();
            refillButtonHandWritingCoords = LoadSpecialButtonCoordinates(data, "SpecialButtonCoordinates", "refillButtonHandWritingSongs");
            var buttonImages = LoadButtonImages(data, "RefillButtonImagesHandWriting");

            refillButtonHandWritingSongs = CreateSpecialButton(
                "refillButtonHandWritingSongs",
                refillButtonHandWritingCoords,
                buttonImages.normal,
                buttonImages.mouseOver,
                buttonImages.mouseDown,
                RefillButtonHandWritingSongs_Click
            );
        }

        private void RefillButtonHandWritingSongs_Click(object sender, EventArgs e)
        {
            handwritingInputBoxForSongs.Text = "";
        }

        private void InitializeClearButtonHandWritingSongs()
        {
            var data = LoadConfigData();
            clearButtonHandWritingCoords = LoadSpecialButtonCoordinates(data, "SpecialButtonCoordinates", "clearButtonHandWritingSongs");
            var buttonImages = LoadButtonImages(data, "ClearButtonImagesHandWriting");

            clearButtonHandWritingSongs = CreateSpecialButton(
                "clearButtonHandWritingSongs",
                clearButtonHandWritingCoords,
                buttonImages.normal,
                buttonImages.mouseOver,
                buttonImages.mouseDown,
                ClearButtonHandWritingSongs_Click
            );
        }

        private void ClearButtonHandWritingSongs_Click(object sender, EventArgs e)
        {
            if (this.Controls.Contains(handWritingPanelForSongs) && inkOverlayForSongs != null)
            {
                inkOverlayForSongs.Ink.DeleteStrokes();
                handWritingPanelForSongs.Invalidate(); // 请求重绘，更新界面
            }
        }

        private void InitializeCloseButtonForSongs()
        {
            var data = LoadConfigData();
            closeButtonHandWritingCoords = LoadSpecialButtonCoordinates(data, "SpecialButtonCoordinates", "closeButtonForSongs");
            var buttonImages = LoadButtonImages(data, "CloseButtonImagesHandWriting");

            closeButtonForSongs = CreateSpecialButton(
                "closeButtonForSongs",
                closeButtonHandWritingCoords,
                buttonImages.normal,
                buttonImages.mouseOver,
                buttonImages.mouseDown,
                CloseButtonForSongs_Click
            );
        }

        private void CloseButtonForSongs_Click(object sender, EventArgs e)
        {
            // 暂停布局
            this.SuspendLayout();

            SetHandWritingForSongsAndButtonsVisibility(false);

            // 恢复布局
            this.ResumeLayout();
        }
    }
}