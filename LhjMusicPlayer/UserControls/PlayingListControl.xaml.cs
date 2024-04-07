using LhjMusicPlayer.Models;
using LhjMusicPlayer.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LhjMusicPlayer.UserControls
{
    public sealed partial class PlayingListControl : UserControl
    {
        private MusicListViewModel MusicListViewModel => (MusicListViewModel)DataContext;
        private readonly MusicPlayer player;
        public PlayingListControl()
        {
            DataContext = App.Current.Services.GetService<MusicListViewModel>();
            player = App.Current.Services.GetRequiredService<MusicPlayer>();
            this.InitializeComponent();
            
        }

        private void ListViewItem_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {

            if (!player.PlayStatus)
                player.PlayStatus = true;
            var index = playing_list.SelectedIndex;
            Models.MusicPlayer.SwitchMusic(index);
        }

        private void Playing_list_Loaded(object sender, RoutedEventArgs e)
        {
            playing_list.SelectedIndex = player.CurrentPlayIndex;
            playing_list.ScrollIntoView(playing_list.Items[player.CurrentPlayIndex]);
        }

        private void Remove_Button_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
