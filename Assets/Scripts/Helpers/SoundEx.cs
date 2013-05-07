/* SoundEx Unity/Futile Sound Extension Library
 * © 2013 Adam K Dean / Imdsm
 * http://www.adamkdean.co.uk 
 */

public enum SoundEffect
{
    Explosion, Turret
}

public static class SoundEx
{    
    public static void PlaySound(SoundEffect soundEffect, float volume = 1.0f)
    {
        if (soundEffect == SoundEffect.Explosion)
            FSoundManager.PlaySound("explosion", volume);
        else if (soundEffect == SoundEffect.Turret)
            FSoundManager.PlaySound("turret", volume);
    }
}