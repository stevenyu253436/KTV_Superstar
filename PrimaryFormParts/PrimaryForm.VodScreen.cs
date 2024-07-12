using System;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using System.Linq;

namespace DualScreenDemo
{
    public partial class PrimaryForm
    {
        private Button vodButton;
        private Button insertButton;
        private Button albumButton;
        private Button favoriteButton;
        private Panel disabledPanel;
        private Button vodScreenCloseButton;
        
        private void InitializeButtonsForVodScreenPictureBox()
        {
            int screenWidth = 1440;
            int screenHeight = 900;
            int pictureBoxWidth = 938;
            int pictureBoxHeight = 209;

            int xPosition = (screenWidth - pictureBoxWidth) / 2;
            int yPosition = (screenHeight - pictureBoxHeight) / 2;

            // 创建点播按钮
            vodButton = new Button();
            vodButton.Text = "";
            ResizeAndPositionButton(vodButton, xPosition + 18, yPosition + 121, 147, 73);
            vodButton.BackgroundImage = Image.FromFile(Path.Combine(Application.StartupPath, @"themes\superstar\點播介面\點播介面_點歌.png"));
            vodButton.BackgroundImageLayout = ImageLayout.Stretch;
            vodButton.FlatStyle = FlatStyle.Flat;
            vodButton.FlatAppearance.BorderSize = 0; // Remove border
            vodButton.BackColor = Color.Transparent;
            vodButton.FlatAppearance.MouseDownBackColor = Color.Transparent; // Mousedown background color
            vodButton.FlatAppearance.MouseOverBackColor = Color.Transparent; // Mouseover background color
            vodButton.Click += VodButton_Click;  // 添加点击事件处理
            vodButton.Visible = false;

            // 创建插播按钮
            insertButton = new Button();
            insertButton.Text = "";
            ResizeAndPositionButton(insertButton, xPosition + 182, yPosition + 121, 147, 73);
            insertButton.BackgroundImage = Image.FromFile(Path.Combine(Application.StartupPath, @"themes\superstar\點播介面\點播介面_插播.png"));
            insertButton.BackgroundImageLayout = ImageLayout.Stretch;
            insertButton.FlatStyle = FlatStyle.Flat;
            insertButton.FlatAppearance.BorderSize = 0; // Remove border
            insertButton.BackColor = Color.Transparent;
            insertButton.FlatAppearance.MouseDownBackColor = Color.Transparent; // Mousedown background color
            insertButton.FlatAppearance.MouseOverBackColor = Color.Transparent; // Mouseover background color
            insertButton.Click += InsertButton_Click;
            insertButton.Visible = false;

            // 创建历年专辑按钮
            albumButton = new Button();
            albumButton.Text = "";
            ResizeAndPositionButton(albumButton, xPosition + 345, yPosition + 121, 199, 73);
            albumButton.BackgroundImage = Image.FromFile(Path.Combine(Application.StartupPath, @"themes\superstar\點播介面\點播介面_歷年專輯.png"));
            albumButton.BackgroundImageLayout = ImageLayout.Stretch;
            albumButton.FlatStyle = FlatStyle.Flat;
            albumButton.FlatAppearance.BorderSize = 0; // Remove border
            albumButton.BackColor = Color.Transparent;
            albumButton.FlatAppearance.MouseDownBackColor = Color.Transparent; // Mousedown background color
            albumButton.FlatAppearance.MouseOverBackColor = Color.Transparent; // Mouseover background color
            albumButton.Click += AlbumButton_Click;
            albumButton.Visible = false;

            // 创建我的最爱按钮
            favoriteButton = new Button();
            favoriteButton.Text = "";
            ResizeAndPositionButton(favoriteButton, xPosition + 560, yPosition + 121, 199, 73);
            favoriteButton.BackgroundImage = Image.FromFile(Path.Combine(Application.StartupPath, @"themes\superstar\點播介面\點播介面_我的最愛.png"));
            favoriteButton.BackgroundImageLayout = ImageLayout.Stretch;
            favoriteButton.FlatStyle = FlatStyle.Flat;
            favoriteButton.FlatAppearance.BorderSize = 0; // Remove border
            favoriteButton.BackColor = Color.Transparent;
            favoriteButton.FlatAppearance.MouseDownBackColor = Color.Transparent; // Mousedown background color
            favoriteButton.FlatAppearance.MouseOverBackColor = Color.Transparent; // Mouseover background color
            // 创建一个半透明的面板
            disabledPanel = new Panel();
            disabledPanel.BackColor = Color.FromArgb(128, Color.Black); // 设置半透明灰色
            disabledPanel.Dock = DockStyle.Fill; // 铺满按钮
            disabledPanel.Visible = !IsUserLoggedIn(); // 如果用户未登录，则显示面板

            // 将面板添加到按钮上
            favoriteButton.Controls.Add(disabledPanel);
            favoriteButton.Click += FavoriteButton_Click;
            // 判断用户是否已输入内容来启用或禁用
            // 如果用户尚未登录，则禁用按钮
            if (!IsUserLoggedIn()) {
                favoriteButton.Enabled = false;
                favoriteButton.BackColor = SystemColors.Control; // 设置为默认的禁用背景色
            }
            favoriteButton.Visible = IsUserLoggedIn();

            // 创建关闭按钮
            vodScreenCloseButton = new Button();
            vodScreenCloseButton.Text = "";
            ResizeAndPositionButton(vodScreenCloseButton, xPosition + 773, yPosition + 121, 147, 73);
            vodScreenCloseButton.BackgroundImage = Image.FromFile(Path.Combine(Application.StartupPath, @"themes\superstar\點播介面\點播介面_關閉.png"));
            vodScreenCloseButton.BackgroundImageLayout = ImageLayout.Stretch;
            vodScreenCloseButton.FlatStyle = FlatStyle.Flat;
            vodScreenCloseButton.FlatAppearance.BorderSize = 0; // Remove border
            vodScreenCloseButton.BackColor = Color.Transparent;
            vodScreenCloseButton.FlatAppearance.MouseDownBackColor = Color.Transparent; // Mousedown background color
            vodScreenCloseButton.FlatAppearance.MouseOverBackColor = Color.Transparent; // Mouseover background color
            vodScreenCloseButton.Click += VodScreenCloseButton_Click;
            vodScreenCloseButton.Visible = false;

            // 添加按钮到 PictureBox 或其父控件
            this.Controls.Add(vodButton);
            this.Controls.Add(insertButton);
            this.Controls.Add(albumButton);
            this.Controls.Add(favoriteButton);
            this.Controls.Add(vodScreenCloseButton);
        }
        
