using GGJ.Utilities;
using GGJ.Utilities.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GGJ.Utilities
{
    public class FileModelLibrary : MonoBehaviour
    {
        private static FileModelLibrary _instance;

        public static GameObject GetModel()
        {
            // get random model from list
            return _instance.TryGetModel();
        }

        [SerializeField]
        private List<GameObject> prefabList;

        //Unity Functions
        //============================================================================================================//

        private void Awake()
        {
            _instance = this;
        }

        //============================================================================================================//

        private GameObject TryGetModel()
        {
            // get random model from list
            GameObject newModel = Instantiate(prefabList.GetRandomItem());
            return newModel;
        }
    }
}