using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LhjMusicPlayer.Models
{
    public partial class LrcParser
    {



        public static Lrc GetLrcFromFile(string path)
        {
            Dictionary<string,string> metadata = [];
            List<LrcWord> words = [];
            using (FileStream fs = new(path,FileMode.Open,FileAccess.Read,FileShare.Read))
            {
                string? line;
                Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                var codes = Encoding.GetEncodings().ToArray();
                using StreamReader sr = new(fs,Encoding.GetEncoding("gb2312"));
                while ((line = sr.ReadLine()) != null)
                {
                    Regex metaDataRegex = LrcMetadataRegex();
                    Regex wordsRegex = LrcWordsRegex();
                    var m1 = metaDataRegex.Match(line);
                    var m2 = wordsRegex.Match(line);

                    if (m2.Success)
                    {
                        double time = TimeSpan.Parse("00:" + m2.Groups[1].Value).TotalSeconds;
                        string word = m2.Groups[2].Value;
                        words.Add(new(time,word));
                    }
                    else if (m1.Success)
                    {
                        metadata.Add(m1.Groups[1].Value, m1.Groups[2].Value);
                    }

                }
                fs.Close();
            }
            Lrc lrc = new()
            {
                Title = metadata.GetValueOrDefault("ti"),
                Offset = metadata.GetValueOrDefault("offset"),
                Artist = metadata.GetValueOrDefault("ar"),
                Album = metadata.GetValueOrDefault("al"),
                LrcBy = metadata.GetValueOrDefault("by"),
                Words = new System.Collections.ObjectModel.ObservableCollection<LrcWord>(words),
            };
            return lrc;
        }
        static string SplitInfo(string line)
        {
            return line[(line.IndexOf(':') + 1)..].TrimEnd(']');
        }

        [GeneratedRegex(@"^\[(.*):(.*)\]$", RegexOptions.Compiled)]
        private static partial Regex LrcMetadataRegex();
        //[GeneratedRegex(@"^\[([0-9.:]*)\]+(.*)$", RegexOptions.Compiled)]
        [GeneratedRegex(@"^\[([0-9]{2}:[0-9]{2}\.[0-9]{2})\]+(.*)$", RegexOptions.Compiled)]
        private static partial Regex LrcWordsRegex();        
    }

}
