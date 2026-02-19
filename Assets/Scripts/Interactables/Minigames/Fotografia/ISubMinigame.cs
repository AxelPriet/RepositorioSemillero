public interface ISubMinigame
{
    void StartSubMinigame(MinigameCombinado parent);
    void CompleteSubMinigame();
    void FailSubMinigame();
}