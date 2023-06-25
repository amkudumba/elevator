namespace Entities
{
    public abstract class Elevator : IElevator
    {
        public event EventHandler<MovingEventArgs> RaiseIsMoving;

        public event EventHandler<MovingEventArgs> RaiseAtFloor;

        public event EventHandler<LoadingEventArgs> RaiseRemaining;

        /// <summary>
        /// can be set by the implementing elevator type
        /// </summary>
        protected abstract int WeightLimit { get; }

        /// <summary>
        /// can be set by the implementing elevator type
        /// </summary>
        protected abstract int Speed { get; }

        /// <summary>
        /// use for UI displaying which elevator has passengers
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// When moving, this is empty.  Populate when you arrive at a floor
        /// </summary>
        public int CurrentFloor { get; set; }

        /// <summary>
        /// When stopped, this is empty.  Populate when moving
        /// </summary>
        public int? NextFloor { get; set; }

        /// <summary>
        /// used to determine the relative floor when finding the closest to a floor
        /// </summary>
        /// <param name="floor"></param>
        /// <returns></returns>
        public int Relative(int floor)
        {
            return (Math.Abs(CurrentFloor - floor));
        }

        /// <summary>
        /// Ultimate destination floor of the elevator
        /// </summary>
        public int Destination { get; set; }

        /// <summary>
        /// indicator that the elevator is going up
        /// </summary>
        public bool DirectionIsUp { get; set; }

        /// <summary>
        /// indicator is stopped or moving
        /// </summary>
        public ElevatorStatusEnum ElevatorStatus { get; set; }

        /// <summary>
        /// populated when people are loaded
        /// </summary>
        public Request Request { get; set; }

        public List<Person> Passengers { get; set; } = new List<Person>();

        /// <summary>
        /// calculate the available capacity
        /// </summary>
        public int AvailableCapacity
        {
            get
            {
                return WeightLimit - Passengers.Count;
            }
        }

        /// <summary>
        /// Board the people waiting, ensure that the capacity is not exceeded
        /// </summary>
        public void Board()
        {
            int numberWaiting = Request.NumberWaiting;
            do
            {
                Passengers.Add(new Person { });
                numberWaiting--;
            } while (AvailableCapacity > 0 && numberWaiting > 0);
            Request.NumberWaiting = 0;
            if (numberWaiting != 0)
            {
                OnRemaining(new LoadingEventArgs { ElevatorId = Id, Floor = CurrentFloor, Remaining = numberWaiting });
            }
        }

        /// <summary>
        /// initate the move.  
        /// </summary>
        /// <param name="isFinalDestination">
        /// indicates if this move is to the final destination flow (true) else if false it will be to the requesting floor
        /// </param>
        /// <param name="isUp">
        /// direction of motion
        /// </param>
        /// <returns></returns>
        private async Task Move(bool isFinalDestination, bool isUp)
        {
            do
            {
                OnMoving(new MovingEventArgs
                {
                    ElevatorId = Id,
                    IsUp = isUp,
                    CurrentFloor = CurrentFloor,
                    People = Passengers.Count,
                    Elevator = this
                });
                await Task.Delay((1+(Speed/10)) * 1000);
                CurrentFloor = CurrentFloor + (isUp ? 1 : -1);
            } while (CurrentFloor != Destination);

            OnArrived(new MovingEventArgs
            {
                ElevatorId = Id,
                IsUp = isUp,
                CurrentFloor = CurrentFloor,
                Elevator = this,
                People = Passengers.Count,
                FinalDestination = isFinalDestination
            });

        }

        /// <summary>
        /// start elevator moving up
        /// </summary>
        /// <param name="isFinalDestination"></param>
        /// <returns></returns>
        public async Task MoveUp(bool isFinalDestination)
        {
            DirectionIsUp = true;
            ElevatorStatus = ElevatorStatusEnum.Moving;
            await Move(isFinalDestination, true);
        }

        /// <summary>
        /// start elevator moving down
        /// </summary>
        /// <param name="isFinalDestination"></param>
        /// <returns></returns>
        public async Task MoveDown(bool isFinalDestination)
        {
            DirectionIsUp = false;
            ElevatorStatus = ElevatorStatusEnum.Moving;
            await Move(isFinalDestination, false);
        }

        /// <summary>
        /// event to update the shaft / UI when moving
        /// </summary>
        /// <param name="e"></param>
        protected void OnMoving(MovingEventArgs e)
        {
            RaiseIsMoving?.Invoke(this, e);
        }

        /// <summary>
        /// event to notify callers that arrived at a floor
        /// </summary>
        /// <param name="e"></param>
        protected void OnArrived(MovingEventArgs e)
        {
            Disembark();
            RaiseAtFloor?.Invoke(this, e);
        }

        /// <summary>
        /// event to alert caller that some people could not be accomodated
        /// </summary>
        /// <param name="e"></param>
        protected void OnRemaining(LoadingEventArgs e)
        {
            RaiseRemaining?.Invoke(this, e);
        }

        /// <summary>
        /// disembark at the appropriate floor.  Can be enhanced to allow different floors for different people
        /// </summary>
        private void Disembark()
        {
            Passengers.Clear();
        }
    }
}
