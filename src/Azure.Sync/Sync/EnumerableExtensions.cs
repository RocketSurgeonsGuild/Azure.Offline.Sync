using System;
using System.Collections.Generic;
using System.Linq;

namespace Rocket.Surgery.Azure
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<IEnumerable<T>> Split<T>(this IEnumerable<T> source, int maxLength)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (maxLength <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxLength));
            }

            var enumerator = source.GetEnumerator();
            var batch = new List<T>(maxLength);

            while (enumerator.MoveNext())
            {
                batch.Add(enumerator.Current);

                //Have we finished this batch? Yield it and start a new one.
                if (batch.Count == maxLength)
                {
                    yield return batch;
                    batch = new List<T>(maxLength);
                }
            }

            //Yield the final batch if it has any elements
            if (batch.Any())
            {
                yield return batch;
            }
        }
    }
}