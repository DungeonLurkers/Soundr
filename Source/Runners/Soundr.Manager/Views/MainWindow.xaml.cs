using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using DynamicData.Binding;
using Microsoft.Extensions.Logging;
using ReactiveUI;
using Soundr.Manager.Extensions;
using Soundr.Manager.ViewModels;

namespace Soundr.Manager.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
    {
        public static readonly string PlayScript = "Document..getElementsByTagName('{1}')[0].play()";
        private readonly ILogger<MainWindow> _logger;

        public MainWindow(MainWindowViewModel viewModel, ILogger<MainWindow> logger)
        {
            _logger = logger;
            InitializeComponent();
            ViewModel = viewModel;

            this.WhenActivated(Block);
        }

        private void Block(CompositeDisposable d)
        {
            if (ViewModel is not { } viewModel) return;
            
            SongId
                .Events().TextChanged
                .Select(_ => SongId.Text)
                .WhereNotNull()
                .BindTo(ViewModel, x => x.SpotifySongId)
                .DisposeWith(d);

            viewModel.WhenValueChanged(x => x.SpotifySongId)
                .WhereNotNull()
                .Select(x => $"https://localhost:5001/SpotifyWebPlayer/{x}")
                .BindTo(this, x => x.SongUri.Text)
                .DisposeWith(d);

            this.LoadButton
                .Events().Click
                .Select(_ => SongUri.Text)
                .Select(s =>
                {
                    Uri.TryCreate(s, UriKind.Absolute, out var uri);
                    return uri;
                })
                .WhereNotNull()
                .BindTo(this, x => x.WebView.Source)
                .DisposeWith(d);

            PlayButton
                .Events().Click
                .Do(_ => _logger.LogInformation("Click!"))
                .Select(async _ => await WebView.ExecuteScriptAsync($"eval('{PlayScript}');"))
                .Subscribe()
                .DisposeWith(d);
        }
    }
}