        // 示例事件处理器
        private void VodButton_Click(object sender, EventArgs e)
        {
            // 点播按钮的逻辑
            OverlayForm.MainForm.AddSongToPlaylist(currentSelectedSong);
            SetVodScreenPictureBoxAndButtonsVisibility(false);
        }

        private void InsertButton_Click(object sender, EventArgs e)
        {
            // 插播按钮的逻辑
            OverlayForm.MainForm.InsertSongToPlaylist(currentSelectedSong);
            SetVodScreenPictureBoxAndButtonsVisibility(false);
        }

        private void AlbumButton_Click(object sender, EventArgs e)
        {
            // 历年专辑按钮的逻辑
            var selectedSongs = allSongs.Where(song => song.ArtistA == currentSelectedSong.ArtistA)
                            .OrderByDescending(song => song.AddedTime)
                            .ToList();

            UpdateSongList(selectedSongs);
            SetVodScreenPictureBoxAndButtonsVisibility(false);
        }

        private void FavoriteButton_Click(object sender, EventArgs e)
        {
            // 打印日誌
            Console.WriteLine("Favorite Button Clicked");

            // 我的最爱按钮的逻辑
            SongListManager.Instance.AddToFavorite(currentSelectedSong.SongNumber);
            SetVodScreenPictureBoxAndButtonsVisibility(false);
        }

        private void VodScreenCloseButton_Click(object sender, EventArgs e)
        {
            // 关闭按钮的逻辑
            SetVodScreenPictureBoxAndButtonsVisibility(false);
        }

        // 检查用户输入的方法
        private bool IsUserLoggedIn()
        {
            // 返回用户是否有输入（这里需要你根据实际应用场景来定义）
            return SongListManager.Instance.IsUserLoggedIn; // 示例返回值
        }

        private void SetVodScreenPictureBoxAndButtonsVisibility(bool isVisible)
        {
            // Set the visibility of PictureBoxToggleLight itself
            overlayPanel.Visible = isVisible;
            VodScreenPictureBox.Visible = isVisible;

            // Set visibility of each control directly added to the form that is related to the PictureBoxToggleLight
            vodButton.Visible = isVisible;
            insertButton.Visible = isVisible;
            albumButton.Visible = isVisible;
            favoriteButton.Visible = isVisible;
            vodScreenCloseButton.Visible = isVisible;

            // Console.WriteLine(String.Format("Location: {0}, Size: {1}, Visible: {2}", VodScreenPictureBox.Location, VodScreenPictureBox.Size, VodScreenPictureBox.Visible));

            if (isVisible)
            {
                // 判断用户是否已输入内容来启用或禁用
                if (IsUserLoggedIn())
                {
                    favoriteButton.Enabled = true;
                    favoriteButton.Controls.Remove(disabledPanel); // 移除panel
                }
                else
                {
                    favoriteButton.Enabled = false;
                    // favoriteButton.BackColor = SystemColors.Control; // 设置为默认的禁用背景色
                }

                // Bring the PictureBox to the front
                overlayPanel.BringToFront();
                VodScreenPictureBox.BringToFront();

                // Also bring each button to the front if visible
                vodButton.BringToFront();
                insertButton.BringToFront();
                albumButton.BringToFront();
                favoriteButton.BringToFront();
                vodScreenCloseButton.BringToFront();
            }
        }
    }
}