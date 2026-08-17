using System.Collections.Generic;

namespace Message.Encoder.Extensions
{
    internal static class ListExtensions
    {
        public static void AddRange<TObject>(this List<TObject> @this, params object[] args)
        {
            foreach (TObject[] arg in args)
            {
                @this.AddRange(arg);
            }
        }
    }
}
