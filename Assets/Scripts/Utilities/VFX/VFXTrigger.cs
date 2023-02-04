using GGJ.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXTrigger : MonoBehaviour
{
    [SerializeField] private GameObject _targetTransform;
    [SerializeField] private VFX _vfxType;

    [ContextMenu("TriggerVFX")]
    void TriggerVFX()
    {
        TriggerEffectAtGameobject(_targetTransform, _vfxType);
    }

    [ContextMenu("TriggerParentedVFX")]
    void TriggerParentedVFX()
    {
        TriggerEffectUnderParent(_targetTransform, _vfxType, _targetTransform.transform);
    }

    public void TriggerEffectAtGameobject(GameObject gameObject, VFX vfxType)
    {
        try
        {
            VFXManager.CreateVFX(vfxType, gameObject.transform.position);
        }
        catch (NullReferenceException e)
        {
            Debug.Log("VFXManager not set\n" + e);
        }
    }

    public void TriggerEffectUnderParent(GameObject gameObject, VFX vfxType, Transform parent)
    {
        try
        {
            VFXManager.CreateVFX(vfxType, gameObject.transform.position, parent);
        }
        catch (NullReferenceException e)
        {
            Debug.Log("VFXManager not set\n" + e);
        }
    }

}
