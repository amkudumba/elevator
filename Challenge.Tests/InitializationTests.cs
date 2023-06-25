using Domain;

namespace Challenge.Tests
{
    public class InitializationTests
    {
        [Fact]
        public void Init_Default_Elevator_Count()
        {
            Shaft shaft = new Shaft();
            Assert.Equal(4, shaft.Elevators.Count);
        }

        [Fact]
        public void Init_Default_Floor_Count()
        {
            Shaft shaft = new Shaft();
            Assert.Equal(12, shaft.Floors.Count);
        }

        [Fact]
        public void Init_Default_Floor_Lowest()
        {
            Shaft shaft = new Shaft();
            Assert.Equal(-3, shaft.LowestFloor);
        }

        [Fact]
        public void Init_Default_Floor_Highest()
        {
            Shaft shaft = new Shaft();
            Assert.Equal(8, shaft.HighestFloor);
        }

        [Fact]
        public void Init_Extended_Elevator_Count()
        {
            Shaft shaft = new Shaft(4, -2, 3);
            Assert.Equal(4, shaft.Elevators.Count);
        }

        [Fact]
        public void Init_Extended_Floor_Count()
        {
            Shaft shaft = new Shaft(4, -2, 3);
            Assert.Equal(6, shaft.Floors.Count);
        }
    }
}