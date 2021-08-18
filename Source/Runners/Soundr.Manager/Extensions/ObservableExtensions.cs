using System;
using System.Reactive.Linq;

namespace Soundr.Manager.Extensions
{
    public static class ObservableExtensions
    {
        public static IObservable<string> WhereNotNullOrEmpty(this IObservable<string> o) => o.Where(s => !string.IsNullOrEmpty(s));
        public static IObservable<string> WhereNotNullOrWhiteSpace(this IObservable<string> o) => o.Where(s => !string.IsNullOrWhiteSpace(s));
    }
}