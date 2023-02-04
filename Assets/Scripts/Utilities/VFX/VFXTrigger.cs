using GGJ.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXTrigger : MonoBehaviour
{
    [SerializeField] private Transform _targetTransform;
    [SerializeField] private VFX _vfxType;

    [Header("Spin Attack")]
    [SerializeField] private Transform _playerAnimatorContainer;
    [SerializeField] private Transform _spinAttackAnchor;
    private VFX _spinAttackType = VFX.SPIN_ATTACK;


    [ContextMenu("TriggerVFX")]
    void TriggerVFX()
    {
        TriggerEffectAtGameobject(_targetTransform, _vfxType);
    }

    [ContextMenu("TriggerParentedVFX")]
    void TriggerParentedVFX()
    {
        try
        {
            TriggerEffectUnderParent(_targetTransform, _vfxType, _targetTransform);
        }
        catch (UnassignedReferenceException e)
        {
            TriggerEffectUnderParent(transform, _vfxType, null);
        }
    }

    [ContextMenu("TriggerSpinAttack")]
    void TriggerSpinAttack()
    {
        Animator animator = _playerAnimatorContainer.GetComponent<Animator>();
        //animator.SetBool("Do Attack", true);
        animator.Play("Spin_Attack");

        TriggerEffectUnderParent(_playerAnimatorContainer, _spinAttackType, _spinAttackAnchor);
    }

    public void TriggerEffectAtGameobject(Transform transform, VFX vfxType)
    {
        Vector3 targetPosition = Vector3.zero;

        if(transform != null) { targetPosition = transform.position; }

        try
        {
            VFXManager.CreateVFX(vfxType, targetPosition);
        }
        catch (NullReferenceException e)
        {
            Debug.Log("VFXManager not set\n" + e);
        }
    }

    public void TriggerEffectUnderParent(Transform transform, VFX vfxType, Transform parent)
    {
        try
        {
            VFXManager.CreateVFX(vfxType, transform.position, parent);
        }
        catch (NullReferenceException e)
        {
            Debug.Log("VFXManager not set\n" + e);
        }
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            TriggerSpinAttack();
        }
    }
}
