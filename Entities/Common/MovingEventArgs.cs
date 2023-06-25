namespace Entities
{
    public class MovingEventArgs : EventArgs
    {
        public string ElevatorId { get; set; }

        public bool IsUp { get; set; }

        public int CurrentFloor { get; set; }

        public int People { get; set; }

        public IElevator Elevator { get; set; }

        public bool FinalDestination { get; set; }
    }
}