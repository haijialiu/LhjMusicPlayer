using Microsoft.UI.Xaml.Media.Imaging;
using LhjMusicPlayer.Models;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace LhjMusicPlayer.Common
{
    public class MediaInfo
    {
        private const string MediaInfoDllPath = @"C:\Users\haijialiu\source\repos\LhjMusicPlayer\x64\Debug\MusicInfo.dll";  //TODO: 回头路径记得改回来

        [DllImport(MediaInfoDllPath, CallingConvention = CallingConvention.Cdecl,CharSet = CharSet.Unicode)]
        private static extern int music_load(string url);




        [DllImport(MediaInfoDllPath, CallingConvention = CallingConvention.Cdecl)]
        private static extern MediaInfos get_music_info();
        [DllImport(MediaInfoDllPath, CallingConvention = CallingConvention.Cdecl)]
        private static extern void music_free();
        public static Music? GetMusic(string filePath)
        {
            if (!File.Exists(filePath)) { throw new FileNotFoundException(); }
            Music? music = null;
            int ret = music_load(filePath);
            if (ret >= 0)
            {

                MediaInfos infos = get_music_info();
                music = new Music(infos.title ?? Path.GetFileNameWithoutExtension(filePath), filePath); 
                music.Title = infos.title ?? Path.GetFileNameWithoutExtension(filePath);
                music.Artist = infos.artist;
                music.Album = infos.album;
                music.Time = infos.total_time;
                var albumData = infos.album_info;
                if (albumData.ImageSize > 0)
                {
                    byte[] buffer = new byte[albumData.ImageSize];
                    Marshal.Copy(albumData.Image, buffer, 0, albumData.ImageSize);
                    music.AlbumImage = buffer;
                }
                else
                {
                    music.AlbumImage = null;
                }
            }
            music_free();
            return music;
        }
    }

    public struct AlbumImageInfo
    {
        public int ImageSize;
        public IntPtr Image;
    }

    [StructLayout(LayoutKind.Sequential,CharSet = CharSet.Unicode)]
    public struct MediaInfos
    {
        [MarshalAs(UnmanagedType.ByValTStr,SizeConst = 256)] public string title;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)] public string artist;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)] public string album;
        public int total_time;
        public AlbumImageInfo album_info;
    }
}