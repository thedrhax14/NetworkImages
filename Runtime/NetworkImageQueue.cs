using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace com.outrealxr.networkimages.Runtime
{
    public class NetworkImageQueue : MonoBehaviour
    {
        Queue<NetworkImage> queue = new Queue<NetworkImage>();
        NetworkImage current;

        public static NetworkImageQueue instance;

        private void Awake()
        {
            instance = this;
        }

        public void Enqueue(NetworkImage networkImage)
        {
            if(queue.Contains(networkImage))
            {
                Debug.LogWarning($"[NetworkImageQueue] Already queued ${networkImage}. Skipped");
                return;
            }
            Debug.Log($"[NetworkImageQueue] Queued ${networkImage}");
            queue.Enqueue(networkImage);
            TryNext();
        }

        void TryNext()
        {
            if (current == null && queue.Count > 0)
            {
                current = queue.Dequeue();
                StartCoroutine(GetTexture());
            }
        }

        IEnumerator GetTexture()
        {
            Debug.Log($"[NetworkImageQueue] Dequeued ${current}");
            using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(current.url))
            {
                yield return uwr.SendWebRequest();

                if (uwr.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError($"[NetworkImageQueue] Error while downloading {current}: {uwr.error}");
                }
                else
                {
                    current.SetTexture(DownloadHandlerTexture.GetContent(uwr));
                    current = null;
                    TryNext();
                }
            }
        }
    }
}