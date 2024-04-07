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
    public class MusicTests
    {
        [TestMethod()]
        public void SearchMusicsFromFolderTest()
        {
            var folderPath = @"E:\music";
            var musics = Music.SearchMusicsFromFolder(folderPath);


        }

        [TestMethod()]
        public void LoadLocalMusicsTest()
        {
            var folderPath = @"E:\music";
            Music.LoadLocalMusics(folderPath);
        }
    }
}