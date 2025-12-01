namespace BoleteHell.Gameplay.Droppables
{
    public interface ILootable
    {
        public DropSettings DropSettings { get; set; }
        public IDropManager dropManager { get; set; }
    }
}
