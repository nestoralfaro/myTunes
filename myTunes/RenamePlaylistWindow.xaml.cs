﻿using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace myTunes
{
    /// <summary>
    /// Interaction logic for RenamePlaylistWindow.xaml
    /// </summary>
    public partial class RenamePlaylistWindow : Window
    {
        public RenamePlaylistWindow()
        {
            InitializeComponent();
        }

        private void cancelNewPlaylistNameButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void OkNewPlaylistNameButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
