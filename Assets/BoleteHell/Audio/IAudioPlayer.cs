using System.Threading.Tasks;
using UnityEngine;

namespace BoleteHell.Audio
{
    public interface IAudioPlayer
    {
        Task PlaySoundAsync(string clipName, Vector3 position);
    }
}