using BoleteHell.Code.Gameplay.Character;

namespace BoleteHell.Code.Gameplay.GameState
{
    public class GameState
    {
        public int Corruption { get; private set; }
        
        public ISceneObject Player { get; private set; }
        public ISceneObject Entities { get; private set; }
    }
}