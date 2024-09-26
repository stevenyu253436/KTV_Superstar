using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Diagnostics;

namespace DualScreenDemo
{
    public class CommandHandler
    {
        public static bool readyForSongListInput = false;

        private readonly SongListManager songListManager;

        public CommandHandler(SongListManager songListManager)
        {
            this.songListManager = songListManager;
        }

        public async Task ProcessData(string indata)
        {
            string filePath = Path.Combine(Application.StartupPath, "dataLog.txt");
            if (CheckLogForShutdown(filePath))
            {
                Console.WriteLine("Shutdown condition met. Application will now close.");
                ShutdownComputer();
            }

            switch (indata)
            {
                case "A261A4":
                    HandleInputA();
                    break;
                case "A262A4":
                    HandleInputB();
                    break;
                case "A263A4":
                    ClearDisplay();
                    break;
                case "A268A4":
                    OverlayForm.MainForm.currentPage = 1;
                    DisplaySongHistory();
                    break;
                case "A26AA4":
                    PreviousPage();
                    break;
                case "A26BA4":
                    NextPage();
                    break;
                case "A271A4":
                    HandleNewSongAnnouncements();
                    break;
                case "A273A4":
                    HandleHotSongAnnouncements();
                    break;
                case "A267A4":
                    SkipToNextSong();
                    break;
                case "i":
                    ReplayCurrentSong();
                    break;
                case "A26DA4":
                    PauseOrResumeSong();
                    break;
                case "A276A4":
                    ToggleMute();
                    break;
                case "A274A4":
                    HandleArtistAnnouncements();
                    break;
                case "A2B3A4":
                    InvokeAction(() => OverlayForm.MainForm.ShowVolumeUpLabel());
                    break;
                case "A2B4A4":
                    InvokeAction(() => OverlayForm.MainForm.ShowVolumeDownLabel());
                    break;
                case "A2B5A4":
                    InvokeAction(() => OverlayForm.MainForm.ShowMicUpLabel());
                    break;
                case "A2B6A4":
                    InvokeAction(() => OverlayForm.MainForm.ShowMicDownLabel());
                    break;
                case "A2C2A4":
                    InvokeAction(() => OverlayForm.MainForm.ShowStandardLabel());
                    break;
                case "A2C3A4":
                    InvokeAction(() => OverlayForm.MainForm.ShowProfessionalLabel());
                    break;
                case "A2C4A4":
                    InvokeAction(() => OverlayForm.MainForm.ShowSquareLabel());
                    break;
                case "A2C1A4":
                    InvokeAction(() => OverlayForm.MainForm.ShowSingDownLabel());
                    break;
                case "A2D5A4":
                    InvokeAction(() => OverlayForm.MainForm.ShowBrightLabel());
                    break;
                case "A2D7A4":
                    InvokeAction(() => OverlayForm.MainForm.ShowRomanticLabel());
                    break;
                case "A2D6A4":
                    InvokeAction(() => OverlayForm.MainForm.ShowSoftLabel());
                    break;
                case "A2D8A4":
                    InvokeAction(() => OverlayForm.MainForm.ShowDynamicLabel());
                    break;
                case "A275A4":
                    InvokeAction(() => OverlayForm.MainForm.ShowTintLabel());
                    break;
                default:
                    if (Regex.IsMatch(indata, @"^A23\d+A4$")) // 确保trimmedData符合格式A23xA4
                    {
                        HandleNumberInput(indata);
                    }
                    break;
            }
        }

        void InvokeAction(Action action)
        {
            if (OverlayForm.MainForm.InvokeRequired)
            {
                OverlayForm.MainForm.Invoke(action);
            }
            else
            {
                action();
            }
        }

        // 其他方法按照原文件中定义
        
        private static void SkipToNextSong()
        {
            if (PrimaryForm.Instance.InvokeRequired)
            {
                PrimaryForm.Instance.Invoke(new System.Action(() => PrimaryForm.Instance.videoPlayerForm.SkipToNextSong()));
            }
            else
            {
                PrimaryForm.Instance.videoPlayerForm.SkipToNextSong();
            }
        }

