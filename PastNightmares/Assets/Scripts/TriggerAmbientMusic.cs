using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerAmbientMusic : MonoBehaviour
{
    [SerializeField] private AudioClip atticAmbientMusic, level1Music;
    [SerializeField] private TriggerLevelEnding triggerLevelEnding;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameMaster.gm.ambientMusicTriggered = true;
            SoundManager.instance.SwapTrack(atticAmbientMusic);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && !triggerLevelEnding.endingTriggered)
        {
            GameMaster.gm.ambientMusicTriggered = false;
            SoundManager.instance.SwapTrack(level1Music);
        }
        else if (other.CompareTag("Player") && triggerLevelEnding.endingTriggered) //don't play the level's music if the ending was triggered
        {
            GameMaster.gm.ambientMusicTriggered = false;
            SoundManager.instance.SwapTrack(level1Music);
            SoundManager.instance.PauseMusic();
        }
    }
}
