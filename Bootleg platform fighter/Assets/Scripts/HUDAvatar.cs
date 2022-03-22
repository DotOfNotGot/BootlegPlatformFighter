using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BootlegPlatformFighter
{
    public class HUDAvatar : MonoBehaviour
    {
        [SerializeField]
        private int _CharacterIndex = -1; // CharacterIndex it's displaying data for

        public void setCharacterIndex(int idx)
        {
            _CharacterIndex = idx;
        }
        public int getCharacterIndex()
        {
            return _CharacterIndex;
        }
    }
}
