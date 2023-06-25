namespace Entities
{
    public class FastElevator : Elevator
    {
        protected override int WeightLimit => 15;

        protected override int Speed => 1;
    }
}
