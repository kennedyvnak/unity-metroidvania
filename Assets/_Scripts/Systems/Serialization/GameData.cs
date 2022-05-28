using UnityEngine;

namespace Metroidvania.Serialization
{
    [System.Serializable]
    public partial class GameData
    {
        public int userId;
        public long lastSerialization;

        public GameData(int userId)
        {
            this.userId = userId;
        }
    }
}
