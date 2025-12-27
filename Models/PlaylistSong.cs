using SQLite;

namespace MusicPlayerApp.Models
{
    public class PlaylistSong
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [Indexed] // untuk performa loading playlist
        public int PlaylistId { get; set; }

        [Indexed] // untuk performa hapus lagu
        public int SongId { get; set; }

        // Urutan lagu di dalam playlist
        public int OrderIndex { get; set; }
    }
}