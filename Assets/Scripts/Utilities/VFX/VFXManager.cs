using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGJ.Utilities
{
    public enum VFX
    {
        NONE = 0,
        //TEMPLATE_EMITTER_STOP = 1,
        //TEMPLATE_EMITTER_DESTROY = 2,
        SPIN_CHARGE = 3,
        SPIN_ATTACK = 4,
        HIT_EFFECT = 5,
        //SPIN_ATTACK2 = 6,
        //SPIN_ATTACK3 = 7
    }

    public enum EMITTER_ACTION
    {
        DESTROY,
        STOP,
        NOTHING
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

        // function to attach an effect to a specific parent - example: for spin attack rotation
        public static GameObject CreateVFX(VFX vfx, Vector3 worldPosition, Transform parent)
        {
            return _instance.TryCreateVFX(vfx, worldPosition, parent);
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
            return TryCreateVFX(vfx, worldPosition, null);
        }

        private GameObject TryCreateVFX(VFX vfx, Vector3 worldPosition, Transform customParent)
        {
            // make sure the type is not NONE
            if (vfx == VFX.NONE) { return null; }

            VFXData data = _instance._vfxDatas[vfx];
            GameObject targetPrefab = data.prefab;
            Transform vfxParentTransform = transform;
            if(customParent != null) { vfxParentTransform = customParent; }
            GameObject newVfxObject = Instantiate(targetPrefab, worldPosition, Quaternion.identity, vfxParentTransform);

            switch (data.emitterEOL)
            {
                case EMITTER_ACTION.DESTROY:
                    Destroy(newVfxObject, data.lifetime);
                    break;
                case EMITTER_ACTION.STOP:
                    StartCoroutine(StopAfterCoroutine(newVfxObject, data));
                    break;
                case EMITTER_ACTION.NOTHING:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return newVfxObject;
        }

        private IEnumerator StopAfterCoroutine(GameObject vfxObject, VFXData data)
        {
            var particles = vfxObject.GetComponent<ParticleSystem>();
            
            if(particles == null)
                yield break;
            
            yield return new WaitForSeconds(data.lifetime);
            
            particles.Stop(true);

            yield return new WaitUntil(() => particles.particleCount == 0);
            
            Destroy(vfxObject);
        }
        //============================================================================================================//
    }
}