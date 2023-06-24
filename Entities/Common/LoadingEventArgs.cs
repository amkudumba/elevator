namespace Entities
{
    public class LoadingEventArgs:EventArgs
    {
        public int Floor { get; set; }

        public string ElevatorId { get; set; }

        public int Remaining { get; set; }
    }
}
