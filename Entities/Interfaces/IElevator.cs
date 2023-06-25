namespace Entities
{
    public interface IElevator
    {
        int AvailableCapacity { get; }
        
        int CurrentFloor { get; set; }
        
        int Destination { get; set; }
        
        bool DirectionIsUp { get; set; }
        
        ElevatorStatusEnum ElevatorStatus { get; set; }
        
        string Id { get; set; }
        
        int? NextFloor { get; set; }
        
        List<Person> Passengers { get; set; }
        
        Request Request { get; set; }

        event EventHandler<MovingEventArgs> RaiseAtFloor;
        event EventHandler<MovingEventArgs> RaiseIsMoving;
        event EventHandler<LoadingEventArgs> RaiseRemaining;

        void Board();
        
        Task MoveDown(bool isFinalDestination);
        
        Task MoveUp(bool isFinalDestination);
        
        int Relative(int floor);
    }
}