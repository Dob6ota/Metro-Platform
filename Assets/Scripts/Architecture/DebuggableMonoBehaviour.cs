using UnityEngine;

namespace MetroPlatform
{
    public class DebuggableMonoBehaviour : MonoBehaviour
    {
        [SerializeField] protected bool Debugging;

        protected void ConditDebugLog(string debugMessage)
        {
            if (Debugging)
            {
                Debug.Log(debugMessage);
            }
        }
    }
}
