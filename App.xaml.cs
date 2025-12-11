using MusicPlayerApp.Services;
using MusicPlayerApp.Controllers;
using MusicPlayerApp.Views;
using System.IO;
using System.Windows;

namespace MusicPlayerApp
{
    public partial class App : Application
    {
        public static DatabaseService Db { get; private set; }
        public static AudioPlayerService Player { get; private set; }
        public static MusicController Music { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // 1. Tentukan lokasi database
            string folder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "MusicPlayerApp"
            );

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            string dbPath = Path.Combine(folder, "musicplayer.db");

            // 2. Inisialisasi services
            Db = new DatabaseService(dbPath);
            Player = new AudioPlayerService();
            Music = new MusicController(Db, Player);

            // 3. Sync database dengan folder saat startup
            Music.RemoveMissingFiles();
            Music.RefreshMetadata();

            // 4. Tampilkan aplikasi
            new MainWindow().Show();
        }
    }
}
