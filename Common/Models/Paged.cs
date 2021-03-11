using System.Collections;
using System.Collections.Generic;

namespace Common
{
    public class Paged<T> : ISlice
    {   
        public IEnumerable<T> Content { get; }
        public int CurrentPageIndex { get; }
        public int TotalPageCount { get; }
        public int TotalElementCount { get; }

        public Paged(
            IEnumerable<T> content,
            ISlice slice
        ) : this(
            content,
            slice.CurrentPageIndex,
            slice.TotalPageCount,
            slice.TotalElementCount)
        {
        }
        
        public Paged(
            IEnumerable<T> content,
            int currentPageIndex,
            int totalPageCount,
            int totalElementCount)
        {
            Content = content;
            
            CurrentPageIndex = currentPageIndex;
            TotalPageCount = totalPageCount;
            TotalElementCount = totalElementCount;
        }
    }
}