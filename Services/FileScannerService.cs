using MusicPlayerApp.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TagLib;

namespace MusicPlayerApp.Services
{
    public class FileScannerService
    {
        private readonly string[] AllowedExtensions = { ".mp3", ".wav", ".flac", ".aac" };

        public bool IsAudioFile(string path)
        {
            var ext = Path.GetExtension(path).ToLower();
            return AllowedExtensions.Contains(ext);
        }

        // Scan folder dan kembalikan semua lagu
        public List<Song> ScanFolder(string folderPath)
        {
            var songs = new List<Song>();

            foreach (var file in Directory.GetFiles(folderPath, "*.*", SearchOption.AllDirectories))
            {
                if (!IsAudioFile(file))
                    continue;

                songs.Add(ReadMetadata(file));
            }

            return songs;
        }

        // Baca metadata 1 file
        public Song ReadMetadata(string filePath)
        {
            try
            {
                var tfile = TagLib.File.Create(filePath);

                string title = tfile.Tag.Title ?? Path.GetFileNameWithoutExtension(filePath);

                // Ambil artist dari berbagai kemungkinan field
                string artist =
                    tfile.Tag.FirstAlbumArtist ??
                    tfile.Tag.FirstArtist ??
                    tfile.Tag.Performers?.FirstOrDefault() ??
                    tfile.Tag.Artists?.FirstOrDefault() ??
                    "Unknown";

                double duration = tfile.Properties.Duration.TotalSeconds;

                return new Song
                {
                    Title = title,
                    Artist = artist,
                    Duration = duration,
                    FilePath = filePath
                };
            }
            catch
            {
                return new Song
                {
                    Title = Path.GetFileNameWithoutExtension(filePath),
                    Artist = "Unknown",
                    Duration = 0,
                    FilePath = filePath
                };
            }
        }
    }
}
