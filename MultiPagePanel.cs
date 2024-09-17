// MultiPagePanel.cs
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO; // 添加这个行

namespace DualScreenDemo
{
    public class MultiPagePanel : Panel
    {
        // 枚舉類型 ViewMode 用於區分顯示“歌星”還是“歌曲”
        public enum ViewMode
        {
            Singer,
            Song
        }

        private ViewMode currentViewMode = ViewMode.Song; // 默認為顯示歌曲
        public ImagePanel prevPagePanel;
        public ImagePanel currentPagePanel;
        public ImagePanel nextPagePanel;

        private List<SongData> currentSongList;
        private List<Artist> currentSingerList;
        private List<PlayState> currentPlayStates;
        private const int SongsPerPage = 16;
        private const int Rows = 8;
        private const int Columns = 2;
        public int currentPageIndex = 0;

        private Point initialMousePosition;
        private bool isDragging = false;
        private const int DragThreshold = 50; // 可以根据需要调整这个值
        // 定义页面滑动的阈值
        private int maxPositiveDeltaX = 0; // 最大正向偏移
        private int maxNegativeDeltaX = 0; // 最大负向偏移
        private const int ShiftThreshold = 150; // 调整这个值以改变滑动距离
        private bool isSimplified = false;
        private bool usePlayStates = false;  // 默认为不使用播放状态

        public bool IsSimplified
        {
            get { return isSimplified; }
            set
            {
                isSimplified = value;
                // Reload the current, previous, and next pages to reflect the change
                LoadPage(currentPageIndex - 1);
                LoadPage(currentPageIndex);
                LoadPage(currentPageIndex + 1);
            }
        }

        public MultiPagePanel()
        {
            this.DoubleBuffered = true; // 减少闪烁
            this.AutoScroll = false; // 禁用自动滚动条

            // 初始化三个子Panel，每个都有800宽度
            InitializePages();

            // 注册鼠标事件
            this.MouseDown += MultiPagePanel_MouseDown;
            this.MouseMove += MultiPagePanel_MouseMove;
            this.MouseUp += MultiPagePanel_MouseUp;

            // 输出面板的位置信息
            Console.WriteLine("Prev Page Panel Location: " + prevPagePanel.Location);
            Console.WriteLine("Current Page Panel Location: " + currentPagePanel.Location);
            Console.WriteLine("Next Page Panel Location: " + nextPagePanel.Location);

            // 输出面板的大小信息
            Console.WriteLine("Prev Page Panel Size: " + prevPagePanel.Size);
            Console.WriteLine("Current Page Panel Size: " + currentPagePanel.Size);
            Console.WriteLine("Next Page Panel Size: " + nextPagePanel.Size);
        }

        private void InitializePages()
        {
            prevPagePanel = new ImagePanel();
            currentPagePanel = new ImagePanel();
            nextPagePanel = new ImagePanel();

            // Example: setting a background image with cropping
            string imagePath = Path.Combine(Application.StartupPath, @"themes\superstar\555011.jpg"); // 更改为你的图片路径
            Rectangle cropArea = new Rectangle(0, 227, 1201, 512); // 根据需要调整裁剪区域

            prevPagePanel.SetBackgroundImageFromFile(imagePath, cropArea);
            currentPagePanel.SetBackgroundImageFromFile(imagePath, cropArea);
            nextPagePanel.SetBackgroundImageFromFile(imagePath, cropArea);

            PrimaryForm.ResizeAndPositionControl(prevPagePanel, -1150, 0, 1150, 512);
            PrimaryForm.ResizeAndPositionControl(currentPagePanel, 0, 0, 1150, 512);
            PrimaryForm.ResizeAndPositionControl(nextPagePanel, 1150, 0, 1150, 512);

            this.Controls.Add(prevPagePanel);
            this.Controls.Add(currentPagePanel);
            this.Controls.Add(nextPagePanel);
        }

        public void LoadSingers(List<Artist> singerList)
        {
            currentSingerList = singerList;
            currentViewMode = ViewMode.Singer;
            currentPageIndex = 0;
            LoadPage(currentPageIndex - 1);
            LoadPage(currentPageIndex);
            LoadPage(currentPageIndex + 1);
        }

