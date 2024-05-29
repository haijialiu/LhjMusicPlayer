using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LhjMusicPlayer.Models
{
    [Table("user")]
    [Comment("用户表")]
    public class User
    {
        [Column(Order = 0)]
        public int Id { get; set; }

        [Column(Order = 1), Comment("用户名")]
        public required string Username { get; set; }
    }
}
