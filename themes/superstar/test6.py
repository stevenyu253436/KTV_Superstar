from PIL import Image

def change_image_dpi(input_path, output_path, new_dpi):
    # 打开图像
    image = Image.open(input_path)
    
    # 获取当前的dpi值（如果有）
    dpi = image.info.get('dpi', (72, 72))
    print(f"当前DPI: {dpi}")
    
    # 保存图像并设置新的dpi值
    image.save(output_path, dpi=(new_dpi, new_dpi))
    print(f"DPI已更改为: {new_dpi}")

# 输入图像路径
input_image_path = "image.jpg"

# 输出图像路径
output_image_path = "image.jpg"

# 目标DPI
new_dpi = 96

# 调用函数
change_image_dpi(input_image_path, output_image_path, new_dpi)