        public void LoadSongs(List<SongData> songs)
        {
            currentSongList = songs;
            currentViewMode = ViewMode.Song;
            usePlayStates = false;
            LoadPage(currentPageIndex - 1);
            LoadPage(currentPageIndex);
            LoadPage(currentPageIndex + 1);
        }

        public void LoadPlayedSongs(List<SongData> songs, List<PlayState> playStates)
        {
            currentSongList = songs;
            currentPlayStates = playStates;
            currentViewMode = ViewMode.Song;
            usePlayStates = true;
            LoadPageWithStates(currentPageIndex - 1);
            LoadPageWithStates(currentPageIndex);
            LoadPageWithStates(currentPageIndex + 1);
        }

        private void LoadPage(int pageIndex)
        {
            ImagePanel targetPanel = IdentifyTargetPanel(pageIndex);
            targetPanel.Controls.Clear();

            if (currentViewMode == ViewMode.Singer)
            {
                LoadSingersPage(pageIndex, targetPanel);
            }
            else if (currentViewMode == ViewMode.Song)
            {
                LoadSongsPage(pageIndex, targetPanel);
            }
        }


        private void LoadSongsPage(int pageIndex, ImagePanel targetPanel)
        {
            if (pageIndex < 0 || pageIndex * SongsPerPage >= currentSongList.Count) return;

            int start = pageIndex * SongsPerPage;
            int end = Math.Min(start + SongsPerPage, currentSongList.Count);

            for (int i = start; i < end; i++)
            {
                int row = (i % SongsPerPage) % Rows;
                int column = (i % SongsPerPage) / Rows;
                var song = currentSongList[i];
                AddSongLabel(song, row, column, targetPanel);
            }
        }

        private void LoadSingersPage(int pageIndex, ImagePanel targetPanel)
        {
            int totalSingers = currentSingerList.Count;
            const int SingersPerPage = SongsPerPage;

            if (pageIndex < 0 || pageIndex * SingersPerPage >= totalSingers) return;

            int start = pageIndex * SingersPerPage;
            int end = Math.Min(start + SingersPerPage, totalSingers);

            for (int i = start; i < end; i++)
            {
                int row = (i % SingersPerPage) % Rows;
                int column = (i % SingersPerPage) / Rows;
                Artist artist = currentSingerList[i];  // 直接從 currentSingerList 中獲取 Artist 對象

                Label singerLabel = new Label()
                {
                    Text = artist.Name,  // 顯示歌手的名字
                    ForeColor = Color.White,
                    BackColor = Color.Transparent,
                    Font = new Font("微軟正黑體", 24, FontStyle.Bold),
                    Location = new Point(30 + column * 550, row * 64),
                    Size = new Size(510, 40),
                    TextAlign = ContentAlignment.TopLeft,
                    Tag = artist.Name  // 將歌手名存儲在標籤的 Tag 中
                };

                singerLabel.Click += (sender, e) =>
                {
                    string clickedSinger = ((Label)sender).Tag.ToString();
                    LoadSongsBySinger(clickedSinger);  // 根據歌手名加載該歌手的歌曲
                };

                PrimaryForm.ResizeAndPositionControl(singerLabel, singerLabel.Location.X, singerLabel.Location.Y, singerLabel.Size.Width, singerLabel.Size.Height);
                targetPanel.Controls.Add(singerLabel);
            }
        }

        public void LoadSongsBySinger(string singerName)
        {
            // 通過歌手名從 SongManager 或 ArtistManager 加載該歌手的所有歌曲
            var songsBySinger = SongListManager.Instance.GetSongsByArtist(singerName); 
            LoadSongs(songsBySinger);  // 將歌曲加載到界面上
        }

