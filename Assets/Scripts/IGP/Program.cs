using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace IGP
{
    public class Program
    {
        readonly List<Statement> statements;
        public IReadOnlyList<Statement> Statements => statements;

        public Program(List<Statement> statements)
        {
            this.statements = statements;
        }

        public override string ToString()
        {
            StringBuilder sb = new();
            foreach (var statement in statements)
            {
                sb.AppendLine(statement.ToString());
            }
            return sb.ToString();
        }
    }

    public struct Statement
    {
        public int CodeLine;
        public string Comment;
        public string Opcode;
        public string[] Args;
        public string[] Rslts;

        public override string ToString()
        {
            StringBuilder sb = new();
            if (Rslts.Length > 0)
            {
                sb.Append(string.Join(',', Rslts));
                sb.Append(" = ");
            }
            sb.Append(Opcode);
            sb.Append("(");
            sb.Append(string.Join(',', Args));
            sb.Append(")");
            if (!string.IsNullOrEmpty(Comment))
            {
                sb.Append(" # ");
                sb.Append(Comment);
            }
            return sb.ToString();
        }
    }
}