        private static void ReplayCurrentSong()
        {
            if (PrimaryForm.Instance.InvokeRequired)
            {
                PrimaryForm.Instance.Invoke(new System.Action(() => PrimaryForm.Instance.videoPlayerForm.ReplayCurrentSong()));
            }
            else
            {
                PrimaryForm.Instance.videoPlayerForm.ReplayCurrentSong();
            }
        }

        private static void PauseOrResumeSong()
        {
            if (PrimaryForm.Instance.InvokeRequired)
            {
                PrimaryForm.Instance.Invoke(new System.Action(() => PrimaryForm.Instance.videoPlayerForm.PauseOrResumeSong()));
            }
            else
            {
                PrimaryForm.Instance.videoPlayerForm.PauseOrResumeSong();
            }
        }

        public static void ToggleMute()
        {
            if (VideoPlayerForm.Instance.InvokeRequired)
            {
                VideoPlayerForm.Instance.Invoke(new System.Action(ToggleMute));
            }
            else
            {
                if (VideoPlayerForm.Instance.isMuted)
                {
                    // 取消静音，恢复之前的音量
                    VideoPlayerForm.Instance.SetVolume(VideoPlayerForm.Instance.previousVolume);
                    // muteButton.Text = "Mute"; // Assuming you have a mute button to update
                    VideoPlayerForm.Instance.isMuted = false;
                    OverlayForm.MainForm.Invoke(new System.Action(() => OverlayForm.MainForm.HideMuteLabel())); // Assuming this method exists to hide a mute label
                }
                else
                {
                    // 静音，将音量设置为-10000
                    VideoPlayerForm.Instance.previousVolume = VideoPlayerForm.Instance.GetVolume();
                    VideoPlayerForm.Instance.SetVolume(-10000);
                    // muteButton.Text = "Unmute"; // Assuming you have a mute button to update
                    VideoPlayerForm.Instance.isMuted = true;
                    OverlayForm.MainForm.Invoke(new System.Action(() => OverlayForm.MainForm.ShowMuteLabel())); // Assuming this method exists to show a mute label
                }
            }
        }

        // 示例: 处理输入 'a'
        private void HandleInputA()
        {
            // 具体逻辑处理
            // 停止计时器以避免冲突
            OverlayForm.displayTimer.Stop();
            string input = "a"; // 设置 'a' 为输入

            // 读取歌曲编号
            string songNumber = OverlayForm.ReadSongNumber(); // 读取歌曲编号
            var song = songListManager.SearchSongByNumber(songNumber); // 搜索歌曲
            
            // 確保此處調用是安全的
            // Check if the system is ready to process this number as a song list input
            if (readyForSongListInput)
            {
                // Process number as song list selection
                if (OverlayForm.MainForm.InvokeRequired)
                {
                    OverlayForm.MainForm.Invoke(new System.Action(() =>
                    {
                        OverlayForm.MainForm.OnUserInput(input);  // Handle numbered inputs for song lists
                    }));
                }
                else
                {
                    OverlayForm.MainForm.OnUserInput(input);
                }
            }
            else
            {
                if (OverlayForm.MainForm.InvokeRequired)
                {
                    OverlayForm.MainForm.Invoke(new System.Action(() =>
                    {
                        if (song != null)
                        {
                            ClearDisplay();
                            OverlayForm.MainForm.displayLabel.Text = String.Format("已點歌曲\n{0}", song);
                            OverlayForm.MainForm.AddSongToPlaylist(song);
                            OverlayForm.displayTimer.Start();
                        }
                        else
                        {
                            ClearDisplay();
                            OverlayForm.MainForm.displayLabel.Text = "輸入錯誤!!!";
                            OverlayForm.displayTimer.Start();
                        }
                    }));
                }
                else
                {
                    if (song != null)
                    {
                        ClearDisplay();
                        OverlayForm.MainForm.displayLabel.Text = String.Format("已點歌曲\n{0}", song);
                        OverlayForm.MainForm.AddSongToPlaylist(song);
                        OverlayForm.displayTimer.Start();
                    }
                    else
                    {
                        ClearDisplay();
                        OverlayForm.MainForm.displayLabel.Text = "輸入錯誤!!!";
                        OverlayForm.displayTimer.Start();
                    }
                }
            }
        }

