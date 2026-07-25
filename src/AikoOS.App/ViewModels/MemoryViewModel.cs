using System.Collections.ObjectModel;
using AikoOS.Memory.Models;
using AikoOS.Memory.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;

namespace AikoOS.App.ViewModels;

public partial class MemoryViewModel : ObservableObject
{
    private readonly IMemoryService _memoryService;
    private readonly ILogger<MemoryViewModel> _logger;

    public MemoryViewModel(
        IMemoryService memoryService,
        ILogger<MemoryViewModel> logger)
    {
        _memoryService = memoryService;
        _logger = logger;
    }

    public ObservableCollection<MemoryEntry> Memories { get; } = [];

    [ObservableProperty]
    private string _newMemoryContent = string.Empty;

    [ObservableProperty]
    private MemoryEntry? _selectedMemory;

    [ObservableProperty]
    private string _statusMessage =
        "Memory system is ready.";

    [ObservableProperty]
    private bool _isBusy;

    [RelayCommand]
    private async Task LoadAsync()
    {
        if (IsBusy)
        {
            return;
        }

        try
        {
            IsBusy = true;
            StatusMessage = "Loading memories...";

            IReadOnlyList<MemoryEntry> memories =
                await _memoryService.GetAllAsync();

            Memories.Clear();

            foreach (MemoryEntry memory in memories)
            {
                Memories.Add(memory);
            }

            StatusMessage =
                $"{Memories.Count} memory item(s) loaded.";
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Could not load memories.");

            StatusMessage =
                "Could not load memories.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task AddAsync()
    {
        string content = NewMemoryContent.Trim();

        if (string.IsNullOrWhiteSpace(content))
        {
            StatusMessage =
                "Memory content cannot be empty.";

            return;
        }

        try
        {
            IsBusy = true;
            StatusMessage = "Saving memory...";

            MemoryEntry memory =
                await _memoryService.AddAsync(content);

            Memories.Insert(0, memory);

            NewMemoryContent = string.Empty;

            StatusMessage = "Memory saved.";
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Could not save memory.");

            StatusMessage =
                "Could not save memory.";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task DeleteSelectedAsync()
    {
        if (SelectedMemory is null)
        {
            StatusMessage =
                "Select a memory to delete.";

            return;
        }

        MemoryEntry memoryToDelete = SelectedMemory;

        try
        {
            IsBusy = true;
            StatusMessage = "Deleting memory...";

            await _memoryService.DeleteAsync(
                memoryToDelete.Id);

            Memories.Remove(memoryToDelete);
            SelectedMemory = null;

            StatusMessage = "Memory deleted.";
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Could not delete memory.");

            StatusMessage =
                "Could not delete memory.";
        }
        finally
        {
            IsBusy = false;
        }
    }
}