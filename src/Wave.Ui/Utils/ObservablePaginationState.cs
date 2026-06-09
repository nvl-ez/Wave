using System;
using System.ComponentModel;
using Wave.Domain.Utils;

namespace Wave.Ui.Utils;

public record class ObservablePaginationState : INotifyPropertyChanged
{
    public int Index
    {
        get;
        set
        {
            if (value != field)
            {
                field = value;
                OnPropertyChanged(nameof(Index));
            }
        }
    }
    public int PageSize
    {
        get;
        set
        {
            if (value != field)
            {
                field = value;
                OnPropertyChanged(nameof(PageSize));
            }
        }
    }
    public int ResultCount
    {
        get;
        set
        {
            if (value != field)
            {
                field = value;
                OnPropertyChanged(nameof(ResultCount));
            }
        }
    }
    public int TotalCount
    {
        get;
        set
        {
            if (value != field)
            {
                field = value;
                OnPropertyChanged(nameof(TotalCount));
            }
        }
    }
    public event PropertyChangedEventHandler? PropertyChanged;

    public ObservablePaginationState(PaginationState paginationState)
    {
        Index = paginationState.Index;
        PageSize = paginationState.PageSize;
        ResultCount = paginationState.ResultCount;
        TotalCount = paginationState.TotalCount;
    }

    private void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public PaginationState AsPaginationState()
    {
        return new()
        {
            Index = Index,
            PageSize = PageSize,
            ResultCount = ResultCount,
            TotalCount = TotalCount
        };
    }

    public void Apply(PaginationState paginationState)
    {
        Index = paginationState.Index;
        PageSize = paginationState.PageSize;
        ResultCount = paginationState.ResultCount;
        TotalCount = paginationState.TotalCount;
    }
}
