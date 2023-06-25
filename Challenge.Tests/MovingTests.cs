using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Challenge.Tests
{
    public class MovingTests
    {
        [Fact]
        public void Move_To_Requesting_Floor_First_Request()
        {
            Shaft shaft = new Shaft();
            shaft.AddRequest(2, 5, true);
            //pause until the elevator has moved
            Thread.Sleep(14 * 1000);
            //should be first elevator that moves to floor 2
            Assert.Equal(2, shaft.Elevators.First().CurrentFloor);
        }

        [Fact]
        public void Move_To_Requesting_Floor_First_Execute()
        {
            Shaft shaft = new Shaft();
            shaft.AddRequest(2, 5, true);
            //pause until the elevator has moved
            Thread.Sleep(14 * 1000);
            shaft.ExecuteRequest(shaft.Elevators.First(), -3);

            Thread.Sleep(14 * 1000);
            Assert.Equal(-3, shaft.Elevators.First().CurrentFloor);
        }

        [Fact]
        public void Move_To_Requesting_Floor_Second_Request()
        {
            Shaft shaft = new Shaft();
            shaft.AddRequest(2, 5, true);
            //pause until the elevator has moved
            Thread.Sleep(14 * 1000);
            shaft.ExecuteRequest(shaft.Elevators.First(), -3);

            Thread.Sleep(14 * 1000);
            Assert.Equal(-3, shaft.Elevators.First().CurrentFloor);


            shaft.AddRequest(4, 5, true);
            //pause until the elevator has moved
            Thread.Sleep(14 * 1000);
            Assert.Equal(4, shaft.Elevators.Where(p=>p.Id== "Lift #: 2").First().CurrentFloor);
        }
    }
}
