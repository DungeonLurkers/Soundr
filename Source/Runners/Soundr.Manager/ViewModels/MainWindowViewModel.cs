using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading.Tasks;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Soundr.Manager.ViewModels
{
    public class MainWindowViewModel : ReactiveObject
    {
        [Reactive] public string SpotifySongId { get; set; } = "";
        
    }
}