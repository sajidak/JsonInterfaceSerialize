using System;
using System.Collections.Generic;

namespace JsonInterfaceSerialize.Utilities.Helpers
{
    public class ListHelpers<T>
    {
        public static void CopyListItems(IList<T> src, IList<T> dst)
        {
            // Test for nulls and handle
            try
            {
                (dst as List<T>).AddRange(src);
            }
            catch (Exception)
            {
            }
        }
    }
}
