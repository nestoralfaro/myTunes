using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Collections.ObjectModel;

namespace myTunes
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MusicRepo musicRepo;
        private readonly ObservableCollection<string> playlists;
        private readonly ObservableCollection<DataTable> songs;
        public MainWindow()
        {
            InitializeComponent();

            try
            {
                musicRepo = new MusicRepo();
                // this will display the songs in a more staticky way (they will not update if there is a change)
                // thus we need this into a ObservableCollection
                musicDataGrid.ItemsSource = musicRepo?.Songs.DefaultView;
            }
            catch (Exception e)
            {
                MessageBox.Show("Error loading file: " + e.Message, "MiniPlayer", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }

            playlists = new ObservableCollection<string>(musicRepo?.Playlists);
            playlistListBox.ItemsSource = playlists;

            // get musicRepo.Songs.DefaultView into a ObservableCollection
        }

        private void addPlaylistButton_Click(object sender, RoutedEventArgs e)
        {
            NewPlaylistWindow newPlaylistWindow = new NewPlaylistWindow();
            newPlaylistWindow.ShowDialog();
            string newPlaylistName = newPlaylistWindow.playlistTextBox.Text;
            if (newPlaylistName != "")
            {
                if (musicRepo.AddPlaylist(newPlaylistWindow.playlistTextBox.Text))
                {
                    // Enable for persist state
                    //musicRepo.Save();
                    playlists.Add(newPlaylistWindow.playlistTextBox.Text);
                }
                else
                {
                    //playlist probably already exists
                }

            }
        }

        private void addSongButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void aboutButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private void playButton_Click(object sender, RoutedEventArgs e)
        {
        }

        private void stopButton_Click(object sender, RoutedEventArgs e)
        {
        }
    }
}
