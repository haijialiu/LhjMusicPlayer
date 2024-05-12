using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LhjMusicPlayer.Models
{

    [Comment("评论")]
    [EntityTypeConfiguration(typeof(CommentEntityTypeConfiguration))]
    public class Comment
    {
        [Column(Order = 0)]
        public int Id { get; set; }        
        [Column(Order = 1)]
        public string? Content { get; set; }
        [Column(Order = 2)]
        public string? UserName { get; set; }
        [Column(Order = 3)]
        public DateTime CreatedTime { get; set; }

        public int MusicId { get; set; }
        public Music Music { get; set; } = null!;
    }
    public class CommentEntityTypeConfiguration : IEntityTypeConfiguration<Comment>
    {
        public void Configure(EntityTypeBuilder<Comment> builder)
        {
            builder
                .Property(comment => comment.CreatedTime)
                .HasDefaultValueSql("Datetime(CURRENT_TIMESTAMP,'localtime')");
        }
    }
}
