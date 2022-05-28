namespace Metroidvania.Serialization
{
    public interface IDataPersistance
    {
        void LoadData(GameData data);

        void SaveData(GameData data);
    }
}