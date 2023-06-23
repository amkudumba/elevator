using Domain;

namespace Challenge.Tests
{
    public class InitializeTests
    {
        [Fact]
        public void InitDefault()
        {
            Shaft shaft = new Shaft();
            Assert.Single(shaft.Elevators);
            Assert.Equal(9, shaft.Floors.Count);
        }

        [Fact]
        public void InitExtended()
        {
            Shaft shaft = new Shaft(4, -2, 3);
            Assert.Equal(4, shaft.Elevators.Count);
            Assert.Equal(Math.Abs(-2) + 3 + 1, shaft.Floors.Count);
        }

        [Fact]
        public void SingleMove()
        {
            Shaft shaft = new Shaft(4, -2, 3);
            shaft.AddRequest(2, 5, false);
            Assert.Equal(4, shaft.Elevators.Count);
            Assert.Equal(Math.Abs(-2) + 3 + 1, shaft.Floors.Count);
        }

        [Fact]
        public void MultipleMove()
        {
            Shaft shaft = new Shaft(4, -3, 5);
            shaft.AddRequest(2, 5, false);
            shaft.AddRequest(-1, 4, true);
            shaft.AddRequest(-3, 7, true);
            //add tests here
        }
    }
}