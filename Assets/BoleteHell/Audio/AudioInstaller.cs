using Zenject;

namespace BoleteHell.Audio
{
    public class AudioInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.Bind<IAudioPlayer>().To<AudioPlayer>().AsSingle();
        }
    }
}
