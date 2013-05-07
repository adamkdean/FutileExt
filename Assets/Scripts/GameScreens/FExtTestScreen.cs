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
    }

    public void Update(float dt)
    {
        //
    }

    public static FExtTestScreen Instance { get; private set; }    
    private FSprite bgSprite;
}