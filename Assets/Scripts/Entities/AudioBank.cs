using UnityEngine;
using System;
using System.Collections.Generic;
public class AudioBank : ScriptableObject
{
    [Serializable]
    public class AudioLink
    {
        public AudioIDs ID;
        public AudioClip Clip;
        public float volume;
    }

    public AudioLink[] Audios;

    private Dictionary<AudioIDs, List<AudioItem>> audiosDictionary;

    public List<AudioItem> this[AudioIDs audioID]
    {
        get
        {
            FillDictionary();
            if (audiosDictionary.ContainsKey(audioID))
            {
                return audiosDictionary[audioID].Count > 0 ? audiosDictionary[audioID] : null;
            }
            else { return null; }
        }
#if UNITY_EDITOR
            set
            {
                FillDictionary();

                Audios = Array.FindAll<AudioLink>(Audios, x => x.ID != audioID);
                if (audiosDictionary.ContainsKey(audioID)) { audiosDictionary.Remove(audioID); }
                
                if (value != null)
                {
                    Array.Resize<AudioLink>(ref Audios, Audios.Length + value.Count);
                    for (int i = 0; i < value.Count; i++)
                    {
                        Audios[Audios.Length - i - 1] = new AudioLink() { ID = audioID, Clip = value[i].clip, volume = value[i].volume };
                    }
                    audiosDictionary[audioID] = value;
                }
            }
#endif
    }

    private void FillDictionary()
    {
        if (audiosDictionary == null)
        {
            audiosDictionary = new Dictionary<AudioIDs, List<AudioItem>>();
            if (Audios != null)
            {

                for (int i = 0; i < Audios.Length; i++)
                {

                    if (audiosDictionary.ContainsKey(Audios[i].ID))
                    {
                        if (Audios[i].Clip != null)
                        {
                            audiosDictionary[Audios[i].ID].Add(new AudioItem() { clip = Audios[i].Clip, volume = Audios[i].volume });
                        }

                    }
                    else
                    {

                        audiosDictionary[Audios[i].ID] = new List<AudioItem>();
                        if (Audios[i].Clip != null) audiosDictionary[Audios[i].ID].Add(new AudioItem() { clip = Audios[i].Clip, volume = Audios[i].volume });
                    }

                }
            }
        }
    }
}
