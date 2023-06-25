using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class Request
    {
        /// <summary>
        /// direction of the request (button pressed requesting elevator in up to down direction)
        /// </summary>
        public bool IsUp { get; set; }

        /// <summary>
        /// number of people waiting for the elevator
        /// </summary>
        public int NumberWaiting { get; set; }
    }
}
