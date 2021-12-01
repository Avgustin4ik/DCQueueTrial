public interface IBrain
{
    void OnTapped();
    void OnServed();
    void OnExpired();
    void OnDestinationReached();
    void OnEmotePlayed();
        
    void MakeStepInQueue();
}
