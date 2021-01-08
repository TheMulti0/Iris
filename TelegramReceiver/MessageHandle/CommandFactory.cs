using System;
using Microsoft.Extensions.DependencyInjection;

namespace TelegramReceiver
{
    public class CommandFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public CommandFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public INewCommand Create<TCommand>(Context context) where TCommand : INewCommand
        {
            return ActivatorUtilities.CreateInstance<TCommand>(
                _serviceProvider,
                context);
        }
    }
}