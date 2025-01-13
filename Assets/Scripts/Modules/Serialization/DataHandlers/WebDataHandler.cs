using System.IO;
using System;
using System.Runtime.InteropServices;

namespace Metroidvania.Serialization.Handlers
{
    public class WebDataHandler : FileDataHandler
    {
        public override string dataPath => "idbfs/Metroidvania/";

        public override void Serialize(GameData data, int slot)
        {
            if (!File.Exists(dataPath)) Directory.CreateDirectory(dataPath);
            base.Serialize(data, slot);
            JS_FileSystem_Sync();
        }

#if UNITY_WEBGL
        [DllImport("__Internal")]
        private static extern void JS_FileSystem_Sync();
#else
        private void JS_FileSystem_Sync() { throw new NotImplementedException(); }
#endif
    }
}