        // 示例: 处理输入 'b'
        private void HandleInputB()
        {
            // 停止计时器以避免冲突
            OverlayForm.displayTimer.Stop();
            string input = "b"; // 设置 'a' 为输入

            // 读取歌曲编号
            string songNumber = OverlayForm.ReadSongNumber(); // 读取歌曲编号
            var song = songListManager.SearchSongByNumber(songNumber); // 搜索歌曲
            
            // 確保此處調用是安全的
            // Check if the system is ready to process this number as a song list input
            if (readyForSongListInput)
            {
                // Process number as song list selection
                if (OverlayForm.MainForm.InvokeRequired)
                {
                    OverlayForm.MainForm.Invoke(new System.Action(() =>
                    {
                        OverlayForm.MainForm.OnUserInput(input);  // Handle numbered inputs for song lists
                    }));
                }
                else
                {
                    OverlayForm.MainForm.OnUserInput(input);
                }
            }
            else
            {
                if (OverlayForm.MainForm.InvokeRequired)
                {
                    OverlayForm.MainForm.Invoke(new System.Action(() =>
                    {
                        if (song != null)
                        {
                            ClearDisplay();
                            OverlayForm.MainForm.displayLabel.Text = String.Format("插播歌曲\n{0}", song);
                            OverlayForm.MainForm.InsertSongToPlaylist(song);
                            OverlayForm.displayTimer.Start();
                        }
                        else
                        {
                            ClearDisplay();
                            OverlayForm.MainForm.displayLabel.Text = "輸入錯誤!!!";
                            OverlayForm.displayTimer.Start();
                        }
                    }));
                }
                else
                {
                    if (song != null)
                    {
                        ClearDisplay();
                        OverlayForm.MainForm.displayLabel.Text = String.Format("已點歌曲\n{0}", song);
                    }
                    else
                    {
                        ClearDisplay();
                        OverlayForm.MainForm.displayLabel.Text = "輸入錯誤!!!";
                        OverlayForm.displayTimer.Start();
                    }
                }
            }
        }

        private static void ClearDisplay()
        {
            // 停止计时器以避免冲突
            OverlayForm.displayTimer.Stop();
            
            // 確保此處調用是安全的
            if (OverlayForm.MainForm.InvokeRequired)
            {
                OverlayForm.MainForm.Invoke(new System.Action(() =>
                {
                    // Clear existing controls
                    foreach (var control in OverlayForm.MainForm.Controls.OfType<Control>().ToArray())
                    {
                        if (control != OverlayForm.MainForm.displayLabel &&
                            control != OverlayForm.MainForm.pauseLabel &&
                            control != OverlayForm.MainForm.muteLabel &&
                            control != OverlayForm.MainForm.volumeUpLabel &&
                            control != OverlayForm.MainForm.volumeDownLabel &&
                            control != OverlayForm.MainForm.micUpLabel &&
                            control != OverlayForm.MainForm.micDownLabel &&
                            control != OverlayForm.MainForm.standardKeyLabel &&
                            control != OverlayForm.MainForm.keyUpLabel &&
                            control != OverlayForm.MainForm.keyDownLabel &&
                            control != OverlayForm.MainForm.maleKeyLabel &&
                            control != OverlayForm.MainForm.femaleKeyLabel &&
                            control != OverlayForm.MainForm.squareLabel &&
                            control != OverlayForm.MainForm.professionalLabel &&
                            control != OverlayForm.MainForm.standardLabel &&
                            control != OverlayForm.MainForm.singDownLabel &&
                            control != OverlayForm.MainForm.brightLabel &&
                            control != OverlayForm.MainForm.softLabel &&
                            control != OverlayForm.MainForm.autoLabel &&
                            control != OverlayForm.MainForm.romanticLabel &&
                            control != OverlayForm.MainForm.dynamicLabel &&
                            control != OverlayForm.MainForm.tintLabel) // Keep the specified controls
                        {
                            OverlayForm.MainForm.Controls.Remove(control);
                            control.Dispose();
                        }
                    }

                    OverlayForm.MainForm.displayLabel.Text = ""; // 将文本设置为空字符串
                    readyForSongListInput = false;
                    OverlayForm.SetUIState(OverlayForm.UIState.Initial);
                    Console.WriteLine(OverlayForm.MainForm.displayLabel.Text);
                }));
            }
            else
            {
                // Clear existing controls
                foreach (var control in OverlayForm.MainForm.Controls.OfType<Control>().ToArray())
                {
                    if (control != OverlayForm.MainForm.displayLabel &&
                        control != OverlayForm.MainForm.pauseLabel &&
                        control != OverlayForm.MainForm.muteLabel &&
                        control != OverlayForm.MainForm.volumeUpLabel &&
                        control != OverlayForm.MainForm.volumeDownLabel &&
                        control != OverlayForm.MainForm.micUpLabel &&
                        control != OverlayForm.MainForm.micDownLabel &&
                        control != OverlayForm.MainForm.standardKeyLabel &&
                        control != OverlayForm.MainForm.keyUpLabel &&
                        control != OverlayForm.MainForm.keyDownLabel &&
                        control != OverlayForm.MainForm.maleKeyLabel &&
                        control != OverlayForm.MainForm.femaleKeyLabel &&
                        control != OverlayForm.MainForm.squareLabel &&
                        control != OverlayForm.MainForm.professionalLabel &&
                        control != OverlayForm.MainForm.standardLabel &&
                        control != OverlayForm.MainForm.singDownLabel &&
                        control != OverlayForm.MainForm.brightLabel &&
                        control != OverlayForm.MainForm.softLabel &&
                        control != OverlayForm.MainForm.autoLabel &&
                        control != OverlayForm.MainForm.romanticLabel &&
                        control != OverlayForm.MainForm.dynamicLabel &&
                        control != OverlayForm.MainForm.tintLabel) // Keep the specified controls
                    {
                        OverlayForm.MainForm.Controls.Remove(control);
                        control.Dispose();
                    }
                }

                OverlayForm.MainForm.displayLabel.Text = ""; // 将文本设置为空字符串
                readyForSongListInput = false;
                OverlayForm.SetUIState(OverlayForm.UIState.Initial);
                Console.WriteLine(OverlayForm.MainForm.displayLabel.Text);
            }
        }

