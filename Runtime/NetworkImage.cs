using UnityEngine;

namespace com.outrealxr.networkimages
{
    public abstract class NetworkImage : MonoBehaviour
    {
        public enum State
        {
            None,
            Queued,
            Loading,
            Error
        }

        internal string url;

        [Header("Optional")]
        [Tooltip("Activated when actually queued and diactivated when dequeued and started loading")]
        public GameObject queued;
        [Tooltip("Activated when actually dequeued and started loading and diactivated when done loading or error")]
        public GameObject loading;
        [Tooltip("Activated when error happened while loading")]
        public GameObject error;

        public virtual void SetAndEnqueue(string url)
        {
            this.url = url;
            SetViewState(State.Queued);
            NetworkImageQueue.instance.Enqueue(this);
        }

        /// <summary>
        /// Make sure to clear existing texture after setting new one
        /// </summary>
        /// <param name="texture"></param>
        public virtual void SetTexture(Texture texture)
        {
            SetViewState(State.None);
        }

        public override int GetHashCode()
        {
            return gameObject.name.GetHashCode();
        }

        public override bool Equals(object other)
        {
            return other.GetType() == typeof(NetworkImage) && ((NetworkImage) other).gameObject.GetInstanceID().Equals(GetInstanceID());
        }

        public void SetViewState(State state)
        {
            if (queued) queued.SetActive(state == State.Queued);
            if (loading) loading.SetActive(state == State.Loading);
            if (error) error.SetActive(state == State.Error);
        }

        public override string ToString()
        {
            return gameObject.name + " (" + url + ")";
        }
    }
}