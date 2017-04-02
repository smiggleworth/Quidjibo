using System;

namespace Quidjibo.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class QueueNameAttribute : Attribute
    {
        public string Name { get; }

        public QueueNameAttribute(string name)
        {
            Name = name;
        }
    }
}