        private static void DisplaySongHistory()
        {
            ClearDisplay();

            // 更新总歌曲数
            OverlayForm.MainForm.totalSongs = PrimaryForm.playedSongsHistory.Count;

            // 构造要显示的信息
            StringBuilder displayText = new StringBuilder();

            // 添加页数信息
            int totalPages = (int)Math.Ceiling(OverlayForm.MainForm.totalSongs / (double)OverlayForm.MainForm.songsPerPage);
            displayText.AppendLine($"已點歌曲: ({OverlayForm.MainForm.currentPage} / {totalPages})");

            // 计算当前页的歌曲索引范围
            int startIndex = (OverlayForm.MainForm.currentPage - 1) * OverlayForm.MainForm.songsPerPage;
            int endIndex = Math.Min(startIndex + OverlayForm.MainForm.songsPerPage, OverlayForm.MainForm.totalSongs);

            // 遍历当前页的已点歌曲
            for (int i = startIndex; i < endIndex; i++)
            {
                var song = PrimaryForm.playedSongsHistory[i];
                PlayState state = PrimaryForm.playStates[i];

                string status = "";
                switch (state)
                {
                    case PlayState.Played:
                        status = " (播畢)";
                        break;
                    case PlayState.Playing:
                        status = " (播放中)";
                        break;
                    case PlayState.NotPlayed:
                        status = "";
                        break;
                    default:
                        status = "";
                        break;
                }

                // 添加歌曲信息和状态到显示文本
                if (!string.IsNullOrWhiteSpace(song.ArtistB))
                {
                    displayText.AppendLine($"{song.Song} - {song.ArtistA} - {song.ArtistB}{status}");
                }
                else
                {
                    displayText.AppendLine($"{song.Song} - {song.ArtistA}{status}");
                }
            }

            // 检查是否需要在UI线程上更新UI
            if (OverlayForm.MainForm.InvokeRequired)
            {
                OverlayForm.MainForm.Invoke(new System.Action(() =>
                {
                    OverlayForm.MainForm.displayLabel.Text = displayText.ToString();
                }));
            }
            else
            {
                OverlayForm.MainForm.displayLabel.Text = displayText.ToString();
            }

            // 设置当前状态为播放历史
            OverlayForm.CurrentUIState = OverlayForm.UIState.PlayHistory;
        }

