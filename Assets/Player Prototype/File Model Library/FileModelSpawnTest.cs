using GGJ.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FileModelSpawnTest : MonoBehaviour
{
    [SerializeField] private Transform _targetTransform;

    [ContextMenu("TriggerGetModel")]
    void TriggerGetModel()
    {
        GetModel();
    }

    public void GetModel()
    {
        try
        {
            //============================================================================================================//
            GameObject newModel = FileModelLibrary.GetModel();
            //============================================================================================================//

            Vector3 targetPosition = Vector3.zero;
            Quaternion targetRotation = Quaternion.identity;
            // make sure _targetTransfrom is not null
            try
            {
                targetPosition = _targetTransform.position;
                targetRotation = _targetTransform.rotation;
            }
            catch (UnassignedReferenceException e)
            {
                Debug.Log("TargetTransform not set\n" + e);
            }

            // move model position and rotation
            newModel.transform.position = targetPosition;
            newModel.transform.rotation = targetRotation;
        }
        catch (NullReferenceException e)
        {
            Debug.Log("FileModelLibrary not set\n" + e);
        }
    }
}
