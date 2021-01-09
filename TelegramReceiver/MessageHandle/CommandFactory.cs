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

        public ICommand Create<TCommand>(Context context) where TCommand : ICommand
        {
            return ActivatorUtilities.CreateInstance<TCommand>(
                _serviceProvider,
                context);
        }
        
        public ICommand Create(Type type, Context context)
        {
            if (! type.IsAssignableTo(typeof(ICommand)))
            {
                return null;
            }
            
            return ActivatorUtilities.CreateInstance(
                _serviceProvider,
                type,
                context) as ICommand;
        }
    }
}