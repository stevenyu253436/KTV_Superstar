import cv2
import numpy as np

# 初始化坐标点
roi_pts = []
drawing = False # True if the mouse is pressed down

# 鼠标回调函数
def select_roi(event, x, y, flags, param):
    global roi_pts, drawing
    
    # 当按下左键是记录起始位置坐标
    if event == cv2.EVENT_LBUTTONDOWN:
        drawing = True
        roi_pts = [(x, y)]

    # 当鼠标左键按下并移动是绘制图形
    elif event == cv2.EVENT_MOUSEMOVE:
        if drawing == True:
            temp_image = param.copy()
            cv2.rectangle(temp_image, roi_pts[0], (x, y), (0, 255, 0), 2)
            cv2.imshow('image', temp_image)

    # 当松开鼠标左键停止绘画
    elif event == cv2.EVENT_LBUTTONUP:
        drawing = False
        roi_pts.append((x, y))
        cv2.rectangle(param, roi_pts[0], (x, y), (0, 255, 0), 2)
        cv2.imshow('image', param)

# 读取图像
image = cv2.imread('image.png')
image_copy = image.copy()
cv2.namedWindow('image')
cv2.setMouseCallback('image', select_roi, image)

# Keep looping until the 'q' key is pressed
while True:
    cv2.imshow('image', image)
    key = cv2.waitKey(1) & 0xFF
    
    # 按下'r'重置选择区域
    if key == ord('r'):
        image = image_copy.copy()
        roi_pts = []

    # 按下'q'退出循环
    elif key == ord('q'):
        break

# 关闭所有打开的窗口
cv2.destroyAllWindows()

# 如果选择了区域，那么进行黄色检测
if len(roi_pts) == 2:
    roi = image_copy[roi_pts[0][1]:roi_pts[1][1], roi_pts[0][0]:roi_pts[1][0]]
    hsv_roi = cv2.cvtColor(roi, cv2.COLOR_BGR2HSV)
    # 定义黑色的 HSV 范围
    lower_black = np.array([0, 0, 0])
    upper_black = np.array([180, 255, 50])  # 可以根据实际情况调整亮度上限
    mask = cv2.inRange(hsv_roi, lower_black, upper_black)
    
    # 在掩码上找出轮廓
    contours, _ = cv2.findContours(mask, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)

    # 如果有轮廓则找出最大的轮廓
    if contours:
        max_contour = max(contours, key=cv2.contourArea)
        x, y, w, h = cv2.boundingRect(max_contour)
        cv2.rectangle(roi, (x, y), (x+w, y+h), (0, 255, 0), 2)
        print(f"Yellow area at X: {x}, Y: {y}, W: {w}, H: {h}")
        
        # 将边界框位置映射回原始图像
        x += roi_pts[0][0]
        y += roi_pts[0][1]
        cv2.rectangle(image_copy, (x, y), (x+w, y+h), (0, 255, 0), 2)
        print(f"Yellow area at X: {x}, Y: {y}, W: {w}, H: {h}")
    
    cv2.imshow('Detected Yellow Area in ROI', image_copy)
    cv2.waitKey(0)
    cv2.destroyAllWindows()
