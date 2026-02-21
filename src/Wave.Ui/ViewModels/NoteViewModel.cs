using System;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Wave.Infrastructure.Out;
using Wave.Application.Out;

namespace Wave.Ui.ViewModels;

public class NoteViewModel : ObservableObject, IQueryAttributable
{
    private Domain.Note note;

    public string Text
    {
        get => note.Text;
        set
        {
            if (note.Text != value)
            {
                note.Text = value;
                OnPropertyChanged();
            }
        }
    }

    public DateTime Date => note.Date;

    public string Identifier => note.Filename;

    public ICommand SaveCommand { get; private set; }
    public ICommand DeleteCommand { get; private set; }

    private INoteRepository noteRepository;

    public NoteViewModel(INoteRepository noteRepository)
    {
        note = new Domain.Note();
        SaveCommand = new AsyncRelayCommand(Save);
        DeleteCommand = new AsyncRelayCommand(Delete);

        this.noteRepository = noteRepository;
    }

    public NoteViewModel(INoteRepository noteRepository, Domain.Note note)
    {
        this.note = note;
        SaveCommand = new AsyncRelayCommand(Save);
        DeleteCommand = new AsyncRelayCommand(Delete);

        this.noteRepository = noteRepository;
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.ContainsKey("load"))
        {
            note = noteRepository.Load(query["load"].ToString());
            RefreshProperties();
        }
    }

    public void Reload()
    {
        note = noteRepository.Load(note.Filename);
        RefreshProperties();
    }

    public void RefreshProperties()
    {
        OnPropertyChanged(nameof(Text));
        OnPropertyChanged(nameof(Date));
    }

    private async Task Save()
    {
        noteRepository.Save(note);
        await Shell.Current.GoToAsync($"..?saved={note.Filename}");
    }

    private async Task Delete()
    {
        noteRepository.Delete(note.Filename);
        await Shell.Current.GoToAsync($"..?deleted={note.Filename}");
    }
}
