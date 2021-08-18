using ReactiveUI;

namespace Soundr.Manager.Services
{
    public interface INavigationService
    {
        RoutingState Router { get; }
        void NavigateTo<T>() where T : IRoutableViewModel;
        void NavigateToAndReset<T>() where T : IRoutableViewModel;
        void NavigateBack();
    }
}