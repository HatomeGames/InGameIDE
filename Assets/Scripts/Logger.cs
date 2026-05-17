using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VContainer;
using UniRx;

public enum LogType { Debug, Warning, Error }

public readonly struct LogEntry
{
    public readonly LogType Type;
    public readonly string Message;

    public LogEntry(LogType type, string message)
    {
        Type = type;
        Message = message;
    }
}

public class Logger : MonoBehaviour
{
    [SerializeField] Toggle toggleClearOnExecute;
    [SerializeField] Button buttonClear;
    [SerializeField] GameObject logEntryViewPrefab;
    [SerializeField] Transform logArea;

    readonly List<LogEntryView> logEntrieViews = new();

    [Inject]
    public void Construct()
    {
        buttonClear.OnClickAsObservable().Subscribe(_ => ClearLog()).AddTo(this);
    }

    public void AppendLog(LogEntry entry)
    {
        LogEntryView logEntry = Instantiate(logEntryViewPrefab, logArea).GetComponent<LogEntryView>();
        logEntry.SetEntry(entry);
        logEntrieViews.Add(logEntry);
    }

    public void OnExecute()
    {
        if (toggleClearOnExecute.isOn) ClearLog();
    }

    public void ClearLog()
    {
        for (int i = 0; i < logEntrieViews.Count; i++)
        {
            Destroy(logEntrieViews[i].gameObject);
        }
        logEntrieViews.Clear();
    }
}
