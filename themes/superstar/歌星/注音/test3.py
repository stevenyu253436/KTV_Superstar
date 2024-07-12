import cv2
import numpy as np
import os

# 使用絕對路徑
image_path = 'image.png'
if not os.path.exists(image_path):
    print("File does not exist:", image_path)
else:
    print("File exists, attempting to load...")
image = cv2.imread(image_path)

if image is None:
    print("But failed to load.")
else:
    print("Image loaded successfully, processing...")

# Convert to HSV color space
hsv = cv2.cvtColor(image, cv2.COLOR_BGR2HSV)

# Define the range for yellow color in HSV
# Adjust these ranges based on your specific yellow color and lighting conditions
lower_yellow = np.array([20, 100, 100])
upper_yellow = np.array([30, 255, 255])
lower_pink = np.array([140, 100, 100])
upper_pink = np.array([170, 255, 255])
lower_purple = np.array([129, 50, 50])  # Lower bound of purple
upper_purple = np.array([158, 255, 255])  # Upper bound of purple
lower_blue = np.array([110, 50, 50])  # Lower bound of blue
upper_blue = np.array([130, 255, 255])  # Upper bound of blue
lower_blue_violet = np.array([120, 50, 50])  # Lower bound of blue-violet
upper_blue_violet = np.array([160, 255, 255])  # Upper bound of blue-violet
lower_red2 = np.array([170, 100, 100])
upper_red2 = np.array([180, 255, 255])
lower_white = np.array([0, 0, 200])
upper_white = np.array([180, 55, 255])

# Create a mask for yellow color
mask = cv2.inRange(hsv, lower_white, upper_white)

# Find contours on the mask
contours, _ = cv2.findContours(mask, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)

# Draw all contours
for contour in contours:
    x, y, w, h = cv2.boundingRect(contour)
    cv2.rectangle(image, (x, y), (x+w, y+h), (0, 255, 0), 2)
    coordinates_text = f"X: {x}, Y: {y}, W: {w}, H: {h}"
    # if w > 100:
    print(coordinates_text)
    # Display coordinates on the image
    cv2.putText(image, coordinates_text, (x, y-10), cv2.FONT_HERSHEY_SIMPLEX, 0.5, (0, 255, 0), 1)

# Show the image with all bounding boxes drawn
cv2.imshow('Image with yellow contours', image)
cv2.waitKey(0)
cv2.destroyAllWindows()