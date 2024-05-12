using LhjMusicPlayer.Models;
using LhjMusicPlayer.Models.Database;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LhjMusicPlayer.ViewModels
{
    public class CommentViewModel
    {
        public int MusicId { get; set; }
        public ObservableCollection<Comment> Comments { get; set; } = [];
        public CommentViewModel() 
        {

        }
        public void LoadComments()
        {
            using var context = new DataContext();
            var comments = context.CommentList;
            Comments.Clear();
            var a = comments.Where(comment => comment.MusicId == MusicId).ToList();
            comments.Where(comment => comment.MusicId == MusicId).ToList().ForEach(comment => Comments.Add(comment));
        }
        public void SaveComment(string comment)
        {
            using var context = new DataContext();
            context.CommentList.Add(new Comment { MusicId = MusicId,Content = comment});
            context.SaveChanges();
            LoadComments();
        }

    }
}
