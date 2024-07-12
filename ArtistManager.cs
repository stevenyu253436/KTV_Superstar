using System;
using System.Collections.Generic;
using System.Data.SQLite; // SQLite 命名空间
using System.IO; // Path 命名空间
using System.Linq; // Linq 命名空间
using System.Windows.Forms; // Application 命名空间

namespace DualScreenDemo
{
    public class ArtistManager
    {
        private static ArtistManager _instance;
        public List<Artist> AllArtists { get; private set; }
        
        public static ArtistManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new ArtistManager();
                }
                return _instance;
            }
        }

        public ArtistManager()
        {
            AllArtists = new List<Artist>();
            LoadArtists();

        }

        private void LoadArtists()
        {
            string databaseFileName = "KSongDatabase.db";
            string databasePath = Path.Combine(Application.StartupPath, databaseFileName);
            // string databasePath = Path.Combine(@"\\SVR01\SuperStar", databaseFileName);
            Console.WriteLine(databasePath);
            string connectionString = String.Format("Data Source={0};Version=3;", databasePath);

            using (var connection = new SQLiteConnection(connectionString))
            {
                try
                {
                    connection.Open();  // 打開數據庫連接

                    string sql = "SELECT 歌手姓名, 歌手注音, 歌手分類, 歌手筆畫 FROM ArtistLibrary";  // 替換 your_table_name 為您的數據表名
                    using (var command = new SQLiteCommand(sql, connection))
                    {
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())  // 讀取查詢結果
                            {
                                string artist = reader["歌手姓名"].ToString();
                                string phonetic = reader["歌手注音"].ToString();
                                string category = reader["歌手分類"].ToString();
                                string strokesStr = reader["歌手筆畫"].ToString();

                                // Console.WriteLine($"Read values: Name={artist}, Phonetic={phonetic}, Category={category}, Strokes={strokesStr}");

                                // 尝试将歌手笔画值转换为double
                                if (double.TryParse(strokesStr, out double strokesDouble))
                                {
                                    int strokes = (int)Math.Round(strokesDouble); // 将double转换为int，如果有必要
                                    AllArtists.Add(new Artist(artist, phonetic, category, strokes));
                                    // Console.WriteLine($"Added artist: {artist}");
                                }
                                else
                                {
                                    Console.WriteLine($"Failed to parse '歌手筆畫' value: {strokesStr}");
                                }
                            }
                        }
                    }

                    connection.Close();  // 關閉數據庫連接

                    // 打印所有加载的艺术家
                    // PrintAllArtists();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Failed to load artists from SQLite database: " + ex.Message);
                }
            }
        }

        private void PrintAllArtists()
        {
            Console.WriteLine("All Artists:");
            foreach (var artist in AllArtists)
            {
                Console.WriteLine(artist.ToString());
            }
        }

        public List<Artist> GetArtistsByCategoryAndStrokeCountRange(string category, int minStrokes, int maxStrokes)
        {
            if (category == "全部")
            {
                return AllArtists.Where(artist => artist.Strokes >= minStrokes && artist.Strokes <= maxStrokes).ToList();
            }
            else
            {
                return AllArtists.Where(artist => artist.Category == category && artist.Strokes >= minStrokes && artist.Strokes <= maxStrokes).ToList();
            }
        }
    }
}