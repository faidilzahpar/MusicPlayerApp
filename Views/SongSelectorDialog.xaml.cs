using MusicPlayerApp.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MusicPlayerApp.Views
{
    public partial class SongSelectorDialog : Window
    {
        private List<Song> _originalList;
        public List<Song> SelectedSongs { get; private set; } = new List<Song>();

        // Constructor menerima daftar semua lagu yang ada di Library
        public SongSelectorDialog(List<Song> allSongs)
        {
            InitializeComponent();
            _originalList = allSongs;

            // Tampilkan semua lagu di awal
            SongsList.ItemsSource = _originalList;
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string query = SearchBox.Text.ToLower();

            if (string.IsNullOrWhiteSpace(query))
            {
                SongsList.ItemsSource = _originalList;
            }
            else
            {
                // Filter list berdasarkan pencarian
                var filtered = _originalList
                    .Where(s => s.Title.ToLower().Contains(query) ||
                                s.Artist.ToLower().Contains(query))
                    .ToList();
                SongsList.ItemsSource = filtered;
            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            // Ambil semua item yang dipilih (SelectionMode=Extended)
            foreach (Song song in SongsList.SelectedItems)
            {
                SelectedSongs.Add(song);
            }

            DialogResult = true; // Tutup dialog dengan status OK
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}