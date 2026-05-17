using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IGP
{
    public class CompileResult
    {
        public readonly bool Succeeded;
        public readonly Executable Executable;
        public readonly LogEntry ErrorLog;

        public CompileResult(Executable executable)
        {
            Succeeded = true;
            Executable = executable;
            ErrorLog = default;
        }

        public CompileResult(string logMessage)
        {
            Succeeded = false;
            Executable = null;
            ErrorLog = new LogEntry(LogType.Error, logMessage);
        }
    }

    public static class Compiler
    {
        class NodeAndStatement
        {
            public Node Node;
            public Statement Statement;

            public NodeAndStatement(Node node, Statement statement)
            {
                Node = node;
                Statement = statement;
            }
        }

        static readonly Dictionary<string, Wire> wires = new();
        static readonly List<NodeAndStatement> nodeAndStatements = new();

        static readonly List<Node> evaluationOrder = new();
        static readonly List<SequentialNode> sequentialNodes = new();
        static readonly List<PassiveNode> passiveNodes = new();

        public static CompileResult Compile(Program program)
        {
            wires.Clear();
            nodeAndStatements.Clear();

            // 多重定義を検出
            foreach (var statement in program.Statements)
            {
                foreach (var rslt in statement.Rslts)
                {
                    if (rslt == "_") continue; // アンダースコアは破棄

                    if ((rslt.StartsWith("\"") && rslt.EndsWith("\"")) || rslt == "true" || rslt == "false" || int.TryParse(rslt, out var i) || float.TryParse(rslt, out var f)) return new CompileResult($"Definition should not literal: {rslt}"); // Wire名の形式がリテラルの場合

                    if (wires.ContainsKey(rslt)) return new CompileResult($"Duplicate definition: {rslt}"); // 同じWire名が複数回定義されている場合

                    wires[rslt] = null;
                }
            }

            // 返り値を登録
            foreach (var statement in program.Statements)
            {
                Node node = CreateNode(statement.Opcode);
                if (node == null) return new CompileResult($"Unknown function: {statement.Opcode}"); // 不明なオペコードの場合

                int rsltCount = statement.Rslts.Length;

                // 返り値の数を一致確認
                if (rsltCount != node.RsltTypes.Count) return new CompileResult($"The number of results is different. {statement.Opcode} needs {node.RsltTypes.Count} but there are {statement.Rslts.Length} results.");

                List<Wire> results = new();
                for (int i = 0; i < rsltCount; i++)
                {
                    var rslt = statement.Rslts[i];
                    var wire = new Wire(new Value(node.RsltTypes[i].ValueType), rslt, node, i, node.RsltTypes[i].IsSequential);
                    wires[rslt] = wire;
                    results.Add(wire);
                }
                node.RegistResults(results);

                nodeAndStatements.Add(new(node, statement));
            }

            // 引数を登録
            foreach (var nodeAndStatement in nodeAndStatements)
            {
                var node = nodeAndStatement.Node;
                var statement = nodeAndStatement.Statement;

                int argsCount = statement.Args.Length;

                // 引数の数を一致確認
                if (argsCount != node.ArgTypes.Count) return new CompileResult($"The number of arguments is different. {statement.Opcode} needs {node.ArgTypes.Count} but there are {statement.Args.Length} arguments.");

                List<IArgument> arguments = new();
                for (int i = 0; i < argsCount; i++)
                {
                    // 引数の参照元を取得あるいはリテラルを生成
                    string arg = statement.Args[i];
                    var argument = ParseArg(arg, wires);
                    if (argument == null) return new CompileResult($"Undefined symbol: {arg}"); // 引数が未定義の場合

                    // 引数の型を一致確認
                    ValueType valueType = argument.Value.Type;
                    if (valueType != node.ArgTypes[i]) return new CompileResult($"The type of arguments is different. {statement.Opcode} needs {node.ArgTypes[i]} but there is {valueType}.");

                    arguments.Add(argument);
                }
                node.RegistArgs(arguments);
            }

            return SortEvaluationOrder(nodeAndStatements);
        }

        static Node CreateNode(string opcode)
        {
            return opcode switch
            {
                "OUT" => new OutNode(),
                "AND" => new AndNode(),
                "OR" => new OrNode(),
                "NOT" => new NotNode(),
                "XOR" => new XorNode(),
                "NAND" => new NandNode(),
                "NOR" => new NorNode(),
                "XNOR" => new XnorNode(),
                "KEY" => new KeyNode(),
                "DFF" => new DFlipFlopNode(),
                "RSFF" => new RSFlipFlopNode(),
                "TFF" => new TFlipFlopNode(),
                "JKFF" => new JKFlipFlopNode(),
                _ => null // 一致する関数名がないとき
            };
        }

        static IArgument ParseArg(string arg, Dictionary<string, Wire> wires)
        {
            arg = arg.Trim();

            if (arg.StartsWith("\"") && arg.EndsWith("\""))
                return new GraphLiteral(Value.FromString(arg.Substring(1, arg.Length - 2)));
            if (arg == "true" || arg == "false")
                return new GraphLiteral(Value.FromBool(arg == "true"));
            if (int.TryParse(arg, out var i))
                return new GraphLiteral(Value.FromInt(i));
            if (float.TryParse(arg, out var f))
                return new GraphLiteral(Value.FromFloat(f));

            if (!wires.ContainsKey(arg)) return null; // Wire名が未定義の場合

            return wires[arg];
        }

        static CompileResult SortEvaluationOrder(List<NodeAndStatement> nodeAndStatements)
        {
            evaluationOrder.Clear();
            sequentialNodes.Clear();
            passiveNodes.Clear();

            HashSet<Node> visited = new();
            HashSet<Node> visiting = new();

            foreach (var nodeAndStatement in nodeAndStatements)
            {
                var node = nodeAndStatement.Node;

                if (node is SequentialNode sequentialNode)
                {
                    sequentialNodes.Add(sequentialNode);
                }
                else if (node is PassiveNode passiveNode)
                {
                    passiveNodes.Add(passiveNode);
                }
                else
                {
                    bool succeeded = Visit(node, visited, visiting);
                    if (!succeeded) return new CompileResult($"Combinational cycle detected: {nodeAndStatement.Statement}");
                }
            }

            var executable = new Executable(evaluationOrder, sequentialNodes, passiveNodes);
            return new CompileResult(executable);
        }

        static bool Visit(Node node, HashSet<Node> visited, HashSet<Node> visiting) // エラー以外ではtrueを返す
        {
            if (visited.Contains(node)) return true;

            if (visiting.Contains(node)) return false; // 閉路が検出された場合

            visiting.Add(node);

            foreach (var arg in node.Args)
            {
                if (arg is not Wire wire) continue;

                if (wire.IsSequential) continue;

                bool succeeded = Visit(wire.Node, visited, visiting);
                if (!succeeded) return false;
            }

            visiting.Remove(node);
            visited.Add(node);

            evaluationOrder.Add(node);

            return true;
        }
    }
}
