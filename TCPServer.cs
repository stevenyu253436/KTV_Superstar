using System;
using System.Net;
using System.Net.Sockets; // Add this for TcpListener
using System.Text;
using System.Threading.Tasks;

namespace DualScreenDemo
{
    public class TCPServer
    {
        private TcpListener listener;
        private const int Port = 1000; // 監聽端口
        private readonly string hostNameSuffix;

        public TCPServer()
        {
            listener = new TcpListener(IPAddress.Any, Port);
            hostNameSuffix = GetHostNameSuffix();
        }

        public void Start()
        {
            listener.Start();
            Console.WriteLine("Server started on port " + Port + ".");

            try
            {
                while (true)
                {
                    Console.WriteLine("Waiting for connections...");
                    using (TcpClient client = listener.AcceptTcpClient())
                    {
                        Console.WriteLine("Connected!");
                        NetworkStream stream = client.GetStream();

                        while (client.Connected)
                        {
                            byte[] buffer = new byte[1024];
                            int bytesRead = stream.Read(buffer, 0, buffer.Length);
                            if (bytesRead == 0)
                                break; // 如果没有读到数据，意味着客户端已经断开

                            string request = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                            Console.WriteLine("Received: " + request.Trim());

                            // 确保请求字符串至少有5个字符（3个主机名称末尾字符 + 逗号 + 命令字符）
                            if (request.Length < 5)
                            {
                                continue;
                            }

                            string requestHostSuffix = request.Substring(0, 3);
                            string command = request.Substring(4);

                            if (requestHostSuffix.Equals(hostNameSuffix, StringComparison.OrdinalIgnoreCase))
                            {
                                // 处理特定命令：显示送客画面
                                if (command.Trim().Equals("X", StringComparison.OrdinalIgnoreCase))
                                {
                                    if (PrimaryForm.Instance != null)
                                    {
                                        PrimaryForm.Instance.Invoke(new System.Action(() =>
                                        {
                                            PrimaryForm.Instance.ShowSendOffScreen(); // 在PrimaryForm上显示送客画面
                                        }));
                                    }
                                    continue; // 继续监听其他命令
                                }

                                // 处理特定命令：隐藏送客画面
                                if (command.Trim().Equals("O", StringComparison.OrdinalIgnoreCase))
                                {
                                    if (PrimaryForm.Instance != null)
                                    {
                                        PrimaryForm.Instance.Invoke(new System.Action(() =>
                                        {
                                            PrimaryForm.Instance.HideSendOffScreen(); // 在PrimaryForm上隐藏送客画面
                                        }));
                                    }
                                    continue; // 继续监听其他命令
                                }
                            }

                            // 在 TCPServer 的适当位置调用更新方法
                            if (OverlayForm.MainForm != null)
                            {
                                string message = "系統公告: " + request.Trim();
                                OverlayForm.MainForm.UpdateMarqueeTextSecondLine(message);
                            }

                            // 处理命令
                            if (request.Trim().Equals("exit", StringComparison.OrdinalIgnoreCase))
                            {
                                break; // 如果收到退出命令，断开连接
                            }
                        }

                        Console.WriteLine("Connection closed.");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }
            finally
            {
                listener.Stop();
            }
        }

        private string GetHostNameSuffix()
        {
            string hostName = Dns.GetHostName();
            return hostName.Length > 3 ? hostName.Substring(hostName.Length - 3) : hostName;
        }
    }
}