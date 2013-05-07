/* MathEx Unity/Futile Math Extension Library
 * © 2013 Adam K Dean / Imdsm
 * http://www.adamkdean.co.uk 
 */

using UnityEngine;

public static class MathEx
{
	public static float DegreesToRadians(float degrees)
	{
		return degrees * RXMath.DTOR;
	}
	
	public static float RadiansToDegrees(float radians)
	{
		return radians * RXMath.RTOD;
	}
		
	public static Vector2 DegreesToXY(float degrees)
	{
		// because unity's y+ goes up instead of down, we invert the y axis
		// and because 0 degrees is up, and not pointing "right", we take 90 degrees off the angle
		return new Vector2(
			Mathf.Cos(DegreesToRadians(degrees - 90)),
			Mathf.Sin(DegreesToRadians(degrees - 90)) * -1
		);
	}
	
	public static Vector2 RadiansToXY(float radians)
	{
		return DegreesToXY(RadiansToDegrees(radians));		
	}

    public static bool IsOffScreen(float x, float y, float width, float height, float cutoffScale = 1.0f)
	{
        return (x + width < -Futile.screen.halfWidth * cutoffScale ||  // left
                x - width > Futile.screen.halfWidth * cutoffScale ||   // right
                y - height > Futile.screen.halfHeight * cutoffScale || // bottom
                y + height < -Futile.screen.halfHeight * cutoffScale); // top
	}

    public static float ClampAngle(float angle)
    {
        angle %= 360;
        if (angle < 0) angle = 360 + angle;
        return angle;
    }
}