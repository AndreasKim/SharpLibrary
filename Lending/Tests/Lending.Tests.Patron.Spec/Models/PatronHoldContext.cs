using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PatronAggregate.Spec.Models
{
    public class PatronHoldContext
    {
        public Book Book { get; set; }
        public Patron Patron { get; set; }
        public Action HoldAction { get; set; }
    }
}
