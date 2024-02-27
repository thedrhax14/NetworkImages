using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace com.outrealxr.networkimages
{
    public class NetworkImageQueue : MonoBehaviour
    {
        public bool verboseLogging;
        Queue<NetworkImage> queue = new Queue<NetworkImage>();
        NetworkImage current;

        UnityWebRequest uwr;
        float timeout = 0;
        string lastMsg;

        public static NetworkImageQueue instance;

        public List<NetworkImageQueueObserver> observers = new();

        void Awake()
        {
            instance = this;
        }

        void Update()
        {
            string msg = current != null && uwr != null ? "Loading (isDone: " + uwr.isDone + " - " + (timeout - Time.time).ToString("00.00") + ") " + current : "";
            if (lastMsg.Equals(msg)) return;
            OnQueueChanged(msg);
        }

        void OnQueueChanged(string msg)
        {
            if (lastMsg.Equals(msg)) return;
            foreach (var observer in observers) observer.OnNotify(msg);
            lastMsg = msg;
        }

        public void Enqueue(NetworkImage networkImage)
        {
            if (string.IsNullOrWhiteSpace(networkImage.url))
            {
                Debug.LogWarning($"[NetworkImageQueue] networkImage {networkImage.gameObject.name} skipped because url is empty. Clearing texture...");
                networkImage.SetTexture(null);
                return;
            }
            if (!networkImage.url.StartsWith("https"))
            {
                Debug.LogError($"[NetworkImageQueue] current image is not hosted properly online: {networkImage}");
                return;
            }
            if (!networkImage.url.ToLower().EndsWith("jpg") && !networkImage.url.ToLower().EndsWith("jpeg") && !networkImage.url.ToLower().EndsWith("png"))
            {
                Debug.LogError($"[NetworkImageQueue] current image format is not supported: {networkImage}");
                return;
            }
            if (verboseLogging) Debug.Log($"[NetworkImageQueue] Queued ${networkImage}");
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
                if(verboseLogging) Debug.LogWarning($"[NetworkImageQueue] unable to dequeue: there is already an image {current} downloading. Next attempt after that one.");
            } 
            else if(queue.Count == 0)
            {
                Debug.LogWarning("[NetworkImageQueue] Nothing to dequeue");
                OnQueueChanged("");
            }
            else
            {
                Debug.LogWarning("[NetworkImageQueue] Uknown reason");
                OnQueueChanged("");
            }
        }

        IEnumerator GetTexture()
        {
            if(current == null)
            {
                Debug.LogWarning($"[NetworkImageQueue] Currently served image is not available any more.");
                TryNext();
                yield break;
            }
            if (string.IsNullOrWhiteSpace(current.url))
            {
                Debug.LogWarning($"[NetworkImageQueue] networkImage {current.gameObject.name} skipped because url is empty");
                current = null;
                TryNext();
                yield break;
            }
            timeout = Time.time + current.timeout;
            uwr = null;
            if (verboseLogging) Debug.Log($"[NetworkImageQueue] Dequeued ${current} as valid web image");
            using (uwr = UnityWebRequestTexture.GetTexture(current.url))
            {
                uwr.timeout = current.timeout;
                current.SetViewState(NetworkImage.State.Loading);
                yield return uwr.SendWebRequest();
                if (uwr.isDone)
                {
                    if (current != null)
                    {
                        if (uwr.result != UnityWebRequest.Result.Success)
                        {
                            current.SetViewState(NetworkImage.State.Error);
                            if (verboseLogging) Debug.LogError($"[NetworkImageQueue] Error while downloading {current}: {uwr.error}");
                        }
                        else
                        {
                            current.SetTexture(DownloadHandlerTexture.GetContent(uwr));
                        }
                        current = null;
                    }
                }
                else if (timeout < Time.time)
                {
                    if (verboseLogging) Debug.LogError($"[NetworkImageQueue] Timed out after {current.timeout} while downloading {current}");
                }
                TryNext();
            }
        }
    }
}