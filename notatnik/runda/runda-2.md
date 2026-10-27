using System.Collections.Generic;
using System.Text;
        // Plik notatki.md w folderze projektu (cofamy się z bin/Debug/net10.0)
        var baseDir = AppContext.BaseDirectory;
        var projectDir = Path.GetFullPath(Path.Combine(baseDir, "..", "..", ".."));
        _filePath = Path.Combine(projectDir, "notatki.md");
            CreatedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm")
    /// Wczytuje notatki z pliku notatki.md.
    /// Format wpisu:
    /// ## 2026-09-08 14:32 — Tytuł notatki
    ///
    /// treść notatki
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
    /// Zapisuje wszystkie notatki do pliku notatki.md.
    /// Najnowsza notatka na górze. Polskie znaki zachowane (UTF-8).
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