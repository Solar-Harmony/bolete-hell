namespace BoleteHell.Code.Arsenal.Cannons
{
    public interface ICannonService
    {
        void Tick(CannonInstance cannon);
        bool TryShoot(CannonInstance cannon, ShotLaunchParams parameters);
        void FinishFiring(CannonInstance cannon);
    }
}