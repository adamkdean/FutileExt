/* FutileExt - Futile2D Extensions
 * © 2013 Adam K Dean / Imdsm
 * http://www.adamkdean.co.uk 
 */

using UnityEngine;
using System.Collections;
using System;
using System.Text.RegularExpressions;

public class FTextbox : FContainer, FSingleTouchableInterface
{
    public event Action<FTextbox> SignalFocus;
    public event Action<FTextbox> SignalFocusLost;
    public event Action<FTextbox> SignalTextChanged;
    public event Action<FTextbox> SignalMaxLengthHit;
    public event Action<FTextbox> SignalReturnPressed;    
    public event Action<FTextbox> SignalCharacterAdded;
    public event Action<FTextbox> SignalCharacterRemoved;
    
    public string AllowedCharacters
    {
        get { return _allowedCharacters; }
        set
        {
            _allowedCharacters = value;
            _regex = new Regex(_allowedCharacters);
        }
    }

    public string Text
    {
        get { return _label.text; }
        set { _label.text = value; }
    }

    public int MaxLength { get; set; }
    public float Padding { get; set; }

    public EaseType FadeEaseType { get; set; }
    public float FadeInDuration { get; set; }
    public float FadeOutDuration { get; set; }

    protected FAtlasElement _activeElement, _inactiveElement, _caretElement;
    protected FSprite _bgActiveSprite, _bgInactiveSprite, _caretSprite;
    protected FLabel _label, _preLabel;
    protected bool _hasFocus = false;

    private Regex _regex;    
    private string _allowedCharacters;
    private int _caretCount = 0;
    private bool _caretVisible = false;
    private bool _fadeDisabled = false;
    private float _anchorX = 0.5f;
    private float _anchorY = 0.5f;

    public FTextbox(string activeElementName, string inactiveElementName, string caretElementName, string fontName, Color fontColor)
    {
        if (activeElementName == inactiveElementName) _fadeDisabled = true;

        _activeElement = Futile.atlasManager.GetElementWithName(activeElementName);
        _inactiveElement = Futile.atlasManager.GetElementWithName(inactiveElementName);
        _caretElement = Futile.atlasManager.GetElementWithName(caretElementName);
                
        _bgActiveSprite = new FSprite(_activeElement.name);
        _bgActiveSprite.anchorX = _anchorX;
        _bgActiveSprite.anchorY = _anchorY;
        _bgActiveSprite.alpha = 0f;
        AddChild(_bgActiveSprite);

        _bgInactiveSprite = new FSprite(_inactiveElement.name);
        _bgInactiveSprite.anchorX = _anchorX;
        _bgInactiveSprite.anchorY = _anchorY;
        AddChild(_bgInactiveSprite);

        _caretSprite = new FSprite(_caretElement.name);
        _caretSprite.anchorX = _anchorX;
        _caretSprite.anchorY = _anchorY;
        _caretSprite.alpha = 0f;
        AddChild(_caretSprite);

        _preLabel = new FLabel(fontName, "");
        _label = new FLabel(fontName, "");
        _label.color = fontColor;
        AddChild(_label);

        // defaults
        MaxLength = -1;
        Padding = 5f;
        AllowedCharacters = "^[A-Za-z0-9]+$";
        FadeEaseType = EaseType.Linear;
        FadeInDuration = FadeOutDuration = 0.5f;
    }

