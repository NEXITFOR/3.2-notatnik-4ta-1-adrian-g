using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
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

        // Plik notatki.md w folderze projektu (cofamy się z bin/Debug/net10.0)
        var baseDir = AppContext.BaseDirectory;
        var projectDir = Path.GetFullPath(Path.Combine(baseDir, "..", "..", ".."));
        _filePath = Path.Combine(projectDir, "notatki.md");

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
            CreatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm")
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
    /// Wczytuje notatki z pliku notatki.md.
    /// Format wpisu:
    /// ## 2026-09-08 14:32 — Tytuł notatki
    ///
    /// treść notatki
    /// </summary>
    private ObservableCollection<Note> LoadNotes()
    {
        if (!File.Exists(_filePath))
            return new ObservableCollection<Note>();

        try
        {
            var text = File.ReadAllText(_filePath, Encoding.UTF8);
            var notes = new ObservableCollection<Note>();

            // Dzielimy po nagłówkach ## 
            var parts = text.Split(new[] { "\n## ", "\r\n## " }, StringSplitOptions.None);

            for (int i = 0; i < parts.Length; i++)
            {
                var part = parts[i].Trim();
                if (string.IsNullOrEmpty(part))
                    continue;

                // Pierwszy element może zaczynać się od "## " jeśli plik zaczyna się od nagłówka
                if (i == 0 && part.StartsWith("## "))
                    part = part.Substring(3);

                // Pierwsza linia to "data — tytuł", reszta to treść
                var firstNewLine = part.IndexOfAny(new[] { '\r', '\n' });
                string headerLine;
                string content;

                if (firstNewLine >= 0)
                {
                    headerLine = part.Substring(0, firstNewLine).Trim();
                    content = part.Substring(firstNewLine).Trim();
                }
                else
                {
                    headerLine = part.Trim();
                    content = "";
                }

                // Parsujemy nagłówek: "2026-09-08 14:32 — Tytuł"
                string createdAt;
                string title;

                var dashIndex = headerLine.IndexOf(" — ");
                if (dashIndex >= 0)
                {
                    createdAt = headerLine.Substring(0, dashIndex).Trim();
                    title = headerLine.Substring(dashIndex + 3).Trim();
                }
                else
                {
                    createdAt = "";
                    title = headerLine;
                }

                notes.Add(new Note
                {
                    Id = Guid.NewGuid().ToString(),
                    Title = title,
                    Content = content,
                    CreatedAt = createdAt
                });
            }

            return notes;
        }
        catch
        {
            return new ObservableCollection<Note>();
        }
    }

    /// <summary>
    /// Zapisuje wszystkie notatki do pliku notatki.md.
    /// Najnowsza notatka na górze. Polskie znaki zachowane (UTF-8).
    /// </summary>
    private void SaveNotes()
    {
        var sb = new StringBuilder();

        for (int i = 0; i < _notes.Count; i++)
        {
            var note = _notes[i];
            sb.AppendLine($"## {note.CreatedAt} — {note.Title}");
            sb.AppendLine();
            sb.AppendLine(note.Content);

            if (i < _notes.Count - 1)
                sb.AppendLine();
        }

        File.WriteAllText(_filePath, sb.ToString(), Encoding.UTF8);
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