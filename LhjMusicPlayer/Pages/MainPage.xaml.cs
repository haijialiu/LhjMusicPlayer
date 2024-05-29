
using LhjMusicPlayer.Models;
using LhjMusicPlayer.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ApplicationSettings;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LhjMusicPlayer.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public static MainPage? mainPage;
        private MusicListViewModel ViewModel => (MusicListViewModel)DataContext;

        public MainPage()  
        {
            DataContext = App.Current.Services.GetService<MusicListViewModel>();

            this.InitializeComponent();
            mainPage = this;
            
            ContentFrame.Navigated += ContentFrame_Navigated;
            MainNavView.SelectedItem = MainNavView.MenuItems[1];
            foreach (var item in ViewModel.UserLists)
            {
                MainNavView.MenuItems.Add(new NavigationViewItem()
                {
                    Content = item.Name,
                    DataContext = item.Id,
                    Tag = "MusicList"
                });
            }
            ViewModel.UserLists.CollectionChanged += UserLists_CollectionChanged;
        }
        private void UserLists_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {


            while(MainNavView.MenuItems.Count > 5)
            {
                MainNavView.MenuItems.Remove(MainNavView.MenuItems[5]);
            }
            foreach(var item in ViewModel.UserLists)
            {
                MainNavView.MenuItems.Add(new NavigationViewItem()
                {
                    Content = item.Name,
                    DataContext = item.Id,
                    Tag = "MusicList"
                });
            }
        }



        private void ContentFrame_Navigated(object sender, NavigationEventArgs e)
        {
            MainNavView.IsBackEnabled = ContentFrame.CanGoBack;
        }

        public void MainNavigate(string? header, Type? type, object param)
        {
            MainNavView.Header = header;
            ContentFrame.Navigate(type, param);

        }
        public void MainNavigate(string? header, Type? type)
        {
            MainNavView.Header = header;
            ContentFrame.Navigate(type);
        }

        private void MainNavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
        {
            if (args.SelectedItem is NavigationViewItem item)
            {
                if (item.Tag?.ToString() == "MusicList")
                {
                    var id = int.Parse(item.DataContext.ToString()!);
                    MainNavigate(item.Content.ToString(), typeof(MusicListPage), id);
                }
                else if (item.Tag?.ToString() == "HomePage")
                {
                    MainNavigate("主页", typeof(HomePage));
                }
            }

        }
    }
}
