using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BootlegPlatformFighter
{
    public class Options : MonoBehaviour
    {
        [SerializeField]
        AudioSource mainAudio;

        // Start is called before the first frame update
        void Start()
        {
            ResetToDefault(); // We don't save configs yet
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void ResetToDefault()
        {
            Application.targetFrameRate = -1; // Default per platform, uncapped for standalone platforms
            QualitySettings.vSyncCount = 0; // Vsync off

            if (mainAudio) // Not implemented rn
                mainAudio.volume = 1; // 0-1f
        }
    }
}
