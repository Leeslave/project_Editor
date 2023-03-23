using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class SpineAnimation : MonoBehaviour
{
    public enum AnimState
    {
        Idle
    }

    protected AnimState currentAnimState;                   //현재 애니메이션 상태                           
    private string currentAnimation;                        //현재 애니메이션 이름

    [Header("스파인 스켈리톤 애니메이션")]
    public SkeletonAnimation skeletonAnimation;
    [Header("애니메이션 클립")]
    public AnimationReferenceAsset[] animClip;

    protected virtual void SetCurrentAnimation(AnimState state, bool loop, float timeScale)
    {
        AsyncAnimation(animClip[(int)state], loop, timeScale);
    }

    protected virtual void AsyncAnimation(AnimationReferenceAsset animClip, bool loop, float timeScale)
    {
        if (animClip.name.Equals(currentAnimation))
            return;
        skeletonAnimation.state.SetAnimation(0, animClip, loop).TimeScale = timeScale;
        skeletonAnimation.loop = loop;
        skeletonAnimation.timeScale = timeScale;
        currentAnimation = animClip.name;
    }
}
