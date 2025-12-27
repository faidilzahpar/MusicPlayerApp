using System.Windows;

namespace MusicPlayerApp.Views
{
    public partial class CreatePlaylistDialog : Window
    {
        public string PlaylistName { get; private set; }

        public CreatePlaylistDialog()
        {
            InitializeComponent();
            PlaylistNameBox.Focus();
            PlaylistNameBox.SelectAll();
        }

        private void Create_Click(object sender, RoutedEventArgs e)
        {
            PlaylistName = PlaylistNameBox.Text.Trim();
            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
