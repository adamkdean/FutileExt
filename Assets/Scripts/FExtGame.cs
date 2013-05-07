/* FutileExt - Futile2D Extensions
 * © 2013 Adam K Dean / Imdsm
 * http://www.adamkdean.co.uk 
 */

using UnityEngine;
using System.Collections;

public class FExtGame
{
    public FExtGame()
    {
        Instance = this;

        testScreen = new FExtTestScreen();
        Futile.stage.AddChild(testScreen);
    }

    public void Update(float dt)
    {
        testScreen.Update(dt);
    }

    public static FExtGame Instance { get; private set; }
    private FExtTestScreen testScreen;
}