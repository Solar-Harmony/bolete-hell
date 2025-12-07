namespace BoleteHell.Utils.Advisor
{
    public enum TutorialEvent
    {
        KilledFirstBolete,
    }
    
    public interface ITutorialService
    {
        void ShowTutorial(TutorialEvent tutorialEvent);
    }
}