namespace BoleteHell.Code.Arsenal.Cannons
{
    public interface ICannonService
    {
        void Tick(CannonInstance cannon);
        void TryShoot(CannonInstance cannon, ShotLaunchParams parameters);
        void FinishFiring(CannonInstance cannon);
    }
}