using System;
using System.Collections.Generic;
using UnityEngine;

namespace GGJ.Utilities
{
    public enum VFX
    {
        NONE,
        HIT_EFFECT,
        SPIN_EFFECT,
        
    }
    
    public class VFXManager : MonoBehaviour
    {
        //============================================================================================================//
        [Serializable]
        public struct VFXData
        {
            public string name;
            public VFX type;
            public GameObject prefab;
            public float lifetime;
        }

        //============================================================================================================//
        
        private static VFXManager _instance;

        public static GameObject CreateVFX(VFX vfx, Vector3 worldPosition)
        {
            return _instance.TryCreateVFX(vfx, worldPosition);
        }

        //============================================================================================================//
        
        [SerializeField]
        private VFXData[] vfx;

        private Dictionary<VFX, VFXData> _vfxDatas;

        //Unity Functions
        //============================================================================================================//

        private void Awake()
        {
            _instance = this;
        }

        private void Start()
        {
            foreach (var vfxData in vfx)
            {
                //todo fill dictionary
            }
        }
        //============================================================================================================//
        
        private GameObject TryCreateVFX(VFX vfx, Vector3 worldPosition)
        {
            throw new NotImplementedException();
        }
        //============================================================================================================//
    }
}