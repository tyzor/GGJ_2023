using System;
using GGJ.Animations;
using GGJ.Player;
using UnityEngine;

namespace GGJ.Enemies
{
    public class RangedEnemyAnimationController : MonoBehaviour
    {
        private static readonly int IDLE_ANIMATION = Animator.StringToHash("Idle");
        private static readonly int MOVE_ANIMATION = Animator.StringToHash("Walk");
        private static readonly int ATTACK_ANIMATION = Animator.StringToHash("Fire");

        [SerializeField]
        private Animator animator;

        private RangedEnemy _rangedEnemy;

        private ANIMATION _currentAnimation;

        //Unity Functions
        //============================================================================================================//

        private void Start()
        {
            _rangedEnemy = GetComponent<RangedEnemy>();
        }

        private void Update()
        {
            switch (_rangedEnemy.enemyState)
            {
                case RangedEnemy.EnemyState.Idle:
                    Play(ANIMATION.IDLE);
                    break;
                case RangedEnemy.EnemyState.Pursuit:
                case RangedEnemy.EnemyState.Retreat:
                    Play(ANIMATION.MOVE);
                    break;
                case RangedEnemy.EnemyState.cooldown:
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        //============================================================================================================//
        
        public void Play(ANIMATION animation)
        {
            if (animation == _currentAnimation)
                return;
            
            _currentAnimation = animation;
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