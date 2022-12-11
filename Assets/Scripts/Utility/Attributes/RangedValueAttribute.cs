namespace Metroidvania {
    public class RangedValueAttribute : UnityEngine.PropertyAttribute {
        public readonly float min, max;

        public RangedValueAttribute(float min, float max) {
            this.min = min;
            this.max = max;
        }
    }
}