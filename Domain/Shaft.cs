using Entities;

namespace Domain
{
    public class Shaft
    {
        private int lowestFloor;
        private int highestFloor;

        public event EventHandler<MovingEventArgs> RaiseIsMoving;

        public event EventHandler<MovingEventArgs> RaiseAtFloor;

        protected void OnMoving(MovingEventArgs e)
        {
            RaiseIsMoving?.Invoke(this, e);
        }

        protected void OnArrived(MovingEventArgs e)
        {
            RaiseAtFloor?.Invoke(this, e);
        }

        public List<Elevator> Elevators { get; set; } = new List<Elevator>();

        public List<Floor> Floors { get; set; } = new List<Floor>();

        public Shaft()
        {
            Initialize(2, -3, 5);
        }

        public Shaft(int elevatorCount, int bottomFloor, int topFloor)
        {
            Initialize(elevatorCount, bottomFloor, topFloor);
        }

        public bool AddRequest(int floor, int people, bool isUp)
        {
            var toSet = Floors.Where(p => p.Number == floor).FirstOrDefault();

            if (toSet != null)
            {
                var request = toSet.Requests.Where(p => p.IsUp == isUp).FirstOrDefault();
                if (request != null)
                {
                    request.NumberWaiting += people;    //there is already a request, so more people have arrived
                }
                else
                {
                    request = new Request { NumberWaiting = people, IsUp = isUp };
                    toSet.Requests.Add(request);
                }

                //find the elevator that is closest
                var closest = FindClosest(floor, request);

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

        private Elevator FindClosest(int floor, Request request)
        {
            //check for elevators with capacity
            var withCapacity = Elevators.Where(p => p.AvailableCapacity >= request.NumberWaiting);

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

        private void Initialize(int elevatorCount, int bottomFloor, int topFloor)
        {
            InitializeFloors(bottomFloor, topFloor);

            InitializeElevators(elevatorCount);
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

        private void InitializeElevators(int elevatorCount)
        {
            for (int i = 0; i < elevatorCount; i++)
            {
                Elevator add = new Elevator { Id = $"Lift #: {Elevators.Count + 1}", CurrentFloor = 0 };
                add.RaiseIsMoving += ElevatorIsMoving;
                add.RaiseAtFloor += ElevatorIsAtFloor;
                Elevators.Add(add);
            }
        }

        private void ElevatorIsAtFloor(object sender, MovingEventArgs e)
        {
            if (sender != null && sender is Elevator elevator)
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
            if (sender != null && sender is Elevator elevator)
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
