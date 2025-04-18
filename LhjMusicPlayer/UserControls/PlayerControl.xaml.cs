using LhjMusicPlayer.Common;
using LhjMusicPlayer.Models;
using LhjMusicPlayer.ViewModels;
using LhjMusicPlayer.Views;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Dispatching;
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
using Windows.System.Threading;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LhjMusicPlayer.UserControls
{
    public sealed partial class PlayerControl : UserControl
    {
        public MusicListViewModel ViewModel => (MusicListViewModel)DataContext;
        public MusicPlayer player;

        public PlayerControl()
        {
            DataContext = App.Current.Services.GetService<MusicListViewModel>();
            player = App.Current.Services.GetRequiredService<MusicPlayer>();
            this.InitializeComponent();

        }
        private void Play_Button_Click(object sender, RoutedEventArgs e)
        {
            if (player.PlayStatus)
            {
                MusicPlayer.Operate("pause", "1");
            }
            else
            {
                MusicPlayer.Operate("resume", "1");
            }
            player.PlayStatus = !player.PlayStatus;

        }
        private void Prev_Button_Click(object sender, RoutedEventArgs e)
        {
            //player.CurrentPlayIndex -= 1;
            MusicPlayer.Operate("switch", (player.CurrentPlayIndex-1).ToString());
        }
        private void Next_Button_Click(object sender, RoutedEventArgs e)
        {
            //player.CurrentPlayIndex += 1;
            MusicPlayer.Operate("switch", (player.CurrentPlayIndex+1).ToString());
        }        
        private void Lyric_Button_Click(object sender, RoutedEventArgs e)
        {

            if(MainWindow.mainWindow?.MainFrame.BackStack.Count>0)
                MainWindow.mainWindow?.MainFrame.GoBack();
            else
                MainWindow.mainWindow?.MainFrame.Navigate(typeof(LyricPage));

        }
        private void play_progress_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            ViewModel.spyPlayer = false;
        }

        private void play_progress_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            MusicPlayer.Operate("seek", ((Slider)sender).Value.ToString());
            ViewModel.spyPlayer = true;
        }

    }
}
