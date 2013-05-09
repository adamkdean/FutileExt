using UnityEngine;
using System;

public class TGame : MonoBehaviour
{
    public static TGame Instance { get; private set; }

    private TScreenType _currentScreenType = TScreenType.None;
    private TScreen _currentScreen;    

    private void Start()
    {
        Instance = this;

        Go.defaultEaseType = EaseType.Linear;
        Go.duplicatePropertyRule = DuplicatePropertyRuleType.RemoveRunningProperty;

        FutileParams fparams = new FutileParams(true, true, false, false);
        fparams.AddResolutionLevel(480.0f, 1.0f, 1.0f, ""); // iPhone
        fparams.origin = new Vector2(0.5f, 0.5f);
        Futile.instance.Init(fparams);
        
        Futile.atlasManager.LoadAtlas("Atlases/Art");
        Futile.atlasManager.LoadFont("visitor", "visitor_0", "Atlases/visitor", 0f, -2f);

        GoToScreen(TScreenType.TestScreen);
    }

    private void GoToScreen(TScreenType ScreenType)
    {
        if (_currentScreenType == ScreenType) return;

        TScreen ScreenToCreate = null;
        if (ScreenType == TScreenType.TestScreen) ScreenToCreate = new TTestScreen();        

        if (ScreenToCreate != null)
        {
            _currentScreenType = ScreenType;
            if (_currentScreen != null) Futile.stage.RemoveChild(_currentScreen);
            _currentScreen = ScreenToCreate;
            Futile.stage.AddChild(_currentScreen);
            _currentScreen.Start();
        }
    }
}
