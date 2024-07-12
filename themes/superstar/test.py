import cv2
import numpy as np

# Load the image
image_path = 'toggle_light.jpg'
image = cv2.imread(image_path)

# Convert to grayscale
gray = cv2.cvtColor(image, cv2.COLOR_BGR2GRAY)

# Set threshold for non-white color
# Note: Adjust the threshold values based on your image's specific conditions
upper_threshold = 220  # Anything below this will be considered as non-white

# Create a mask for non-white regions (below the threshold value)
mask = cv2.inRange(gray, 0, upper_threshold)

# Find contours from the mask
contours, hierarchy = cv2.findContours(mask, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)

# Draw contours or compute bounding boxes
for contour in contours:
    # Calculate the bounding rectangle for each non-white region
    x, y, w, h = cv2.boundingRect(contour)
    print(f"Bounding box coordinates: X: {x}, Y: {y}, Width: {w}, Height: {h}")

    # Optional: Draw the bounding box on the original image
    cv2.rectangle(image, (x, y), (x+w, y+h), (0, 255, 0), 2)

# Display the results
cv2.imshow('Image with Bounding Boxes', image)
cv2.waitKey(0)
cv2.destroyAllWindows()
