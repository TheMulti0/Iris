using System.Collections;
using System.Collections.Generic;

namespace Common
{
    public class Slice<T> : ISliceInfo
    {   
        public IEnumerable<T> Content { get; }
        public int StartIndex { get; }
        public int Limit { get; }
        public int TotalElementCount { get; }

        public Slice(
            IEnumerable<T> content,
            ISliceInfo sliceInfo
        ) : this(
            content,
            sliceInfo.StartIndex,
            sliceInfo.Limit,
            sliceInfo.TotalElementCount)
        {
        }
        
        public Slice(
            IEnumerable<T> content,
            int startIndex,
            int limit,
            int totalElementCount)
        {
            Content = content;
            StartIndex = startIndex;
            Limit = limit;
            TotalElementCount = totalElementCount;
        }
    }
}