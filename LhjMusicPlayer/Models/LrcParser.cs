using System;
using System.Collections.Generic;
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
            Dictionary<string,string> metadata = new Dictionary<string,string>();
            Dictionary<double,string> words = new Dictionary<double,string>();
            using (FileStream fs = new FileStream(path,FileMode.Open,FileAccess.Read,FileShare.Read))
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
                    if (m1.Success)
                    {
                        metadata.Add(m1.Groups[1].Value, m1.Groups[2].Value);
                    }
                    if(m2.Success)
                    {
                        double time = TimeSpan.Parse("00:" + m2.Groups[1].Value).TotalSeconds;
                        string word = m2.Groups[2].Value;
                        words.Add(time, word);
                    }

                }
                fs.Close();
            }
            Lrc lrc = new(metadata["ti"], metadata["offset"])
            {
                Artist = metadata["ar"],
                Album = metadata["al"],
                LrcBy = metadata["by"]
            };

            

            return lrc;
        }
        static string SplitInfo(string line)
        {
            return line[(line.IndexOf(':') + 1)..].TrimEnd(']');
        }

        [GeneratedRegex(@"^\[(.*):(.*)\]$", RegexOptions.Compiled)]
        private static partial Regex LrcMetadataRegex();
        [GeneratedRegex(@"^\[([0-9.:]*)\]+(.*)$", RegexOptions.Compiled)]
        private static partial Regex LrcWordsRegex();        
    }

}
