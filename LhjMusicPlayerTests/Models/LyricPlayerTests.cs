using Microsoft.VisualStudio.TestTools.UnitTesting;
using LhjMusicPlayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LhjMusicPlayer.Models.Tests
{
    [TestClass()]
    public class LyricPlayerTests
    {
        [TestMethod()]
        public void FindLyricByTimeTest()
        {
            var lyric = LrcParser.GetLrcFromFile(@"C:\Users\haijialiu\Desktop\media\八木海莉 - Fluorite Eye's Song.lrc");
            var lyricplayer = new LyricPlayer
            {
                Lyric = lyric
            };
            var index = lyricplayer.FindLyricByTime(19);
            if (index < 0)
            {
                index = ~index -1;
            }
            lyricplayer.MusicPlayedTime = 19;
            Console.WriteLine(index.ToString());
        }
    }
}