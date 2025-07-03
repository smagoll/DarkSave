using UnityEngine;

namespace DarkSave.Runtime
{
    public class AutoSaveManager
    {
        private readonly ISaveSystem _saveSystem;

        private Saver _saver;

        public AutoSaveManager(ISaveSystem saveSystem)
        {
            _saveSystem = saveSystem;
        }
        
        public bool AutoSaveEnabled { get; private set; }
        
        public void Enable(float timePeriodic = 15f)
        {
            if (_saver == null)
            {
                var saverGameObject = new GameObject("Saver");
                _saver = saverGameObject.AddComponent<Saver>();
            }
            else
            {
                Disable();
            }
            
            _saver.Initialize(_saveSystem);
            _saver.Launch(timePeriodic);

            AutoSaveEnabled = true;
        }

        public void Disable()
        {
            _saver?.Stop();
        }
    }
}