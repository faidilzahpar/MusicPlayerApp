using MusicPlayerApp.Models;
using SQLite;
using System.IO;

namespace MusicPlayerApp.Services
{
    public class DatabaseService
    {
        private SQLiteConnection _db;

        public DatabaseService(string path)
        {
            bool firstTimeCreate = !File.Exists(path);

            // Buat folder jika belum ada
            Directory.CreateDirectory(Path.GetDirectoryName(path));

            // Hubungkan ke database yang SUDAH ADA, atau buat baru jika belum ada
            _db = new SQLiteConnection(path);

            // Jika database BARU dibuat → baru buat tabel
            if (firstTimeCreate)
            {
                _db.CreateTable<Song>();
            }
        }

        public void InsertSong(Song song)
        {
            _db.Insert(song);
        }

        public List<Song> GetAllSongs()
        {
            return _db.Table<Song>().ToList();
        }

        public void UpdateSong(Song song)
        {
            _db.Update(song);
        }

        public void DeleteSong(int id)
        {
            _db.Delete<Song>(id);
        }
    }
}