        private void LoadPageWithStates(int pageIndex)
        {
            // Identify the panel that should be loaded based on pageIndex
            ImagePanel targetPanel = IdentifyTargetPanel(pageIndex);
            targetPanel.Controls.Clear();

            // Check if the pageIndex is within the valid range
            if (pageIndex < 0 || pageIndex * SongsPerPage >= currentSongList.Count)
            {
                return;  // If the page index is out of range, exit the method
            }

            int start = pageIndex * SongsPerPage;
            int end = Math.Min(start + SongsPerPage, currentSongList.Count);

            for (int i = start; i < end; i++)
            {
                int row = (i % SongsPerPage) % Rows;
                int column = (i % SongsPerPage) / Rows;
                var song = currentSongList[i];
                var playState = currentPlayStates[i];
                AddSongLabelWithState(song, playState, row, column, targetPanel);
            }
        }

        private ImagePanel IdentifyTargetPanel(int pageIndex)
        {
            if (pageIndex == currentPageIndex - 1)
                return prevPagePanel;
            else if (pageIndex == currentPageIndex + 1)
                return nextPagePanel;
            else
                return currentPagePanel;
        }

        private void AddSongLabel(SongData song, int row, int column, Panel targetPanel)
        {
            // Calculate base coordinates for the labels
            int baseX = 25 + column * 550;  // Horizontal spacing
            int baseY = row * 64;      // Vertical spacing

            // Determine the text to display based on isSimplified
            string songTitle = isSimplified ? song.SongSimplified : song.Song;

            // Create and configure the label for the song title
            Label titleLabel = new Label()
            {
                Text = songTitle,
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Font = new Font("微軟正黑體", 24, FontStyle.Bold),
                Location = new Point(baseX + 32, baseY),
                Size = new Size(510, 37),
                TextAlign = ContentAlignment.TopLeft,
                Tag = song  // Storing the song object for reference
            };
            // Attach mouse event handlers for the title label
            titleLabel.MouseDown += Label_MouseDown;
            titleLabel.MouseMove += Label_MouseMove;
            titleLabel.MouseUp += Label_MouseUp;

            // Check if HumanVoice is 1 and add an icon if so
            if (song.HumanVoice == 1)
            {
                PictureBox icon = new PictureBox()
                {
                    Image = Image.FromFile(Path.Combine(Application.StartupPath, @"themes\superstar\其他符號_人聲\其他符號_人聲.png")), // Set your icon path
                    SizeMode = PictureBoxSizeMode.AutoSize,
                    Location = new Point(baseX + 5, baseY + 5) // Adjust the icon position as needed
                };

                PrimaryForm.ResizeAndPositionControl(icon, icon.Location.X, icon.Location.Y, icon.Size.Width, icon.Size.Height);

                targetPanel.Controls.Add(icon);
            }

            // Determine the text for the artist label based on the presence of ArtistB
            string artistText = isSimplified ? 
                (string.IsNullOrWhiteSpace(song.ArtistBSimplified) ? song.ArtistASimplified : song.ArtistASimplified + " - " + song.ArtistBSimplified) :
                (string.IsNullOrWhiteSpace(song.ArtistB) ? song.ArtistA : song.ArtistA + " - " + song.ArtistB);

            // Create and configure the label for the artist
            Label artistLabel = new Label()
            {
                Text = artistText,
                ForeColor = Color.LightGreen,
                BackColor = Color.Transparent,
                Font = new Font("微軟正黑體", 16, FontStyle.Bold),
                Location = new Point(baseX, baseY + 37), // Positioned right below the title label within the same cell
                Size = new Size(550, 27),
                TextAlign = ContentAlignment.BottomRight,
                Tag = song  // Storing the song object for reference
            };
            // Attach mouse event handlers for the artist label
            artistLabel.MouseDown += Label_MouseDown;
            artistLabel.MouseMove += Label_MouseMove;
            artistLabel.MouseUp += Label_MouseUp;

            PrimaryForm.ResizeAndPositionControl(titleLabel, titleLabel.Location.X, titleLabel.Location.Y, titleLabel.Size.Width, titleLabel.Size.Height);
            PrimaryForm.ResizeAndPositionControl(artistLabel, artistLabel.Location.X, artistLabel.Location.Y, artistLabel.Size.Width, artistLabel.Size.Height);
            
            // Console.WriteLine("titleLabel Size: ", titleLabel.Size);
            // Console.WriteLine("artistLabel Size: ", artistLabel.Size);

            // Add the labels to the target panel
            targetPanel.Controls.Add(titleLabel);
            targetPanel.Controls.Add(artistLabel);
        }

