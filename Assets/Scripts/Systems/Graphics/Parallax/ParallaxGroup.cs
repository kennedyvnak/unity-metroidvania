using UnityEngine;
using System.Collections.Generic;

namespace Metroidvania.Graphics
{
    public class ParallaxGroup : MonoBehaviour
    {
        public readonly struct GroupObjectData
        {
            public readonly Transform t;
            public readonly ParallaxObjectData data;

            public GroupObjectData(Transform t, ParallaxObjectData data)
            {
                this.t = t;
                this.data = data;
            }
        }

        [SerializeField] private bool m_Recursive = false;
        private List<GroupObjectData> _group; // Transform and it start position

        private void Start()
        {
            _group = new List<GroupObjectData>();
            if (!m_Recursive)
            {
                foreach (Transform child in transform)
                {
                    AddTransform(child);
                }
            }
            else
            {
                GetChildTransformsRecursively(transform);
            }
        }

        private void LateUpdate()
        {
            _group.ForEach((unit) =>
            {
                unit.t.position = Parallax.GetParallaxPosition(unit.data, unit.t.position);
            });
        }

        private void GetChildTransformsRecursively(Transform parent)
        {
            AddTransform(parent);

            foreach (Transform child in parent)
            {
                GetChildTransformsRecursively(child);
            }
        }

        private bool CanParallax(Transform t)
        {
            return Mathf.Abs(t.position.z) - 0.1f > 0.0f;
        }

        private void AddTransform(Transform t)
        {
            if (CanParallax(t))
                _group.Add(new GroupObjectData(t, new ParallaxObjectData(t.transform.position)));
        }
    }
}