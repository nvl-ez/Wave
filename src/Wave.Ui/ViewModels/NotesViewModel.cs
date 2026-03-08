using System.Collections.ObjectModel;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using Wave.Application.Out;

namespace Wave.Ui.ViewModels;

public class NotesViewModel : IQueryAttributable
{
    public ObservableCollection<NoteViewModel> AllNotes { get; }
    public ICommand NewCommand { get; }
    public ICommand SelectNoteCommand { get; }

    private INoteRepository noteRepository;

    public NotesViewModel(INoteRepository noteRepository)
    {
        this.noteRepository = noteRepository;

        AllNotes = new ObservableCollection<NoteViewModel>(noteRepository.LoadAll().Select(n => AppComposition.CreateNoteViewModel(n)));
        NewCommand = new AsyncRelayCommand(NewNoteAsync);
        SelectNoteCommand = new AsyncRelayCommand<NoteViewModel>(SelectNoteAsync);
    }

    private async Task NewNoteAsync()
    {
        await Shell.Current.GoToAsync(nameof(NotePage));
    }

    private async Task SelectNoteAsync(NoteViewModel note)
    {
        if (note is not null)
            await Shell.Current.GoToAsync($"{nameof(NotePage)}?load={note.Identifier}");

    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.ContainsKey("deleted"))
        {
            string noteId = query["deleted"].ToString();
            NoteViewModel matchedNote = AllNotes.FirstOrDefault(n => n.Identifier == noteId);

            if (matchedNote is not null)
            {
                AllNotes.Remove(matchedNote);
                Console.WriteLine("DELETED" + matchedNote.Identifier);
            }

        }
        else if (query.ContainsKey("saved"))
        {
            string noteId = query["saved"].ToString();
            NoteViewModel matchedNote = AllNotes.FirstOrDefault(n => n.Identifier == noteId);

            if (matchedNote != null)
            {
                matchedNote.Reload();
                AllNotes.Move(AllNotes.IndexOf(matchedNote), 0);
            }

            else
                AllNotes.Insert(0, AppComposition.CreateNoteViewModel(noteRepository.Load(noteId)));
        }
    }
}