        private void AddSongLabelWithState(SongData song, PlayState playState, int row, int column, Panel targetPanel)
        {
            // Calculate base coordinates for the labels
            int baseX = 25 + column * 573;  // Horizontal spacing
            int baseY = row * 62;      // Vertical spacing

            // Create and configure the label for the song title
            Label titleLabel = new Label()
            {
                Text = song.Song,
                ForeColor = Color.White,
                BackColor = Color.Transparent,
                Font = new Font("微軟正黑體", 24, FontStyle.Bold),
                Location = new Point(baseX, baseY),
                Size = new Size(573, 37),
                TextAlign = ContentAlignment.TopLeft,
                Tag = song  // Storing the song object for reference
            };
            // Attach mouse event handlers for the title label
            titleLabel.MouseDown += Label_MouseDown;
            titleLabel.MouseMove += Label_MouseMove;
            titleLabel.MouseUp += Label_MouseUp;

            // Determine the text for the artist label based on the presence of ArtistB
            string artistText = string.IsNullOrWhiteSpace(song.ArtistB) ? song.ArtistA : song.ArtistA + " - " + song.ArtistB;

            // Create and configure the label for the artist
            Label artistLabel = new Label()
            {
                Text = artistText,
                ForeColor = Color.LightGreen,
                BackColor = Color.Transparent,
                Font = new Font("微軟正黑體", 16, FontStyle.Bold),
                Location = new Point(baseX, baseY + 37), // Positioned right below the title label within the same cell
                Size = new Size(573, 25),
                TextAlign = ContentAlignment.BottomRight,
                Tag = song  // Storing the song object for reference
            };
            // Attach mouse event handlers for the artist label
            artistLabel.MouseDown += Label_MouseDown;
            artistLabel.MouseMove += Label_MouseMove;
            artistLabel.MouseUp += Label_MouseUp;

            // 根据播放状态添加不同的提示
            switch (playState)
            {
                case PlayState.Playing:
                    titleLabel.Text += " (播放中)";
                    titleLabel.ForeColor = Color.LightGreen; // 播放中的文本颜色为绿色
                    break;
                case PlayState.Played:
                    titleLabel.Text += " (播放完畢)";
                    titleLabel.ForeColor = Color.Gray; // 播放完毕的文本颜色为灰色
                    break;
                case PlayState.NotPlayed:
                    // 此处可以添加其他状态的显示逻辑
                    titleLabel.ForeColor = Color.White;
                    break;
            }

            // Add the labels to the target panel
            targetPanel.Controls.Add(titleLabel);
            targetPanel.Controls.Add(artistLabel);
        }

        private void Label_MouseDown(object sender, MouseEventArgs e)
        {
            // 检查 sender 是否为目标标签
            Label clickedLabel = sender as Label;
            if (e.Button == MouseButtons.Left)
            {
                initialMousePosition = new Point(e.Location.X + clickedLabel.Location.X, e.Location.Y + clickedLabel.Location.Y); // 记录鼠标按下的位置
                Console.WriteLine("Label MouseDown at: " + initialMousePosition.ToString());
                isDragging = false;
            }
        }

        private void Label_MouseMove(object sender, MouseEventArgs e)
        {
            // 检查 sender 是否为目标标签
            Label clickedLabel = sender as Label;

            if (e.Button == MouseButtons.Left)
            {
                if (Math.Abs(e.X + clickedLabel.Location.X - initialMousePosition.X) > DragThreshold ||
                    Math.Abs(e.Y + clickedLabel.Location.Y - initialMousePosition.Y) > DragThreshold)
                {
                    Console.WriteLine("Label MouseMove at: " + new Point(e.Location.X + clickedLabel.Location.X, e.Location.Y + clickedLabel.Location.Y).ToString());
                    isDragging = true; // 更新拖动状态为真
                    MultiPagePanel_MouseMove(sender, e);
                    // ShiftPages(e.X + clickedLabel.Location.X - initialMousePosition.X);
                }
            }
        }

