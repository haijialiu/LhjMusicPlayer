using LhjMusicPlayer.common;
using LhjMusicPlayer.Models;
using LhjMusicPlayer.ViewModels;
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
        private readonly DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        private readonly TimeSpan span = TimeSpan.FromMilliseconds(100);
        private readonly ThreadPoolTimer threadPoolTimer;
        private bool spyPlayer = true;
        public PlayerControl()
        {
            DataContext = App.Current.Services.GetService<MusicListViewModel>();
            player = App.Current.Services.GetRequiredService<MusicPlayer>();
            this.InitializeComponent();
            Unloaded += PlayControllerCloseEvent;
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
                            player.CurrentPlayIndex = current_index;
                            player.CurrentTime = FFmpegPlayer.PlayedTime();

                        });
                    }
                }
            }, span);
        }
        private void PlayControllerCloseEvent(object sender, RoutedEventArgs e)
        {
            threadPoolTimer.Cancel();
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
            player.CurrentPlayIndex -= 1;
            MusicPlayer.Operate("switch", player.CurrentPlayIndex.ToString());
        }
        private void Next_Button_Click(object sender, RoutedEventArgs e)
        {
            player.CurrentPlayIndex += 1;
            MusicPlayer.Operate("switch", player.CurrentPlayIndex.ToString());
        }
        private void play_progress_ManipulationStarted(object sender, ManipulationStartedRoutedEventArgs e)
        {
            spyPlayer = false;
        }

        private void play_progress_ManipulationCompleted(object sender, ManipulationCompletedRoutedEventArgs e)
        {
            MusicPlayer.Operate("seek", ((Slider)sender).Value.ToString());
            spyPlayer = true;
        }

    }
}
