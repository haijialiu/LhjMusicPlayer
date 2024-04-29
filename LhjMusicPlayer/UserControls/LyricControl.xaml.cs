using LhjMusicPlayer.Common;
using LhjMusicPlayer.Models;
using LhjMusicPlayer.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System.Threading;
using Windows.UI.Core;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LhjMusicPlayer.UserControls
{
    public sealed partial class LyricControl : UserControl
    {
        private MusicListViewModel ViewModel => (MusicListViewModel)DataContext;
        private MusicPlayer player;
        private readonly LyricPlayer lyricPlayer;
        private readonly DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();
        private readonly TimeSpan span = TimeSpan.FromMilliseconds(100);
        private readonly TimeSpan period = TimeSpan.FromSeconds(5);
        private readonly ThreadPoolTimer threadPoolTimer;
        private ThreadPoolTimer PeriodicTimer;
        private int currentIndex = 0;
        private bool mainScroll = false;
        public LyricControl()
        {
            
            player = App.Current.Services.GetRequiredService<MusicPlayer>();
            DataContext = App.Current.Services.GetService<MusicListViewModel>();
            lyricPlayer = App.Current.Services.GetRequiredService<LyricPlayer>();
            this.InitializeComponent();
            Unloaded += LyricControllerCloseEvent;
            //LyricList.ItemsSource = player.CurrentLyric?.Words;
            threadPoolTimer = ThreadPoolTimer.CreatePeriodicTimer((source) =>
            {

                var current_play_time = FFmpegPlayer.PlayedTime();
                if (dispatcherQueue.HasThreadAccess)
                {

                }
                else
                {
                    //update UI
                    bool isQueued = dispatcherQueue.TryEnqueue(() =>
                    {
                        if (lyricPlayer.Lyric != null)
                        {
                            if (!mainScroll)
                            {
                                if (lyricPlayer.CurrentLyricIndex + 8 < lyricPlayer.Words.Count)
                                {
                                    if (lyricPlayer.CurrentLyricIndex - 6 >= 0)
                                    {
                                        LyricList.ScrollIntoView(lyricPlayer.Words[lyricPlayer.CurrentLyricIndex - 6]);
                                    }
                                    if (lyricPlayer.CurrentLyricIndex >= 0)
                                    {
                                        LyricList.ScrollIntoView(lyricPlayer.Words[lyricPlayer.CurrentLyricIndex]);
                                        LyricList.ScrollIntoView(lyricPlayer.Words[lyricPlayer.CurrentLyricIndex + 8]);
                                    }
                                }
                                else if (LyricList.Items.Count > 0)
                                {
                                    LyricList.ScrollIntoView(lyricPlayer.Words[^1]);
                                }
                            }
                            if (lyricPlayer.CurrentLyricIndex != -1 && lyricPlayer.CurrentLyricIndex < LyricList.Items.Count)
                            {
                                var next = LyricList.Items[lyricPlayer.CurrentLyricIndex];
                                if(LyricList.SelectedItem != next)
                                    LyricList.SelectedItem = LyricList.Items[lyricPlayer.CurrentLyricIndex];
                            }
                        }
                    });

                }
            }, span);
            PeriodicTimer = ThreadPoolTimer.CreatePeriodicTimer((source) =>
            {
                if (mainScroll)
                {
                    mainScroll = false;
                }
            }, period);

        }
        private void LyricControllerCloseEvent(object sender, RoutedEventArgs e)
        {
            threadPoolTimer.Cancel();
        }
        private void ListViewItem_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {

            MusicPlayer.Operate("seek", ((LrcWord)((ListViewItem)sender).DataContext).Time.ToString());
        }

        private void LyricList_ItemClick(object sender, ItemClickEventArgs e)
        {

        }


        private void LyricScroll_ViewChanging(object sender, ScrollViewerViewChangingEventArgs e)
        {
            //Your logic to hide the button
            mainScroll = true;
        }

        private void ListViewItem_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            Border? border = VisualTreeHelper.GetChild(LyricList, 0) as Border;
            ScrollViewer? scrollViewer = VisualTreeHelper.GetChild(border, 0) as ScrollViewer;
            if (scrollViewer != null)
            {
                scrollViewer.ViewChanging += LyricScroll_ViewChanging!;
                
            }
        }
    }
}
