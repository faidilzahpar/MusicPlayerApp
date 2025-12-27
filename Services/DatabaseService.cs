using MusicPlayerApp.Models;
using SQLite;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MusicPlayerApp.Services
{
    public class DatabaseService
    {
        private SQLiteConnection _db;

        public DatabaseService(string path)
        {
            bool firstTimeCreate = !File.Exists(path);

            Directory.CreateDirectory(Path.GetDirectoryName(path));

            _db = new SQLiteConnection(path);

            if (firstTimeCreate)
            {
                _db.CreateTable<Song>();
                _db.CreateTable<Playlist>();
                _db.CreateTable<PlaylistSong>();
            }
        }

        // RESET DATABASE (dipakai saat user ganti folder)
        public void Reset()
        {
            _db.DropTable<PlaylistSong>();
            _db.DropTable<Playlist>();
            _db.DropTable<Song>();

            _db.CreateTable<Song>();
            _db.CreateTable<Playlist>();
            _db.CreateTable<PlaylistSong>();
        }

        // =====================
        // SONG
        // =====================

        public void InsertSong(Song song) => _db.Insert(song);

        public List<Song> GetAllSongs() => _db.Table<Song>().ToList();

        public void UpdateSong(Song song) => _db.Update(song);

        public void DeleteSong(int id) => _db.Delete<Song>(id);

        public Song? GetByPath(string path)
        {
            return _db.Table<Song>().FirstOrDefault(s => s.FilePath == path);
        }

        public void DeleteBySignature(string signature)
        {
            var existing = GetBySignature(signature);
            if (existing == null) return;

            _db.Table<PlaylistSong>()
               .Where(ps => ps.SongId == existing.Id)
               .ToList()
               .ForEach(ps => _db.Delete(ps));

            _db.Delete(existing);
        }

        public void UpdateSongPath(int songId, string newPath)
        {
            var song = _db.Find<Song>(songId);
            if (song == null) return;

            song.FilePath = newPath;
            _db.Update(song);
        }

        public Song? GetBySignature(string signature)
        {
            return _db.Table<Song>()
                      .FirstOrDefault(s => s.Signature == signature);
        }

        // PLAYLIST
        public void InsertPlaylist(Playlist playlist)
        {
            _db.Insert(playlist);
        }

        public void UpdatePlaylist(Playlist playlist)
        {
            _db.Update(playlist);
        }

        public void DeletePlaylist(int playlistId)
        {
            // Hapus relasi lagu dulu
            _db.Table<PlaylistSong>()
               .Where(p => p.PlaylistId == playlistId)
               .ToList()
               .ForEach(p => _db.Delete(p));

            // Hapus playlist
            _db.Delete<Playlist>(playlistId);
        }

        public List<Playlist> GetAllPlaylists()
        {
            return _db.Table<Playlist>()
                      .OrderBy(p => p.CreatedAt)
                      .ToList();
        }

        public Playlist? GetPlaylistById(int id)
        {
            return _db.Table<Playlist>().FirstOrDefault(p => p.Id == id);
        }

        // PLAYLIST SONG
        public void InsertPlaylistSong(PlaylistSong item)
        {
            _db.Insert(item);
        }

        public void RemoveSongFromPlaylist(int playlistId, int songId)
        {
            var item = _db.Table<PlaylistSong>()
                          .FirstOrDefault(p =>
                              p.PlaylistId == playlistId &&
                              p.SongId == songId);

            if (item != null)
                _db.Delete(item);
        }

        public int GetNextOrderIndex(int playlistId)
        {
            var last = _db.Table<PlaylistSong>()
                          .Where(p => p.PlaylistId == playlistId)
                          .OrderByDescending(p => p.OrderIndex)
                          .FirstOrDefault();

            return last == null ? 0 : last.OrderIndex + 1;
        }

        public List<Song> GetSongsByPlaylist(int playlistId)
        {
            var query =
                from ps in _db.Table<PlaylistSong>()
                join s in _db.Table<Song>()
                    on ps.SongId equals s.Id
                where ps.PlaylistId == playlistId
                orderby ps.OrderIndex
                select s;

            return query.ToList();
        }
    }
}
