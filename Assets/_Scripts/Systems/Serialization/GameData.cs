using UnityEngine;

namespace Metroidvania.Serialization
{
    [System.Serializable]
    public partial class GameData : System.ICloneable
    {
        public int userId;
        public long lastSerialization;

        public GameData(int userId)
        {
            this.userId = userId;
        }

        public GameData Clone() => this.MemberwiseClone() as GameData;
        object System.ICloneable.Clone() => Clone();
    }
}
