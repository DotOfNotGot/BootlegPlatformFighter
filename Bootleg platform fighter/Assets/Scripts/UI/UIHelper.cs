using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace BootlegPlatformFighter
{
    public class UIHelper : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI label;
        [SerializeField]
        private string ExtraLabelText;
        public void UpdateLabelFromSlider(float val)
        {
            label.text = val.ToString() + ExtraLabelText;
        }
    }
}
