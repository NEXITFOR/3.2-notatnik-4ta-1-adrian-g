using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace notatnik;

public partial class MainWindow : Window
{
    private readonly string _filePath;
    private ObservableCollection<Note> _notes;

    public MainWindow()
    {
        InitializeComponent();

        // Plik z notatkami obok pliku wykonywalnego
        _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "notatki.json");

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
            CreatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
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
    /// Wczytuje notatki z pliku JSON.
    /// </summary>
    private ObservableCollection<Note> LoadNotes()
    {
        if (!File.Exists(_filePath))
            return new ObservableCollection<Note>();

        try
        {
            var json = File.ReadAllText(_filePath);
            var notes = JsonSerializer.Deserialize<ObservableCollection<Note>>(json);
            return notes ?? new ObservableCollection<Note>();
        }
        catch
        {
            return new ObservableCollection<Note>();
        }
    }

    /// <summary>
    /// Zapisuje notatki do pliku JSON.
    /// </summary>
    private void SaveNotes()
    {
        var options = new JsonSerializerOptions { WriteIndented = true };
        var json = JsonSerializer.Serialize(_notes, options);
        File.WriteAllText(_filePath, json);
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
}