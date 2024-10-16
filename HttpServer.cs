using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json; // For JsonConvert and JsonException
using System.Windows.Forms; // For MessageBox
using System.Drawing; // For Image
using System.Drawing.Imaging; // For ImageFormat
using ZXing; // For BarcodeWriter and BarcodeFormat
using ZXing.QrCode; // For QrCodeEncodingOptions
using ActionString = System.Action<string>; // 现在可以在代码中使用 ActionString

namespace DualScreenDemo
{
    public class HttpServer
    {
        private static string _localIP = GetLocalIPAddress();
        private static int _port = 8080; // 或其他方式設置
        // 服务器类变量
        private static SongListManager songListManager;
        // 使用完整命名空间来避免歧义
        public static event ActionString OnDisplayBarrage;
        private static DateTime lastClickTime = DateTime.MinValue;
        public static string randomFolderPath; // 声明全局变量

        public static async Task StartServer(string baseDirectory, int port, SongListManager manager)
        {
            songListManager = manager; // 保存传递的SongListManager实例
            string randomFolderName = CreateRandomFolderAndRedirectHTML(baseDirectory);
            randomFolderPath = randomFolderName; // 初始化全局变量

            // Read the server address from the file
            string serverAddressFilePath = Path.Combine(Application.StartupPath, "txt", "ip.txt");
            if (!File.Exists(serverAddressFilePath))
            {
                Console.WriteLine("Server address file not found: " + serverAddressFilePath);
                return;
            }

            string serverAddress = File.ReadAllText(serverAddressFilePath).Trim();

            string qrContent = String.Format("http://{0}/{1}/index.html", serverAddress, randomFolderName);
            // string qrContent = String.Format("http://{0}:{1}/{2}/index.html", _localIP, _port, randomFolderName);
            string qrImagePath = GenerateQRCode(qrContent, Path.Combine(baseDirectory, randomFolderName, "qrcode.png"));

            // 启动服务器的逻辑
            HttpListener listener = new HttpListener();

            // 确保 URL 格式正确且以斜杠结尾
            // string prefix = String.Format("http://{0}:{1}/{2}/", _localIP, _port, randomFolderName);
            string prefix = String.Format("http://{0}:{1}/", _localIP, _port);
            Console.WriteLine("Adding prefix: " + prefix); // 这行代码帮助确认添加的前缀是什么
            listener.Prefixes.Add(prefix);
            
            try
            {
                listener.Start();
                Console.WriteLine("Server started.");

                // 在程序关闭时删除随机文件夹
                AppDomain.CurrentDomain.ProcessExit += (s, e) => DeleteRandomFolder(baseDirectory);
            }
            catch (HttpListenerException ex)
            {
                Console.WriteLine("Error starting server: " + ex.Message);
                return;
            }

            while (true)
            {
                HttpListenerContext context = await listener.GetContextAsync();
                await ProcessRequestAsync(context, baseDirectory, randomFolderName);
            }
        }

        public static void DeleteRandomFolder(string baseDirectory)
        {
            string fullPath = Path.Combine(baseDirectory, randomFolderPath);
            if (Directory.Exists(fullPath))
            {
                try
                {
                    Directory.Delete(fullPath, true);
                    Console.WriteLine("Deleted random folder: " + fullPath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error deleting random folder: " + ex.Message);
                }
            }
        }

        public static string GetServerAddress()
        {
            return String.Format("http://{0}:{1}/", _localIP, _port);
            // return String.Format("http://111.246.145.170:8080/");
        }

        private static string CreateRandomFolderAndRedirectHTML(string baseDirectory)
        {
            string randomFolderName = Path.GetRandomFileName().Replace(".", "");
            string randomFolderPath = Path.Combine(baseDirectory, randomFolderName);
            Directory.CreateDirectory(randomFolderPath);
            Console.WriteLine(String.Format("Created random folder: {0}", randomFolderPath));

            string sourceHTMLPath = Path.Combine(baseDirectory, "windows.html");
            string targetHTMLPath = Path.Combine(baseDirectory, randomFolderPath, "index.html");
            File.Copy(sourceHTMLPath, targetHTMLPath, true);
            Console.WriteLine(String.Format("Copied windows.html to {0}", targetHTMLPath));

            // 在生成的 HTML 中注入随机路径
            string htmlContent = File.ReadAllText(sourceHTMLPath);
            htmlContent = htmlContent.Replace("var randomFolder = '';", String.Format("var randomFolder = '{0}';", randomFolderName));
            File.WriteAllText(targetHTMLPath, htmlContent);

            return randomFolderName;
        }

        // 生成 QR 码并返回图像路径
        public static string GenerateQRCode(string content, string savePath)
        {
            var writer = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new QrCodeEncodingOptions
                {
                    Height = 165,
                    Width = 165,
                    Margin = 0
                }
            };

            using (var bitmap = writer.Write(content))
            {
                bitmap.Save(savePath, ImageFormat.Png);
                return savePath;
            }
        }

