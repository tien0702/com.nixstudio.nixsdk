using System;

namespace NIX.Core.DesignPatterns
{
    public interface ITopicMessage : IMessage
    {
        ReadOnlySpan<char> Topic { get; }
    }
}