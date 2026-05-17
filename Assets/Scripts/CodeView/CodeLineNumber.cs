using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CodeLineNumber : MonoBehaviour
{
    [SerializeField] LayoutElement layoutElement;
    [SerializeField] TMP_Text lineNumber;
    [SerializeField] TMP_InputField inputField;
    [SerializeField] Scrollbar scrollbar;

    void Awake()
    {
        inputField.onValueChanged.AddListener(_ => RefreshLineNumber());
        scrollbar.onValueChanged.AddListener(_ => ChangeLineNumberPos());

        lineNumber.fontSize = inputField.pointSize;

        RefreshLineNumber();
    }

    public void RefreshLineNumber()
    {
        var textComponent = inputField.textComponent;
        textComponent.ForceMeshUpdate();
        var textInfo = textComponent.textInfo;
        var text = inputField.text;
        int visualLineCount = textInfo.lineCount;
        int logicalLineCount = string.IsNullOrEmpty(text) ? 1 : text.Split('\n').Length;
        StringBuilder sb = new();
        int line = 0;
        for (int i = 0; i < visualLineCount; i++)
        {
            var lineInfo = textInfo.lineInfo[i];

            int firstCharIndex = lineInfo.firstCharacterIndex;
            bool isNewLine = firstCharIndex == 0 || textInfo.characterInfo[firstCharIndex - 1].character == '\n';

            if (isNewLine) sb.Append((++line).ToString());

            sb.Append('\n');
        }

        if (logicalLineCount > visualLineCount)
        {
            for (int i = visualLineCount; i < logicalLineCount; i++)
            {
                sb.Append((++line).ToString());
            }
        }

        lineNumber.SetText(sb.ToString());

        layoutElement.preferredWidth = lineNumber.preferredWidth;
    }

    void ChangeLineNumberPos()
    {
        lineNumber.rectTransform.anchoredPosition = new Vector2(0, inputField.textComponent.rectTransform.anchoredPosition.y);
    }
}
