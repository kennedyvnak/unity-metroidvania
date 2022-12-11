namespace Metroidvania {
    [System.Serializable]
    public struct RangedInt {
        public int min, max;

        public int RandomRange() => UnityEngine.Random.Range(min, max + 1);

        public RangedInt(int min, int max) {
            this.min = min;
            this.max = max;
        }

        public bool Contains(float time) => time >= min && time <= max;
    }
}
