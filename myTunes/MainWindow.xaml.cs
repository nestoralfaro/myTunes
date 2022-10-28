﻿using System;
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
        private readonly MediaPlayer mediaPlayer;
        private readonly ObservableCollection<string> playlists;
        private ObservableCollection<Song> songs;
        //private ObservableCollection<Song> songsOnGrid;
        private Point startPoint;
        public MainWindow()
        {
            InitializeComponent();
            mediaPlayer = new MediaPlayer();
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

            playlists = new ObservableCollection<string>(musicRepo?.Playlists!);
            playlists.Insert(0, "All Music");
            playlistListBox.ItemsSource = playlists;
            // Select the All Music play list
            playlistListBox.SelectedIndex = 0;
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
                    Title = row["title"].ToString()!,
                    Artist = row["artist"].ToString()!,
                    Album = row["album"].ToString()!,
                    Genre = row["genre"].ToString()!,
                    Length = row["length"].ToString()!,
                    Filename = row["filename"].ToString()!,
                    AlbumImageUrl = row["albumImage"].ToString()!,
                    AboutUrl = row["url"].ToString()!
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
            Song? selectedSong = musicDataGrid.SelectedItem as Song;
            if (selectedSong != null)
            {
                mediaPlayer.Open(new Uri(selectedSong.Filename));
                mediaPlayer.Play();
            }
        }

        private void stopButton_Click(object sender, RoutedEventArgs e)
        {
            if (mediaPlayer != null)
            {
                mediaPlayer.Stop();
            }
        }

        private void moreInfoUrl_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            // Exclusively for .Net6?
            Process.Start(new ProcessStartInfo { FileName=e.Uri.AbsoluteUri, UseShellExecute=true});
        }

        private void playlistListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var currentSelectedPlaylist = playlistListBox.SelectedItem;
            if (currentSelectedPlaylist is not null and not (object)"All Music")
            {
                // filter all the songs by ID.
                var songsInPlaylist = musicRepo.SongsForPlaylist((string)currentSelectedPlaylist).AsEnumerable();
                songs = new ObservableCollection<Song>();
                foreach (var song in songsInPlaylist)
                {
                    Song curSong = musicRepo.GetSong(Int32.Parse((string)song["id"]));
                    songs.Add(new Song
                    {
                        Id = curSong.Id,
                        Title = curSong.Title,
                        Artist = curSong.Artist,
                        Album = curSong.Album,
                        Genre = curSong.Genre,
                        Length = curSong.Length,
                        Filename = curSong.Filename,
                        AlbumImageUrl = curSong.AlbumImageUrl,
                        AboutUrl = curSong.AboutUrl,
                    });
                }
                musicDataGrid.ItemsSource = songs;
                removeContextItem.Header = $"Remove from '{currentSelectedPlaylist}' Playlist";
            }
            else
            {
                populateDataGridWithAllSongs();
                removeContextItem.Header = "Remove";
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
            TextBlock textController = (TextBlock)e.OriginalSource;
            Song song = (Song)e.Data.GetData("myTunes.Song");
            int songId = song.Id;
            string playlist = textController.Text;
            musicRepo.AddSongToPlaylist(songId, playlist);
            //musicRepo.Save();
        }

        //private void removeSong (int position, int id, string playlist)
        //{
        //    var songInPlaylist = musicRepo.SongsForPlaylist(playlist)?.AsEnumerable().Where(s => Int32.Parse((string)s["id"]) == selectedSong.Id)?.FirstOrDefault();
        //    if (songInPlaylist != null)
        //    {
        //        musicRepo.RemoveSongFromPlaylist(Int32.Parse((string)songInPlaylist["position"]), Int32.Parse((string)songInPlaylist["id"]), playlist);
        //    }
        //}

        private void removeContextItem_Click(object sender, RoutedEventArgs e)
        {
            Trace.WriteLine("About to remove!");
            var currentSelectedPlaylist = playlistListBox.SelectedItem;
            var allPlaylists = musicRepo.Playlists;
            Trace.WriteLine("What it was before");
            Song selectedSong = (Song)musicDataGrid.SelectedItem;
            musicRepo.PrintAllTables();
            if (currentSelectedPlaylist is not null and not (object)"All Music")
            {
                // Remove the song from the selected playlist
                var songInPlaylist = musicRepo.SongsForPlaylist((string)currentSelectedPlaylist)?.AsEnumerable().Where(s => Int32.Parse((string)s["id"]) == selectedSong.Id)?.FirstOrDefault();
                if (songInPlaylist != null)
                {
                    // Remove song on playlist from disk
                    musicRepo.RemoveSongFromPlaylist(Int32.Parse((string)songInPlaylist["position"]), Int32.Parse((string)songInPlaylist["id"]), (string)currentSelectedPlaylist);
                }
                // Remove song from the UI
                songs.RemoveAt(musicDataGrid.SelectedIndex);
            }
            else
            {
                // Remove from everywhere
                foreach (string playlist in allPlaylists)
                {
                    var songInPlaylist = musicRepo.SongsForPlaylist(playlist)?.AsEnumerable().Where(s => Int32.Parse((string)s["id"]) == selectedSong.Id)?.FirstOrDefault();
                    if (songInPlaylist != null)
                    {
                        // Remove song on playlist from disk
                        musicRepo.RemoveSongFromPlaylist(Int32.Parse((string)songInPlaylist["position"]), Int32.Parse((string)songInPlaylist["id"]), playlist);
                    }
                }
                // Remove song from the main song table (on disk)
                musicRepo.DeleteSong(selectedSong.Id);
                // Remove song from the UI
                songs.Remove(selectedSong);
            }
            musicRepo.PrintAllTables();
            // Write to disk
            //musicRepo.Save();
        }

        private void Label_DragOver(object sender, DragEventArgs e)
        {
            Label? playlist = sender as Label;
            if (playlist != null)
            {
                playlist.AllowDrop = (string)playlist.Content! != "All Music";
            }
        }

        private void searchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            string query = searchBar.Text;
            var currentSongs = new ObservableCollection<Song>(songs);
            if (!string.IsNullOrEmpty(query))
            {
                musicDataGrid.ItemsSource = currentSongs.Where(song => song.Title.Contains(query)
                                                                    || song.Artist.Contains(query)
                                                                    || song.Genre.Contains(query)
                                                                    ).ToList();
            }
            else
            {
                musicDataGrid.ItemsSource = currentSongs;
            }
        }
    }
} 