        private void Label_MouseUp(object sender, MouseEventArgs e)
        {
            // 检查 sender 是否为目标标签
            Label clickedLabel = sender as Label;

            Console.WriteLine("Label MouseUp at: " + new Point(e.Location.X + clickedLabel.Location.X, e.Location.Y + clickedLabel.Location.Y).ToString());
            if (isDragging)
            {
                // 执行面板的移动操作
                // ShiftPages(e.X + clickedLabel.Location.X - initialMousePosition.X);
                MultiPagePanel_MouseUp(sender, e);
            }
            else
            {
                // 触发标签的点击事件
                OnLabelClick(sender, e);
            }
            isDragging = false; // 重置拖动状态
        }

        private void OnLabelClick(object sender, EventArgs e)
        {
            PrimaryForm.Instance.Label_Click(sender, e);
        }

        private void MultiPagePanel_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                initialMousePosition = new Point(e.Location.X + currentPagePanel.Location.X, e.Location.Y + currentPagePanel.Location.Y); // 记录鼠标按下的位置
                Console.WriteLine("MouseDown at: " + initialMousePosition.ToString());
                isDragging = true; // 开始拖动
                this.Capture = true; // 捕获鼠标
            }
        }

        private void MultiPagePanel_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                int deltaX = e.X - initialMousePosition.X;

                // 检查 sender 是否为 Label
                Label clickedLabel = sender as Label;
                if (clickedLabel != null)
                {
                    // 在计算 deltaX 时添加标签的 Location 值
                    deltaX += clickedLabel.Location.X;

                    // 使用 String.Format 打印标签相关信息
                    Console.WriteLine(String.Format(
                        "MouseMove on Label '{0}' at: {1} (Label Location: {2})",
                        clickedLabel.Text,
                        new Point(e.Location.X + clickedLabel.Location.X, e.Location.Y + clickedLabel.Location.Y).ToString(),
                        clickedLabel.Location.ToString()
                    ));
                }
                else
                {
                    // 使用 String.Format 打印面板相关信息
                    Console.WriteLine(String.Format(
                        "MouseMove on Panel at: {0}",
                        e.Location.ToString()
                    ));
                }

                // 更新最大偏移量
                if (deltaX > maxPositiveDeltaX) maxPositiveDeltaX = deltaX;
                if (deltaX < maxNegativeDeltaX) maxNegativeDeltaX = deltaX;

                // 打印信息
                Console.WriteLine(String.Format("MouseMove at: {0}, DeltaX: {1}, Max Positive: {2}, Max Negative: {3}", 
                    e.Location.ToString(), deltaX, maxPositiveDeltaX, maxNegativeDeltaX));

                // 移动页面
                ShiftPages(deltaX);
            }
        }

        private void MultiPagePanel_MouseUp(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                int deltaX = e.X - initialMousePosition.X;

                // 检查 sender 是否为 Label
                Label clickedLabel = sender as Label;
                if (clickedLabel != null)
                {
                    // 在计算 deltaX 时添加标签的 Location 值
                    deltaX += clickedLabel.Location.X;

                    // 使用 String.Format 打印标签相关信息
                    Console.WriteLine(String.Format(
                        "MouseUp on Label '{0}' at: {1} (Label Location: {2})",
                        clickedLabel.Text,
                        new Point(e.Location.X + clickedLabel.Location.X, e.Location.Y + clickedLabel.Location.Y).ToString(),
                        clickedLabel.Location.ToString()
                    ));
                }
                else
                {
                    // 使用 String.Format 打印面板相关信息
                    Console.WriteLine(String.Format(
                        "MouseUp on Panel at: {0}",
                        e.Location.ToString()
                    ));
                }

                Console.WriteLine("MouseUp at: " + e.Location.ToString());
                isDragging = false; // 停止拖动
                this.Capture = false; // 释放鼠标捕获
                FinishShift(deltaX); // 调用MultiPagePanel的方法来完成页面切换
            }
        }

        // 假设方法：根据用户的滑动来调整Panel位置，显示不同的页面
        public void ShiftPages(int deltaX)
        {
            // 根据 deltaX 调整位置
            int newLeft = currentPagePanel.Left + deltaX;

            // 输出调试信息
            Console.WriteLine(String.Format("DeltaX: {0}, Current Left: {1}, New Left: {2}", deltaX, currentPagePanel.Left, newLeft));

            // 限制滑动范围
            if (newLeft > 1201) newLeft = 1201;
            if (newLeft < -1201) newLeft = -1201;

            prevPagePanel.Location = new Point(newLeft - 1201, 0);
            currentPagePanel.Location = new Point(newLeft, 0);
            nextPagePanel.Location = new Point(newLeft + 1201, 0);

            // 强制重新绘制
            this.Invalidate();
            this.Update();
        }

        public void FinishShift(int deltaX)
        {
            // 输出调试信息
            Console.WriteLine(String.Format("DeltaX: {0}, Current Left: {1}", deltaX, currentPagePanel.Left));

            // 根据当前位置决定是回到原页面还是切换到新页面
            // 并实现动画效果
            // 根据滑动方向和阈值，确定要移动到的最终位置
            if (maxPositiveDeltaX > ShiftThreshold)
            {
                // 向右滑动超过阈值，移动到下一页
                Console.WriteLine("Shifting based on max positive deltaX");
                LoadPreviousPage();
            }
            else if (maxNegativeDeltaX < -ShiftThreshold)
            {
                Console.WriteLine("Shifting based on max negative deltaX");
                LoadNextPage();
            }
            else
            {
                // 滑动没有超过阈值，保持当前页面位置
                Console.WriteLine("Remaining on the Current Page");
            }

            prevPagePanel.Location = new Point(-1201, 0);
            currentPagePanel.Location = new Point(0, 0);
            nextPagePanel.Location = new Point(1201, 0);

            // 重置最大偏移量
            maxPositiveDeltaX = 0;
            maxNegativeDeltaX = 0;
        }

        public void LoadPreviousPage()
        {
            // 如果還有上一頁，才進行頁面加載
            if (currentPageIndex > 0)
            {
                currentPageIndex--;

                // 根據播放狀態和當前的ViewMode來加載上一頁
                if (usePlayStates && currentViewMode == ViewMode.Song)
                {
                    LoadPageWithStates(currentPageIndex - 1);
                    LoadPageWithStates(currentPageIndex);
                    LoadPageWithStates(currentPageIndex + 1);
                }
                else
                {
                    LoadPage(currentPageIndex - 1);
                    LoadPage(currentPageIndex);
                    LoadPage(currentPageIndex + 1);
                }
            }
        }

        public void LoadNextPage()
        {
            int totalPages;

            // 確定根據當前 ViewMode 計算總頁數
            if (currentViewMode == ViewMode.Singer)
            {
                int totalSingers = currentSingerList.Count;  // 直接使用 currentSingerList 的數量
                totalPages = (int)Math.Ceiling((double)totalSingers / SongsPerPage);
            }
            else // ViewMode.Song
            {
                totalPages = (int)Math.Ceiling((double)currentSongList.Count / SongsPerPage);
            }

            // 如果還有下一頁，才進行頁面加載
            if (currentPageIndex < totalPages - 1)
            {
                currentPageIndex++;

                // 根據播放狀態和當前的ViewMode來加載下一頁
                if (usePlayStates && currentViewMode == ViewMode.Song)
                {
                    LoadPageWithStates(currentPageIndex - 1);
                    LoadPageWithStates(currentPageIndex);
                    LoadPageWithStates(currentPageIndex + 1);
                }
                else
                {
                    LoadPage(currentPageIndex - 1);
                    LoadPage(currentPageIndex);
                    LoadPage(currentPageIndex + 1);
                }
            }
        }
    }
}