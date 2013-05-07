/* FutileExt - Futile2D Extensions
 * © 2013 Adam K Dean / Imdsm
 * http://www.adamkdean.co.uk 
 */

using UnityEngine;
using System.Collections;

public class FutileExt : MonoBehaviour
{
	private void Start ()
	{
		// initialise Futile
		FutileParams fparams = new FutileParams(true, true, false, false);
		fparams.AddResolutionLevel(800.0f, 1.0f, 1.0f, ""); // iPhone
		fparams.origin = new Vector2(0.5f, 0.5f);		
		Futile.instance.Init(fparams);
		
		// initialize game
		Futile.atlasManager.LoadAtlas("Atlases/Art");
        game = new FExtGame();
	}
		
	private void Update ()
	{
		game.Update(Time.deltaTime);
	}

    private FExtGame game;
}
