using System;
using System.Drawing;
using System.Windows.Forms;

namespace DualScreenDemo
{
    public partial class PrimaryForm : Form
    {
        public Panel primaryScreenPanel;
        public Button syncServiceBellButton;
        public Button syncCutSongButton;
        public Button syncReplayButton;
        public Button syncOriginalSongButton;
        public Button syncMuteButton;
        public Button syncPauseButton;
        public Button syncPlayButton;
        public Button syncVolumeUpButton;
        public Button syncVolumeDownButton;
        public Button syncMicUpButton;
        public Button syncMicDownButton;
        public Button syncCloseButton;
        
        private void InitializeSyncScreen()
        {
            this.primaryScreenPanel = new System.Windows.Forms.Panel();
            this.syncServiceBellButton = new System.Windows.Forms.Button();
            this.syncCutSongButton = new System.Windows.Forms.Button();
            this.syncReplayButton = new System.Windows.Forms.Button();
            this.syncOriginalSongButton = new System.Windows.Forms.Button();
            this.syncMuteButton = new System.Windows.Forms.Button();
            this.syncPauseButton = new System.Windows.Forms.Button();
            this.syncPlayButton = new System.Windows.Forms.Button();
            this.syncVolumeUpButton = new System.Windows.Forms.Button();
            this.syncVolumeDownButton = new System.Windows.Forms.Button();
            this.syncMicUpButton = new System.Windows.Forms.Button();
            this.syncMicDownButton = new System.Windows.Forms.Button();
            this.syncCloseButton = new System.Windows.Forms.Button();
            this.SuspendLayout();

            // 
            // primaryScreenPanel
            // 
            // this.primaryScreenPanel.Location = new System.Drawing.Point(0, 0);
            // this.primaryScreenPanel.Size = new System.Drawing.Size(1440, 900);
            ResizeAndPositionControl(this.primaryScreenPanel, 0, 0, 1440, 900);
            this.primaryScreenPanel.TabIndex = 0;
            this.primaryScreenPanel.BorderStyle = BorderStyle.FixedSingle;
            this.primaryScreenPanel.BackColor = System.Drawing.Color.Black; // 设置背景颜色为黑色

            //
            // ServiceBellButton
            //
            ConfigureButton(this.syncServiceBellButton, 1240, 17, 161, 161, 
                resizedNormalStateImageForSyncScreen, resizedNormalStateImageForSyncScreen, resizedNormalStateImageForSyncScreen, 
                (sender, e) => SendCommandThroughSerialPort("a2 53 a4"));
            
            //
            // CutSongButton
            //
            ConfigureButton(this.syncCutSongButton, 1218, 195, 205, 56, resizedNormalStateImageForSyncScreen, resizedNormalStateImageForSyncScreen, resizedNormalStateImageForSyncScreen, (sender, e) => videoPlayerForm.SkipToNextSong());

            //
            // OriginalSongButton
            //
            ConfigureButton(this.syncReplayButton, 1218, 265, 205, 56, resizedNormalStateImageForSyncScreen, resizedNormalStateImageForSyncScreen, resizedNormalStateImageForSyncScreen, ReplayButton_Click);

            //
            // OriginalSongButton
            //
            ConfigureButton(this.syncOriginalSongButton, 1218, 335, 205, 56, resizedNormalStateImageForSyncScreen, resizedNormalStateImageForSyncScreen, resizedNormalStateImageForSyncScreen, null);

            //
            // muteButton
            //
            ConfigureButton(this.syncMuteButton, 1218, 406, 205, 55, resizedNormalStateImageForSyncScreen, resizedNormalStateImageForSyncScreen, resizedNormalStateImageForSyncScreen, MuteUnmuteButton_Click);

            //
            // pauseButton
            //
            ConfigureButton(this.syncPauseButton, 1218, 475, 205, 56, resizedNormalStateImageForSyncScreen, resizedNormalStateImageForSyncScreen, resizedNormalStateImageForSyncScreen, SyncPauseButton_Click);
            ConfigureButton(this.syncPlayButton, 1218, 475, 205, 56, resizedNormalStateImageForSyncScreen, resizedNormalStateImageForSyncScreen, resizedNormalStateImageForSyncScreen, SyncPlayButton_Click);

            //
            // volumeUpButton
            //
            ConfigureButton(this.syncVolumeUpButton, 1218, 546, 205, 55, resizedNormalStateImageForSyncScreen, resizedNormalStateImageForSyncScreen, resizedNormalStateImageForSyncScreen, null);
            this.syncVolumeUpButton.MouseDown += (sender, e) => { OverlayForm.MainForm.ShowVolumeUpLabel(); volumeUpTimer.Start(); };
            this.syncVolumeUpButton.MouseUp += (sender, e) => { OverlayForm.MainForm.HideVolumeUpLabel(); volumeUpTimer.Stop(); };

            //
            // volumeDownButton
            //
            ConfigureButton(this.syncVolumeDownButton, 1218, 616, 205, 55, resizedNormalStateImageForSyncScreen, resizedNormalStateImageForSyncScreen, resizedNormalStateImageForSyncScreen, null);
            this.syncVolumeDownButton.MouseDown += (sender, e) => { OverlayForm.MainForm.ShowVolumeDownLabel(); volumeDownTimer.Start(); };
            this.syncVolumeDownButton.MouseUp += (sender, e) => { OverlayForm.MainForm.HideVolumeDownLabel(); volumeDownTimer.Stop(); };

            //
            // micUpButton
            //
            ConfigureButton(this.syncMicUpButton, 1218, 686, 205, 56, resizedNormalStateImageForSyncScreen, resizedNormalStateImageForSyncScreen, resizedNormalStateImageForSyncScreen, null);
            this.syncMicUpButton.MouseDown += (sender, e) => { OverlayForm.MainForm.ShowMicUpLabel(); micControlTimer.Tag = "a2 b5 a4"; micControlTimer.Start(); };
            this.syncMicUpButton.MouseUp += (sender, e) => { OverlayForm.MainForm.HideMicUpLabel(); micControlTimer.Stop(); };

            //
            // micDownButton
            //
            ConfigureButton(this.syncMicDownButton, 1218, 756, 205, 56, resizedNormalStateImageForSyncScreen, resizedNormalStateImageForSyncScreen, resizedNormalStateImageForSyncScreen, null);
            this.syncMicDownButton.MouseDown += (sender, e) => { OverlayForm.MainForm.ShowMicDownLabel(); micControlTimer.Tag = "a2 b6 a4"; micControlTimer.Start(); };
            this.syncMicDownButton.MouseUp += (sender, e) => { OverlayForm.MainForm.HideMicDownLabel(); micControlTimer.Stop(); };

            // 
            // closeButton
            // 
            ConfigureButton(this.syncCloseButton, 1218, 826, 205, 56, resizedNormalStateImageForSyncScreen, resizedNormalStateImageForSyncScreen, resizedNormalStateImageForSyncScreen, SyncCloseButton_Click);

            // 
            // PrimaryForm
            // 
            this.ClientSize = new System.Drawing.Size(1440, 900); // Set form size to fit panel and button
            this.Controls.Add(this.primaryScreenPanel);
            this.Controls.Add(this.syncCloseButton);
            this.Name = "PrimaryForm";
            this.ResumeLayout(false);
        }

        private void SyncPauseButton_Click(object sender, EventArgs e)
        {
            videoPlayerForm.Pause();

            // 切换按钮的可见性
            syncPauseButton.Visible = false;
            syncPlayButton.Visible = true;
            syncPlayButton.BringToFront();
            pauseButton.Visible = false;
            playButton.Visible = true;
        }
        
        private void SyncPlayButton_Click(object sender, EventArgs e)
        {
            videoPlayerForm.Play();

            // 切换按钮的可见性
            syncPauseButton.Visible = true;
            syncPauseButton.BringToFront();
            syncPlayButton.Visible = false;
            pauseButton.Visible = true;
            playButton.Visible = false;
        }
        
        private void SyncCloseButton_Click(object sender, EventArgs e)
        {
            VideoPlayerForm.Instance.ClosePrimaryScreenPanel();
        }
        
        private void SyncScreenButton_Click(object sender, EventArgs e)
        {
            videoPlayerForm.IsSyncToPrimaryMonitor = true;
            videoPlayerForm.SyncToPrimaryMonitor();  // 调用同步播放方法
        }
    }
}