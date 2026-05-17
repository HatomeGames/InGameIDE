using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

namespace IGP
{
    public readonly struct ResultType
    {
        public readonly ValueType ValueType;
        public readonly bool IsSequential;

        public ResultType(ValueType valueType, bool isSequential)
        {
            ValueType = valueType;
            IsSequential = isSequential;
        }
    }

    public abstract class Node
    {
        protected List<IArgument> args;
        public IReadOnlyList<IArgument> Args => args;
        protected List<Wire> rslts;
        public IReadOnlyList<Wire> Rslts => rslts;

        public readonly string Opcode;
        readonly ValueType[] argTypes;
        public IReadOnlyList<ValueType> ArgTypes => argTypes;
        readonly ResultType[] rsltTypes;
        public IReadOnlyList<ResultType> RsltTypes => rsltTypes;

        protected Node(string opcode, ValueType[] argTypes, ResultType[] rsltTypes)
        {
            Opcode = opcode;
            this.argTypes = argTypes;
            this.rsltTypes = rsltTypes;
        }

        public void RegistArgs(List<IArgument> args)
        {
            this.args = args;
        }

        public void RegistResults(List<Wire> rslts)
        {
            this.rslts = rslts;
        }

        public virtual void Inject(GameAPI gameAPI) { }
        public virtual void Init() { }
        public abstract void Evaluate(float deltaTime);
    }

    class KeyNode : Node
    {
        public KeyNode() : base(
            "KEY",
            new ValueType[] { ValueType.String },
            new ResultType[] { new(ValueType.Bool, false) })
        { }

        KeyControl keyControl = null;

        public override void Init()
        {
            string key = args[0].Value.AsString();
            if (!(string.IsNullOrEmpty(key) || char.IsLower(key[0]))) key = char.ToLower(key[0]) + key.Substring(1);
            var control = InputSystem.FindControl($"<Keyboard>/{key}");

            if (control is KeyControl keyControl)
            {
                this.keyControl = keyControl;
            }
        }

        public override void Evaluate(float _)
        {
            rslts[0].Value = Value.FromBool(keyControl?.isPressed ?? false);
        }
    }

    public abstract class PassiveNode : Node
    {
        protected PassiveNode(string opcode, ValueType[] argTypes) : base(opcode, argTypes, new ResultType[0]) { }
    }

    class OutNode : PassiveNode
    {
        public OutNode() : base(
            "OUT",
            new ValueType[] { ValueType.String, ValueType.Bool })
        { }

        IOutputProvider outputProvider;

        public override void Inject(GameAPI gameAPI)
        {
            outputProvider = gameAPI.OutputProvider;
        }

        public override void Evaluate(float _)
        {
            outputProvider.SetEntry(args[0].Value.AsString(), args[1].Value.AsBool());
        }
    }

    public class AndNode : Node
    {
        public AndNode() : base(
            "AND",
            new ValueType[] { ValueType.Bool, ValueType.Bool },
            new ResultType[] { new(ValueType.Bool, false) })
        { }

        public override void Evaluate(float _)
        {
            rslts[0].Value = Value.FromBool(args[0].Value.AsBool() && args[1].Value.AsBool());
        }
    }

    public class OrNode : Node
    {
        public OrNode() : base(
            "OR",
            new ValueType[] { ValueType.Bool, ValueType.Bool },
            new ResultType[] { new(ValueType.Bool, false) })
        { }

        public override void Evaluate(float _)
        {
            rslts[0].Value = Value.FromBool(args[0].Value.AsBool() || args[1].Value.AsBool());
        }
    }

    public class NotNode : Node
    {
        public NotNode() : base(
            "NOT",
            new ValueType[] { ValueType.Bool },
            new ResultType[] { new(ValueType.Bool, false) })
        { }

        public override void Evaluate(float _)
        {
            rslts[0].Value = Value.FromBool(!args[0].Value.AsBool());
        }
    }

    public class XorNode : Node
    {
        public XorNode() : base(
            "XOR",
            new ValueType[] { ValueType.Bool, ValueType.Bool },
            new ResultType[] { new(ValueType.Bool, false) })
        { }

        public override void Evaluate(float _)
        {
            rslts[0].Value = Value.FromBool(args[0].Value.AsBool() ^ args[1].Value.AsBool());
        }
    }

    public class NandNode : Node
    {
        public NandNode() : base(
            "NAND",
            new ValueType[] { ValueType.Bool, ValueType.Bool },
            new ResultType[] { new(ValueType.Bool, false) })
        { }

        public override void Evaluate(float _)
        {
            rslts[0].Value = Value.FromBool(!(args[0].Value.AsBool() && args[1].Value.AsBool()));
        }
    }

