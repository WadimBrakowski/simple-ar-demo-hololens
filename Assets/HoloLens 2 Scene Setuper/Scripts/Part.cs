using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.Utilities;
using UnityEngine;

namespace HoloLens_2_Scene_Setuper
{
    public class Part : MonoBehaviour
    {
        private GameObject _part;
        
        private Vector3 _position;
        
        private GameObject _parentForParts;
        private Vector3 _parentPosition;
        private GridObjectCollection _gridObjectCollection;
        
        private Shader _shader;

        public Part(GameObject gameObject, Vector3 vector3)
        {
            _part = gameObject;
            _position = vector3;

            CreateParentObjectForPart();
            
        }
        
        GameObject CreateParentObjectForPart()
        {
            _parentForParts = new GameObject("Container for parts");
            _parentForParts.transform.position = _parentPosition;

            return _parentForParts;
        }
    }
}
