using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using VContainer;
using UnityEngine.UI;
using UniRx;

namespace CodeView
{
    public class CodeViewManager : MonoBehaviour
    {
        public string Code => codeInputField.text;

        [SerializeField] Button buttonUndo;
        [SerializeField] Button buttonRedo;
        [SerializeField] TMP_InputField codeInputField;
        [SerializeField] CodeLineNumber codeLineNumber;

        float timer;
        bool snapshotTooked = false;
        string codePrev = "";
        string addedPrev = "";
        string removedPrev = "";

        readonly Stack<string> undoBuffer = new();
        readonly Stack<string> redoBuffer = new();

        [Inject]
        public void Construct(GameInput gameInput)
        {
            codeInputField.onValueChanged.AsObservable().Subscribe(s => OnEditCode(s)).AddTo(this);
            buttonUndo.OnClickAsObservable().Subscribe(_ => Undo()).AddTo(this);
            buttonRedo.OnClickAsObservable().Subscribe(_ => Redo()).AddTo(this);
            gameInput.Code.Undo.SubscribePerform(_ => Undo()).AddTo(this);
            gameInput.Code.Redo.SubscribePerform(_ => Redo()).AddTo(this);
        }

        void Update()
        {
            if (!snapshotTooked && timer >= 1)
            {
                RecordSnapshot();
            }
            timer += Time.deltaTime;
        }

        void OnEditCode(string code)
        {
            snapshotTooked = false;
            timer = 0;
            (string added, string removed) = TextDiff(codePrev, code);
            codePrev = code;
            if (!string.IsNullOrEmpty(added))
            {
                if (added == "=" || added == "#" || added == "\n" || added == "(" || added == ")" || added == "," || (added == " " && addedPrev != " "))
                {
                    addedPrev = added;
                    RecordSnapshot();
                    return;
                }
            }

            if (!string.IsNullOrEmpty(removed))
            {
                if (removed == "=" || removed == "#" || removed == "\n" || removed == "(" || removed == ")" || removed == "," || (removed == " " && removedPrev != " "))
                {
                    removedPrev = removed;
                    RecordSnapshot();
                    return;
                }
            }
        }

        void RecordSnapshot()
        {
            redoBuffer.Clear();
            undoBuffer.Push(codeInputField.text);
            snapshotTooked = true;
        }

        void Undo()
        {
            if (!snapshotTooked) RecordSnapshot();

            var code = "";
            if (undoBuffer.Count >= 1)
            {
                code = undoBuffer.Pop();
                redoBuffer.Push(code);
            }
            codeInputField.SetTextWithoutNotify(code);
            codeLineNumber.RefreshLineNumber();
        }

        void Redo()
        {
            if (redoBuffer.Count <= 0) return;
            var code = redoBuffer.Pop();
            undoBuffer.Push(code);
            codeInputField.SetTextWithoutNotify(code);
            codeLineNumber.RefreshLineNumber();
        }

        (string, string) TextDiff(string prev, string current)
        {
            int start = 0;

            // 前方一致部分をスキップ
            while (start < prev.Length && start < current.Length && prev[start] == current[start])
            {
                start++;
            }

            int endPrev = prev.Length - 1;
            int endCurr = current.Length - 1;

            // 後方一致部分をスキップ
            while (endPrev >= start && endCurr >= start && prev[endPrev] == current[endCurr])
            {
                endPrev--;
                endCurr--;
            }

            // 差分抽出
            string removed = (start <= endPrev) ? prev.Substring(start, endPrev - start + 1) : "";
            string added = (start <= endCurr) ? current.Substring(start, endCurr - start + 1) : "";

            return (added, removed);
        }
    }
}
