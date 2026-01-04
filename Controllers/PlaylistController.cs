using MusicPlayerApp.Models;
using MusicPlayerApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MusicPlayerApp.Controllers
{
    public class PlaylistController
    {
        private readonly DatabaseService _db;

        public PlaylistController(DatabaseService db)
        {
            _db = db;
        }

        // Buat playlist baru
        public void CreatePlaylist(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return;

            var playlist = new Playlist
            {
                Name = name
                // CreatedAt otomatis terisi oleh Constructor Model
            };

            _db.InsertPlaylist(playlist);
        }

        // Hapus playlist
        public void DeletePlaylist(int playlistId)
        {
            _db.DeletePlaylist(playlistId);
        }

        // Rename playlist
        public void RenamePlaylist(int playlistId, string newName)
        {
            if (string.IsNullOrWhiteSpace(newName))
                return;

            var playlist = _db.GetPlaylistById(playlistId);
            if (playlist == null) return;

            playlist.Name = newName;
            _db.UpdatePlaylist(playlist);
        }

        // Ambil semua playlist
        public List<Playlist> GetAllPlaylists()
        {
            return _db.GetAllPlaylists();
        }

        // Ambil lagu dalam playlist (sudah terurut dari DatabaseService)
        public List<Song> GetSongsInPlaylist(int playlistId)
        {
            return _db.GetSongsByPlaylist(playlistId);
        }

        // Tambah lagu ke playlist
        public void AddSongToPlaylist(int playlistId, Song song)
        {
            if (song == null) return;

            // Cek apakah lagu sudah ada di playlist ini (Mencegah duplikat)
            var existingSongs = _db.GetSongsByPlaylist(playlistId);
            if (existingSongs.Any(s => s.Id == song.Id))
                return;

            // Ambil nomor urut terakhir + 1
            int nextIndex = _db.GetNextOrderIndex(playlistId);

            var item = new PlaylistSong
            {
                PlaylistId = playlistId,
                SongId = song.Id,
                OrderIndex = nextIndex
            };

            _db.InsertPlaylistSong(item);
        }

        // Hapus lagu dari playlist
        public void RemoveSongFromPlaylist(int playlistId, int songId)
        {
            _db.RemoveSongFromPlaylist(playlistId, songId);
        }

        // Pindahkan urutan lagu
        public void ReorderSong(int playlistId, int oldIndex, int newIndex)
        {
            _db.ReorderPlaylist(playlistId, oldIndex, newIndex);
        }
    }
}