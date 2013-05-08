/* FutileExt - Futile2D Extensions
 * © 2013 Adam K Dean / Imdsm
 * http://www.adamkdean.co.uk 
 */

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class FExtTestScreen : FContainer
{
    public FExtTestScreen()
    {
        Instance = this;

        bgSprite = new FSprite("background.png");
        AddChild(bgSprite);

        textbox1 = new FExtTextbox("active.png", "inactive.png", "caret.png");
        textbox1.y = -50f;
        AddChild(textbox1);

        textbox2 = new FExtTextbox("inactive.png", "caret.png");
        textbox2.y = 50f;
        AddChild(textbox2);
    }

    public void Update(float dt)
    {
        //
    }

    public static FExtTestScreen Instance { get; private set; }    
    
    private FSprite bgSprite;
    private FExtTextbox textbox1, textbox2;
}