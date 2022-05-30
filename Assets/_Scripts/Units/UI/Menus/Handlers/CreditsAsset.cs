using System.Text;
using UnityEngine;

namespace Metroidvania.Credits
{
    // TODO: Localize credits
    [CreateAssetMenu(fileName = "new Credits", menuName = "Scriptable/Credits")]
    public class CreditsAsset : ScriptableObject
    {
        [System.Serializable]
        public struct ExternalAssetInfo
        {
            public string author;
            public string produtLabel;

            public string url;
        }


        [TextArea] public string header;

        public ExternalAssetInfo[] externalAssets;

        [TextArea] public string end;

        public string GenerateText()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(header);
            foreach (var externalAsset in externalAssets)
                sb.AppendLine($"{externalAsset.author}: <style=\"Link\"><link={externalAsset.url}>{externalAsset.produtLabel}</link></style>");
            sb.Append('\n');
            sb.AppendLine(end);
            return sb.ToString();
        }
    }
}