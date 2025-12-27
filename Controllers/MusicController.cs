using MusicPlayerApp.Models;
using MusicPlayerApp.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace MusicPlayerApp.Controllers
{
    public class MusicController
    {
        private readonly DatabaseService _db;
        private readonly FileScannerService _scanner = new FileScannerService();
        private readonly AudioPlayerService _player;
        public bool IsPlaying { get; private set; } = false;

        // Debounce dictionary (hindari event berulang)
        private static Dictionary<string, DateTime> _eventTracker = new();
        private static readonly object _lock = new();

        public MusicController(DatabaseService db, AudioPlayerService player)
        {
            _db = db;
            _player = player;
        }

        // Digunakan untuk debounce FileSystemWatcher
        private bool ShouldProcess(string path, int debounceMs = 500)
        {
            lock (_lock)
            {
                if (_eventTracker.TryGetValue(path, out var last))
                {
                    if ((DateTime.Now - last).TotalMilliseconds < debounceMs)
                        return false;
                }

                _eventTracker[path] = DateTime.Now;
                return true;
            }
        }

        // Sinkronisasi awal folder TANPA reset database
        public void SyncInitialFolder(string folder)
        {
            if (!Directory.Exists(folder)) return;

            var filesInFolder = Directory
                .GetFiles(folder, "*.*", SearchOption.AllDirectories)
                .Where(_scanner.IsAudioFile)
                .ToList();

            var dbSongs = _db.GetAllSongs();

            var scannedSongs = filesInFolder
    .Select(f => _scanner.ReadMetadata(f))
    .ToList();

            // 1. INSERT / UPDATE PATH
            foreach (var scanned in scannedSongs)
            {
                var existing = _db.GetBySignature(scanned.Signature);

                if (existing == null)
                {
                    _db.InsertSong(scanned);
                }
                else if (existing.FilePath != scanned.FilePath)
                {
                    _db.UpdateSongPath(existing.Id, scanned.FilePath);
                }
            }

            // 2. DELETE YANG SUDAH HILANG DARI FOLDER
            var existingSignatures = scannedSongs.Select(s => s.Signature).ToHashSet();

            foreach (var song in dbSongs)
            {
                if (!existingSignatures.Contains(song.Signature))
                {
                    _db.DeleteBySignature(song.Signature);
                }
            }

            RefreshUI();
        }

        // FILE ADDED
        public void OnFileAdded(string path)
        {
            try
            {
                if (!ShouldProcess(path)) return;
                if (!_scanner.IsAudioFile(path)) return;
                if (!File.Exists(path)) return;

                // Tunggu sampai file benar-benar selesai ditulis
                Thread.Sleep(300);

                var scanned = _scanner.ReadMetadata(path);
                var existing = _db.GetBySignature(scanned.Signature);

                if (existing == null)
                {
                    _db.InsertSong(scanned);
                }
                else if (existing.FilePath != path)
                {
                    _db.UpdateSongPath(existing.Id, path);
                }


                RefreshUI();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("OnFileAdded: " + ex);
            }
        }

        // FILE REMOVED
        public void OnFileRemoved(string path)
        {
            try
            {
                if (!ShouldProcess(path)) return;

                // Cari lagu di DB berdasarkan path TERAKHIR yang tercatat
                var song = _db.GetAllSongs()
                              .FirstOrDefault(s =>
                                  string.Equals(s.FilePath, path, StringComparison.OrdinalIgnoreCase));

                if (song == null)
                    return;

                // HAPUS BERDASARKAN SIGNATURE (INI KUNCI)
                _db.DeleteBySignature(song.Signature);

                RefreshUI();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("OnFileRemoved: " + ex);
            }
        }

        // FILE RENAMED
        public void OnFileRenamed(string oldPath, string newPath)
        {
            try
            {
                if (!ShouldProcess(newPath)) return;

                var scanned = _scanner.ReadMetadata(newPath);
                var existing = _db.GetBySignature(scanned.Signature);

                if (existing == null) return;

                existing.FilePath = newPath;
                existing.Title = scanned.Title;
                existing.Artist = scanned.Artist;
                existing.Duration = scanned.Duration;

                _db.UpdateSong(existing);

                RefreshUI();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("OnFileRenamed: " + ex);
            }
        }

        // FILE CHANGED (METADATA UPDATE)
        public void OnFileChanged(string path)
        {
            try
            {
                // Debounce lebih lama karena metadata editor memicu banyak event
                if (!ShouldProcess(path, 800)) return;
                if (!File.Exists(path)) return;

                // Tunggu metadata benar-benar stabil
                Thread.Sleep(500);

                var updated = _scanner.ReadMetadata(path);
                var song = _db.GetBySignature(updated.Signature);

                if (song == null) return;

                if (song.Title == updated.Title &&
                    song.Artist == updated.Artist &&
                    song.Duration == updated.Duration)
                    return;

                song.Title = updated.Title;
                song.Artist = updated.Artist;
                song.Duration = updated.Duration;

                _db.UpdateSong(song);
                RefreshUI();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("OnFileChanged: " + ex);
            }
        }

        // GET ALL SONGS
        public List<Song> GetAllSongs()
        {
            return _db.GetAllSongs();
        }

        // AUDIO CONTROL

        // Memutar lagu baru dari awal
        public void PlaySong(Song song)
        {
            _player.Play(song.FilePath);
            IsPlaying = true;
        }

        // Pause lagu
        public void Pause()
        {
            if (!IsPlaying) return;

            _player.Pause();
            IsPlaying = false;
        }

        // Resume lagu
        public void Resume()
        {
            if (IsPlaying) return;

            _player.Resume();
            IsPlaying = true;
        }

        // Stop total
        public void StopSong()
        {
            _player.Stop();
            IsPlaying = false;
        }

        // REAL-TIME UI REFRESH
        private void RefreshUI()
        {
            // 1. Cek apakah referensi ke MainUI ada (Aman dicek dari thread manapun)
            if (App.MainUI == null)
                return;

            // 2. JANGAN CEK 'IsLoaded' DI SINI. 
            // Mengakses properti UI (App.MainUI.IsLoaded) dari background thread menyebabkan CRASH.

            // 3. Masuk ke UI Thread dulu menggunakan Dispatcher
            try
            {
                App.MainUI.Dispatcher.InvokeAsync(() =>
                {
                    // 4. Sekarang kita sudah aman berada di UI Thread
                    // Baru boleh cek apakah window sudah dimuat
                    if (App.MainUI.IsLoaded)
                    {
                        App.MainUI.ReloadSongList();
                    }
                });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Gagal Dispatch ke UI: " + ex.Message);
            }
        }

        public List<Song> GetSongsByActiveFolder()
        {
            if (string.IsNullOrWhiteSpace(App.CurrentMusicFolder))
                return _db.GetAllSongs();

            string root = App.CurrentMusicFolder;

            return _db.GetAllSongs()
                      .Where(s => s.FilePath.StartsWith(root, StringComparison.OrdinalIgnoreCase))
                      .ToList();
        }
    }
}
