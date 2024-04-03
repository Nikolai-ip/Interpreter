namespace Lab5.Types
{
    public abstract class BaseType
    {
        public object Value { get; protected set; }

        public abstract object Add(BaseType self, BaseType other);
    }
}