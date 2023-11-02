using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorComponents.TimeMatrix
{
    internal class CellEventData
    {
        internal CellEventData() {
            this.BGEvents = new List<Event>();
            this.Events = new List<Event>();
        }

        internal int X { get; set; }

        internal int Y { get; set; }

        internal string DisplayTime { get; set; }

        internal Cell Cell { get; set; }

        internal List<Event> BGEvents { get; set; }

        internal List<Event> Events { get; set; }
    }
}
