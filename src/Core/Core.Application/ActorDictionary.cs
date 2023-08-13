namespace Core.Application
{
    public class ActorDictionary : Dictionary<Type,Type>
    {
        public ActorDictionary(Dictionary<Type, Type> dict) : base(dict)
        {
        }
    }
}
