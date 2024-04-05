﻿using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using LhjMusicPlayer.Common;
using LhjMusicPlayer.ViewModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Microsoft.VisualBasic;


namespace LhjMusicPlayer.Models
{
    //播放器
    public partial class MusicPlayer : ObservableObject
    {
        //播放列表
        public ObservableCollection<Music> PlayList { get; set; } = [];
        //当前播放歌曲的序号
        private int currentIndex = -1;

        public int CurrentPlayIndex
        {
            get => currentIndex;
            set
            {
                if (value < 0)
                {
                    if (PlayList.Count == 0)
                    {
                        value = 0;
                    }
                    else
                    {
                        value = PlayList.Count - 1;

                    }
                }
                else if (value >= PlayList.Count)
                {
                    value = 0;
                }
                SetProperty(ref currentIndex, value);
                //OnPropertyChanged(nameof(CurrentMusic));
                if (value < PlayList.Count)
                {
                    //当前歌曲
                    CurrentMusic = PlayList[value];
                    //当前歌词
                    if (CurrentMusic?.LyricFilePath != null)
                    {
                        if (currentLyricFilePath != CurrentMusic.LyricFilePath)
                        {
                            currentLyricFilePath = CurrentMusic.LyricFilePath;
                            //CurrentLyric = LrcParser.GetLrcFromFile(CurrentMusic.LyricFilePath);
                            //LoadLyric(currentLyricFilePath);
                            Task.Run(() => { 
                                CurrentLyric = LrcParser.GetLrcFromFile(currentLyricFilePath);
                            });
                        }
                    }
                }
            }
        }
        [ObservableProperty]
        private bool playStatus = false;

        //当前播放的音乐，其实就是PlayList[CurrentPlayIndex]
        [ObservableProperty]
        private Music? currentMusic;

        //TODO: 音量调节(暂未实现）
        private int volume { get; set; }
        //播放模式 0 单曲 1 顺序播放 2 列表循环
        [ObservableProperty]
        private int playMode = 1;

        //当前播放时间
        [ObservableProperty]
        private double currentTime = 0;

        [ObservableProperty]
        private Lrc? currentLyric;
        private string? currentLyricFilePath = null;
        //总时长
        [ObservableProperty]
        private double totalTime;

        public MusicPlayer()
        {
            FFmpegPlayer.InitPlayer();

        }
        ~MusicPlayer()
        {
            FFmpegPlayer.DestroyPlayer();
        }
        //切换播放列表
        public void ReplacePlayList(List<Music> list)
        {

            FFmpegPlayer.InputMusic(list);
            PlayList?.Clear();

            list.ForEach(PlayList!.Add);
            if (PlayList.Count > 0)
            {
                CurrentMusic = PlayList?[0];
            }
        }
        //操作播放器
        public static void Operate(string action, string value)
        {
           FFmpegPlayer.OperatePlayer(action, value);
        }
        //切歌
        public static void SwitchMusic(int index)
        {

            Operate("switch",index.ToString());
        }
        public void Resume()
        {
            PlayStatus = true;
            Operate("resueme", "1");
        }
        private async Task LoadLyric(string filePath)
        {
            await Task.Run(() =>
            {
                CurrentLyric = LrcParser.GetLrcFromFile(filePath);
            });
        }
    }
}
