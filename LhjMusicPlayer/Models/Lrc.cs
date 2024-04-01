using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LhjMusicPlayer.Models
{
    public class Lrc(string title, string offset)
    {

        /// <summary>
        /// 歌曲
        /// </summary>
        public string Title { get; set; } = title;
        /// <summary>
        /// 艺术家
        /// </summary>
        public string? Artist { get; set; }
        /// <summary>
        /// 专辑
        /// </summary>
        public string? Album { get; set; }
        /// <summary>
        /// 歌词作者
        /// </summary>
        public string? LrcBy { get; set; }
        /// <summary>
        /// 偏移量
        /// </summary>
        public string Offset { get; set; } = offset;

        public Dictionary<double, string> LrcWords = [];
    }
}
