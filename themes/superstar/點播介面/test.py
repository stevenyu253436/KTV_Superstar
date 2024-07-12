from PIL import Image

def trim_border(image):
    # 將圖像轉換為 RGBA 模式，以處理透明度
    image = image.convert("RGBA")
    # 獲取圖像大小
    width, height = image.size
    # 獲取圖像的像素數據
    pixels = image.load()

    # 遍歷圖像的邊框像素，將白色部分變為透明
    for x in range(width):
        for y in range(height):
            # 如果是邊框的白色像素，則將其設置為透明
            if (x < 5 or x > width - 5 or y < 5 or y > height - 5) and pixels[x, y][:3] == (255, 255, 255):
                pixels[x, y] = (255, 255, 255, 0)  # 將白色像素設置為完全透明

    # 回傳處理後的圖像
    return image

# 讀取圖像
input_image = Image.open("點播介面_關閉.png")

# 裁剪邊框並將白色部分變為透明
output_image = trim_border(input_image)

# 保存處理後的圖像
output_image.save("點播介面_關閉.png")