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

    protected AnimState currentAnimState;                   //���� �ִϸ��̼�                                 
    private string currentAnimation;                        //���� �ִϸ��̼� �̸�

    [Header("�ִϸ��̼� ���̷���")]
    public SkeletonAnimation skeletonAnimation;
    [Header("�ִϸ��̼� ���¹迭")]
    public AnimationReferenceAsset[] animClip;

    protected virtual void SetCurrentAnimation(AnimState state, bool loop, float timeScale)//�־��� �ִϸ��̼� ���� ���¿� ���� �ִϸ��̼� ���
    {
        AsyncAnimation(animClip[(int)state], loop, timeScale);
    }

    protected virtual void AsyncAnimation(AnimationReferenceAsset animClip, bool loop, float timeScale)//�־��� ���ǿ� ���� �ִϸ��̼� ����ȭ
    {
        if (animClip.name.Equals(currentAnimation))
            return;
        skeletonAnimation.state.SetAnimation(0, animClip, loop).TimeScale = timeScale;
        skeletonAnimation.loop = loop;
        skeletonAnimation.timeScale = timeScale;
        currentAnimation = animClip.name;
    }
}
