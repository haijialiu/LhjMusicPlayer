using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using LhjMusicPlayer.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using LhjMusicPlayer.Models;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LhjMusicPlayer.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MusicListManagePage : Page
    {
        private MusicListViewModel ViewModel => (MusicListViewModel)DataContext;
        public MusicListManagePage()
        {
            DataContext = App.Current.Services.GetService<MusicListViewModel>();
            this.InitializeComponent();
        }

        private void AddList_Button_Click(object sender, RoutedEventArgs e)
        {
            var title = ListNameBox.Text;
            ViewModel.UserLists.Add(new Models.MusicList() {
                Title = title,
                Name = title
            });

        }

        private void Remove_Button_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.UserLists.Remove((MusicList)((Button)sender).DataContext);
        }
    }
}
