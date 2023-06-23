using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class Elevator
    {
        public event EventHandler<MovingEventArgs> RaiseIsMoving;

        public event EventHandler<MovingEventArgs> RaiseAtFloor;

        private const int WeightLimit = 15;

        private const int Speed = 2;
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

        public List<Person> Passengers { get; set; } = new List<Person>();

        public int AvailableCapacity
        {
            get
            {
                return WeightLimit - Passengers.Count;
            }
        }

        public void Board(Person person)
        {
            Passengers.Add(person);
        }

        private async Task Move(bool isFinalDestination, bool isUp)
        {
            do
            {
                OnMoving(new MovingEventArgs
                {
                    ElevatorId = Id,
                    IsUp = true,
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
                IsUp = true,
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
            RaiseAtFloor?.Invoke(this, e);
        }
    }
}
