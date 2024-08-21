using System;

namespace DualScreenDemo
{
    public class SongData
    {
        public string SongNumber { get; set; } // 添加歌曲编号属性
        public string Category { get; set; }
        public string Song { get; set; }
        public double Plays { get; set; }
        public string ArtistA { get; set; }
        public string ArtistB { get; set; }
        public string ArtistACategory { get; set; }
        public string ArtistBCategory { get; set; }
        public DateTime AddedTime { get; set; } // 新增加入时间属性
        public string SongFilePathHost1 { get; set; } // 添加属性以存储歌曲文件路径
        public string SongFilePathHost2 { get; set; }
        public string PhoneticNotation { get; set; } // Add a new property for phonetic notation
        public string PinyinNotation { get; set; } // 拼音字段
        public string ArtistAPhonetic { get; set; } // 新增歌手A注音
        public string ArtistBPhonetic { get; set; } // 新增歌手B注音
        public string ArtistASimplified { get; set; } // 新增歌手A簡體中文
        public string ArtistBSimplified { get; set; } // 新增歌手B簡體中文
        public string SongSimplified { get; set; } // 新增歌曲簡體中文
        public string SongGenre { get; set; }
        public string ArtistAPinyin { get; set; } // 新增歌手A拼音
        public string ArtistBPinyin { get; set; } // 新增歌手B拼音
        public int HumanVoice { get; set; } // Add HumanVoice property

        // 更新构造函数以包括歌曲文件路径参数
        public SongData(string songNumber, string category, string song, double plays, string artistA, string artistB, string artistACategory, string artistBCategory, DateTime addedTime, string songFilePathHost1, string songFilePathHost2, string phoneticNotation, string pinyinNotation, string artistAPhonetic, string artistBPhonetic, string artistASimplified, string artistBSimplified, string songSimplified, string songGenre, string artistAPinyin, string artistBPinyin, int humanVoice)
        {
            SongNumber = songNumber;
            Category = category;
            Song = song;
            Plays = plays;
            ArtistA = artistA;
            ArtistB = artistB;
            ArtistACategory = artistACategory;
            ArtistBCategory = artistBCategory;
            AddedTime = addedTime;
            SongFilePathHost1 = songFilePathHost1; // 分配歌曲文件路径参数到 SongFilePath 属性
            SongFilePathHost2 = songFilePathHost2;
            PhoneticNotation = phoneticNotation; // Assign the new parameter
            PinyinNotation = pinyinNotation;
            ArtistAPhonetic = artistAPhonetic;
            ArtistBPhonetic = artistBPhonetic;
            ArtistASimplified = artistASimplified;
            ArtistBSimplified = artistBSimplified;
            SongSimplified = songSimplified;
            SongGenre = songGenre;
            ArtistAPinyin = artistAPinyin;
            ArtistBPinyin = artistBPinyin;
            HumanVoice = humanVoice; // Assign the HumanVoice parameter
        }

        public override string ToString()
        {
            // 检查是否存在 ArtistB 并相应调整输出格式
            return !string.IsNullOrWhiteSpace(ArtistB)
                ? String.Format("{0} - {1} - {2}", ArtistA, ArtistB, Song)
                : String.Format("{0} - {1}", ArtistA, Song);
        }
    }
}