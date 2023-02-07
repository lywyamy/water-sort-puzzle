public class UserAction
{
    public int sourceIndex;
    public int destinationIndex;
    public int numberOfWaterMoved;

    public UserAction(int sourceIndex, int destinationIndex, int numberOfWaterMoved)
    {
        this.sourceIndex = sourceIndex;
        this.destinationIndex = destinationIndex;
        this.numberOfWaterMoved = numberOfWaterMoved;
    }
}