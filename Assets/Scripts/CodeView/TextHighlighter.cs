using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace CodeView
{
    public class TextHighlighter : MonoBehaviour
    {
        [SerializeField] TMP_Text text;

        void LateUpdate()
        {
            Highlight();
        }

        void Highlight()
        {
            text.ForceMeshUpdate();

            var textInfo = text.textInfo;

            for (int i = 0; i < textInfo.characterCount; i++)
            {
                var c = textInfo.characterInfo[i].character;

                if (c == '(' || c == ')') SetCharColor(textInfo, i, new Color(0.85f, 0.44f, 0.84f));
                if (c == '#') SetCharColor(textInfo, i, new Color(0.42f, 0.6f, 0.33f));
                if (c == '=' || c == '_') SetCharColor(textInfo, i, new Color(0.34f, 0.61f, 0.84f));
                if (c == ',') SetCharColor(textInfo, i, new Color(0.61f, 0.86f, 1f));
                if (c == '\"') SetCharColor(textInfo, i, new Color(0.81f, 0.57f, 0.47f));
            }

            text.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
        }

        void SetCharColor(TMP_TextInfo textInfo, int charIndex, Color color)
        {
            var charInfo = textInfo.characterInfo[charIndex];

            int meshIndex = charInfo.materialReferenceIndex;
            int vertexIndex = charInfo.vertexIndex;

            Color32[] colors = textInfo.meshInfo[meshIndex].colors32;

            colors[vertexIndex + 0] = color;
            colors[vertexIndex + 1] = color;
            colors[vertexIndex + 2] = color;
            colors[vertexIndex + 3] = color;
        }
    }
}
