using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LogEntryView : MonoBehaviour
{
    [SerializeField] Sprite iconDebug;
    [SerializeField] Sprite iconWarning;
    [SerializeField] Sprite iconError;
    [SerializeField] Image logTypeIcon;
    [SerializeField] TMP_Text logText;

    public void SetEntry(LogEntry entry)
    {
        switch (entry.Type)
        {
            case LogType.Debug:
                logTypeIcon.sprite = iconDebug;
                break;
            case LogType.Warning:
                logTypeIcon.sprite = iconWarning;
                break;
            case LogType.Error:
                logTypeIcon.sprite = iconError;
                break;
        }
        logText.SetText(entry.Message);
    }
}
