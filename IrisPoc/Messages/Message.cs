using System.Collections.Generic;
using Common;

namespace IrisPoc
{
    internal record Message(
        Update Update,
        List<string> Chats);
}