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
using LhjMusicPlayer.Models.Database;
using LhjMusicPlayer.Common;
using Microsoft.UI.Dispatching;
using System.Threading.Tasks;
using LhjMusicPlayer.Models;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace LhjMusicPlayer.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HomePage : Page, ISubPage
    {

        public HomePage()
        {
            this.InitializeComponent();
        }

        public string? NavTitle => null;


        private readonly DispatcherQueue dispatcherQueue = DispatcherQueue.GetForCurrentThread();

        private async void Local_import_Button_Click(object sender, RoutedEventArgs e)
        {
            
            var folderPicker = new Windows.Storage.Pickers.FolderPicker
            {
                SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.MusicLibrary
            };
            folderPicker.FileTypeFilter.Add("*");

            var window = MainWindow.mainWindow;
            var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
            WinRT.Interop.InitializeWithWindow.Initialize(folderPicker, hWnd);

            Windows.Storage.StorageFolder folder = await folderPicker.PickSingleFolderAsync();

            if (folder.Path != null)
            {
                var path = folder.Path;
                using var context = new DataContext();
                var files_path = Directory.GetFiles(path);
                var infoBar = MainWindow.mainWindow!.MainInfoBar;
                var text = MainWindow.mainWindow!.MainInfoBarContextText;
                infoBar.IsOpen = true;
                await LoadMusic(path);
                text.Text = "完成";
                await Task.Delay(3000);
                infoBar.IsOpen = false;
            }
        }

        private Task LoadMusic(string path)
        {
            var files_path = Directory.GetFiles(path);
            return Task.Run(async () =>
            {
                var text = MainWindow.mainWindow!.MainInfoBarContextText;
                foreach (var file_path in files_path)
                {
                    var music = MediaInfo.GetMusic(file_path);
                    if(this.DispatcherQueue.HasThreadAccess)
                    {
                        text.Text = $"当前：{music?.Title}";
                        
                    }
                    else
                    {
                        bool isQueued = this.DispatcherQueue.TryEnqueue(
                            Microsoft.UI.Dispatching.DispatcherQueuePriority.Normal,
                            () => 
                            { 
                                text.Text = $"当前：{music?.Title}"; 
                            });
                    }
                    await Task.Delay(10);
                }
            });
        }

        private void List_Manage_Button_Click(object sender, RoutedEventArgs e)
        {
            MainPage.mainPage?.MainNavigate("歌单管理",typeof(MusicListManagePage));
        }
    }
}
