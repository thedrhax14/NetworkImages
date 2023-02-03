using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace com.outrealxr.networkimages
{
    public class NetworkImageQueue : MonoBehaviour
    {
        public TMPro.TextMeshProUGUI text;
        Queue<NetworkImage> queue = new Queue<NetworkImage>();
        NetworkImage current;

        public static NetworkImageQueue instance;

        private void Awake()
        {
            instance = this;
        }

        private void Update()
        {
            if (current != null && uwr != null) text.text = $"Loading (isDone: {uwr.isDone} - {timeout - Time.time:00.00}) {current}";
            else text.gameObject.SetActive(false);
        }

        public void Enqueue(NetworkImage networkImage)
        {
            if (string.IsNullOrWhiteSpace(networkImage.url))
            {
                Debug.LogWarning($"[NetworkImageQueue] networkImage {networkImage.gameObject.name} skipped because url is empty");
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
                Debug.LogWarning($"[NetworkImageQueue] unable to dequeue: there is already an image {current} downloading. Next attempt after that one.");
            } 
            else if(queue.Count == 0)
            {
                Debug.LogWarning("[NetworkImageQueue] Nothing to dequeue");
                text.text = "";
            }
            else
            {
                Debug.LogWarning("[NetworkImageQueue] Uknown reason");
                text.text = "";
            }
        }

        UnityWebRequest uwr;
        float timeout = 0;

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
            text.gameObject.SetActive(true);
            if (current.url.StartsWith("http"))
            {
                Debug.Log($"[NetworkImageQueue] Dequeued ${current} as web image");
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
                                Debug.LogError($"[NetworkImageQueue] Error while downloading {current}: {uwr.error}");
                            }
                            else
                            {
                                current.SetTexture(DownloadHandlerTexture.GetContent(uwr));
                            }
                            current = null;
                        }
                    }
                    else if(timeout < Time.time)
                    {
                        Debug.LogError($"[NetworkImageQueue] Timed out after {current.timeout} while downloading {current}");
                    }
                    TryNext();
                }
            }
            else
            {
                Debug.Log($"[NetworkImageQueue] Dequeued ${current} as addressable image");
                AsyncOperationHandle<IList<IResourceLocation>> locationsHandle = Addressables.LoadResourceLocationsAsync(current.url);
                yield return locationsHandle;
                AsyncOperationHandle<Texture2D> handle;
                if (locationsHandle.Result.Count > 0)
                {
                    handle = Addressables.LoadAssetAsync<Texture2D>(current.url);
                    yield return handle;
                    Debug.Log($"[AddressableAvatarOperation] Loaded {current.url}");
                    current.SetTexture(handle.Result);
                }
                else
                {
                    Debug.Log($"[NetworkImageQueue] {current.url} is missing.");
                }
            }
        }
    }
}