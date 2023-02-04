using System;
using GGJ.Animations;
using UnityEngine;

namespace GGJ.Player
{
    
    
    [RequireComponent(typeof(PlayerMovementController), typeof(PlayerAttackController))]
    public class PlayerAnimationController : MonoBehaviour
    {
        private static readonly int IDLE_ANIMATION = Animator.StringToHash("Idle");
        private static readonly int MOVE_ANIMATION = Animator.StringToHash("Move");
        private static readonly int ATTACK_ANIMATION = Animator.StringToHash("Spin_Attack");

        [SerializeField]
        private Animator animator;

        private PlayerMovementController _playerMovementController;
        private PlayerAttackController _playerAttackController;

        private ANIMATION _currentAnimation;

        //Unity Functions
        //============================================================================================================//

        private void Start()
        {
            _playerMovementController = GetComponent<PlayerMovementController>();
            _playerAttackController = GetComponent<PlayerAttackController>();
        }

        private void Update()
        {
            if (_playerAttackController.IsCharging || _playerAttackController.IsAttacking)
                Play(ANIMATION.ATTACK);
            else if(_playerMovementController.IsMoving)
                Play(ANIMATION.MOVE);
            else
                Play(ANIMATION.IDLE);
        }

        //============================================================================================================//
        
        private void Play(ANIMATION animation)
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