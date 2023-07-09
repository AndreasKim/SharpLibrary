using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Domain;

namespace BookAggregate
{
    public class HoldLifeType : Enumeration
    {
        public static HoldLifeType OpenEnded = new(1, nameof(OpenEnded));
        public static HoldLifeType CloseEnded = new(2, nameof(CloseEnded));

        public HoldLifeType(int id, string name)
            : base(id, name)
        {
        }

    }
}
