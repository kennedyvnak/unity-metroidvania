using TMPro;
using UnityEngine;

namespace Metroidvania.UI.Menus
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI versionText;
        
        private void Awake()
        {
            versionText.text = Application.version;
        }
    }
}