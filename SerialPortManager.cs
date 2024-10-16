using System;
using System.IO.Ports;
using System.Text; // 添加这一行
using System.Windows.Forms; // 添加这一行
using System.Threading.Tasks; // 添加这一行
using System.IO; // 添加此行
using System.Diagnostics; // 添加此行

namespace DualScreenDemo
{
    public class SerialPortManager
    {
        internal static SerialPort mySerialPort;
        private readonly CommandHandler commandHandler;

        public SerialPortManager(CommandHandler commandHandler)
        {
            this.commandHandler = commandHandler;
        }

        public void InitializeSerialPort()
        {
            string[] ports = SerialPort.GetPortNames();
            Console.WriteLine("Available serial ports:");
            foreach (var port in ports)
            {
                Console.WriteLine(port);
            }

            if (ports.Length > 0)
            {
                mySerialPort = new SerialPort(ports[0]);
                Console.WriteLine(String.Format("Selected serial port: {0}", ports[0]));
            }
            else
            {
                MessageBox.Show("No serial ports found!");
                return;
            }

            mySerialPort.BaudRate = 9600;
            mySerialPort.Parity = Parity.None;
            mySerialPort.StopBits = StopBits.One;
            mySerialPort.DataBits = 8;
            mySerialPort.Handshake = Handshake.None;

            mySerialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);

            try
            {
                mySerialPort.Open();
                Console.WriteLine("Serial port opened successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error opening serial port: " + ex.Message);
            }
        }

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sp = (SerialPort)sender;
            int bytesToRead = sp.BytesToRead;
            byte[] buffer = new byte[bytesToRead];
            int bytesRead = sp.Read(buffer, 0, bytesToRead);

            // 将接收到的字节数据转换为16进制字符串
            StringBuilder hexData = new StringBuilder(bytesRead * 2);
            for (int i = 0; i < bytesRead; i++)
            {
                hexData.AppendFormat("{0:X2}", buffer[i]);
            }

            string indata = hexData.ToString();
            Console.WriteLine("Data Received (Hex):");
            Console.WriteLine(indata);

            Task.Run(() => commandHandler.ProcessData(indata));
        }

        public static void CloseSerialPortSafely()
        {
            if (mySerialPort != null)
            {
                try
                {
                    if (mySerialPort.IsOpen)
                    {
                        mySerialPort.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error closing serial port: " + ex.Message);
                }
            }
        }

        public static void LogData(string data)
        {
            string filePath = Path.Combine(Application.StartupPath, "dataLog.txt"); // Path to your log file

            // Append received data to the log file
            File.AppendAllText(filePath, data + Environment.NewLine);
        }

        private static bool CheckLogForShutdown(string filePath)
        {
            if (File.Exists(filePath))
            {
                // Read all contents and remove new lines for checking
                string content = File.ReadAllText(filePath).Replace(Environment.NewLine, "");
                if (content.Length >= 6 && content.Substring(content.Length - 6) == "bbbaaa")
                {
                    return true;
                }
            }
            return false;
        }

        private static void ShutdownComputer()
        {
            try
            {
                ProcessStartInfo processStartInfo = new ProcessStartInfo("cmd.exe", "/c shutdown /s /f /t 0") // "/s" for shutdown, "/f" force close applications, "/t 0" time delay set to 0 seconds
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