        private static void PreviousPage()
        {
            if (OverlayForm.CurrentUIState == OverlayForm.UIState.SelectingSong)
            {
                // Check if UI update needs to be invoked on the UI thread
                if (OverlayForm.MainForm.InvokeRequired)
                {
                    OverlayForm.MainForm.Invoke(new System.Action(() =>
                    {
                        // Call the PreviousPage method of the OverlayForm
                        OverlayForm.MainForm.PreviousPage();
                    }));
                }
                else
                {
                    // Directly call the PreviousPage if already on the UI thread
                    OverlayForm.MainForm.PreviousPage();
                }
            }
            else if (OverlayForm.CurrentUIState == OverlayForm.UIState.SelectingArtist)
            {
                // Check if UI update needs to be invoked on the UI thread
                if (OverlayForm.MainForm.InvokeRequired)
                {
                    OverlayForm.MainForm.Invoke(new System.Action(() =>
                    {
                        // Call the NextPage method of the OverlayForm
                        OverlayForm.MainForm.PreviousPage();
                    }));
                }
                else
                {
                    // Directly call the NextPage if already on the UI thread
                    OverlayForm.MainForm.PreviousPage();
                }
            }
            else if (OverlayForm.CurrentUIState == OverlayForm.UIState.PlayHistory)
            {
                if (OverlayForm.MainForm.currentPage > 1)
                {    
                    OverlayForm.MainForm.currentPage--;

                    if (OverlayForm.MainForm.InvokeRequired)
                    {
                        OverlayForm.MainForm.Invoke(new System.Action(() =>
                        {
                            DisplaySongHistory();
                        }));
                    }
                    else
                    {
                        DisplaySongHistory();
                    }
                }
            }
            else
            {
                Console.WriteLine("Page turning is not allowed in the current state.");
            }
        }

