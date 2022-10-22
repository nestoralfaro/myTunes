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
        private readonly ObservableCollection<Song> songs;
        public MainWindow()
        {
            InitializeComponent();

            try
            {
                //nuevo comment
                musicRepo = new MusicRepo();
            }
            catch (Exception e)
            {
                MessageBox.Show("Error loading file: " + e.Message, "MiniPlayer", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            }

            playlists = new ObservableCollection<string>(musicRepo?.Playlists);
            playlists.Insert(0, "All Music");
            playlistListBox.ItemsSource = playlists;
            songs = new ObservableCollection<Song>();
            foreach (DataRow row in musicRepo.Songs.AsEnumerable())
            {
                Trace.WriteLine(row["title"]);
                songs.Add(new Song
                {
                    Id = (int)row["id"],
                    Title = row["title"].ToString(),
                    Artist = row["artist"].ToString(),
                    Album = row["album"].ToString(),
                    Genre = row["genre"].ToString(),
                    Length = row["length"].ToString(),
                    Filename = row["filename"].ToString(),
                    AlbumImageUrl = row["albumImage"].ToString(),
                    AboutUrl = row["url"].ToString()
                });
            }
            musicDataGrid.ItemsSource = songs;
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
            var openFileDialog = new Microsoft.Win32.OpenFileDialog
            {
                FileName = "",
                DefaultExt = "*.wma;*.wav;*mp3;*.m4a",
                Filter = "Media files|*.mp3;*.m4a;*.wma;*.wav|MP3 (*.mp3)|*.mp3|M4A (*.m4a)|*.m4a|Windows Media Audio (*.wma)|*.wma|Wave files (*.wav)|*.wav|All files|*.*"
            };

            bool? result = openFileDialog.ShowDialog();
            if (result == true)
            {
                // Selected file is openFileDialog.FileName
                Song newSong = musicRepo.AddSong(openFileDialog.FileName);

                if (newSong != null)
                {
                    songs.Add(newSong);
                    // Select the song just added 
                    musicDataGrid.SelectedIndex = musicDataGrid.Items.Count - 1;
                    // write to the file
                    //musicRepo.Save();
                } 
            }
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

        private void moreInfoUrl_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            // Exclusively for .Net6?
            Process.Start(new ProcessStartInfo { FileName=e.Uri.AbsoluteUri, UseShellExecute=true});
        }
    }
}
