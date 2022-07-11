using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace com.outrealxr.networkimages
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
            else if(current != null)
            {
                Debug.LogWarning("[NetworkImageQueue] unable to dequeue: there is already an image downloading. Next attempt after that one.");
            } 
            else if(queue.Count == 0)
            {
                Debug.LogWarning("[NetworkImageQueue] Nothing to dequeue");
            }
            else
            {
                Debug.LogWarning("[NetworkImageQueue] Uknown reason");
            }
        }

        IEnumerator GetTexture()
        {
            if (current.url.Contains(".mp4")) current.url = current.url.Replace(".mp4", ".jpg");
            else if (current.url.Contains(".m3u8")) current.url = current.url.Replace(".m3u8", ".jpg");
            Debug.Log($"[NetworkImageQueue] Dequeued ${current}");
            using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(current.url))
            {
                yield return uwr.SendWebRequest();

                if (uwr.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogWarning($"[NetworkImageQueue] Error while downloading {current}: {uwr.error}");
                }
                else
                {
                    current.SetTexture(DownloadHandlerTexture.GetContent(uwr));
                }
                current = null;
                TryNext();
            }
        }
    }
}