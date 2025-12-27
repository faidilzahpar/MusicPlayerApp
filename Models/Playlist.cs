using SQLite;
using System;

namespace MusicPlayerApp.Models
{
    public class Playlist
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}