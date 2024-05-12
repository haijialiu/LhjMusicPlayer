using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LhjMusicPlayer.Models
{
    public class MusicLists
    {
        public MusicList? Item { get; set; }
        public List<MusicList>? ChildList {  get; set; } 
    }
}
