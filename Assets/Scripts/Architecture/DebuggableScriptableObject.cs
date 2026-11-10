using UnityEngine;

namespace MetroPlatform
{
    public class DebuggableScriptableObject : ScriptableObject
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
