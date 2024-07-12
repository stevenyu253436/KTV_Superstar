import sqlite3
from pypinyin import lazy_pinyin, Style
from tqdm import tqdm

def chinese_to_initials(chinese_str):
    pinyin_list = lazy_pinyin(chinese_str, style=Style.FIRST_LETTER)
    initials = ''.join([char.upper() for char in pinyin_list])
    return initials

# 連接到 SQLite 資料庫
conn = sqlite3.connect('KSongDatabase.db')
cursor = conn.cursor()

# 確保 SongLibrary 表中有 SingerA_pinyin 和 SingerB_pinyin 欄位
# cursor.execute('''ALTER TABLE SongLibrary ADD COLUMN 歌星A拼音 TEXT''')
# cursor.execute('''ALTER TABLE SongLibrary ADD COLUMN 歌星B拼音 TEXT''')

# 讀取所有的歌手名稱
cursor.execute("SELECT [歌曲編號], [歌星 A], [歌星 B] FROM SongLibrary")
rows = cursor.fetchall()

# 使用 tqdm 來顯示進度條
for row in tqdm(rows, desc="Processing songs", unit="song"):
    song_id, singer_a, singer_b = row
    if singer_a:
        singer_a_pinyin = chinese_to_initials(singer_a)
    else:
        singer_a_pinyin = None

    if singer_b:
        singer_b_pinyin = chinese_to_initials(singer_b)
    else:
        singer_b_pinyin = None

    # 更新資料庫
    cursor.execute(
        "UPDATE SongLibrary SET 歌星A拼音 = ?, 歌星B拼音 = ? WHERE [歌曲編號] = ?",
        (singer_a_pinyin, singer_b_pinyin, song_id)
    )

# 提交變更並關閉資料庫連接
conn.commit()
conn.close()
