namespace Entities
{
    public class SlowElevator : Elevator
    {
        protected override int WeightLimit => 20;

        protected override int Speed => 2;
    }
}
