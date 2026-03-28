using System;
using Wave.Application.In;
using Wave.Infrastructure.In;

namespace Wave.Ui.Pages.ExecutionContent;

public class ExecutionViewModel
{
    private readonly IServerExecutorService serverExecutorService;
    public ExecutionViewModel(IServerExecutorService serverExecutorService)
    {
        this.serverExecutorService = serverExecutorService;
    }
}