        private static void NextPage()
        {
            if (OverlayForm.CurrentUIState == OverlayForm.UIState.SelectingSong)
            {
                // Check if UI update needs to be invoked on the UI thread
                if (OverlayForm.MainForm.InvokeRequired)
                {
                    OverlayForm.MainForm.Invoke(new System.Action(() =>
                    {
                        // Call the NextPage method of the OverlayForm
                        OverlayForm.MainForm.NextPage();
                    }));
                }
                else
                {
                    // Directly call the NextPage if already on the UI thread
                    OverlayForm.MainForm.NextPage();
                }
            }
            else if (OverlayForm.CurrentUIState == OverlayForm.UIState.SelectingArtist)
            {
                // Check if UI update needs to be invoked on the UI thread
                if (OverlayForm.MainForm.InvokeRequired)
                {
                    OverlayForm.MainForm.Invoke(new System.Action(() =>
                    {
                        // Call the NextPage method of the OverlayForm
                        OverlayForm.MainForm.NextPage();
                    }));
                }
                else
                {
                    // Directly call the NextPage if already on the UI thread
                    OverlayForm.MainForm.NextPage();
                }
            }
            else if (OverlayForm.CurrentUIState == OverlayForm.UIState.PlayHistory)
            {
                if (OverlayForm.MainForm.currentPage * OverlayForm.MainForm.songsPerPage < OverlayForm.MainForm.totalSongs)
                {    
                    OverlayForm.MainForm.currentPage++;
                    if (OverlayForm.CurrentUIState == OverlayForm.UIState.PlayHistory)
                    {
                        if (OverlayForm.MainForm.InvokeRequired)
                        {
                            OverlayForm.MainForm.Invoke(new System.Action(() =>
                            {
                                DisplaySongHistory();
                            }));
                        }
                        else
                        {
                            DisplaySongHistory();
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("Page turning is not allowed in the current state.");
            }
        }

        private static void HandleNewSongAnnouncements()
        {
            ClearDisplay();

            // Set the category to New Songs
            OverlayForm.CurrentCategory = OverlayForm.Category.NewSongs;

            // Construct the messages to display
            string[] messages = new string[]
            {
                "新歌快訊",
                "1. 國語  4. 英文",
                "2. 台語  5. 日語",
                "3. 粵語  6. 韓語"
            };

            // Set the flag indicating that the next input should be processed as a song list input
            readyForSongListInput = true;

            // Set UI state to selecting language
            OverlayForm.SetUIState(OverlayForm.UIState.SelectingLanguage);

            // Update the display labels with the messages
            UpdateDisplayLabels(messages);
        }

        private static void HandleHotSongAnnouncements()
        {
            ClearDisplay();

            // Set the category to Hot Songs
            OverlayForm.CurrentCategory = OverlayForm.Category.HotSongs;

            // Construct the messages to display
            string[] messages = new string[]
            {
                "熱門排行",
                "1. 國語人氣排行",
                "2. 台語人氣排行",
                "3. 英語人氣排行",
                "4. 日語人氣排行",
                "5. 韓語人氣排行"
            };

            // Set the flag indicating that the next input should be processed as a song list input
            readyForSongListInput = true;

            // Set UI state to selecting language
            OverlayForm.SetUIState(OverlayForm.UIState.SelectingLanguage);

            // Update the display labels with the messages
            UpdateDisplayLabels(messages);
        }

        private static void HandleArtistAnnouncements()
        {
            ClearDisplay();

            // Set the category to New Songs
            OverlayForm.CurrentCategory = OverlayForm.Category.Artists;

            // Construct the messages to display
            string[] messages = new string[]
            {
                "歌星選歌",
                "1. 男歌星  4. 外語",
                "2. 女歌星  5. 全部",
                "3. 團體"
            };

            // Set the flag indicating that the next input should be processed as a song list input
            readyForSongListInput = true;

            // Set UI state to selecting language
            OverlayForm.SetUIState(OverlayForm.UIState.SelectingArtistCategory);

            // Update the display labels with the messages
            UpdateDisplayLabels(messages);
        }

        private static void UpdateDisplayLabels(string[] messages)
        {
            if (OverlayForm.MainForm.InvokeRequired)
            {
                OverlayForm.MainForm.Invoke(new System.Action(() =>
                {
                    // Clear existing controls
                    foreach (var control in OverlayForm.MainForm.Controls.OfType<Control>().ToArray())
                    {
                        if (control != OverlayForm.MainForm.displayLabel &&
                            control != OverlayForm.MainForm.pauseLabel &&
                            control != OverlayForm.MainForm.muteLabel &&
                            control != OverlayForm.MainForm.volumeUpLabel &&
                            control != OverlayForm.MainForm.volumeDownLabel &&
                            control != OverlayForm.MainForm.micUpLabel &&
                            control != OverlayForm.MainForm.micDownLabel &&
                            control != OverlayForm.MainForm.standardKeyLabel &&
                            control != OverlayForm.MainForm.keyUpLabel &&
                            control != OverlayForm.MainForm.keyDownLabel &&
                            control != OverlayForm.MainForm.maleKeyLabel &&
                            control != OverlayForm.MainForm.femaleKeyLabel &&
                            control != OverlayForm.MainForm.squareLabel &&
                            control != OverlayForm.MainForm.professionalLabel &&
                            control != OverlayForm.MainForm.standardLabel &&
                            control != OverlayForm.MainForm.singDownLabel &&
                            control != OverlayForm.MainForm.brightLabel &&
                            control != OverlayForm.MainForm.softLabel &&
                            control != OverlayForm.MainForm.autoLabel &&
                            control != OverlayForm.MainForm.romanticLabel &&
                            control != OverlayForm.MainForm.dynamicLabel &&
                            control != OverlayForm.MainForm.tintLabel) // Keep the specified controls
                        {
                            OverlayForm.MainForm.Controls.Remove(control);
                            control.Dispose();
                        }
                    }
                    OverlayForm.MainForm.UpdateDisplayLabels(messages);
                }));
            }
            else
            {
                // Clear existing controls
                foreach (var control in OverlayForm.MainForm.Controls.OfType<Control>().ToArray())
                {
                    if (control != OverlayForm.MainForm.displayLabel &&
                        control != OverlayForm.MainForm.pauseLabel &&
                        control != OverlayForm.MainForm.muteLabel &&
                        control != OverlayForm.MainForm.volumeUpLabel &&
                        control != OverlayForm.MainForm.volumeDownLabel &&
                        control != OverlayForm.MainForm.micUpLabel &&
                        control != OverlayForm.MainForm.micDownLabel &&
                        control != OverlayForm.MainForm.standardKeyLabel &&
                        control != OverlayForm.MainForm.keyUpLabel &&
                        control != OverlayForm.MainForm.keyDownLabel &&
                        control != OverlayForm.MainForm.maleKeyLabel &&
                        control != OverlayForm.MainForm.femaleKeyLabel &&
                        control != OverlayForm.MainForm.squareLabel &&
                        control != OverlayForm.MainForm.professionalLabel &&
                        control != OverlayForm.MainForm.standardLabel &&
                        control != OverlayForm.MainForm.singDownLabel &&
                        control != OverlayForm.MainForm.brightLabel &&
                        control != OverlayForm.MainForm.softLabel &&
                        control != OverlayForm.MainForm.autoLabel &&
                        control != OverlayForm.MainForm.romanticLabel &&
                        control != OverlayForm.MainForm.dynamicLabel &&
                        control != OverlayForm.MainForm.tintLabel) // Keep the specified controls
                    {
                        OverlayForm.MainForm.Controls.Remove(control);
                        control.Dispose();
                    }
                }
                OverlayForm.MainForm.UpdateDisplayLabels(messages);
            }
        }

        private static void HandleNumberInput(string trimmedData)
        {
            string number = trimmedData; // 将匹配的数字直接作为字符串使用

            // 提取数字部分
            var match = Regex.Match(trimmedData, @"^A23(\d)A4$");
            if (match.Success)
            {
                number = match.Groups[1].Value;
                // 处理提取出的字符串类型的数字
                Console.WriteLine($"Handling number: {number}");
                // 在这里添加处理字符串类型数字的逻辑
            }

            // Check if the system is ready to process this number as a song list input
            if (readyForSongListInput)
            {
                // Process number as song list selection
                if (OverlayForm.MainForm.InvokeRequired)
                {
                    OverlayForm.MainForm.Invoke(new System.Action(() =>
                    {
                        OverlayForm.MainForm.OnUserInput(number);  // Handle numbered inputs for song lists
                    }));
                }
                else
                {
                    OverlayForm.MainForm.OnUserInput(number);
                }
                // readyForSongListInput = false; // Reset the flag after processing the input
            }
            else
            {
                // If not ready for song list input, do other number-related processing
                if (OverlayForm.MainForm.InvokeRequired)
                {
                    OverlayForm.MainForm.Invoke(new System.Action(() =>
                    {
                        OverlayForm.DisplayNumberAtTopLeft(number);  // E.g., display the number somewhere else or other processing
                        OverlayForm.MainForm.HideAllLabels(); // 调用HideAllLabels
                    }));
                }
                else
                {
                    OverlayForm.DisplayNumberAtTopLeft(number);
                    OverlayForm.MainForm.HideAllLabels(); // 调用HideAllLabels
                }
            }
        }

        public static bool CheckLogForShutdown(string filePath)
        {
            if (File.Exists(filePath))
            {
                // 读取所有内容并去掉换行符以进行检查
                string content = File.ReadAllText(filePath).Replace(Environment.NewLine, "");
                if (content.Length >= 6 && content.Substring(content.Length - 6) == "bbbaaa")
                {
                    return true;
                }
            }
            return false;
        }

        public static void ShutdownComputer()
        {
            try
            {
                ProcessStartInfo processStartInfo = new ProcessStartInfo("cmd.exe", "/c shutdown /s /f /t 0") // "/s" 关机, "/f" 强制关闭应用程序, "/t 0" 延迟时间设为 0 秒
                {
                    CreateNoWindow = true,
                    UseShellExecute = false,
                    RedirectStandardOutput = true
                };
                Process process = Process.Start(processStartInfo);
                process.WaitForExit();
                Console.WriteLine("Computer is shutting down...");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error shutting down computer: " + ex.Message);
            }
        }
    }
}