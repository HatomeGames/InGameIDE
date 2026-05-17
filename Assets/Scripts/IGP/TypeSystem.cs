using System.Collections;
using System.Collections.Generic;

namespace IGP
{
    public enum ValueType { None, Bool, Int, Float, String }

    public struct Value
    {
        public ValueType Type;

        bool b;
        int i;
        float f;
        string s;

        public Value(ValueType type)
        {
            Type = type;
            b = default;
            i = default;
            f = default;
            s = default;
        }

        public static Value FromBool(bool v) => new() { Type = ValueType.Bool, b = v };
        public static Value FromInt(int v) => new() { Type = ValueType.Int, i = v };
        public static Value FromFloat(float v) => new() { Type = ValueType.Float, f = v };
        public static Value FromString(string v) => new() { Type = ValueType.String, s = v };

        public readonly bool AsBool() => b;
        public readonly int AsInt() => i;
        public readonly float AsFloat() => f;
        public readonly string AsString() => s;

        public override readonly string ToString()
        {
            return Type switch
            {
                ValueType.Bool => b ? "true" : "false",
                ValueType.Int => i.ToString(),
                ValueType.Float => f.ToString(),
                ValueType.String => s,
                _ => "None"
            };
        }
    }
}
