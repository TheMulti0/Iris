namespace Common
{
    public interface ISlice
    {
        int CurrentPageIndex { get; }
        
        int TotalPageCount { get; }
        
        int TotalElementCount { get; }
    }
}