namespace Common
{
    public interface ISliceInfo
    {
        int StartIndex { get; }
        
        int Limit { get; }
        
        int TotalElementCount { get; }
    }
}