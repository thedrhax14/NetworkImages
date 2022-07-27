using UnityEngine;

namespace com.outrealxr.networkimages
{
    public abstract class NetworkImage : MonoBehaviour
    {

        internal string url;

        public virtual void SetAndEnqueue(string url)
        {
            this.url = url;
            NetworkImageQueue.instance.Enqueue(this);
        }

        /// <summary>
        /// Make sure to clear existing texture after setting new one
        /// </summary>
        /// <param name="texture"></param>
        public abstract void SetTexture(Texture texture);

        public override int GetHashCode()
        {
            return gameObject.name.GetHashCode();
        }

        public override bool Equals(object other)
        {
            return other.GetType() == typeof(NetworkImage) && ((NetworkImage) other).gameObject.GetInstanceID().Equals(GetInstanceID());
        }

        public override string ToString()
        {
            return gameObject.name + " (" + url + ")";
        }
    }
}