        private static async Task ProcessRequestAsync(HttpListenerContext context, string baseDirectory, string randomFolderName)
        {
            Console.WriteLine("Processing request...");
            // 处理请求的逻辑
            if (context.Request.HttpMethod == "POST")
            {
                Console.WriteLine("Handling POST request...");
                // 检查是否是搜索请求或信号请求
                string relativePath = context.Request.Url.AbsolutePath.Replace(String.Format("/{0}", randomFolderName), "");
                Console.WriteLine("Received request for path: " + relativePath);
                switch (relativePath)
                {
                    case "/search":
                        await HandleSearchRequest(context);
                        break;
                    case "/signal":
                        await HandleSignalRequest(context);
                        break;
                    case "/sound-control":
                        await HandleSoundControlRequest(context);
                        break;
                    case "/send-sticker":
                        await HandleStickerRequest(context);
                        break;
                    case "/order-song":
                        await HandleOrderSongRequest(context);
                        break;
                    case "/insert-song": // 添加对 /insert-song 的处理
                        await HandleInsertSongRequest(context);
                        break;
                    case "/ordered-song":
                        await HandleOrderSongListRequest(context);
                        break;
                    default:
                        // Handle other POST requests
                        break;
                }
            }
            else if (context.Request.HttpMethod == "GET")
            {
                // Handle GET request
                string requestedFile = context.Request.Url.LocalPath.Replace(String.Format("/{0}/", randomFolderName), ""); // 使用 string.Format 替换随机文件夹部分
                            
                // 打印出 LocalPath 和 requestedFile 以调试
                Console.WriteLine("LocalPath: " + context.Request.Url.LocalPath);
                Console.WriteLine("Requested File after replacement: " + requestedFile);
                
                if (string.IsNullOrEmpty(requestedFile.Trim('/')) || !File.Exists(Path.Combine(baseDirectory, requestedFile.TrimStart('/'))))
                {
                    requestedFile = Path.Combine(randomFolderPath, "index.html");
                }
                string filePath = Path.Combine(baseDirectory, requestedFile.TrimStart('/'));

                if (File.Exists(filePath))
                {
                    try
                    {
                        byte[] buffer = File.ReadAllBytes(filePath);
                        // Determine the MIME type
                        string contentType = GetMimeType(filePath);
                        context.Response.ContentType = contentType; // 设置内容类型
                        context.Response.ContentLength64 = buffer.Length;
                        await context.Response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(String.Format("Error serving {0}: {1}", requestedFile, ex.Message));
                        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    }
                }
                else
                {
                    Console.WriteLine(String.Format("File not found: {0}", requestedFile));
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                }
            }
            // You might want to handle other HTTP methods here as well

            context.Response.Close();
        }

        private static async Task HandleSearchRequest(HttpListenerContext context)
        {
            // 读取POST请求体中的内容
            string requestBody;
            using (var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding))
            {
                requestBody = await reader.ReadToEndAsync();
            }

            Console.WriteLine("Request body read: " + requestBody);

            // 解析请求体中的搜索关键词
            var searchRequest = JsonConvert.DeserializeObject<SearchRequest>(requestBody);
            var searchKeyword = searchRequest.Query;
            var queryType = searchRequest.Type;

            // 打印搜索关键词和查询类型
            Console.WriteLine("Received search keyword: " + searchKeyword);
            Console.WriteLine("Query type: " + queryType);

