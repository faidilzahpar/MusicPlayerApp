using MusicPlayerApp.Controllers;
using System.Windows;
using System.Windows.Forms;   
using MessageBox = System.Windows.MessageBox; 

namespace MusicPlayerApp.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
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
                }
            }
        }
    }
}
