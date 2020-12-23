using System;
using System.Collections.Generic;
using Common;

namespace IrisPoc
{
    internal class Sender
    {
        public Sender(IMessagesProducer manager)
        {
            manager.Messages.Subscribe(Send);
        }

        private void Send(Message message)
        {
            (Update update, List<string> chatIds) = message;
            
            foreach (string chatId in chatIds)
            {
                Console.WriteLine($"Sending {update} to {chatId}");
            }
        }
    }
}