using UnityEngine;

namespace com.outrealxr.networkimages
{
    public class NetworkImageQueueObserver : MonoBehaviour
    {
        public TMPro.TextMeshProUGUI text;

        void OnEnable()
        {
            NetworkImageQueue.instance.observers.Add(this);
        }

        public void OnNotify(string msg)
        {
            text.text = msg;
            text.enabled = !string.IsNullOrWhiteSpace(msg);
        }

        void OnDisable()
        {
            NetworkImageQueue.instance.observers.Remove(this);
        }
    }
}