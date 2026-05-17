using System.Collections;
using System.Collections.Generic;

namespace IGP
{
    public interface IArgument
    {
        Value Value { get; }
    }

    public class Wire : IArgument
    {
        public Value Value { get; set; }
        public readonly string Name;
        public readonly Node Node;
        public readonly int Index;
        public readonly bool IsSequential;

        public Wire(Value value, string name, Node node, int index, bool isSequential)
        {
            Value = value;
            Name = name;
            Node = node;
            Index = index;
            IsSequential = isSequential;
        }
    }

    public class GraphLiteral : IArgument
    {
        public Value Value { get; }

        public GraphLiteral(Value value)
        {
            Value = value;
        }
    }
}
