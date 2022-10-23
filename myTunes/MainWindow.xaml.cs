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
        private ObservableCollection<Song> songs;
        //private ObservableCollection<Song> songsOnGrid;
        private Point startPoint;
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
            populateDataGridWithAllSongs();
        }

        private void populateDataGridWithAllSongs()
        {
            songs = new ObservableCollection<Song>();
            foreach (DataRow row in musicRepo.Songs.AsEnumerable())
            {
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

        private void playlistListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var playListName = playlistListBox.SelectedItem;
            if (playListName != null && playListName != "All Music")
            {
                // filter all the songs by ID.
                var songsInPlaylist = musicRepo.SongsForPlaylist((string)playListName).AsEnumerable();
                musicDataGrid.ItemsSource = songs.Where(song => songsInPlaylist.Where(playlistSong => Int32.Parse((string)playlistSong["id"]) == song.Id).Count() > 0);
            } else
            {
                populateDataGridWithAllSongs();
            }
        }

        private void musicDataGrid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            startPoint = e.GetPosition(null);
        }

        private void musicDataGrid_MouseMove(object sender, MouseEventArgs e)
        {
            // Get the current mouse position
            Point mousePos = e.GetPosition(null);
            Vector diff = startPoint - mousePos;

            // Start the drag-drop if mouse has moved far enough
            if (e.LeftButton == MouseButtonState.Pressed &&
                (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                DragDrop.DoDragDrop(musicDataGrid, musicDataGrid.SelectedItem, DragDropEffects.Move);
            }
        }

        private void playlistListBox_Drop(object sender, DragEventArgs e)
        {
            Trace.WriteLine("Drop!");
        }
    }
}
