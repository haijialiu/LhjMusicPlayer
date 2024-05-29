using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using LhjMusicPlayer.Common;
using LhjMusicPlayer.Models;
using LhjMusicPlayer.ViewModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using System.Collections.ObjectModel;
using Windows.ApplicationModel.Contacts;
using LhjMusicPlayer.Pages;
using LhjMusicPlayer.Models.Database;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LhjMusicPlayer.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MusicListPage : Page
    {

        private MusicListViewModel ViewModel => (MusicListViewModel)DataContext;
        private MusicPlayer player;
        private ObservableCollection<Music> musics = [];
        private ObservableCollection<Music> filtedmusics = [];
        private int musicListId = -1;
        
        public MusicListPage()
        {
            DataContext = App.Current.Services.GetService<MusicListViewModel>();
            player = App.Current.Services.GetRequiredService<MusicPlayer>();
            InitializeComponent();
            
            
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if(e.Parameter is int id)
            {
                musicListId = id;
                var musicList = ViewModel.MusicLists.FirstOrDefault(list => list.Id == id)!.Musics;
                musics.Clear();
                filtedmusics.Clear();
                musicList.ForEach(music =>
                {
                    musics.Add(music);
                    filtedmusics.Add(music);
                });
            }
            foreach (var item in music_list.Items)
            {
                
            }
        }


        private void Play_Btn_Click(object sender, RoutedEventArgs e)
        {
            var item = ((FrameworkElement)sender).DataContext;
            int index = music_list.Items.IndexOf(item);
            MusicPlayer.Operate("switch", index.ToString());
            player.PlayStatus = true;
        }

        private void ListViewItem_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            int id = (int)((ListViewItem)sender).Tag;
            var musicIndex = ViewModel.PlayingList.IndexOf(ViewModel.PlayingList.Single(music=>music.Id==id));
            MusicPlayer.Operate("switch", musicIndex.ToString());
            player.PlayStatus = true;
        }

        private void Remove_Btn_Click(object sender, RoutedEventArgs e)
        {
            using var context = new DataContext();

            var item = ((FrameworkElement)sender).DataContext;

            int index = music_list.Items.IndexOf(item);
            musics!.RemoveAt(index);
            filtedmusics.RemoveAt(index);
            var btn = sender as Button;
            var music = btn?.DataContext as Music;
            if(music != null)
            {
                var mlist = context.MusicMusicLists.Single(list => (list.MusicId== music.Id)&&(list.MusicListId==musicListId));
                context.Remove(mlist);
                context.SaveChanges();
                ViewModel.UpdateData("user");
            }

            //MusicPlayer.Operate("remove", index.ToString());
        }

        public static string TimeFormat(int seconds)
        {
            return string.Format("{0}:{1}", seconds/60,seconds%60);
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(sender is TextBox textBox)
            {
                var text = textBox.Text;
                if (text == "")
                {
                    filtedmusics.Clear();
                    for(int i = 0; i < musics.Count;i++)
                    {
                        filtedmusics.Add(musics[i]);
                    }
                    return;
                }
                var filtered = musics.Where(content => content.Title.Contains(text));
                for(int i = filtedmusics.Count - 1; i >= 0; i--)
                {
                    var item = filtedmusics[i];
                    if(!filtered.Contains(item))
                    {
                        filtedmusics.Remove(item);
                    }
                }
                foreach (var item in filtered)
                {
                    if(!filtedmusics.Contains(item))
                    {
                        filtedmusics.Add(item);
                    }
                }

            }                 
        }

        private void Add_Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Add_Button_Loaded(object sender, RoutedEventArgs e)
        {

            if (sender is Button button)
            {
                var flyout = new MenuFlyout();

                if (button.ContextFlyout is MenuFlyout menuFlyout)
                {
                    ViewModel.UserLists.ToList().ForEach(item =>
                    {
                        var menuItem = new MenuFlyoutItem()
                        {
                            Text = item.Title,
                            //DataContext = item.Id,
                            Tag = item.Id
                        };
                        menuItem.Click += Add_MenuItem_Click;
                        menuFlyout.Items.Add(menuItem);
                    });
                    button.Flyout = menuFlyout;
                }
            }
        }
        private void Add_MenuItem_Click(object sender, RoutedEventArgs e)
        {
            if(sender is MenuFlyoutItem item)
            {
                if (item.DataContext is Music music)
                {
                    var list = ViewModel.UserLists.Where(list => list.Id == int.Parse(item.Tag.ToString()!));
                    using var context = new DataContext();
                    context.MusicMusicLists.Add(new MusicMusicList()
                    {
                        MusicId = music.Id,
                        MusicListId = int.Parse(item.Tag.ToString()!)
                    });
                    context.SaveChanges();
                    ViewModel.UpdateData("user");
                }
            }
        }
    }
}
