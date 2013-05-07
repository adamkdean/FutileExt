/* FutileExt - Futile2D Extensions
 * © 2013 Adam K Dean / Imdsm
 * http://www.adamkdean.co.uk 
 */

using UnityEngine;
using System.Collections;
using System;

public class FExtTextbox : FContainer, FSingleTouchableInterface
{
    public event Action<FExtTextbox> SignalPress;
    public event Action<FExtTextbox> SignalRelease;
    public event Action<FExtTextbox> SignalReleaseOutside;

    protected FAtlasElement activeElement;
    protected FAtlasElement inactiveElement;
    protected FSprite bgSprite;    

    private float anchorX = 0.5f;
    private float anchorY = 0.5f;

    public FExtTextbox(string activeElementName, string inactiveElementName)
    {
        activeElement = Futile.atlasManager.GetElementWithName(activeElementName);
        inactiveElement = Futile.atlasManager.GetElementWithName(inactiveElementName);

        bgSprite = new FSprite(inactiveElement.name);
        bgSprite.anchorX = anchorX;
        bgSprite.anchorY = anchorY;

        AddChild(bgSprite);
    }
    public FExtTextbox(string backgroundElementName)
        : this(backgroundElementName, backgroundElementName) { }

    override public void HandleAddedToStage()
    {
        base.HandleAddedToStage();
        Futile.touchManager.AddSingleTouchTarget(this);
    }

    override public void HandleRemovedFromStage()
    {
        base.HandleRemovedFromStage();
        Futile.touchManager.RemoveSingleTouchTarget(this);
    }

    public bool HandleSingleTouchBegan(FTouch touch)
    {
        Vector2 touchPos = inactiveBgSprite.GlobalToLocal(touch.position);

        if (inactiveBgSprite.textureRect.Contains(touchPos))
        {
            _bg.element = _downElement;

            if (_soundName != null) FSoundManager.PlaySound(_soundName);

            if (SignalPress != null) SignalPress(this);

            return true;
        }

        return false;
    }

    public void HandleSingleTouchMoved(FTouch touch)
    {
        Vector2 touchPos = _bg.GlobalToLocal(touch.position);

        //expand the hitrect so that it has more error room around the edges
        //this is what Apple does on iOS and it makes for better usability
        Rect expandedRect = _bg.textureRect.CloneWithExpansion(expansionAmount);

        if (expandedRect.Contains(touchPos))
        {
            _bg.element = _downElement;
        }
        else
        {
            _bg.element = _upElement;
        }
    }

    public void HandleSingleTouchEnded(FTouch touch)
    {
        _bg.element = _upElement;

        Vector2 touchPos = _bg.GlobalToLocal(touch.position);

        //expand the hitrect so that it has more error room around the edges
        //this is what Apple does on iOS and it makes for better usability
        Rect expandedRect = _bg.textureRect.CloneWithExpansion(expansionAmount);

        if (expandedRect.Contains(touchPos))
        {
            if (SignalRelease != null) SignalRelease(this);
        }
        else
        {
            if (SignalReleaseOutside != null) SignalReleaseOutside(this);
        }
    }

    public void HandleSingleTouchCanceled(FTouch touch)
    {
        _bg.element = _upElement;
        if (SignalReleaseOutside != null) SignalReleaseOutside(this);
    }    
}