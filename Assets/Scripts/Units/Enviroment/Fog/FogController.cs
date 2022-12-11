using UnityEngine;

namespace Metroidvania.Environment.Fog {
    public class FogController : MonoBehaviour {
        [System.Serializable]
        public struct FogPlatform {
            public float a;
            public float b;

            public float y;
        }

        [SerializeField] private FogPlatform[] m_FogPlatforms;
        [SerializeField, Min(0.25f)] private float m_ParticlesDistance = 1f;

        [SerializeField] private float m_StartOffset = 0.5f;
        [SerializeField] private float m_EndOffset = -0.5f;
        [SerializeField] private float m_YOffset = -0.1f;

        private ParticleSystem particles { get; set; }
        private ParticleSystem.EmitParams emitParams;

        public float startOffset => m_StartOffset;
        public float endOffset => m_EndOffset;
        public float yOffset => m_YOffset;

        private void Awake() {
            particles = GetComponentInChildren<ParticleSystem>();
        }

        private void OnEnable() {
            CreateParticles();
        }

        public void EmitFogParticle(Vector2 position) {
            emitParams.position = position;
            particles.Emit(emitParams, 1);
        }

        public FogPlatform[] GetPlatforms() => m_FogPlatforms;
        public FogPlatform[] SetPlatforms(FogPlatform[] value) => m_FogPlatforms = value;

        public void CreateParticles() {
            if (!particles)
                return;

            for (int i = 0; i < m_FogPlatforms.Length; i++)
                ForEachPlatformPoint(m_FogPlatforms[i], EmitFogParticle);
        }

        public void ForEachPlatformPoint(FogPlatform platform, System.Action<Vector2> action) {
            GetPlatformStartEnd(platform, out float start, out float end, out float y);

            for (float x = start; x <= end; x += m_ParticlesDistance)
                action(new Vector2(x, y));
        }

        public void GetPlatformStartEnd(FogPlatform platform, out float startX, out float endX, out float y) {
            startX = Mathf.Min(platform.a, platform.b);
            endX = Mathf.Max(platform.a, platform.b);

            startX = Mathf.Clamp(startX + startOffset, startX, endX);
            endX = Mathf.Clamp(endX + endOffset, startX, endX);
            y = platform.y + yOffset;
        }

#if UNITY_EDITOR
        [ContextMenu("Round All Points")]
        public void RoundAllPoints() {
            UnityEditor.Undo.RecordObject(this, "Round Platforms Points");
            for (int i = 0; i < m_FogPlatforms.Length; i++) {
                FogPlatform platform = m_FogPlatforms[i];
                platform.a = Mathf.Round(platform.a);
                platform.b = Mathf.Round(platform.b);
                platform.y = Mathf.Round(platform.y);
                m_FogPlatforms[i] = platform;
            }
        }
#endif
    }
}
