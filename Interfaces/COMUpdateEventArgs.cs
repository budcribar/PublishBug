using System;

namespace Interfaces
{
    public class COMUpdateEventArgs
    {
        public Guid Id { get; set; }
        public string PropertyName { get; set; }
        public string Value { get; set; }
    }
}