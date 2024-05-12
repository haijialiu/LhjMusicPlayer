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
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System.Threading;
using static System.Net.Mime.MediaTypeNames;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LhjMusicPlayer.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LyricPage : Page
    {
        private MusicPlayer player;
        static readonly BitmapImage defaultAlbumImage = new(new Uri(@"C:\Users\haijialiu\source\repos\LhjMusicPlayer\LhjMusicPlayer\Assets\image\music.png"));
        static readonly BitmapImage defaultBackgroundImage = new(new Uri(@"C:\Users\haijialiu\source\repos\LhjMusicPlayer\LhjMusicPlayer\Assets\background.png"));

        public LyricPage()
        {
            player = App.Current.Services.GetRequiredService<MusicPlayer>();
            this.InitializeComponent(); 
        }

        private void Back_Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.mainWindow?.MainFrame.GoBack();

            MainWindow.mainWindow?.SetBackgroundImage(defaultBackgroundImage);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //base.OnNavigatedTo(e);
            //var image = MainWindow.mainWindow?.MainWindowBackgroundImage;
            if (player.CurrentMusic != null)
            {

                if (player.CurrentMusic.AlbumImage != null)
                {
                    MemoryStream ms = new(player.CurrentMusic.AlbumImage);
                    BitmapImage bitmapImage = new();
                    bitmapImage.SetSource(ms.AsRandomAccessStream());
                    MainWindow.mainWindow?.SetBackgroundImage(bitmapImage);
                }
                else
                {
                    MainWindow.mainWindow?.SetBackgroundImage(defaultAlbumImage);
                }
            }
        }
    }
}
