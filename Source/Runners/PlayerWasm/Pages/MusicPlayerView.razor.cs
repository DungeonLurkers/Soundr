using System;
using System.ComponentModel;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Blazorise;
using DynamicData.Binding;
using Microsoft.Extensions.Logging;
using PlayerWasm.ViewModels;
using ReactiveUI;
using ReactiveUI.Blazor;

namespace PlayerWasm.Pages
{
    public partial class MusicPlayerView
    {
        private readonly ILogger<MusicPlayerView> _logger;
        public MusicPlayerView() : this(ServiceLocator.Get<MusicPlayerViewModel>(), ServiceLocator.Get<ILogger<MusicPlayerView>>()) {}
        public MusicPlayerView(MusicPlayerViewModel viewModel, ILogger<MusicPlayerView> logger)
        {
            _logger = logger;
            ViewModel = viewModel;

            this.WhenActivated(disposable =>
            {
                ViewModel.Playlist.ToObservableChangeSet()
                    .Do(x => StateHasChanged())
                    .Subscribe()
                    .DisposeWith(disposable);

                ViewModel.WhenValueChanged(model => model.PlayIconName)
                    .Do(_ => StateHasChanged())
                    .Subscribe()
                    .DisposeWith(disposable);
            });
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if (firstRender)
            {
                _logger.LogInformation("Loading data on first render");
                ViewModel.LoadData.Execute().Subscribe();
            }
            base.OnAfterRender(firstRender);
        }

        private void Play()
        {
            if (ViewModel.PlayIconName == IconName.PlayCircle)
            {
                ViewModel.Play.Execute().Subscribe();
            }
            else
            {
                ViewModel.Pause.Execute().Subscribe();
            }
        }
    }
}