    public class NorNode : Node
    {
        public NorNode() : base(
            "NOR",
            new ValueType[] { ValueType.Bool, ValueType.Bool },
            new ResultType[] { new(ValueType.Bool, false) })
        { }

        public override void Evaluate(float _)
        {
            rslts[0].Value = Value.FromBool(!(args[0].Value.AsBool() || args[1].Value.AsBool()));
        }
    }

    public class XnorNode : Node
    {
        public XnorNode() : base(
            "XNOR",
            new ValueType[] { ValueType.Bool, ValueType.Bool },
            new ResultType[] { new(ValueType.Bool, false) })
        { }

        public override void Evaluate(float _)
        {
            rslts[0].Value = Value.FromBool(!(args[0].Value.AsBool() ^ args[1].Value.AsBool()));
        }
    }

    public abstract class SequentialNode : Node
    {
        protected SequentialNode(string opcode, ValueType[] argTypes, ResultType[] resultTypes) : base(opcode, argTypes, resultTypes) { }

        protected Value next;

        public abstract void ComputeNext(float deltaTime);

        public void Commit()
        {
            rslts[0].Value = next;
        }

        public override void Evaluate(float _) { } // FFはEvaluateしない
    }

    class DFlipFlopNode : SequentialNode
    {
        public Node D;
        public Node Clock;

        bool prevClock;

        public DFlipFlopNode() : base(
            "DFF",
            new ValueType[] { ValueType.Bool, ValueType.Bool },
            new ResultType[] { new(ValueType.Bool, true) })
        { }

        public override void ComputeNext(float _)
        {
            bool rising = !prevClock && args[1].Value.AsBool();

            if (rising)
            {
                next = args[0].Value;
            }
            else
            {
                next = rslts[0].Value; // ホールド
            }

            prevClock = args[1].Value.AsBool();
        }
    }

    class RSFlipFlopNode : SequentialNode
    {
        public Node S;
        public Node R;

        private bool prevS;
        private bool prevR;

        public RSFlipFlopNode() : base(
            "RSFF",
            new ValueType[] { ValueType.Bool, ValueType.Bool },
            new ResultType[] { new(ValueType.Bool, true) })
        { }

        public override void ComputeNext(float _)
        {
            bool s = args[0].Value.AsBool();
            bool r = args[1].Value.AsBool();

            bool sChanged = s != prevS;
            bool rChanged = r != prevR;

            if (s && r)
            {
                // 両方1の場合：最後に変化した側を優先
                if (sChanged && !rChanged)
                {
                    next = Value.FromBool(true);
                }
                else if (!sChanged && rChanged)
                {
                    next = Value.FromBool(false);
                }
                else if (sChanged && rChanged)
                {
                    // 同時変化（同フレーム）
                    // → 優先順位を固定（ここではR優先にする例）
                    next = Value.FromBool(false);
                }
                else
                {
                    // 変化なし（既に両方1）
                    next = rslts[0].Value;
                }
            }
            else if (s)
            {
                next = Value.FromBool(true);
            }
            else if (r)
            {
                next = Value.FromBool(false);
            }
            else
            {
                next = rslts[0].Value; // ホールド
            }

            prevS = s;
            prevR = r;
        }
    }

    class TFlipFlopNode : SequentialNode
    {
        public Node Clock;

        private bool prevClock;

        public TFlipFlopNode() : base(
            "TFF",
            new ValueType[] { ValueType.Bool },
            new ResultType[] { new(ValueType.Bool, true) })
        { }

        public override void ComputeNext(float _)
        {
            bool rising = !prevClock && args[0].Value.AsBool();

            if (rising)
            {
                next = Value.FromBool(!rslts[0].Value.AsBool());
            }
            else
            {
                next = rslts[0].Value;
            }

            prevClock = args[0].Value.AsBool();
        }
    }

    class JKFlipFlopNode : SequentialNode
    {
        public Node J;
        public Node K;
        public Node Clock;

        private bool prevClock;

        public JKFlipFlopNode() : base(
            "JKFF",
            new ValueType[] { ValueType.Bool, ValueType.Bool, ValueType.Bool },
            new ResultType[] { new(ValueType.Bool, true) })
        { }

        public override void ComputeNext(float _)
        {
            bool rising = !prevClock && args[2].Value.AsBool();

            if (rising)
            {
                bool j = args[0].Value.AsBool();
                bool k = args[1].Value.AsBool();

                if (j && k)
                {
                    next = Value.FromBool(!rslts[0].Value.AsBool());
                }
                else if (j)
                {
                    next = Value.FromBool(true);
                }
                else if (k)
                {
                    next = Value.FromBool(false);
                }
                else
                {
                    next = rslts[0].Value;
                }
            }
            else
            {
                next = rslts[0].Value;
            }

            prevClock = args[2].Value.AsBool();
        }
    }
}
