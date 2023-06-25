using Entities;

namespace Domain
{
    public class Shaft
    {
        private int lowestFloor;
        private int highestFloor;

        public event EventHandler<MovingEventArgs> RaiseIsMoving;

        public event EventHandler<MovingEventArgs> RaiseAtFloor;

        public event EventHandler<LoadingEventArgs> RaiseRemaining;

        public int LowestFloor { get { return lowestFloor; } }

        public int HighestFloor { get { return highestFloor; } }

        /// <summary>
        /// validate the requested floor is in valid range
        /// </summary>
        /// <param name="floor"></param>
        /// <returns></returns>
        public bool FloorInRange(int floor)
        {
            return floor >= lowestFloor && floor <= highestFloor;
        }

        protected void OnMoving(MovingEventArgs e)
        {
            RaiseIsMoving?.Invoke(this, e);
        }

        protected void OnArrived(MovingEventArgs e)
        {
            RaiseAtFloor?.Invoke(this, e);
        }

        protected void OnRemaining(LoadingEventArgs e)
        {
            RaiseRemaining?.Invoke(this, e);
        }

        public List<IElevator> Elevators { get; set; } = new List<IElevator>();

        public List<Floor> Floors { get; set; } = new List<Floor>();

        public Shaft()
        {
            Initialize(3, 1, -3, 8);
        }

        public Shaft(int elevatorCount, int bottomFloor, int topFloor)
        {
            Initialize(elevatorCount, 0, bottomFloor, topFloor);
        }

        /// <summary>
        /// add a new request for an elevator
        /// </summary>
        /// <param name="floor"></param>
        /// <param name="people"></param>
        /// <param name="isUp"></param>
        /// <returns></returns>
        public bool AddRequest(int floor, int people, bool isUp)
        {
            var toSet = Floors.Where(p => p.Number == floor).FirstOrDefault();

            if (toSet != null)
            {
                var request = new Request { NumberWaiting = people, IsUp = isUp };

                //find the elevator that is closest
                var closest = FindClosest(floor);
                closest.Request = request;

                if (closest.Destination != floor)
                {
                    bool moveUp = closest.CurrentFloor < floor;
                    closest.Destination = floor;
                    if (moveUp)
                    {
                        _ = closest.MoveUp(false);
                    }
                    else
                    {
                        _ = closest.MoveDown(false);
                    }
                }
                else
                {
                    RaiseAtFloor(this, new MovingEventArgs
                    {
                        CurrentFloor = closest.Destination,
                        ElevatorId = closest.Id,
                        IsUp = isUp,
                        People = closest.Passengers.Count,
                        Elevator = closest
                    });
                }
                return true;
            }

            return false;
        }

        /// <summary>
        /// execute the request at floor, i.e. go to destination
        /// </summary>
        /// <param name="elevator"></param>
        /// <param name="destination"></param>
        /// <returns></returns>
        public bool ExecuteRequest(IElevator elevator, int destination)
        {
            elevator.Destination = destination;
            if (destination > elevator.CurrentFloor)
            {
                _ = elevator.MoveUp(true);
            }
            else
            {
                _ = elevator.MoveDown(true);
            }
            return true;
        }

        /// <summary>
        /// find the losest elevator
        /// </summary>
        /// <param name="floor"></param>
        /// <returns></returns>
        private IElevator FindClosest(int floor)
        {
            //check for elevators with capacity
            var withCapacity = Elevators.Where(p => p.AvailableCapacity >= 0);

            //stationary lifts can be used
            var stationary = withCapacity.Where(p => p.ElevatorStatus == ElevatorStatusEnum.Stopped).ToList();

            //moving lifts must be toward the floor
            //1) up toward the floor - the lift is currently below
            var towardUp = withCapacity.Where(p => p.ElevatorStatus == ElevatorStatusEnum.Moving && p.CurrentFloor < floor && p.DirectionIsUp == true).ToList();

            //2) down toward the floor - the list is currently above
            var towardDown = withCapacity.Where(p => p.ElevatorStatus == ElevatorStatusEnum.Moving && p.CurrentFloor >= floor && p.DirectionIsUp == false).ToList();

            var combined = stationary.Union(towardDown.Union(towardUp)).ToList();

            //determine the closest
            return combined.OrderBy(p => p.Relative(floor)).FirstOrDefault();
        }

        private void Initialize(int fastElevators, int slowElevators, int bottomFloor, int topFloor)
        {
            InitializeFloors(bottomFloor, topFloor);

            InitializeElevators(fastElevators, slowElevators);
        }

        private void InitializeFloors(int bottomFloor, int topFloor)
        {
            lowestFloor = bottomFloor;
            highestFloor = topFloor;
            //add floors to the building
            for (int i = lowestFloor; i <= highestFloor; i++)
            {
                Floors.Add(new Floor { Name = (i == 0 ? "Ground" : (i < 0 ? $"Base {Math.Abs(i)}" : $"Floor {i}")), Number = i });
            }
        }

        private void InitializeElevators(int fastElevators, int slowElevators)
        {
            for (int i = 0; i < fastElevators; i++)
            {
                IElevator add = new FastElevator { Id = $"Lift #: {Elevators.Count + 1}", CurrentFloor = 0 };
                add.RaiseIsMoving += ElevatorIsMoving;
                add.RaiseAtFloor += ElevatorIsAtFloor;
                add.RaiseRemaining += PassengersLeftBehind;
                Elevators.Add(add);
            }

            for (int i = 0; i < slowElevators; i++)
            {
                IElevator add = new SlowElevator { Id = $"Lift #: {Elevators.Count + 1}", CurrentFloor = 0 };
                add.RaiseIsMoving += ElevatorIsMoving;
                add.RaiseAtFloor += ElevatorIsAtFloor;
                add.RaiseRemaining += PassengersLeftBehind;
                Elevators.Add(add);
            }
        }

        private void PassengersLeftBehind(object sender, LoadingEventArgs e)
        {
            OnRemaining(e);
        }

        private void ElevatorIsAtFloor(object sender, MovingEventArgs e)
        {
            if (sender != null && sender is IElevator elevator)
            {
                elevator.CurrentFloor = elevator.NextFloor.GetValueOrDefault();
                elevator.NextFloor = null;
                elevator.ElevatorStatus = ElevatorStatusEnum.Stopped;

                if (elevator.CurrentFloor == highestFloor)
                {
                    elevator.DirectionIsUp = false;
                }
                if (elevator.CurrentFloor == lowestFloor)
                {
                    elevator.DirectionIsUp = true;
                }

                OnArrived(e);
            }
        }

        private void ElevatorIsMoving(object sender, MovingEventArgs e)
        {
            if (sender != null && sender is IElevator elevator)
            {
                if (e.IsUp)
                {
                    elevator.NextFloor = elevator.CurrentFloor + (elevator.CurrentFloor < highestFloor ? 1 : 0);
                }
                else
                {
                    elevator.NextFloor = elevator.CurrentFloor - (elevator.CurrentFloor > lowestFloor ? 1 : 0);
                }

                OnMoving(e);
            }
        }
    }
}
