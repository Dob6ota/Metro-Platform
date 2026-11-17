using UnityEngine;

namespace MetroPlatform
{
    [CreateAssetMenu(fileName = "HeathStats", menuName = "Scriptable Objects/HeathStats")]
    public class HealthStats : ScriptableObject
    {
        public HealthBank MaxHealth;
    }
}
