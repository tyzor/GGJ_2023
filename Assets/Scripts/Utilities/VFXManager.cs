using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGJ.Utilities
{
    public enum VFX
    {
        NONE,
        TEMPLATE_EFFECT,
        HIT_EFFECT,
        SPIN_EFFECT,   
    }

    public enum EMITTER_ACTION
    {
        DESTROY,
        STOP,
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
            public EMITTER_ACTION emitterEOL; // emmiter end of life action
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

        private IEnumerator coroutineDestroyAfterLifetime;

        //Unity Functions
        //============================================================================================================//

        private void Awake()
        {
            _instance = this;
        }

        private void Start()
        {
            _vfxDatas = new Dictionary<VFX, VFXData>();
            foreach (var vfxData in vfx)
            {
                _vfxDatas.Add(vfxData.type, vfxData);
            }
        }
        //============================================================================================================//
        
        private GameObject TryCreateVFX(VFX vfx, Vector3 worldPosition)
        {
            // make sure the type is not NONE
            if (vfx == VFX.NONE) { return null; }

            VFXData data = _instance._vfxDatas[vfx];
            GameObject targetPrefab = data.prefab;
            GameObject newVfxObject = Instantiate(targetPrefab, worldPosition, Quaternion.identity, transform);

            // set vfx to destroy after lifetime
            coroutineDestroyAfterLifetime = SetVfxToDestroy(newVfxObject, data);
            StartCoroutine(coroutineDestroyAfterLifetime);

            SetVfxToDestroy(newVfxObject, data);
            return newVfxObject;
        }

        private IEnumerator SetVfxToDestroy(GameObject vfxObject, VFXData data)
        {
            yield return new WaitForSeconds(data.lifetime);

            // check if the particle emitters should stop or be destroyed immediately
            if (data.emitterEOL == EMITTER_ACTION.STOP)
            {
                // find all children that are emmiters and set them to stop
                foreach(Transform child in vfxObject.transform)
                {
                    ParticleSystem particleSystem = child.GetComponent<ParticleSystem>();
                    if(particleSystem != null)
                    {
                        particleSystem.Stop();
                        // remove them from their parent then set timer to destroy
                        particleSystem.transform.SetParent(transform);
                        Destroy(particleSystem.gameObject, data.lifetime);
                    }
                }
            }

            Destroy(vfxObject);
        }
        //============================================================================================================//
    }
}