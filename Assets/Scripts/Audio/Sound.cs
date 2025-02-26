using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CamKnight
{
    [System.Serializable]
    public class Sound
    {
        [SerializeField]
        private string name;
        [SerializeField]
        private AudioClip clip;

        public string Name { get => name; set => name = value; }
        public AudioClip Clip { get => clip; set => clip = value; }
    }
}
