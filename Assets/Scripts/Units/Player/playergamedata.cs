namespace Metroidvania.Serialization
{
    [System.Serializable]
    public struct PlayerSafePointData
    {
        public string sceneGUID;
        public SerializableGuid pointGuid;
    }

    public partial class GameData
    {
        public bool isPlayerDied;
        public PlayerSafePointData lastPlayerSafePoint;

        public float playerLife;
    }
}