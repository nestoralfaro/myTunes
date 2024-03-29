# myTunes

## 1. Brief summary of what the program does
myTunes is a WPF music player where a user is able to add, remove, and play songs, as well as create, rename, and remove playlists from the player. The XML Schema Definition (music.xsd) and the Extensible Markup Language (music.xml) contain the data that the player will load and use as its model. Currently the supported formats are the following media files: `mp3, m4a, wav, and wma`.

## 2. An indication of whether you met all the requirements of the Procrastination Penalty on time
Yes we did. We had a working DataGrid and ListBox displaying the appropriate data by the due date as required.

## 3. List of any additional requirements that were implemented. Each of the three additional requirements were implemented
1. Rename and delete a playlist.
2. Play and Stop buttons styled with control templates
3. Search bar, which allows the user to search a song from the data grid.

## 4. List what each teammate contributed (e.g., code to transform images, populate the list view, etc.)

| Task | Pts | Responsible |
| ----------- | ----------- |-----------|
|**Show songs from All music and playlists.** List Box lists "All Music" and playlists, and clicking an option shows the appropriate songs in the data grid.| 6 |Nestor
|**Songs in data grid.** Data grid shows each song's title , artist, album, and genre only. Song info is editable when showing All Music but not editable when showing playlist.| 6 |Nestor
|**Play and stop song.** Song can be played by selecting song and pressing Play button or choosing Play from context menu. Stop button stops playing.| 6 |Nestor
|**Toolbar.** Toolbar has options to add a song, add a new playlist, and show About dialog box. All options have hints.| 5 |Anny
|**Add songs**. Songs can be added with open dialog box. Dialog uses filter to show audio files by default. After adding, song is selected in data grid.| 10 |Nestor
|**Create playlist and add songs**. Playlist created by clicking New Playlist button from toolbar and is prompted for playlist name in dialog box. Songs can be dragged onto a playlist but not All Music.| 20 | Both
|**Delete songs**. Song deleted from All Music by selecting "Remove" from context menu; dialog box confirms removal. Song removed from playlist by selecting "Remove from Playlist" from context menu; no confirmation dialog. | 8 |Nestor
|**Changes saved.** All changes to song data and playlists saved in music.xml when app terminates.| 4 |Anny
|**Windows resizable.** Windows can be resized, and contents expand to fill window.| 5 |Anny
|**About dialog box.** About dialog box identifies programmers.| 5 |Anny
|**Use of MusicRepo.** Program uses MusicRepo for the Model.| 5 |Both
|**Rename and delete playlist.** Playlist renamed by choosing "Rename" from context menu and prompted for new name. Delete playlist by choosing "Delete" from context menu.| 5 |Anny
|**Control template/styles for Play and Stop buttons.** Control template or styles with at least one trigger used to alter look and behavior of Play and Stop buttons. Buttons use Play and Stop commands, disabled when appropriate.| 5 |Anny
|**Search for song.** User can enter search string that shows matches (partial match of title, artist, album, or genre) as string is entered.| 5 |Nestor
|**Git usage.** Repo includes .gitignore, good commit summaries, at least 20 commits, existing bugs posted as issues, both teammates making commits.| 5 |Both
|**Coding practices.** Functions used where appropriate, good variable and function names, and other good programming practices employed.| 5 |Both


## 5. The percentage of work performed by each teammate
Anny: 50%
Nestor: 50%
