namespace Entities
{
    public class MovingEventArgs : EventArgs
    {
        public string ElevatorId { get; set; }

        public bool IsUp { get; set; }
    }
}