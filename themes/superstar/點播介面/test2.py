import tkinter as tk
from tkinter import filedialog
from PIL import Image, ImageTk

class ImageSelectApp:
    def __init__(self, root):
        self.root = root
        self.canvas = tk.Canvas(root, cursor="cross")
        self.canvas.pack(fill=tk.BOTH, expand=True)
        self.rect = None
        self.start_x = None
        self.start_y = None
        self._drag_data = {"x": 0, "y": 0, "item": None}

        # 状态栏
        self.status = tk.Label(root, text="", bd=1, relief=tk.SUNKEN, anchor=tk.W)
        self.status.pack(side=tk.BOTTOM, fill=tk.X)

        self.canvas.bind("<ButtonPress-1>", self.on_press)
        self.canvas.bind("<B1-Motion>", self.on_drag)
        self.canvas.bind("<ButtonRelease-1>", self.on_release)

        # 加载图片
        self.load_image()

    def load_image(self):
        file_path = filedialog.askopenfilename()
        self.image = Image.open(file_path)
        self.photo = ImageTk.PhotoImage(self.image)
        self.canvas.create_image(0, 0, image=self.photo, anchor=tk.NW)
        self.canvas.config(scrollregion=self.canvas.bbox(tk.ALL), width=self.image.width, height=self.image.height)

    def on_press(self, event):
        # 记录起始点
        self.start_x = self.canvas.canvasx(event.x)
        self.start_y = self.canvas.canvasy(event.y)
        self.rect = self.canvas.create_rectangle(self.start_x, self.start_y, self.start_x, self.start_y, outline="red")

    def on_drag(self, event):
        curX = self.canvas.canvasx(event.x)
        curY = self.canvas.canvasy(event.y)
        # 更新矩形的大小
        self.canvas.coords(self.rect, self.start_x, self.start_y, curX, curY)

    def on_release(self, event):
        # 显示坐标
        end_x = self.canvas.canvasx(event.x)
        end_y = self.canvas.canvasy(event.y)
        coords = {
            "Top Left": (self.start_x, self.start_y),
            "Bottom Right": (end_x, end_y),
            "Top Right": (end_x, self.start_y),
            "Bottom Left": (self.start_x, end_y)
        }
        text = ", ".join(f"{key}: ({int(val[0])}, {int(val[1])})" for key, val in coords.items())
        self.status.config(text=text)

if __name__ == "__main__":
    root = tk.Tk()
    app = ImageSelectApp(root)
    root.mainloop()
