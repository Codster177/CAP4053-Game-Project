using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyAnimator : MonoBehaviour
{
    [SerializeField] private Animator animatorController;
    [SerializeField] private List<AnimationObject> animationList;
    private AnimationObject animationObject;

    public void ChangeAnimation(string animationName, bool overridePlayFull = false)
    {
        if (animationObject != null)
        {
            if (animationObject.playFull && !overridePlayFull)
            {
                return;
            }
        }
        AnimationObject foundAnimation = GetAnimationFromName(animationName);
        if (foundAnimation == null)
        {
            Debug.LogError($"{gameObject.name}: EnemyAnimator: Called nonexistant animation.");
            return;
        }
        if (!AttemptPlay(foundAnimation))
        {
            return;
        }
        if (foundAnimation.playFull || foundAnimation.nextAnimationName != "")
        {
            QueueAnimation(foundAnimation.nextAnimationName);
        }
    }
    public string CheckCurrentAnimation()
    {
        return animationObject.name;
    }
    private bool AttemptPlay(AnimationObject newAnimation)
    {
        if (animationObject != null && animationObject.name == newAnimation.name)
        {
            return false;
        }
        else
        {
            animatorController.Play(newAnimation.name);
            animationObject = newAnimation;
            return true;
        }
    }
    private void QueueAnimation(string nextAnimationName)
    {
        float curAnimationLength = animatorController.GetCurrentAnimatorClipInfo(0)[0].clip.length;
        StartCoroutine(WaitToQueue(nextAnimationName, curAnimationLength));
    }
    private IEnumerator WaitToQueue(string nextAnimationName, float curAnimationLength)
    {
        yield return new WaitForSeconds(curAnimationLength);
        if (nextAnimationName == "")
        {
            animationObject = null;
        }
        else
        {
            ChangeAnimation(nextAnimationName, true);
        }
    }
    private AnimationObject GetAnimationFromName(string animationName)
    {
        for (int i = 0; i < animationList.Count; i++)
        {
            if (animationName == animationList[i].name)
            {
                return animationList[i];
            }
        }
        return null;
    }
    [Serializable]
    private class AnimationObject
    {
        public string name;
        public bool playFull = false;
        public string nextAnimationName = "";
    }
}
