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
