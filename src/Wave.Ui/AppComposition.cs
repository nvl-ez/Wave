using System;
using Wave.Application.Out;
using Wave.Domain;
using Wave.Infrastructure.Out;
using Wave.Ui.ViewModels;

namespace Wave.Ui;

public static class AppComposition
{
    private static INoteRepository? noteRepository;

    public static void Init()
    {
        string appDataDirectory = FileSystem.AppDataDirectory;

        noteRepository = new NoteRepository(appDataDirectory);
    }

    public static NotesViewModel CreateNotesViewModel() => new NotesViewModel(noteRepository);

    public static NoteViewModel CreateNoteViewModel() => new NoteViewModel(noteRepository);
    public static NoteViewModel CreateNoteViewModel(Note note) => new NoteViewModel(noteRepository, note);
}
