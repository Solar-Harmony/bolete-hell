using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace BoleteHell.Audio
{
    public class AudioPlayer : IAudioPlayer
    {
        // TODO: Having type safety/editor completion for clip names would be nice.
        // (Like there is for the layers or tags)
        public async Task PlaySoundAsync(string clipName, Vector3 position)
        {
            var clip = await Addressables.LoadAssetAsync<AudioClip>(clipName).Task;
            if (!clip)
            {
                Debug.LogWarningFormat("Tried to play missing audio clip: {0}", clipName);
                return;
            }
            
            AudioSource.PlayClipAtPoint(clip, position);
        }
    }
}