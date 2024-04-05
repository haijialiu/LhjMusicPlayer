using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LhjMusicPlayer.Models
{
    public partial class LyricPlayer : ObservableObject
    {
        [ObservableProperty]
        private int currentLyricIndex;

        [ObservableProperty]
        private Lrc? lyric;

        [ObservableProperty]
        private ObservableCollection<LrcWord> words=[];

        [ObservableProperty]
        private double musicPlayedTime;

        partial void OnLyricChanged(Lrc? value)
        {
            Microsoft.UI.Dispatching.DispatcherQueue dispatcherQueue = Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread();
            dispatcherQueue.TryEnqueue(() =>
            {
                // Update UI here
                Words.Clear();
                Words = new ObservableCollection<LrcWord>(value?.Words ?? Enumerable.Empty<LrcWord>());
            });

        }

        partial void OnMusicPlayedTimeChanged(double value)
        {
            CurrentLyricIndex = FindLyricByTime(MusicPlayedTime);
        }
        public int FindLyricByTime(double time)
        {
            if (Lyric == null)
                return -1;
            var index = Lyric.Words.ToList().BinarySearch(new(time, ""));
            if(index < 0)
            {
                index = ~index - 1;
            }
            return index;
        }
        public void LoadLyricAsync(string filePath)
        {
            Task.Run(() => { Lyric = LrcParser.GetLrcFromFile(filePath); });
        }
        public void LoadLyric(string filePath)
        {
            Lyric = LrcParser.GetLrcFromFile(filePath);
        }
    }
}
