using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace analyser.console.cli
{
    internal class EventQuery
    {
        public int Id { get; private set; }
        public string Description { get; private set; }
        public EventQuery(int id, string description)
        {
            Id = id;
            Description = description;
        }
    }
}
