using ManagedBass;
using MusicPlayerApp.Controllers;
using MusicPlayerApp.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Threading;
using MessageBox = System.Windows.MessageBox;

namespace MusicPlayerApp.Views
{
    public partial class MainWindow : Window
    {
        private Song _currentSong;
        private bool isPaused = false;
        DispatcherTimer _timer = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();
            Loaded += Window_Loaded;

            _timer.Interval = TimeSpan.FromMilliseconds(500);
            _timer.Tick += UpdateProgress;
        }

        private void ImportSongs_Click(object sender, RoutedEventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.Description = "Pilih folder musik";

                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    App.Music.ImportSongsFromFolder(dialog.SelectedPath);
                    MessageBox.Show("Lagu berhasil diimport dari:\n" + dialog.SelectedPath);

                    LoadSongs(); // Refresh list
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            LoadSongs();
        }

        private void LoadSongs()
        {
            var songs = App.Music.GetAllSongs();
            NewPlayedList.ItemsSource = songs;
        }

        private void NewPlayedList_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (NewPlayedList.SelectedItem is Song song)
            {
                _currentSong = song;
                App.Music.PlaySong(song);
                _timer.Start();

                CurrentSongTitle.Text = song.Title;
                CurrentSongArtist.Text = song.Artist;
            }
        }

        private void PlayPause_Click(object sender, RoutedEventArgs e)
        {
            if (_currentSong == null) return;

            if (isPaused)
            {
                App.Player.Resume();
                isPaused = false;
            }
            else
            {
                App.Player.Pause();
                isPaused = true;
            }
        }

        private void Next_Click(object sender, RoutedEventArgs e)
        {
            var songs = App.Music.GetAllSongs();
            int index = songs.IndexOf(_currentSong);

            if (index < songs.Count - 1)
            {
                _currentSong = songs[index + 1];
                App.Music.PlaySong(_currentSong);

                CurrentSongTitle.Text = _currentSong.Title;
                CurrentSongArtist.Text = _currentSong.Artist;
            }
        } 

        private void Prev_Click(object sender, RoutedEventArgs e)
        {
            var songs = App.Music.GetAllSongs();
            int index = songs.IndexOf(_currentSong);

            if (index > 0)
            {
                _currentSong = songs[index - 1];
                App.Music.PlaySong(_currentSong);

                CurrentSongTitle.Text = _currentSong.Title;
                CurrentSongArtist.Text = _currentSong.Artist;
            }
        }

        private void UpdateProgress(object sender, EventArgs e)
        {
            if (_currentSong == null)
                return;

            int handle = App.Player.StreamHandle;
            if (handle == 0)
                return;

            var state = Bass.ChannelIsActive(handle);

            // Jika stream sedang berhenti atau buffer habis → jangan update
            if (state == PlaybackState.Stopped || state == PlaybackState.Stalled)
                return;

            long posBytes = Bass.ChannelGetPosition(handle);
            long lenBytes = Bass.ChannelGetLength(handle);

            if (lenBytes <= 0)
                return;

            double posSec = Bass.ChannelBytes2Seconds(handle, posBytes);
            double lenSec = Bass.ChannelBytes2Seconds(handle, lenBytes);

            ProgressSlider.Maximum = lenSec;
            ProgressSlider.Value = posSec;

            CurrentTimeText.Text = TimeSpan.FromSeconds(posSec).ToString(@"mm\:ss");
            TotalTimeText.Text = TimeSpan.FromSeconds(lenSec).ToString(@"mm\:ss");
        }
    }
}