            // 使用搜索关键词进行歌曲搜索
            List<SongData> searchResults = new List<SongData>();
            if (queryType == "new-songs")
            {
                searchResults = SongListManager.NewSongLists["國語"];
            }
            else if (queryType == "top-ranking")
            {
                searchResults = SongListManager.HotSongLists["國語"];
            }
            else if (queryType == "singer")
            {
                searchResults = songListManager.SearchSongsBySinger(searchKeyword);
            }
            else if (queryType == "song")
            {
                searchResults = songListManager.SearchSongsByName(searchKeyword);
            }

            // 将搜索结果转换为JSON格式
            string jsonResponse = JsonConvert.SerializeObject(searchResults);

            // 设置响应头信息
            context.Response.ContentType = "application/json";
            context.Response.ContentLength64 = Encoding.UTF8.GetByteCount(jsonResponse);
            context.Response.StatusCode = (int)HttpStatusCode.OK;

            // 发送响应体
            using (var streamWriter = new StreamWriter(context.Response.OutputStream))
            {
                await streamWriter.WriteAsync(jsonResponse);
                await streamWriter.FlushAsync();
            }
        }

        private static async Task HandleSignalRequest(HttpListenerContext context)
        {
            Console.WriteLine("Handling signal request...");
            string requestBody;
            using (var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding))
            {
                requestBody = await reader.ReadToEndAsync();
            }

            Console.WriteLine("Received POST body: " + requestBody);

            // 处理信号请求的逻辑
            var responseMessage = new { status = "Signal received" };
            string jsonResponse = JsonConvert.SerializeObject(responseMessage);

            context.Response.ContentType = "application/json";
            context.Response.ContentLength64 = Encoding.UTF8.GetByteCount(jsonResponse);
            context.Response.StatusCode = (int)HttpStatusCode.OK;

            using (var streamWriter = new StreamWriter(context.Response.OutputStream))
            {
                await streamWriter.WriteAsync(jsonResponse);
                await streamWriter.FlushAsync();
            }

