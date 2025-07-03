using UnityEngine;

namespace DarkSave.Runtime
{
    public class Saver : MonoBehaviour
    {
        private ISaveSystem _saveSystem;

        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }

        public void Initialize(ISaveSystem saveSystem)
        {
            _saveSystem = saveSystem;
        }
        
        public void Launch(float timePeriod)
        {
            InvokeRepeating(nameof(SavePeriodic), timePeriod, timePeriod);
        }

        public void Stop()
        {
            CancelInvoke(nameof(SavePeriodic));
        }

        private void SavePeriodic()
        {
            _saveSystem.Save();
        }
    }
}

