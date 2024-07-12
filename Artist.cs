namespace DualScreenDemo
{
    public class Artist
    {
        // 歌手姓名
        public string Name { get; set; }
        
        // 歌手注音
        public string Phonetic { get; set; }
        
        // 歌手分类
        public string Category { get; set; }
        
        // 歌手笔画
        public int Strokes { get; set; }

        // 构造函数
        public Artist(string name, string phonetic, string category, int strokes)
        {
            Name = name;
            Phonetic = phonetic;
            Category = category;
            Strokes = strokes;
        }

        // 覆盖 ToString 方法方便调试和显示
        public override string ToString()
        {
            return $"Name: {Name}, Phonetic: {Phonetic}, Category: {Category}, Strokes: {Strokes}";
        }
    }
}