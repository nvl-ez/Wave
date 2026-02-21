using System;
using System.Diagnostics;
using Wave.Application.Out;
using Wave.Domain;

namespace Wave.Infrastructure.Out;

public class NoteRepository : INoteRepository
{
    private readonly string appDataDirectory;

    public NoteRepository(string storageDirectory)
    {
        appDataDirectory = storageDirectory;
    }

    public void Delete(string filename)
    {
        File.Delete(Path.Combine(appDataDirectory, filename));
    }

    public Note Load(string filename)
    {
        string filepath = Path.Combine(appDataDirectory, filename);

        if (!File.Exists(filepath))
            throw new FileNotFoundException("Unable to find file in local storage.", filepath);

        return new Note()
        {
            Filename = Path.GetFileName(filepath),
            Text = File.ReadAllText(filepath),
            Date = File.GetCreationTime(filepath)
        };
    }

    public IEnumerable<Note> LoadAll()
    {
        return Directory
        .EnumerateFiles(appDataDirectory, "*.notes.txt")
        .Select(filepath => Load(Path.GetFileName(filepath)))
        .OrderBy(note => note.Date);
    }

    public void Save(Note note)
    {
        File.WriteAllText(Path.Combine(appDataDirectory, note.Filename), note.Text);
    }
}
