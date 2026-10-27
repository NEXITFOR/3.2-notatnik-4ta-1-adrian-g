        xmlns:local="clr-namespace:notatnik"
        mc:Ignorable="d" d:DesignWidth="800" d:DesignHeight="600"
        Title="Notatnik"
        Width="800" Height="600"
        MinWidth="500" MinHeight="400">

    <Grid Margin="20" RowDefinitions="Auto,Auto,*,Auto">

        <!-- Tytuł notatki -->
        <TextBox x:Name="TitleBox"
                 Grid.Row="0"
                 PlaceholderText="Tytuł notatki..."
                 Margin="0,0,0,10"
                 FontSize="16"/>

        <!-- Treść notatki -->
        <TextBox x:Name="ContentBox"
                 Grid.Row="1"
                 PlaceholderText="Treść notatki..."
                 AcceptsReturn="True"
                 TextWrapping="Wrap"
                 Height="150"
                 Margin="0,0,0,10"
                 FontSize="14"/>

        <!-- Lista zapisanych notatek -->
        <Border Grid.Row="2"
                BorderBrush="Gray"
                BorderThickness="1"
                CornerRadius="4"
                Margin="0,0,0,10"
                Padding="5">
            <ScrollViewer>
                <ItemsControl x:Name="NotesList">
                    <ItemsControl.ItemTemplate>
                        <DataTemplate x:DataType="local:Note">
                            <Border BorderBrush="LightGray"
                                    BorderThickness="0,0,0,1"
                                    Padding="8"
                                    Margin="0,0,0,4">
                                <Grid ColumnDefinitions="*,Auto">
                                    <StackPanel Grid.Column="0">
                                        <TextBlock Text="{Binding Title}"
                                                   FontWeight="Bold"
                                                   FontSize="14"/>
                                        <TextBlock Text="{Binding Content}"
                                                   TextTrimming="CharacterEllipsis"
                                                   MaxLines="2"
                                                   Foreground="Gray"
                                                   FontSize="12"
                                                   Margin="0,4,0,0"/>
                                        <TextBlock Text="{Binding CreatedAt}"
                                                   FontSize="10"
                                                   Foreground="DarkGray"
                                                   Margin="0,4,0,0"/>
                                    </StackPanel>
                                    <Button Grid.Column="1"
                                            Content="Usuń"
                                            VerticalAlignment="Center"
                                            Click="DeleteNote_Click"
                                            Tag="{Binding Id}"/>
                                </Grid>
                            </Border>
                        </DataTemplate>
                    </ItemsControl.ItemTemplate>
                </ItemsControl>
            </ScrollViewer>
        </Border>

        <!-- Przycisk zapisu -->
        <Button x:Name="SaveButton"
                Grid.Row="3"
                Content="Zapisz notatkę"
                HorizontalAlignment="Stretch"
                HorizontalContentAlignment="Center"
                FontSize="16"
                Padding="10"
                Click="SaveNote_Click"/>

    </Grid>




using System;
using System.Collections.ObjectModel;
using System.IO;
using Avalonia.Interactivity;
    private readonly string _filePath;
    private ObservableCollection<Note> _notes;

        _notes = LoadNotes();
        NotesList.ItemsSource = _notes;
    }

    /// <summary>
    /// Obsługa kliknięcia przycisku „Zapisz notatkę".
    /// </summary>
    private void SaveNote_Click(object? sender, RoutedEventArgs e)
    {
        var title = TitleBox.Text?.Trim();
        var content = ContentBox.Text?.Trim();

        if (string.IsNullOrEmpty(title) && string.IsNullOrEmpty(content))
            return;

        var note = new Note
        {
            Id = Guid.NewGuid().ToString(),
            Title = string.IsNullOrEmpty(title) ? "(bez tytułu)" : title,
            Content = content ?? "",
                    };

        _notes.Insert(0, note);
        SaveNotes();

        // Wyczyść pola
        TitleBox.Text = "";
        ContentBox.Text = "";
    }

    /// <summary>
    /// Obsługa kliknięcia przycisku „Usuń" przy notatce.
    /// </summary>
    private void DeleteNote_Click(object? sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is string id)
        {
            for (int i = 0; i < _notes.Count; i++)
            {
                if (_notes[i].Id == id)
                {
                    _notes.RemoveAt(i);
                    break;
                }
            }
            SaveNotes();
        }
    }

    /// <summary>
        /// </summary>
    private ObservableCollection<Note> LoadNotes()
    {
        if (!File.Exists(_filePath))
            return new ObservableCollection<Note>();

        try
        {
                    }
        catch
        {
            return new ObservableCollection<Note>();
        }
        
    /// <summary>
        /// </summary>
    private void SaveNotes()
    {
            }
}

/// <summary>
/// Model pojedynczej notatki.
/// </summary>
public class Note
{
    public string Id { get; set; } = "";
    public string Title { get; set; } = "";
    public string Content { get; set; } = "";
    public string CreatedAt { get; set; } = "";