using Microsoft.WindowsAzure.MobileServices;

namespace Rocket.Surgery.Azure.Sync
{
    public static class ExceptionExtensions
    {
        /// <summary>
        /// Converts the <see cref="MobileServiceInvalidOperationException"/> to an <see cref="ApiSyncException"/>.
        /// </summary>
        /// <param name="exception">The exception.</param>
        /// <returns></returns>
        public static ApiSyncException ToApiException(this MobileServiceInvalidOperationException exception) =>
            new ApiSyncException(exception.Message, exception, exception.Request, exception.Response);
    }
}