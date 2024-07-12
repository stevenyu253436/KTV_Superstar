import cv2
import numpy as np

# Load the image
image_path = '555020.jpg'  # replace with your image path
image = cv2.imread(image_path)

# Convert to grayscale
gray = cv2.cvtColor(image, cv2.COLOR_BGR2GRAY)

# Apply thresholding to get binary image
_, binary = cv2.threshold(gray, 240, 255, cv2.THRESH_BINARY)

# Find contours of the white regions
contours, _ = cv2.findContours(binary, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)

# Draw contours on the original image (optional, for visualization)
for contour in contours:
    cv2.drawContours(image, [contour], -1, (0, 255, 0), 2)

# Print positions of the white regions
for i, contour in enumerate(contours):
    x, y, w, h = cv2.boundingRect(contour)
    if w > 100:
        print(f"White region {i+1}: x={x}, y={y}, width={w}, height={h}")

# Show the image with contours (optional, for visualization)
cv2.imshow('White regions', image)
cv2.waitKey(0)
cv2.destroyAllWindows()
