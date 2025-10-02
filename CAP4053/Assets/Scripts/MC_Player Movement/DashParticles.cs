using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

public class DashParticles : MonoBehaviour
{
    [SerializeField] private ParticleSystem dashPS;
    [SerializeField] private float startFrame;
    private List<Sprite> dashSpriteList = new List<Sprite>();

    public float addCheckSprite(Sprite newSprite)
    {
        int index = -1;
        if (dashSpriteList.Contains(newSprite))
        {
            index = dashSpriteList.IndexOf(newSprite);
        }
        else
        {
            dashSpriteList.Add(newSprite);
            index = dashSpriteList.Count - 1;
            dashPS.textureSheetAnimation.AddSprite(newSprite);
        }
        return (startFrame);
    }
    public void setSprite(float index)
    {
        var textureSheet = dashPS.textureSheetAnimation;
        textureSheet.startFrame = index;
    }
    public void flipXSprite(bool flipped)
    {
        float flipFloat = (flipped) ? 0.0f : math.PI;
        var main = dashPS.main;
        main.startRotationY = flipFloat;
    }
    public void playStop(bool play)
    {
        if (play)
        {
            dashPS.Play();
        }
        else
        {
            dashPS.Stop();
        }
    }
}
