import pandas as pd
import sqlite3

# 读取Excel文件
excel_file_path = '歌星.xlsx'  # 替换为你的Excel文件路径

# 使用ExcelFile类获取工作表名称
excel_file = pd.ExcelFile(excel_file_path, engine='openpyxl')
sheet_names = excel_file.sheet_names

# 选择你想要导入的工作表
sheet_to_import = '工作表1'  # 替换为你实际的工作表名称
df = pd.read_excel(excel_file_path, sheet_name=sheet_to_import, engine='openpyxl')

# 打印前五行内容
print(f"工作表: {sheet_to_import}")
print(df.head(), "\n")

# 连接到SQLite数据库（如果数据库不存在，会自动创建）
conn = sqlite3.connect('KSongDatabase.db')
cursor = conn.cursor()

# 获取列名
columns = df.columns.tolist()

# 动态生成CREATE TABLE语句
table_name = 'ArtistLibrary'
create_table_sql = f'CREATE TABLE IF NOT EXISTS {table_name} ('
create_table_sql += ', '.join([f'"{col}" TEXT' for col in columns])
create_table_sql += ')'

# 执行CREATE TABLE语句
cursor.execute(create_table_sql)

# 将DataFrame的数据写入SQLite表
df.to_sql(table_name, conn, if_exists='append', index=False)

# 验证数据是否插入成功
print("数据插入后前五行内容:")
query_result = pd.read_sql_query(f"SELECT * FROM {table_name} LIMIT 5", conn)
print(query_result)

# 提交事务并关闭连接
conn.commit()
conn.close()