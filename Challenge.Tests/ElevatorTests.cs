using Domain;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Challenge.Tests
{
    public class ElevatorTests
    {
        [Fact]
        public void Initialized_Elevator_No_Passengers()
        {
            var elevator = new FastElevator();
            Assert.True(!elevator.Passengers.Any());
        }

        [Fact]
        public void Elevator_Boarded_Passengers_In_Capacity()
        {
            Shaft shaft = new Shaft();
            shaft.AddRequest(2, 5, true);
            //pause until the elevator has moved
            Thread.Sleep(14 * 1000);
            //should be first elevator that moves to floor 2
            shaft.Elevators.First().Board();

            Assert.Equal(5, shaft.Elevators.First().Passengers.Count);
        }

        [Fact]
        public void Elevator_Boarded_Passengers_In_Exceed_Capacity()
        {
            Shaft shaft = new Shaft();
            shaft.AddRequest(2, 19, true);
            //pause until the elevator has moved
            Thread.Sleep(14 * 1000);
            //should be first elevator that moves to floor 2
            shaft.Elevators.First().Board();
            //should default to current capacity
            Assert.Equal(15, shaft.Elevators.First().Passengers.Count);
        }

        [Fact]
        public void Elevator_Boarded_Passengers_Disembarked_At_Destination()
        {
            Shaft shaft = new Shaft();
            shaft.AddRequest(2, 5, true);
            //pause until the elevator has moved
            Thread.Sleep(14 * 1000);
            //should be first elevator that moves to floor 2
            shaft.Elevators.First().Board();

            shaft.ExecuteRequest(shaft.Elevators.First(), -3);
            Thread.Sleep(14 * 1000);
            Assert.False(shaft.Elevators.First().Passengers.Any());
        }
    }
}
