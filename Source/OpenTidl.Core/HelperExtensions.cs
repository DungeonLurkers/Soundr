using System;
using System.IO;
using System.Threading.Tasks;

namespace OpenTidl.Core
{
    public static class HelperExtensions
    {
        public static byte[] ToArray(this Stream stream, int? timeout = null)
        {
            using (var ms = new MemoryStream())
            {
                var task = stream.CopyToAsync(ms);
                if (timeout.HasValue && !task.Wait(timeout.Value))
                    throw new TimeoutException();
                else if (!timeout.HasValue)
                    task.Wait();
                return ms.ToArray();
            }
        }
        
        /// <summary>
        /// Warning: causes deadlocks when used on UI thread
        /// </summary>
        private static T Sync<T>(this Task<T> task, int? timeout = null)
        {
            if (timeout.HasValue && !task.Wait(timeout.Value))
                throw new TimeoutException();
            /*if (!timeout.HasValue)
                task.Wait();*/
            return task.Result;
        }

        public static T Sync<T>(this Func<Task<T>> task, int? timeout = null)
        {
            return Task.Run(task).Sync(timeout);
        }
    }
}
