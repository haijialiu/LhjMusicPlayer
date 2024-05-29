using LhjMusicPlayer.Common;
using LhjMusicPlayer.Models;
using LhjMusicPlayer.Models.Database;
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
using System.Collections.ObjectModel;
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
    public sealed partial class CommentPage : Page
    {
        private Music? music = MediaInfo.GetMusic(@"C:\Users\haijialiu\Music\神楽七奈 (かぐら なな) - 鼓動.flac");
        private CommentViewModel viewModel;


        public CommentPage()
        {
            viewModel =  App.Current.Services.GetRequiredService<CommentViewModel>();

            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

            viewModel.MusicId = (int)e.Parameter;
            //TODO datacontext可优化
            using var context = new DataContext();
            music = context.Musics.FirstOrDefault(music => music.Id == (int)e.Parameter);
            viewModel.LoadComments();
        }

        private void CommentSubmitButton_Click(object sender, RoutedEventArgs e)
        {
            var comment = CommentTextBox.Text;
            if (comment != "")
            {
                viewModel.SaveComment(comment);
            }
            CommentTextBox.Text = string.Empty;
        }
        private void Remove_Btn_Click(object sender, RoutedEventArgs e)
        {
           
        }
    }
}
