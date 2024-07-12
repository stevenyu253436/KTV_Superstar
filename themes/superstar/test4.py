import cv2
import numpy as np

def detect_light_areas(image_path):
    # 讀取圖片
    image = cv2.imread(image_path)
    if image is None:
        print("Failed to load image.")
        return
    
    # 將圖片從 BGR 轉換到 HSV 色彩空間
    hsv = cv2.cvtColor(image, cv2.COLOR_BGR2HSV)
    
    # 設定 HSV 閾值範圍來選取淺色區域
    lower_hsv = np.array([0, 0, 70])  # 較低的 HSV 閾值
    upper_hsv = np.array([180, 50, 255])  # 較高的 HSV 閾值
    
    # 創建遮罩
    mask = cv2.inRange(hsv, lower_hsv, upper_hsv)
    
    # 找到遮罩上的輪廓
    contours, _ = cv2.findContours(mask, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)
    
    # 在原始圖片上畫框
    for contour in contours:
        x, y, w, h = cv2.boundingRect(contour)
        cv2.rectangle(image, (x, y), (x + w, y + h), (0, 255, 0), 2)  # 綠色方框，2像素寬
        text = f"X:{x} Y:{y} W:{w} H:{h}"
        if w < 40 or h < 40:
            continue
        cv2.putText(image, text, (x, y - 10), cv2.FONT_HERSHEY_SIMPLEX, 0.5, (255, 255, 255), 2)  # 白色文字
    
    # 顯示原圖和結果
    cv2.imshow('Original', image)
    # cv2.imshow('Light Areas Detected', mask)  # 顯示遮罩
    cv2.waitKey(0)
    cv2.destroyAllWindows()

# 使用上面的函數
detect_light_areas('555011.jpg')  # 請更換為實際的圖片路徑
