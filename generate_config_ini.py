from configparser import ConfigParser

# Define the base paths for the normal, mouseDown, and mouseOver images
normal_base_path = "themes\\superstar\\歌星\\拼音\\VOD_歌星查詢_拼音查詢(按鍵)-"
mouse_down_base_path = "themes\\superstar\\歌星\\拼音\\VOD_歌星查詢_拼音查詢(按鍵)-"
mouse_over_base_path = "themes\\superstar\\歌星\\拼音\\VOD_歌星查詢_拼音查詢(按鍵)-"

# Create a ConfigParser object
config = ConfigParser()
config.optionxform = str  # Preserve case for option names

# # Add the section for EnglishLetterButtonImages
# config.add_section("EnglishLetterButtonImages")
config.add_section("PinYinLetterButtonImages")

# # Define the QWERTY layout sequence
# qwerty_layout = "QWERTYUIOPASDFGHJKLZXCVBNM"

# Add the image paths dynamically based on the QWERTY layout coordinates
for i in range(26):
    if i < 19:
        normal_image_number = i + 3
        mouse_down_image_number = i + 32
        mouse_over_image_number = i + 3
    else:
        normal_image_number = i + 4
        mouse_down_image_number = i + 33
        mouse_over_image_number = i + 4
    
    normal_image = f"{normal_base_path}{normal_image_number:02}.png"
    mouse_down_image = f"{mouse_down_base_path}{mouse_down_image_number:02}.png"
    mouse_over_image = f"{mouse_over_base_path}{mouse_over_image_number:02}.png"
    
    # Add the image paths to the config
    config.set("PinYinLetterButtonImages", f"button{i}_normal", normal_image)
    config.set("PinYinLetterButtonImages", f"button{i}_mouseDown", mouse_down_image)
    config.set("PinYinLetterButtonImages", f"button{i}_mouseOver", mouse_over_image)

# Add the image paths dynamically for NumberButtonImages
# for i in range(10):
#     normal_image_number = i + 3
#     mouse_down_image_number = i + 42
#     mouse_over_image_number = i + 3
    
#     normal_image = f"{normal_base_path}{normal_image_number:02}.png"
#     mouse_down_image = f"{mouse_down_base_path}{mouse_down_image_number:02}.png"
#     mouse_over_image = f"{mouse_over_base_path}{mouse_over_image_number:02}.png"
    
#     # Add the image paths to the config
#     config.set("NumberButtonImages", f"button{i}_normal", normal_image)
#     config.set("NumberButtonImages", f"button{i}_mouseDown", mouse_down_image)
#     config.set("NumberButtonImages", f"button{i}_mouseOver", mouse_over_image)

# Write the config to file
with open("config.ini", "a", encoding="utf-8") as configfile:
    config.write(configfile)

print("config.ini file has been updated successfully.")
