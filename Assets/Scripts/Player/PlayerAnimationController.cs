using System;
using UnityEngine;

namespace GGJ.Player
{
    public enum ANIMATION
    {
        NONE,
        IDLE,
        MOVE,
        ATTACK
    }
    
    public class PlayerAnimationController : MonoBehaviour
    {
        private static readonly int IDLE_ANIMATION = Animator.StringToHash("Idle");
        private static readonly int MOVE_ANIMATION = Animator.StringToHash("Idle");
        private static readonly int ATTACK_ANIMATION = Animator.StringToHash("Idle");

        [SerializeField]
        private Animator animator;

        //============================================================================================================//
        
        public void Play(ANIMATION animation)
        {
            int targetAnimation;
            switch (animation)
            {
                case ANIMATION.NONE:
                    return;
                case ANIMATION.IDLE:
                    targetAnimation = IDLE_ANIMATION;
                    break;
                case ANIMATION.MOVE:
                    targetAnimation = MOVE_ANIMATION;
                    break;
                case ANIMATION.ATTACK:
                    targetAnimation = ATTACK_ANIMATION;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(animation), animation, null);
            }
            animator.Play(targetAnimation);
        }
    }
}