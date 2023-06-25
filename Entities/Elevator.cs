namespace Entities
{
    public class Elevator
    {
        public event EventHandler<MovingEventArgs> RaiseIsMoving;

        public event EventHandler<MovingEventArgs> RaiseAtFloor;

        public event EventHandler<LoadingEventArgs> RaiseRemaining;

        private const int WeightLimit = 15;

        private const int Speed = 1;

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


        public int Relative(int floor)
        {
            return (Math.Abs(CurrentFloor - floor));
        }

        /// <summary>
        /// Ultimate destination floor of the elevator
        /// </summary>
        public int Destination { get; set; }

        public bool DirectionIsUp { get; set; }

        public ElevatorStatusEnum ElevatorStatus { get; set; }

        public Request Request { get; set; }

        public List<Person> Passengers { get; set; } = new List<Person>();

        public int AvailableCapacity
        {
            get
            {
                return WeightLimit - Passengers.Count;
            }
        }

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
                await Task.Delay((Speed) * 1000);
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

        public async Task MoveUp(bool isFinalDestination)
        {
            DirectionIsUp = true;
            ElevatorStatus = ElevatorStatusEnum.Moving;
            await Move(isFinalDestination, true);
        }

        public async Task MoveDown(bool isFinalDestination)
        {
            DirectionIsUp = false;
            ElevatorStatus = ElevatorStatusEnum.Moving;
            await Move(isFinalDestination, false);
        }

        protected void OnMoving(MovingEventArgs e)
        {
            RaiseIsMoving?.Invoke(this, e);
        }

        protected void OnArrived(MovingEventArgs e)
        {
            Disembark();
            RaiseAtFloor?.Invoke(this, e);
        }

        protected void OnRemaining(LoadingEventArgs e)
        {
            RaiseRemaining?.Invoke(this, e);
        }

        private void Disembark()
        {
            //future handle different floors (figure out how to capture for each person)
            Passengers.Clear();
        }
    }
}
