using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PrimeCheque.Services.Interfaces;
using System.Threading.Tasks;
using Microsoft.UI.Dispatching;
using System;

namespace PrimeCheque.ViewModels
{
    public class ActivationViewModel : ObservableObject
    {
        private readonly IActivationService _activationService;
        private readonly DispatcherQueue _dispatcherQueue;

        private bool _isActivating;
        public bool IsActivating
        {
            get => _isActivating;
            set
            {
                if (SetProperty(ref _isActivating, value))
                {
                    OnPropertyChanged(nameof(IsNotActivating));
                }
            }
        }

        public bool IsNotActivating => !IsActivating;

        private string _statusMessage = "This installation is not activated.";
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetProperty(ref _statusMessage, value);
        }

        public IAsyncRelayCommand RequestActivationCommand { get; }

        public Action? OnActivationSucceeded { get; set; }

        public ActivationViewModel(IActivationService activationService)
        {
            _activationService = activationService;
            _dispatcherQueue = DispatcherQueue.GetForCurrentThread();
            RequestActivationCommand = new AsyncRelayCommand(RequestActivationAsync);
        }

        private async Task RequestActivationAsync()
        {
            if (IsActivating) return;

            IsActivating = true;
            StatusMessage = "Requesting activation session...";

            var requestId = await _activationService.RequestActivationAsync();

            if (!string.IsNullOrEmpty(requestId))
            {
                StatusMessage = "Waiting for activation in browser...";
                
                // Poll for activation
                _ = Task.Run(async () =>
                {
                    bool activated = false;
                    for (int i = 0; i < 60; i++) // Poll for 5 minutes (5s intervals)
                    {
                        await Task.Delay(5000);
                        activated = await _activationService.PollActivationStatusAsync(requestId);
                        if (activated)
                        {
                            break;
                        }
                    }

                    _dispatcherQueue.TryEnqueue(() =>
                    {
                        if (activated)
                        {
                            StatusMessage = "Activation successful!";
                            OnActivationSucceeded?.Invoke();
                        }
                        else
                        {
                            StatusMessage = "Activation timed out. Try again.";
                            IsActivating = false;
                        }
                    });
                });
            }
            else
            {
                StatusMessage = "Failed to request activation. Check your connection.";
                IsActivating = false;
            }
        }
    }
}
