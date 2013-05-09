using UnityEngine;
using System;

public class TTestScreen : TScreen
{
    private FSprite _background;
    private FTextbox _textbox;
    private FLabel _label;
    private int _frameCount = 0;

    public TTestScreen()
    {
        //
    }

    override public void HandleAddedToStage()
    {
        Futile.instance.SignalUpdate += HandleUpdate;
        Futile.screen.SignalResize += HandleResize;
        base.HandleAddedToStage();
    }

    override public void HandleRemovedFromStage()
    {
        Futile.instance.SignalUpdate -= HandleUpdate;
        Futile.screen.SignalResize -= HandleResize;
        base.HandleRemovedFromStage();
    }

    override public void Start()
    {
        _background = new FSprite("background");
        AddChild(_background);

        _textbox = new FTextbox("active", "inactive", "caret", "visitor", Color.black);        
        _textbox.SignalCharacterAdded += _textbox_SignalCharacterAdded;
        _textbox.SignalCharacterRemoved += _textbox_SignalCharacterRemoved;
        _textbox.SignalMaxLengthHit += _textbox_SignalMaxLengthHit;
        _textbox.SignalReturnPressed += _textbox_SignalReturnPressed;
        _textbox.FadeOutDuration = 0.2f; // matches the sound
        _textbox.AllowedCharacters = "^[A-Za-z0-9 ]+$"; // bug!
        AddChild(_textbox);

        _label = new FLabel("visitor", "");
        _label.color = Color.black;
        AddChild(_label);

        // force resize to position everything at the start
        HandleResize(true);
    }

    private void _textbox_SignalCharacterAdded(FTextbox obj)
    {
        FSoundManager.PlaySound("click");
    }

    private void _textbox_SignalCharacterRemoved(FTextbox obj)
    {
        FSoundManager.PlaySound("click");
    }

    private void _textbox_SignalMaxLengthHit(FTextbox obj)
    {
        FSoundManager.PlaySound("bump");
    }

    private void _textbox_SignalReturnPressed(FTextbox obj)
    {
        _label.text = _textbox.Text;
        _label.x = _textbox.x;
        _label.y = _textbox.y;
        _label.scale = 1.0f;

        Go.to(_label, 0.4f, new TweenConfig()
            .setDelay(0.0f)
            .floatProp("scale", 2.0f)
            .floatProp("y", _label.y + _label.textRect.height * 2)
            .setEaseType(EaseType.SineOut));
        
        FSoundManager.PlaySound("ding");
        obj.Unfocus();
    }

    protected void HandleUpdate()
    {
        // update 

        _frameCount++;
    }

    protected void HandleResize(bool wasOrientationChange)
    {
        // this will scale the background up to fit the screen
        // but it won't let it shrink smaller than 100%
        _background.scale = Math.Max(1.0f, 
            Math.Max(Futile.screen.height / _background.textureRect.height, 
                     Futile.screen.width / _background.textureRect.width));
    }
}