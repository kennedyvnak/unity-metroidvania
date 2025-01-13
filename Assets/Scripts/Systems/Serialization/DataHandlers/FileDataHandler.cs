using System;
using System.IO;
using UnityEngine;

namespace Metroidvania.Serialization.Handlers
{
    public class FileDataHandler : DataHandler
    {
        public virtual string dataPath => Application.persistentDataPath;

        public override GameData Deserialize(int userId)
        {
            GameData data = null;
            string path = GetFilePath(userId);

            if (File.Exists(path))
            {
                try
                {
                    using FileStream stream = new FileStream(path, FileMode.Open);
                    using StreamReader reader = new StreamReader(stream);
                    data = JsonUtility.FromJson<GameData>(EncryptDecrypt(reader.ReadToEnd()));
                }
                catch (Exception e)
                {
                    GameDebugger.LogError($"Caught an error trying to deserialize data file '{path}'.\n{e}");
                }
                data.userId = userId;
            }
            return data;
        }

        public override void Serialize(GameData data, int slot)
        {
            string path = GetFilePath(slot);

            try
            {
                using FileStream stream = new FileStream(path, FileMode.Create);
                using StreamWriter writer = new StreamWriter(stream);
                writer.Write(EncryptDecrypt(JsonUtility.ToJson(data)));
            }
            catch (Exception e)
            {
                GameDebugger.LogError($"An error was detected when trying to serialize data to a file in '{path}'.\n{e}");
            }
        }

        public override void DeleteUser(int userId) => File.Delete(GetFilePath(userId));

        public virtual string GetGlobalDataPath() => Path.Combine(dataPath, "game.data");

        public virtual string GetFilePath(int userId) => Path.Combine(dataPath, $"{string.Format(FileName, userId)}.{FileExtension}");

        public override bool HaveUser(int userID) => File.Exists(GetFilePath(userID));
    }
}