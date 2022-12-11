namespace Metroidvania {
    [System.Serializable]
    public struct RangedFloat {
        public float min, max;

        public float RandomRange() => UnityEngine.Random.Range(min, max);

        public RangedFloat(float min, float max) {
            this.min = min;
            this.max = max;
        }

        public bool Contains(float time) => time >= min && time <= max;
    }
}
