namespace BoleteHell.Code.Arsenal.Cannons
{
    public interface ICannonService
    {
        void Tick(CannonInstance cannon);
        void TryShoot(CannonInstance cannon, ShotParams parameters);
        void FinishFiring(CannonInstance cannon);
    }
}