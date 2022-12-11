using UnityEngine;

namespace Michsky.UI.ModernUIPack
{
    public class LaunchURL : MonoBehaviour
    {
        public string URL; //URL link that will be opened - is set from Unity inspector

        public void urlLinkOrWeb()
        {
            Application.OpenURL(URL);
        }
    }
}