    public FTextbox(string backgroundElementName, string caretElementName, string fontName, Color fontColor)
        : this(backgroundElementName, backgroundElementName, caretElementName, fontName, fontColor) { }
    public FTextbox(string backgroundElementName, string caretElementName, string fontName)
        : this(backgroundElementName, backgroundElementName, caretElementName, fontName, Color.white) { }

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
        if (_hasFocus)
        {
            if (Input.anyKey)
            {
                string _original = _preLabel.text;
                
                // handle backspace
                if (Input.GetKeyDown(KeyCode.Backspace) && _preLabel.text.Length > 0)
                {
                    _preLabel.text = _preLabel.text.Substring(0, _preLabel.text.Length - 1);
                    if (SignalCharacterRemoved != null) SignalCharacterRemoved(this);
                }
                
                // handle return
                if (Input.GetKeyDown(KeyCode.Return) && SignalReturnPressed != null) 
                    SignalReturnPressed(this);                
                
                // handle any allowed text input
                string input = Input.inputString;
                if (input != "" && _regex.IsMatch(input))
                {
                    if (MaxLength == -1)
                    {
                        // no max-length set, so we set it to stay within the bounds of the bg sprite
                        _preLabel.text += input;
                        if (_preLabel.textRect.width > _bgActiveSprite.width - (Padding * 2))
                        {
                            _preLabel.text = _original;
                            if (SignalMaxLengthHit != null) SignalMaxLengthHit(this);
                        }
                        else
                        {
                            if (SignalCharacterAdded != null) SignalCharacterAdded(this);
                        }
                    }
                    else if (_preLabel.text.Length + input.Length <= MaxLength)
                    {
                        // max length is set, ignore bounds, trust the developer to know what he's doing
                        _preLabel.text += input;
                        if (SignalCharacterAdded != null) SignalCharacterAdded(this);
                    }
                    else 
                    {
                        if (SignalMaxLengthHit != null) SignalMaxLengthHit(this);
                    }
                }

                if (_preLabel.text != _original)
                {
                    // update caret position
                    float caretX = _preLabel.x;
                    if (_preLabel.text.Length >= 0) caretX += _preLabel.textRect.width / 2 + (_caretSprite.width * 2);
                    if (_caretSprite.x != caretX) _caretSprite.x = caretX; // (float)Math.Ceiling((float)caretX);
                    _caretCount = 0; // reset flash counter
                    
                    if (SignalTextChanged != null) SignalTextChanged(this);
                }

                // update the visible label with any changes
                if (_label.text != _preLabel.text) _label.text = _preLabel.text;
            }

            // flashing caret gonna flash
            if (_caretCount % 30 == 0)
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

        _caretCount++;
    }

    public bool HandleSingleTouchBegan(FTouch touch)
    {
        Vector2 touchPos = _bgActiveSprite.GlobalToLocal(touch.position);
        
        if (_bgActiveSprite.textureRect.Contains(touchPos) && !_hasFocus)
        {
            Focus();
        }
        else if (!_bgActiveSprite.textureRect.Contains(touchPos))
        {
            Unfocus();
        }

        return false;
    }

    public void Focus()
    {
        if (!_hasFocus)
        {
            if (!_fadeDisabled)
            {
                // fade the textbox to active, because is look nice
                Go.to(_bgInactiveSprite, FadeInDuration, new TweenConfig()
                    .setDelay(0.0f)
                    .floatProp("alpha", 0.0f)
                    .setEaseType(FadeEaseType));
                Go.to(_bgActiveSprite, FadeInDuration, new TweenConfig()
                    .setDelay(0.0f)
                    .floatProp("alpha", 1.0f)
                    .setEaseType(FadeEaseType));
            }

            _caretCount = 0;
            _hasFocus = true;
            if (SignalFocus != null) SignalFocus(this);
        }
    }

    public void Unfocus()
    {
        if (_hasFocus)
        {
            if (!_fadeDisabled)
            {
                // fade the textbox back to inactive, because is look nice
                Go.to(_bgActiveSprite, FadeOutDuration, new TweenConfig()
                    .setDelay(0.0f)
                    .floatProp("alpha", 0.0f)
                    .setEaseType(FadeEaseType));
                Go.to(_bgInactiveSprite, FadeOutDuration, new TweenConfig()
                    .setDelay(0.0f)
                    .floatProp("alpha", 1.0f)
                    .setEaseType(FadeEaseType));
            }

            _caretVisible = false;
            _caretSprite.alpha = 0f;

            _hasFocus = false;
            if (SignalFocusLost != null) SignalFocusLost(this);
        }
    }

    // will decide how to handle these later
    public void HandleSingleTouchMoved(FTouch touch) { }
    public void HandleSingleTouchEnded(FTouch touch) { }
    public void HandleSingleTouchCanceled(FTouch touch) { }
}