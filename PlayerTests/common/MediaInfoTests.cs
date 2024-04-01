using Microsoft.VisualStudio.TestTools.UnitTesting;
using LhjMusicPlayer.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LhjMusicPlayer.common.Tests
{
    [TestClass()]
    public class MediaInfoTests
    {
        [TestMethod()]
        public void GetMusicTest()
        {
            var music = MediaInfo.GetMusic(@"C:\Users\haijialiu\Desktop\media\Aiobahn _ Yunomi (ゆのみ) - 银河鉄道のペンギン (银河铁道的企鹅).flac");

        }
    }
}