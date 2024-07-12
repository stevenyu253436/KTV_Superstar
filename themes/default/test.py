import cv2
import numpy as np

# 加载图像
image = cv2.imread('7-1-1.png')

# 转换为灰度图像
gray = cv2.cvtColor(image, cv2.COLOR_BGR2GRAY)

# 应用阈值来标识非白色区域
_, thresholded = cv2.threshold(gray, 240, 255, cv2.THRESH_BINARY_INV)

# 查找轮廓
contours, _ = cv2.findContours(thresholded, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)

# 假设最大轮廓即为我们要找的区域
c = max(contours, key=cv2.contourArea)

# 计算边界框
x, y, w, h = cv2.boundingRect(c)

print(f"Left upper corner: ({x}, {y}), Width: {w}, Height: {h}")
