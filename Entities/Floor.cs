using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class Floor
    {
        public int Number { get; set; }
        /// <summary>
        /// The floor name (1st, B1, B2, etc)
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Keep track of the elevators at the current floor
        /// </summary>
        public List<Elevator> Elevators { get; set; } = new List<Elevator>();

        /// <summary>
        /// Track the requests received
        /// </summary>
        public List<Request> Requests { get; set; } = new List<Request>();
    }
}
