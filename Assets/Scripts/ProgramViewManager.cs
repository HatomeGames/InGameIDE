using System;
using System.Collections;
using System.Collections.Generic;
using CodeView;
using IGP;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

public enum ViewType { None, Code }

public class ProgramViewManager : MonoBehaviour
{
    readonly Subject<Unit> onExecute = new();
    public IObservable<Unit> OnExecute => onExecute;
    readonly Subject<Unit> onKill = new();
    public IObservable<Unit> OnKill => onKill;
    readonly Subject<bool> onPauseChanged = new();
    public IObservable<bool> OnPauseChanged => onPauseChanged;

    [SerializeField] GameObject codeView;
    [SerializeField] Button buttonUndo;
    [SerializeField] Button buttonRedo;
    [SerializeField] Button buttonCode;
    [SerializeField] Toggle toggleHideOnExecute;
    [SerializeField] Button buttonPause;
    [SerializeField] Button buttonExecute;
    [SerializeField] TMP_Text executeText;

    [Inject] CodeViewManager codeViewManager;
    [Inject] Logger logger;
    [Inject] Runtime runtime;

    GameInput gameInput;
    ViewType currentView = ViewType.None;
    ViewType prevView = ViewType.Code;
    bool isRunning = false;
    bool isPausing = false;

    [Inject]
    public void Construct(GameInput gameInput)
    {
        this.gameInput = gameInput;

        buttonCode.OnClickAsObservable().Subscribe(_ => ChangeControl(ViewType.Code, true));

        buttonExecute.OnClickAsObservable().Subscribe(_ => ChangeRunning()).AddTo(this);
        buttonPause.OnClickAsObservable().Subscribe(_ => TryPause()).AddTo(this);

        gameInput.SetInputActionMaps(InputActionMapType.Basic | InputActionMapType.Run);
        gameInput.Basic.OpenProgramView.SubscribePerform(_ => ChangeControl(currentView == ViewType.None ? prevView : ViewType.None, true)).AddTo(this);
        gameInput.Basic.Run.SubscribePerform(_ => ChangeRunning()).AddTo(this);
    }

    void ChangeRunning()
    {
        if (isRunning)
        {
            Kill();

            ChangeControl(prevView != ViewType.None ? prevView : ViewType.Code, toggleHideOnExecute.isOn);
            onKill.OnNext(Unit.Default);
            executeText.SetText(">");
            buttonUndo.interactable = true;
            buttonRedo.interactable = true;
            buttonCode.interactable = true;
            isRunning = false;
            return;
        }

        isRunning = TryExecute();
    }

    bool TryExecute()
    {
        // パース
        ParseResult parseResult = CodeParser.Parse(codeViewManager.Code);
        if (!parseResult.Succeeded)
        {
            logger.AppendLog(parseResult.ErrorLog);
            return false;
        }

        // コンパイル
        var compileResult = Compiler.Compile(parseResult.Program);
        if (!compileResult.Succeeded)
        {
            logger.AppendLog(compileResult.ErrorLog);
            return false;
        }

        isPausing = false;
        runtime.SetPause(false);
        runtime.Execute(compileResult.Executable);

        buttonUndo.interactable = false;
        buttonRedo.interactable = false;
        buttonCode.interactable = false;

        ChangeControl(ViewType.None, toggleHideOnExecute.isOn);
        onExecute?.OnNext(Unit.Default);
        logger.OnExecute();
        executeText.SetText("[]");

        return true;
    }

    void Kill()
    {
        runtime.Kill();
    }

    void TryPause()
    {
        if (!isRunning) return;

        isPausing = !isPausing;
        runtime.SetPause(isPausing);

        onPauseChanged?.OnNext(isPausing);
    }

    void ChangeControl(ViewType view, bool changeView)
    {
        if (currentView != ViewType.None) prevView = currentView;
        if (currentView == view) view = ViewType.None;
        currentView = view;

        switch (currentView)
        {
            case ViewType.Code:
                gameInput.SetInputActionMaps(InputActionMapType.Basic | InputActionMapType.Code);
                break;
            default:
                gameInput.SetInputActionMaps(InputActionMapType.Basic | InputActionMapType.Run);
                break;
        }

        if (!changeView) return;

        codeView.SetActive(currentView == ViewType.Code);
    }

    void OnDestroy()
    {
        onExecute.OnCompleted();
        onExecute.Dispose();
        onKill.OnCompleted();
        onKill.Dispose();
        onPauseChanged.OnCompleted();
        onPauseChanged.Dispose();
    }
}
