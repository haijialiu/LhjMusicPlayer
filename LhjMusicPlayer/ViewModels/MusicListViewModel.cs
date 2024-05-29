using CommunityToolkit.Mvvm.ComponentModel;
using LhjMusicPlayer.Common;
using LhjMusicPlayer.Models;
using LhjMusicPlayer.Models.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Windows.System.Threading;

namespace LhjMusicPlayer.ViewModels
{
    public sealed partial class MusicListViewModel : ObservableRecipient
    {
        //private static readonly MusicListViewModel instance= new();
        //public static MusicListViewModel GetIntance() => instance;
        private readonly DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        private readonly TimeSpan span = TimeSpan.FromMilliseconds(100);
        public readonly ThreadPoolTimer threadPoolTimer;
        public bool spyPlayer = true;
        public bool mainLyricPage = false;

        [ObservableProperty]
        private BitmapImage? currentAlbum;

        public MusicListViewModel()
        {
            //Init();
            using var context = new DataContext();
            var musics = context.Musics;

            //include负责把相关的关联数据查询进来
            var musicList = context.MusicList.Include(list => list.Musics);
            //用户列表
            UserLists = new ObservableCollection<MusicList>(musicList.Where(list => list.Type == "user"));



            MusicLists = new ObservableCollection<MusicList>(musicList);

            var playingList = musicList.Single(list => list.Title == "playing");
            PlayingList = new ObservableCollection<Music>(context.Entry(playingList).Collection(list => list.Musics).Query());

            var favoList = musicList.Single(list => list.Title == "loving");
            FavoList = new ObservableCollection<Music>(context.Entry(favoList).Collection(list => list.Musics).Query());
            
            var localList = musicList.Single(list => list.Title == "local");
            LocalList = new ObservableCollection<Music>(context.Entry(localList).Collection(list => list.Musics).Query());

            Player = App.Current.Services.GetRequiredService<MusicPlayer>();
            LyricPlayer = App.Current.Services.GetRequiredService<LyricPlayer>();
           
            Player.ReplacePlayList(playingList.Musics);


           
            threadPoolTimer = ThreadPoolTimer.CreatePeriodicTimer((source) =>
            {
                if (spyPlayer)
                {
                    var current_index = FFmpegPlayer.CurrentPlayIndex();
                    var current_play_time = FFmpegPlayer.PlayedTime();
                    if (dispatcherQueue.HasThreadAccess)
                    {
                    }
                    else
                    {
                        bool isQueued = dispatcherQueue.TryEnqueue(() =>
                        {
                            //修改值会触发onChanged 刷新UI
                            if (Player.CurrentPlayIndex != current_index)
                            {
                                Player.CurrentPlayIndex = current_index;
                                if (Player.CurrentMusic?.LyricFilePath != null)
                                {
                                    LyricPlayer.LoadLyric(Player.CurrentMusic.LyricFilePath);
                                }
                                else
                                {
                                    LyricPlayer.Lyric = null;
                                }
                            }
                            LyricPlayer.MusicPlayedTime = current_play_time;
                            Player.CurrentTime = current_play_time;

                        });
                    }
                }
            }, span);
        }

        public void ReplacePlayingList(int listId)
        {
            using var context = new DataContext();
            PlayingList.Clear();
            context.MusicList.Include(list => list.Musics).Single(list => list.Id == listId).Musics.ForEach(PlayingList.Add);
        }
        public void UpdateAllData()
        {
            UpdateData("user");
            UpdateData("favo");
            UpdateData("local");
        }
        public void UpdateData(string name)
        {
            using var context = new DataContext();
            var musicList = context.MusicList.Include(list => list.Musics);
            if (name == "userlist")
            {
                UserLists.Clear();
                
               
                foreach (var item in musicList.Where(list => list.Type == "user"))
                {
                    UserLists.Add(item);
                }
            }
            else if(name == "favo")
            {
                var favoList = musicList.Single(list => list.Title == "loving");
                foreach (var item in context.Entry(favoList).Collection(list => list.Musics).Query())
                {
                    FavoList.Add(item);
                }
            }
            else if (name == "local")
            {
                var localList = musicList.Single(list => list.Title == "local");
                foreach (var item in context.Entry(localList).Collection(list => list.Musics).Query())
                {
                    LocalList.Add(item);
                }
            }
            MusicLists.Clear();
            foreach (var item in musicList)
            {
                MusicLists.Add(item);
            }
        }

        private MusicPlayer Player { get; }
        private LyricPlayer LyricPlayer { get; }

        //全部列表
        public ObservableCollection<MusicList> MusicLists { get; } = [];

        //用户自定义的列表
        public ObservableCollection<MusicList> UserLists { get; set; }



        //本地歌单
        public ObservableCollection<Music> LocalList { get; set; } = [];        
        //收藏歌单
        public ObservableCollection<Music> FavoList { get; set; }

        //当前播放歌单
        public ObservableCollection<Music> PlayingList { get; set; } = [];

        //初始化数据库
        private static void Init()
        {
            using var context = new DataContext();
            var musiclist = context.MusicList;
            musiclist.Add(new Models.MusicList() { Id = 1, Title = "local", Name = "本地歌曲",Type="system" ,CreatedTime = DateTime.Now });
            musiclist.Add(new Models.MusicList() { Id = 2, Title = "loving", Name = "我喜欢", Type = "system", CreatedTime = DateTime.Now });
            musiclist.Add(new Models.MusicList() { Id = 3, Title = "playing", Name = "播放列表", Type = "system", CreatedTime = DateTime.Now });

            var musics = context.Musics;
            var musicMusiclist = context.MusicMusicLists;
            var folderPath = @"C:\Users\haijialiu\Desktop\media";
            var files_path = Directory.GetFiles(folderPath);
            int index = 1;
            foreach (var file_path in files_path)
            {
                var item = MediaInfo.GetMusic(file_path);
                if (item == null)
                { continue; }
                item.Id = index;
                musics.Add(item);
                musicMusiclist.Add(new MusicMusicList() { MusicId = index, MusicListId = 1 });
                musicMusiclist.Add(new MusicMusicList() { MusicId = index, MusicListId = 2 });
                musicMusiclist.Add(new MusicMusicList() { MusicId = index, MusicListId = 3 });
                index++;
            }

           
            context.SaveChanges();
        }

    }
}