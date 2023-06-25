using Domain;

namespace Challenge.Tests
{
    public class ValidationTests
    {
        [Fact]
        public void Floor_In_Range_Lowest()
        {
            Shaft shaft = new Shaft();
            bool inRange = shaft.FloorInRange(-3);
            Assert.True(inRange);
        }

        [Fact]
        public void Floor_In_Range_Highest()
        {
            Shaft shaft = new Shaft();
            bool inRange = shaft.FloorInRange(8);
            Assert.True(inRange);
        }

        [Fact]
        public void Floor_In_Range_Center()
        {
            Shaft shaft = new Shaft();
            bool inRange = shaft.FloorInRange(1);
            Assert.True(inRange);
        }

        [Fact]
        public void Floor_Out_Of_Range_Lowest()
        {
            Shaft shaft = new Shaft();
            bool inRange = shaft.FloorInRange(-4);
            Assert.False(inRange);
        }

        [Fact]
        public void Floor_Out_Of_Range_Highest()
        {
            Shaft shaft = new Shaft();
            bool inRange = shaft.FloorInRange(9);
            Assert.False(inRange);
        }
    }
}
