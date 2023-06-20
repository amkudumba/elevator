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

        private const int Speed = 30;
        /// <summary>
        /// use for UI displaying which elevator has passengers
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// When moving, this is empty.  Populate when you arrive at a floor
        /// </summary>
        public int? CurrentFloor { get; set; }

        /// <summary>
        /// When stopped, this is empty.  Populate when moving
        /// </summary>
        public int? NextFloor { get; set; }


        /// <summary>
        /// Ultimate destination floor of the elevator
        /// </summary>
        public int Destination { get;set; }

        public bool DirectionIsUp { get; set; }

        public List<Person> Passengers { get; set; } = new List<Person>();

        public bool HasCapacity
        {
            get
            {
                return Passengers.Count < WeightLimit;
            }
        }

        public void Board(Person person)
        {
            Passengers.Add(person);
        }

        public void MoveUp()
        {
            //raise event moving up
            DirectionIsUp = true;
            OnMoving(new MovingEventArgs { ElevatorId = Id, IsUp = true });

            //delay to mimic movement ?? need to make this async to free up UI
            Thread.Sleep(Speed*1000);

            throw new NotImplementedException();
        }

        public void MoveDown()
        {
            //raise event moving down
            DirectionIsUp = false;
            OnMoving(new MovingEventArgs { ElevatorId = Id, IsUp = false});
            //delay to mimic movement ?? need to make this async to free up UI
            Thread.Sleep(Speed*1000);
            throw new NotImplementedException();
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
