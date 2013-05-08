/* FutileExt - Futile2D Extensions
 * © 2013 Adam K Dean / Imdsm
 * http://www.adamkdean.co.uk 
 */

using UnityEngine;
using System.Collections;
using System;

public class FExtTextbox : FContainer, FSingleTouchableInterface
{
    public event Action<FExtTextbox> SignalFocus;
    public event Action<FExtTextbox> SignalFocusLost;

    protected FAtlasElement _activeElement, _inactiveElement, _caretElement;
    protected FSprite _bgActiveSprite, _bgInactiveSprite, _caretSprite;
    protected bool _hasFocus = false;

    
    private int _frameCount = 0;
    private bool _caretVisible = false;
    private bool _fadeDisabled = false;
    private float _fadeDuration = 1.0f;
    private float _anchorX = 0.5f;
    private float _anchorY = 0.5f;
    //private float _expansionAmount = 10;

    public FExtTextbox(string activeElementName, string inactiveElementName, string caretElementName)
    {
        if (activeElementName == inactiveElementName) _fadeDisabled = true;

        _activeElement = Futile.atlasManager.GetElementWithName(activeElementName);
        _inactiveElement = Futile.atlasManager.GetElementWithName(inactiveElementName);
        _caretElement = Futile.atlasManager.GetElementWithName(caretElementName);

        _caretSprite = new FSprite(_caretElement.name);
        _caretSprite.anchorX = _anchorX;
        _caretSprite.anchorY = _anchorY;
        _caretSprite.alpha = 0f;

        _bgActiveSprite = new FSprite(_activeElement.name);
        _bgActiveSprite.anchorX = _anchorX;
        _bgActiveSprite.anchorY = _anchorY;
        _bgActiveSprite.alpha = 0f;

        _bgInactiveSprite = new FSprite(_inactiveElement.name);
        _bgInactiveSprite.anchorX = _anchorX;
        _bgInactiveSprite.anchorY = _anchorY;

        AddChild(_bgActiveSprite);
        AddChild(_bgInactiveSprite);
        AddChild(_caretSprite);
    }
    public FExtTextbox(string backgroundElementName, string caretElementName)
        : this(backgroundElementName, backgroundElementName, caretElementName) { }

    override public void HandleAddedToStage()
    {
        base.HandleAddedToStage();
        Futile.instance.SignalUpdate += HandleUpdate;
        Futile.touchManager.AddSingleTouchTarget(this);
    }

    override public void HandleRemovedFromStage()
    {
        base.HandleRemovedFromStage();
        Futile.instance.SignalUpdate -= HandleUpdate;        
        Futile.touchManager.RemoveSingleTouchTarget(this);
    }

    public void HandleUpdate()
    {
        // ok so we want to detect keys pressed, put them into a string,
        // and maybe wrap them, making sure to move the caret, and making sure
        // not to overflowwwwwwwwwww!

        // ok lunch

        // flashing caret gonna flash
        if (_hasFocus)
        {
            if (_frameCount % 30 == 0)
            {
                if (_caretVisible)
                {
                    _caretVisible = false;
                    _caretSprite.alpha = 0f;
                }
                else
                {
                    _caretVisible = true;
                    _caretSprite.alpha = 1f;
                }
            }
        }

        _frameCount++;
    }

    public bool HandleSingleTouchBegan(FTouch touch)
    {
        Vector2 touchPos = _bgActiveSprite.GlobalToLocal(touch.position);

        if (_bgActiveSprite.textureRect.Contains(touchPos) && !_hasFocus)
        {
            if (!_fadeDisabled)
            {
                // fade the textbox to active, because is look nice
                Go.to(_bgInactiveSprite, _fadeDuration, new TweenConfig().
                    setDelay(0.0f).
                    floatProp("alpha", 0.0f).
                    setEaseType(EaseType.BackOut));
                Go.to(_bgActiveSprite, _fadeDuration, new TweenConfig().
                    setDelay(0.0f).
                    floatProp("alpha", 1.0f).
                    setEaseType(EaseType.BackOut));
            }

            _hasFocus = true;
            if (SignalFocus != null) SignalFocus(this);
        }
        else 
        {
            if (_hasFocus && !_fadeDisabled)
            {
                // fade the textbox back to inactive, because is look nice
                Go.to(_bgActiveSprite, _fadeDuration, new TweenConfig().
                    setDelay(0.0f).
                    floatProp("alpha", 0.0f).
                    setEaseType(EaseType.BackOut));
                Go.to(_bgInactiveSprite, _fadeDuration, new TweenConfig().
                    setDelay(0.0f).
                    floatProp("alpha", 1.0f).
                    setEaseType(EaseType.BackOut));                
            }

            _caretVisible = false;
            _caretSprite.alpha = 0f;

            _hasFocus = false;
            if (SignalFocusLost != null) SignalFocusLost(this);
        }

        return false;
    }

    // will decide how to handle these later
    public void HandleSingleTouchMoved(FTouch touch) { }
    public void HandleSingleTouchEnded(FTouch touch) { }
    public void HandleSingleTouchCanceled(FTouch touch) { }
}