using System.Numerics;

namespace Lab5.Types
{
    public class VectorOperator
    {
        public float X { get; private set; }
        public float Y { get; private set; }

        public VectorOperator(float x, float y)
        {
            X = x;
            Y = y;
        }
    }

}