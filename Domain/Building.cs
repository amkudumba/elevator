using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Building
    {
        const int lowestFloor = -3;
        const int highestFloor = 5;

        public List<Elevator> Elevators { get; set; } = new List<Elevator>();

        public List<Floor> Floors { get; set; } = new List<Floor>();

        public Building()
        {
            Elevator add = new Elevator { Id = $"Lift - {Elevators.Count + 1}" };

            add.RaiseIsMoving += ElevatorIsMoving;
            add.RaiseAtFloor += ElevatorIsAtFloor;
            //add floors to the building
            for (int i = -lowestFloor; i < highestFloor; i++)
            {
                Floors.Add(new Floor { Name = (i == 0 ? "Ground" : (i < 0 ? $"Base {Math.Abs(i)}" : $"Floor {i}")), Number = i });
            }
        }

        private void ElevatorIsAtFloor(object? sender, MovingEventArgs e)
        {
            if (sender != null && sender is Elevator elevator)
            {
                elevator.CurrentFloor = elevator.NextFloor;
                elevator.NextFloor = null;
                if (elevator.CurrentFloor == highestFloor)
                {
                    elevator.DirectionIsUp = false;
                }
                if (elevator.CurrentFloor == lowestFloor)
                {
                    elevator.DirectionIsUp = true;
                }
            }
        }

        private void ElevatorIsMoving(object? sender, MovingEventArgs e)
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

                elevator.CurrentFloor = null;
            }
        }
    }
}
