using Microsoft.VisualStudio.TestTools.UnitTesting;
using LhjMusicPlayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Microsoft.UI.Xaml.Shapes;

namespace LhjMusicPlayer.Models.Tests
{
    [TestClass()]
    public partial class LrcParserTests
    {
        [TestMethod()]
        public void GetLrcFromFileTest()
        {
            string path = @"C:\Users\haijialiu\Desktop\media\鹿乃 (かの) - アイロニ (反语).lrc";
            LrcParser.GetLrcFromFile(path);

        }
        [GeneratedRegex(@"^\[([0-9.:]*)\]+(.*)$", RegexOptions.Compiled)]
        private static partial Regex LrcWordsRegex();
        [GeneratedRegex(@"^\[(.*):(.*)\]$", RegexOptions.Compiled)]
        private static partial Regex LrcMetadataRegex();
    }
}