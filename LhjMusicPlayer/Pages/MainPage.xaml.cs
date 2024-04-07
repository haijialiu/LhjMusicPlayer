using LhjMusicPlayer.Views;
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

namespace LhjMusicPlayer.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            contentFrame.Navigate(typeof(HomePage));
        }
        private void MainNavition_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {

            NaviHeader.Text = args.InvokedItem.ToString();
            if (args.InvokedItemContainer.Tag.ToString() == "MusicList")
            {
                //using var context = new DataContext();
                var id = int.Parse(args.InvokedItemContainer.DataContext.ToString()!);
                //var musicList = context.MusicList.Include(list => list.Musics).Single(list => list.Id == id).Musics;
                //ViewModel.SetMusics(musicList);
                contentFrame.Navigate(typeof(MusicListPage), id);
            } else if(args.InvokedItemContainer.Tag.ToString() == "HomePage")
            {
                contentFrame.Navigate(typeof(HomePage));
            }

        }

        private void MainNavition_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            contentFrame.GoBack();
        }
    }
}