            Console.WriteLine("Signal response sent.");
        }

        private static async Task HandleSoundControlRequest(HttpListenerContext context)
        {
            string requestBody;
            using (var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding))
            {
                requestBody = await reader.ReadToEndAsync();
            }

            Console.WriteLine("Received sound control command: " + requestBody);

            try
            {
                var data = JsonConvert.DeserializeObject<SoundControlRequest>(requestBody);

                switch (data.Command)
                {
                    case "pause":
                    // 执行暂停操作
                        if (VideoPlayerForm.Instance.isPaused)
                        {
                            PrimaryForm.Instance.videoPlayerForm.Play();
                            PrimaryForm.Instance.pauseButton.Visible = true;
                            PrimaryForm.Instance.playButton.Visible = false;
                            PrimaryForm.Instance.syncPauseButton.Visible = true;
                            PrimaryForm.Instance.syncPlayButton.Visible = false;
                        }
                        else
                        {
                            PrimaryForm.Instance.videoPlayerForm.Pause();
                            PrimaryForm.Instance.pauseButton.Visible = false;
                            PrimaryForm.Instance.playButton.Visible = true;
                            PrimaryForm.Instance.syncPauseButton.Visible = false;
                            PrimaryForm.Instance.syncPlayButton.Visible = true;
                        }
                        break;
                    case "volume_up":
                        // 执行音量增大操作
                        PrimaryForm.SendCommandThroughSerialPort("a2 b3 a4");
                        OverlayForm.MainForm.Invoke(new System.Action(() => {
                            OverlayForm.MainForm.ShowVolumeUpLabel();
                        }));
                        break;
                    case "mic_up":
                        // 执行麦克风增大操作
                        PrimaryForm.SendCommandThroughSerialPort("a2 b5 a4");
                        OverlayForm.MainForm.Invoke(new System.Action(() => {
                            OverlayForm.MainForm.ShowMicUpLabel();
                        }));
                        break;
                    case "mute":
                        // 执行静音操作
                        PrimaryForm.Instance.Invoke(new System.Action(() =>
                        {
                            if (PrimaryForm.Instance.videoPlayerForm.isMuted)
                            {
                                // 取消静音，恢复之前的音量
                                PrimaryForm.Instance.videoPlayerForm.SetVolume(PrimaryForm.Instance.videoPlayerForm.previousVolume);
                                // muteButton.Text = "Mute";
                                PrimaryForm.Instance.videoPlayerForm.isMuted = false;
                                OverlayForm.MainForm.HideMuteLabel();
                            }
                            else
                            {
                                // 静音，将音量设置为-10000
                                PrimaryForm.Instance.videoPlayerForm.previousVolume = PrimaryForm.Instance.videoPlayerForm.GetVolume();
                                PrimaryForm.Instance.videoPlayerForm.SetVolume(-10000);
                                // muteButton.Text = "Unmute";
                                PrimaryForm.Instance.videoPlayerForm.isMuted = true;
                                OverlayForm.MainForm.ShowMuteLabel();
                            }
                        }));
                        break;
                    case "volume_down":
                        // 执行音量减小操作
                        PrimaryForm.SendCommandThroughSerialPort("a2 b4 a4");
                        OverlayForm.MainForm.Invoke(new System.Action(() => {
                            OverlayForm.MainForm.ShowVolumeDownLabel();
                        }));
                        break;
                    case "mic_down":
                        // 执行麦克风减小操作
                        PrimaryForm.SendCommandThroughSerialPort("a2 b6 a4");
                        OverlayForm.MainForm.Invoke(new System.Action(() => {
                            OverlayForm.MainForm.ShowMicDownLabel();
                        }));
                        break;
                    case "original_song":
                        // 执行原唱操作
                        break;
                    case "service":
                        // 执行服务操作
                        PrimaryForm.SendCommandThroughSerialPort("a2 53 a4");
                        break;
                    case "replay":
                        // 执行重唱操作
                        PrimaryForm.Instance.Invoke(new System.Action(() =>
                        {
                            // 在这里执行按钮点击后的操作
                            // 比如切歌操作
                            PrimaryForm.Instance.videoPlayerForm.ReplayCurrentSong();
                        }));     
                        break;
                    case "male_key":
                        // 执行男调操作
                        OverlayForm.MainForm.Invoke(new System.Action(() => {
                            OverlayForm.MainForm.ShowMaleKeyLabel();
                        }));
                        break;
                    case "female_key":
                        // 执行女调操作
                        OverlayForm.MainForm.Invoke(new System.Action(() => {
                            OverlayForm.MainForm.ShowFemaleKeyLabel();
                        }));
                        break;
                    case "cut":
                        // 执行切歌操作
                        if (PrimaryForm.Instance.InvokeRequired)
                        {
                            PrimaryForm.Instance.Invoke(new System.Action(() => 
                            {
                                PrimaryForm.Instance.videoPlayerForm.SkipToNextSong();
                            }));
                        }
                        else
                        {
                            PrimaryForm.Instance.videoPlayerForm.SkipToNextSong();
                        }
                        break;
                    case "lower_key":
                        // 执行降调操作
                        OverlayForm.MainForm.Invoke(new System.Action(() => {
                            OverlayForm.MainForm.ShowKeyDownLabel();
                        }));

                        // MessageBox.Show("降調功能啟動");
                        if (SerialPortManager.mySerialPort != null && SerialPortManager.mySerialPort.IsOpen)
                        {
                            // 假設 0xA2, 0xC2, 0xA4 是降調的指令
                            byte[] commandBytesDecreasePitch = new byte[] { 0xA2, 0xB2, 0xA4 };
                            SerialPortManager.mySerialPort.Write(commandBytesDecreasePitch, 0, commandBytesDecreasePitch.Length);
                            // MessageBox.Show("降調指令已發送。");
                        }
                        else
                        {
                            // MessageBox.Show("串口未開啟，無法發送降調指令。");
                        }
                        break;
                    case "standard_key":
                        // 执行标准调操作
                        OverlayForm.MainForm.Invoke(new System.Action(() => {
                            OverlayForm.MainForm.ShowStandardKeyLabel();
                        }));
                        break;
                    case "raise_key":
                        // 执行升调操作
                        OverlayForm.MainForm.Invoke(new System.Action(() => {
                            OverlayForm.MainForm.ShowKeyUpLabel();
                        }));

                        // MessageBox.Show("升調功能啟動");
                        if (SerialPortManager.mySerialPort != null && SerialPortManager.mySerialPort.IsOpen)
                        {
                            // 假設 0xA2, 0xC1, 0xA4 是升調的指令
                            byte[] commandBytesIncreasePitch = new byte[] { 0xA2, 0xB1, 0xA4 };
                            SerialPortManager.mySerialPort.Write(commandBytesIncreasePitch, 0, commandBytesIncreasePitch.Length);
                            // MessageBox.Show("升調指令已發送。");
                        }
                        else
                        {
                            MessageBox.Show("串口未開啟，無法發送升調指令。");
                        }
                        break;
                    default:
                        Console.WriteLine("Unknown command: " + data.Command);
                        break;
                }

                var response = new { status = "success" };
                string jsonResponse = JsonConvert.SerializeObject(response);
                context.Response.ContentType = "application/json";
                context.Response.ContentLength64 = Encoding.UTF8.GetByteCount(jsonResponse);
                context.Response.StatusCode = (int)HttpStatusCode.OK;

                using (var streamWriter = new StreamWriter(context.Response.OutputStream))
                {
                    await streamWriter.WriteAsync(jsonResponse);
                    await streamWriter.FlushAsync();
                }
            }
            catch (JsonException ex)
            {
                Console.WriteLine("JSON parsing error: " + ex.Message);
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                await context.Response.OutputStream.WriteAsync(new byte[0], 0, 0);
            }
        }
        private static async Task HandleOrderSongListRequest(HttpListenerContext context)
        {
            Console.WriteLine("Entered HandleOrderSongListRequest");
            // try
            // {
            //     string requestBody;
            //     using (var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding))
            //     {
            //         requestBody = await reader.ReadToEndAsync();
            //     }

            //     Console.WriteLine("Received order song request: " + requestBody);
                
            //     // 打印当前播放列表
            //     PrimaryForm.PrintPlayingSongList();

            //     var response = new
            //     {
            //         playingSongList = PrimaryForm.Instance.playingSongList?.Select(CreateSongResponse).ToList() ?? new List<object>(),
            //         playedSongsHistory = PrimaryForm.playedSongsHistory?.Select(CreateSongResponse).ToList() ?? new List<object>(),
            //         playStates = PrimaryForm.playStates,
            //         currentSongIndexInHistory = PrimaryForm.currentSongIndexInHistory
            //     };

            //     string jsonResponse = JsonConvert.SerializeObject(response);
            //     Console.WriteLine("Serialized JSON Response: " + jsonResponse);

            //     context.Response.ContentType = "application/json";
            //     context.Response.StatusCode = (int)HttpStatusCode.OK;
            //     await SendResponseAsync(context, jsonResponse);
            // }
            // catch (Exception ex)
            // {
            //     Console.WriteLine("Error handling order song request: " + ex.Message);
            //     context.Response.ContentType = "application/json"; 
            //     context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                
            //     var errorResponse = new { status = "error", message = $"An error occurred: {ex.Message}, StackTrace: {ex.StackTrace}" };
            //     await SendResponseAsync(context, JsonConvert.SerializeObject(errorResponse));
            // }
        }

        private static object CreateSongResponse(SongData song)
        {
            return new
            {
                song.Song,
                song.ArtistA,
                song.SongFilePathHost1,
                song.SongFilePathHost2
            };
        }
        private static async Task HandleOrderSongRequest(HttpListenerContext context)
        {
            try
            {
                string requestBody;
                using (var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding))
                {
                    requestBody = await reader.ReadToEndAsync();
                }

                Console.WriteLine("Received order song request: " + requestBody);

                // 解析 JSON 为 Song 对象
                var song = JsonConvert.DeserializeObject<SongData>(requestBody);
                
                if (song != null)
                {
                    Console.WriteLine($"Ordering Song: {song.Song} by {song.ArtistA}");
                    // 这里可以添加处理逻辑，例如将歌曲加入到播放列表或数据库中
                    OverlayForm.MainForm.AddSongToPlaylist(song);

                    var response = new { status = "success", message = "Song ordered successfully" };
                    string jsonResponse = JsonConvert.SerializeObject(response);
                    await SendResponseAsync(context, jsonResponse);
                }
                else
                {
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    await SendResponseAsync(context, "{\"status\": \"error\", \"message\": \"Invalid song data\"}");
                }
            }
            catch (JsonException ex)
            {
                Console.WriteLine("JSON parsing error: " + ex.Message);
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                await SendResponseAsync(context, "{\"status\": \"error\", \"message\": \"Invalid JSON format\"}");
            }
        }

        private static async Task HandleInsertSongRequest(HttpListenerContext context)
        {
            try
            {
                string requestBody;
                using (var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding))
                {
                    requestBody = await reader.ReadToEndAsync();
                }

                Console.WriteLine("Received insert song request: " + requestBody);

                // 解析 JSON 为 Song 对象
                var song = JsonConvert.DeserializeObject<SongData>(requestBody);

                if (song != null)
                {
                    Console.WriteLine($"Inserting Song: {song.Song} by {song.ArtistA}");
                    // 这里可以添加插播歌曲的处理逻辑
                    OverlayForm.MainForm.InsertSongToPlaylist(song);

                    var response = new { status = "success", message = "Song inserted successfully" };
                    string jsonResponse = JsonConvert.SerializeObject(response);
                    await SendResponseAsync(context, jsonResponse);
                }
                else
                {
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    await SendResponseAsync(context, "{\"status\": \"error\", \"message\": \"Invalid song data\"}");
                }
            }
            catch (JsonException ex)
            {
                Console.WriteLine("JSON parsing error: " + ex.Message);
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                await SendResponseAsync(context, "{\"status\": \"error\", \"message\": \"Invalid JSON format\"}");
            }
        }

        public class SoundControlRequest
        {
            public string Command { get; set; }
        }

        private static async Task HandleStickerRequest(HttpListenerContext context)
        {
            string requestBody;
            using (var reader = new StreamReader(context.Request.InputStream, context.Request.ContentEncoding))
            {
                requestBody = await reader.ReadToEndAsync();
            }

            Console.WriteLine("Received sticker ID: " + requestBody);

            try
            {
                var data = JsonConvert.DeserializeObject<StickerRequest>(requestBody);
                string stickerId = data.StickerId;

                // 处理 stickerId 的逻辑，例如显示贴图
                if (OverlayForm.MainForm != null)
                {
                    OverlayForm.MainForm.DisplaySticker(stickerId);
                }
                else
                {
                    Console.WriteLine("MainForm is null.");
                }

                var response = new { status = "success" };
                string jsonResponse = JsonConvert.SerializeObject(response);
                context.Response.ContentType = "application/json";
                context.Response.ContentLength64 = Encoding.UTF8.GetByteCount(jsonResponse);
                context.Response.StatusCode = (int)HttpStatusCode.OK;

                using (var streamWriter = new StreamWriter(context.Response.OutputStream))
                {
                    await streamWriter.WriteAsync(jsonResponse);
                    await streamWriter.FlushAsync();
                }
            }
            catch (JsonException ex)
            {
                Console.WriteLine("JSON parsing error: " + ex.Message);
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                await context.Response.OutputStream.WriteAsync(new byte[0], 0, 0);
            }
        }

        public class StickerRequest
        {
            public string StickerId { get; set; }
        }

        // 封装响应代码以避免重复
        async static Task SendResponseAsync(HttpListenerContext context, string jsonResponse) {
            context.Response.ContentType = "application/json";
            context.Response.ContentLength64 = Encoding.UTF8.GetByteCount(jsonResponse);
            context.Response.StatusCode = (int)HttpStatusCode.OK;
            using (var streamWriter = new StreamWriter(context.Response.OutputStream)) {
                await streamWriter.WriteAsync(jsonResponse);
                await streamWriter.FlushAsync();
            }
        }

        private static string GetMimeType(string filePath)
        {
            // 获取MIME类型的逻辑
            string mimeType = "application/octet-stream"; // Default MIME type
            string extension = Path.GetExtension(filePath).ToLowerInvariant();

            // MIME type lookup based on file extension
            switch (extension)
            {
                case ".html":
                case ".htm":
                    mimeType = "text/html";
                    break;
                case ".css":
                    mimeType = "text/css";
                    break;
                case ".js":
                    mimeType = "application/javascript";
                    break;
                case ".png":
                    mimeType = "image/png";
                    break;
                case ".jpg":
                case ".jpeg":
                    mimeType = "image/jpeg";
                    break;
                case ".gif":
                    mimeType = "image/gif";
                    break;
                case ".svg":
                    mimeType = "image/svg+xml";
                    break;
                case ".json":
                    mimeType = "application/json";
                    break;
                // Add more cases for other file types as needed
            }

            return mimeType;
        }

        private static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }
    }
}