using System;
using Wave.Domain;

namespace Wave.Application.Out;

public interface INoteRepository
{
    public Note Load(string filename);
    public void Delete(string filename);
    public void Save(Note note);
    public IEnumerable<Note> LoadAll();
}
