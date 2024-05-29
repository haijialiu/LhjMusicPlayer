using LhjMusicPlayer.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace LhjMusicPlayer.Common
{
    public class FFmpegPlayer
    {
        private const string FFmpegAudioDllPath = @"C:\Users\haijialiu\source\repos\LhjMusicPlayer\x64\Debug\FFmpegPlayer.dll";

        [DllImport(FFmpegAudioDllPath, CallingConvention = CallingConvention.Cdecl)]
        private static extern int create_player();

        [DllImport(FFmpegAudioDllPath, CallingConvention = CallingConvention.Cdecl)]
        private static extern int operate(string action, string value);

        [DllImport(FFmpegAudioDllPath, CallingConvention = CallingConvention.Cdecl)]
        private static extern int input_music_urls(string[] music_urls, int num);

        [DllImport(FFmpegAudioDllPath, CallingConvention = CallingConvention.Cdecl)]
        private static extern int destroy_player();

        [DllImport(FFmpegAudioDllPath, CallingConvention = CallingConvention.Cdecl)]
        private static extern double share_play_time();

        [DllImport(FFmpegAudioDllPath, CallingConvention = CallingConvention.Cdecl)]
        private static extern int share_play_index();

        [DllImport(FFmpegAudioDllPath, CallingConvention = CallingConvention.Cdecl)]
        private static extern bool share_play_status();

        public static int CurrentPlayIndex()
        {
            return share_play_index();
        }

        public static bool GetPlayStatus()
        {
            return share_play_status();
        }

        public static double PlayedTime()
        {
            var seconds = share_play_time();
            if (seconds > 0)
            {
                return seconds;
            }
            return 0;
        }

        public static void InitPlayer()
        {
            _ = create_player();
        }

        public static void InputMusic(List<Music> musics)
        {
            List<string> musicUrls = [];
            foreach (var item in musics)
            {
                musicUrls.Add(item.FilePath);
            }
            _ = input_music_urls([.. musicUrls], musicUrls.Count);
        }

        public static void OperatePlayer(string action, string value)
        {
            _ = operate(action, value);
        }

        public static void DestroyPlayer()
        {
            _ = destroy_player();
        }
    }
}