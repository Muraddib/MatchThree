using System.Collections.Generic;
using MatchThree;
using UnityEngine;
using System.Collections;


namespace MatchThree
{
    public class AudioController : Singleton<AudioController>
    {
        public AudioBank AudioBank { private get; set; }

        void Awake()
        {
            EventManager.OnSoundEvent += PlaySound;
        }

        public void Init(AudioBank bank)
        {
            AudioBank = bank;
        }

        private void PlaySound(AudioIDs id, Vector3 pos)
        {
            AudioItem aItem = GetAudioItem(id);
            if (aItem != null)
            {
                AudioClip clip = aItem.clip;
                if (clip)
                {
                    AudioSource.PlayClipAtPoint(clip, pos);
                }
            }
        }

        private AudioItem GetAudioItem(AudioIDs audioID)
        {
            List<AudioItem> audioItems = AudioBank[audioID];
            if (audioItems != null && audioItems.Count > 0)
            {
                AudioItem aItem = audioItems[UnityEngine.Random.Range(0, audioItems.Count)];
                return aItem;
            }
            else return null;
        }

    }
}
