using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LhjMusicPlayer.Models
{
    public partial class Lrc : ObservableObject
    {

        /// <summary>
        /// 歌曲
        /// </summary>
        public string? Title { get; set; }
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
        public string? Offset { get; set; }
        //public OrderedDictionary LrcWords = [];
        public ObservableCollection<LrcWord> Words = [];

    }
    public record LrcWord(double Time,string? Word): IComparable<LrcWord>
    {
        public double Time { get; set; } = Time;
        public string? Word { get; set; } = Word;

        public int CompareTo(LrcWord? other)
        {
            return Time.CompareTo(other?.Time);
        }
    }
}
