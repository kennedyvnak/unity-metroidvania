namespace Metroidvania.Serialization {
    [System.Serializable]
    public partial class GameData : System.ICloneable {
        public int userId;
        public long lastSerialization;

        public GameData Clone() => MemberwiseClone() as GameData;
        object System.ICloneable.Clone() => Clone();
    }
}
