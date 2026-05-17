using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IGP
{
    public class ParseResult
    {
        public readonly bool Succeeded;
        public readonly Program Program;
        public readonly LogEntry ErrorLog;

        public ParseResult(Program program)
        {
            Succeeded = true;
            Program = program;
            ErrorLog = default;
        }

        public ParseResult(string logMessage)
        {
            Succeeded = false;
            Program = null;
            ErrorLog = new LogEntry(LogType.Error, logMessage);
        }
    }

    public static class CodeParser
    {
        static readonly List<Statement> statements = new();

        public static ParseResult Parse(string code)
        {
            statements.Clear();

            if (string.IsNullOrEmpty(code)) return new ParseResult("Code is empty"); // codeが空のとき

            var lines = code.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            int linesCount = lines.Length;
            for (int i = 0; i < linesCount; i++)
            {
                var stmt = lines[i].Trim();
                if (string.IsNullOrEmpty(stmt)) continue; // 空の行は処理をスキップ

                Statement statement = new() { CodeLine = i }; // このstatementに要素を書き加えていく

                // コメントを分離
                string expr;
                int sharpPos = stmt.IndexOfWithoutStringLiteral('#');
                if (sharpPos < 0) // '#'がないとき
                {
                    expr = stmt;
                    statement.Comment = "";
                }
                else // '#'があるとき
                {
                    expr = stmt.Substring(0, sharpPos).Trim();
                    statement.Comment = stmt.Substring(sharpPos + 1, stmt.Length - sharpPos - 1);
                }

                if (expr == "")
                {
                    statements.Add(statement); // コメントしかない行の場合
                    continue;
                }

                // Wire名と引数付き関数を分離
                string funcWithArgs;
                int equalPos = expr.IndexOfWithoutStringLiteral('=');
                if (equalPos < 0) // '='がないとき
                {
                    statement.Rslts = new string[0];
                    funcWithArgs = expr;
                }
                else // '='があるとき
                {
                    string[] rslts = expr.Substring(0, equalPos).Trim().Split(',');
                    if (rslts.Length <= 0) return new ParseResult($"Identifier not found: {stmt}"); // 左辺がない場合はエラー
                    statement.Rslts = rslts;

                    funcWithArgs = expr.Substring(equalPos + 1, expr.Length - equalPos - 1);
                }

                int parPos = funcWithArgs.IndexOf('(');
                if (parPos <= 0) return new ParseResult($"Function not found: {stmt}"); // 行に'('と関数名がない場合はエラー

                // 関数名と引数を分離
                statement.Opcode = funcWithArgs.Substring(0, parPos).Trim();
                statement.Args = funcWithArgs.Substring(parPos + 1, funcWithArgs.Length - parPos - 2).SplitWithoutStringLiteral(',');

                statements.Add(statement);
            }

            return new ParseResult(new Program(statements));
        }
    }
}
