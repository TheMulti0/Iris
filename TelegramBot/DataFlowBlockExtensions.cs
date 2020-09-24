using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace TelegramBot
{
    public static class DataFlowBlockExtensions
    {
        public static Task CompleteAsync(this IDataflowBlock block)
        {
            block.Complete();
            
            return block.Completion;
        }
    }
}