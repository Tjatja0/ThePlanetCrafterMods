using BepInEx;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using Unity.Mathematics;
using UnityEngineInternal;
using static UnityEngine.GraphicsBuffer;

namespace SpaceCraft
{
    [RequireComponent(typeof(WorldUniqueId))]
    public class setInterpolationPath : SceneInterpolationPath
    {
        //private List<Vector3> _path;
        [SerializeField]
        private List<GameObject> _objectRoute;
        private List<Vector3> _path;
        private GameObject _base;
        List<Quaternion> rotations = new List<Quaternion>();

        protected override float ComputeNormalizedSpeed(float speed)
        {
            _path = (List<Vector3>)this.GetType().BaseType.GetField("_path", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(this);
            float _totalDistance = (float)this.GetType().BaseType.GetField("_totalDistance", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(this);
            _base = (GameObject)this.GetType().BaseType.GetField("_startingBlock", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance).GetValue(this);
            //base.TryGetComponent<List<Vector3>>(out List<Vector3> _pathing);
            _base.transform.GetPositionAndRotation(out Vector3 globalPos, out Quaternion globalRot);
            for (int i = 0; i < (_objectRoute.Count - 1); i++)
            {
                //_pathing[i] = globalPos + (globalPos - _objectRoute[i].transform.position);
                rotations.Add(_objectRoute[i].transform.rotation);
            }

            if (rotations.Count < 2)
            {
                return 0f;
            }
            _totalDistance = 0f;
            base.transform.rotation = rotations[0];
            for (int i = 1; i < rotations.Count; i++)
            {
                _totalDistance += Quaternion.Angle(rotations[i - 1], rotations[i]);
            }
            return speed / _totalDistance;
        }
        
        protected override void UpdatePositionAndDirection(float value, short direction)
        {
            
            value *= 0.999999f; // <- so value is never 1 => currentStepIndex is never rotations.Count => rotations[currentStepIndex + 1] should never crash
            int currentStepIndex = (int)(value * (rotations.Count - 1));
            float currentStepProgress = (value - (currentStepIndex * (1.0f/(rotations.Count - 1)))) * (rotations.Count - 1);

            this.transform.rotation = Quaternion.Lerp(rotations[currentStepIndex], rotations[currentStepIndex + 1], currentStepProgress);
        }
        
    }
}
