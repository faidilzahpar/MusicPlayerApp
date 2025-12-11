using MusicPlayerApp.Models;
using MusicPlayerApp.Services;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MusicPlayerApp.Controllers
{
    public class MusicController
    {
        private readonly DatabaseService _db;
        private readonly AudioPlayerService _player;

        public MusicController(DatabaseService db, AudioPlayerService player)
        {
            _db = db;
            _player = player;
        }

        // Ambil semua lagu dari database
        public List<Song> GetAllSongs()
        {
            return _db.GetAllSongs();
        }

        // Tambah lagu ke database
        public void AddSong(Song song)
        {
            _db.InsertSong(song);
        }

        // Play lagu
        public void PlaySong(Song song)
        {
            _player.Play(song.FilePath);
        }

        // Stop lagu
        public void Stop()
        {
            _player.Stop();
        }

        public void RemoveMissingFiles()
        {
            var songs = _db.GetAllSongs();

            foreach (var s in songs)
                if (!File.Exists(s.FilePath))
                    _db.DeleteSong(s.Id);
        }

        public void RefreshMetadata()
        {
            var songs = _db.GetAllSongs();
            var scanner = new FileScannerService();

            foreach (var song in songs)
            {
                if (!File.Exists(song.FilePath))
                    continue;

                var updated = scanner.ReadMetadata(song.FilePath);

                song.Title = updated.Title;
                song.Artist = updated.Artist;
                song.Duration = updated.Duration;

                _db.UpdateSong(song);
            }
        }

        public void ImportSongsFromFolder(string folderPath)
        {
            var scanner = new FileScannerService();
            var existingSongs = _db.GetAllSongs();
            var files = Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                if (!scanner.IsAudioFile(file)) continue;

                var existing = existingSongs.FirstOrDefault(s => s.FilePath == file);

                if (existing == null)
                {
                    // File baru → masukkan
                    var metadata = scanner.ReadMetadata(file);
                    _db.InsertSong(metadata);
                }
                else
                {
                    // File lama → perbarui metadata
                    var metadata = scanner.ReadMetadata(file);

                    existing.Title = metadata.Title;
                    existing.Artist = metadata.Artist;
                    existing.Duration = metadata.Duration;

                    _db.UpdateSong(existing);
                }
            }
        }


    }
}
