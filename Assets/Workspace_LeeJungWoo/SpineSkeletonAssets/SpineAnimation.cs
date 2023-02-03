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

    protected AnimState currentAnimState;                   //현재 애니메이션                                 
    private string currentAnimation;                        //현재 애니메이션 이름

    [Header("애니메이션 스켈레톤")]
    public SkeletonAnimation skeletonAnimation;
    [Header("애니메이션 에셋배열")]
    public AnimationReferenceAsset[] animClip;

    protected virtual void SetCurrentAnimation(AnimState state, bool loop, float timeScale)//주어진 애니메이션 상태 상태에 따라 애니메이션 재생
    {
        AsyncAnimation(animClip[(int)state], loop, timeScale);
    }

    protected virtual void AsyncAnimation(AnimationReferenceAsset animClip, bool loop, float timeScale)//주어진 조건에 맞춰 애니메이션 동기화
    {
        if (animClip.name.Equals(currentAnimation))
            return;
        skeletonAnimation.state.SetAnimation(0, animClip, loop).TimeScale = timeScale;
        skeletonAnimation.loop = loop;
        skeletonAnimation.timeScale = timeScale;
        currentAnimation = animClip.name;
    }
}
