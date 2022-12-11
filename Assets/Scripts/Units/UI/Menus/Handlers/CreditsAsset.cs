using System.Text;
using UnityEngine;

namespace Metroidvania.Credits {
    // TODO: Localize credits
    [CreateAssetMenu(fileName = "new Credits", menuName = "Scriptables/Credits")]
    public class CreditsAsset : ScriptableObject {
        [System.Serializable]
        public struct ExternalAssetInfo {
            public string author;

            [UnityEngine.Serialization.FormerlySerializedAs("produtLabel")]
            public string productLabel;

            public string url;
        }


        [TextArea] public string header;

        public ExternalAssetInfo[] externalAssets;

        [TextArea] public string end;

        public string GenerateText() {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(header);
            foreach (ExternalAssetInfo externalAsset in externalAssets)
                sb.AppendLine($"{externalAsset.author}: <style=\"Link\"><link={externalAsset.url}>{externalAsset.productLabel}</link></style>");
            sb.Append('\n');
            sb.AppendLine(end);
            return sb.ToString();
        }
    }
}