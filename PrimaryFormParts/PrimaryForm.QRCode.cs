using System;
using System.Drawing;
using System.IO; // 添加此行
using System.Windows.Forms;

namespace DualScreenDemo
{
    public partial class PrimaryForm : Form
    {
        private PictureBox pictureBoxQRCode;
        private Button closeQRCodeButton;

        private void OverlayQRCodeOnImage(string randomFolderPath)
        {
            try
            {
                // Load the cropped image
                string imagePath = Path.Combine(Application.StartupPath, "themes/superstar/cropped_qrcode.jpg");
                if (!File.Exists(imagePath))
                {
                    Console.WriteLine("Base image not found: " + imagePath);
                    return;
                }

                using (Image baseImage = Image.FromFile(imagePath))
                {
                    // Read the server address from the file
                    string serverAddressFilePath = Path.Combine(Application.StartupPath, "txt", "ip.txt");
                    if (!File.Exists(serverAddressFilePath))
                    {
                        Console.WriteLine("Server address file not found: " + serverAddressFilePath);
                        return;
                    }

                    string serverAddress = File.ReadAllText(serverAddressFilePath).Trim();
                    // Generate the URL content for the QR code
                    string qrContent = String.Format("{0}/{1}/index.html", serverAddress, randomFolderPath);
                    Console.WriteLine("QR Content: " + qrContent);

                    // Generate QR code image
                    string qrImagePath = Path.Combine(Application.StartupPath, "themes/superstar/_www", randomFolderPath, "qrcode.png");
                    if (!File.Exists(qrImagePath))
                    {
                        Console.WriteLine("QR code image not found: " + qrImagePath);
                        return;
                    }

                    // Attempt to open the QR code image with retries
                    Image qrCodeImage = null;
                    for (int i = 0; i < 3; i++)
                    {
                        try
                        {
                            using (var fs = new FileStream(qrImagePath, FileMode.Open, FileAccess.Read))
                            {
                                qrCodeImage = Image.FromStream(fs);
                            }
                            break;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("Error loading QR code image: " + ex.Message);
                            System.Threading.Thread.Sleep(100); // Wait a bit before retrying
                        }
                    }

                    if (qrCodeImage == null)
                    {
                        Console.WriteLine("Failed to load QR code image after multiple attempts.");
                        return;
                    }

                    using (qrCodeImage)
                    {
                        // Create a bitmap from the base image to allow drawing
                        using (Bitmap bitmap = new Bitmap(baseImage.Width, baseImage.Height))
                        {
                            using (Graphics g = Graphics.FromImage(bitmap))
                            {
                                // Draw the base image
                                g.DrawImage(baseImage, 0, 0);

                                // Set the position where you want to overlay the QR code
                                // Adjust the size of qrCodeImage if necessary
                                Rectangle qrCodeRect = new Rectangle(32, 39, 165, 165);
                                // Draw the QR code image over the base image at the specified position and size
                                g.DrawImage(qrCodeImage, qrCodeRect);
                            }

                            // Now display this image in your PictureBox
                            pictureBoxQRCode.Image = new Bitmap(bitmap);
                        }
                    }
                }

                // Resize and reposition the PictureBox after the image has been set
                ResizeAndPositionControl(pictureBoxQRCode, 975, 442, 226, 274);

                // 加載整個圖像
                Bitmap originalImage = new Bitmap(Path.Combine(Application.StartupPath, "themes\\superstar\\cropped_qrcode.jpg"));

                // 定義裁剪區域
                Rectangle closeQRCodeCropArea = new Rectangle(198, 6, 22, 22);

                // 裁剪出指定區域
                Bitmap closeQRCodeCroppedImage = new Bitmap(closeQRCodeCropArea.Width, closeQRCodeCropArea.Height);
                using (Graphics g = Graphics.FromImage(closeQRCodeCroppedImage))
                {
                    g.DrawImage(originalImage, new Rectangle(0, 0, closeQRCodeCropArea.Width, closeQRCodeCropArea.Height), closeQRCodeCropArea, GraphicsUnit.Pixel);
                }

                // 創建按鈕並設置背景圖像
                closeQRCodeButton = new Button { Text = "" };
                closeQRCodeButton.Name = "closeQRCodeButton";
                ResizeAndPositionButton(closeQRCodeButton, 1173, 448, 22, 22);
                closeQRCodeButton.BackgroundImage = closeQRCodeCroppedImage;
                closeQRCodeButton.BackgroundImageLayout = ImageLayout.Stretch;
                closeQRCodeButton.FlatStyle = FlatStyle.Flat;
                closeQRCodeButton.FlatAppearance.BorderSize = 0; // 移除邊框
                closeQRCodeButton.Click += CloseQRCodeButton_Click;
                this.Controls.Add(closeQRCodeButton);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in OverlayQRCodeOnImage: " + ex.Message);
                if (ex.InnerException != null)
                {
                    Console.WriteLine("Inner exception: " + ex.InnerException.Message);
                }
            }
        }

        private void CloseQRCodeButton_Click(object sender, EventArgs e)
        {
            pictureBoxQRCode.Visible = false;
            closeQRCodeButton.Visible = false;
        }
    }
}