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
        TriggerEffectTypeAtGameobject(_targetTransform, _vfxType);
    }

    private void TriggerEffectTypeAtGameobject(GameObject gameObject, VFX vfxType)
    {
        try
        {
            VFXManager.CreateVFX(vfxType, gameObject.transform.position);
        } catch (NullReferenceException e) {
            Debug.Log("VFXManager not set\n" + e);
        }
    }

}
