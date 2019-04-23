using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPT_Reaction
{
    public enum Operation
    {
        add = '+',
        subtract = '-',
        multiply = '*',
        divide = '/'
    }

    public class Problem
    {

        public Operation operation { get; set; }
        public int a { get; set; }
        public int b { get; set; }
        public int result { get; set; }
        public static Random rand { get; private set; } = new Random();

        public static T RandomOperation<T>()
        {
            Array enumValues = Enum.GetValues(typeof(T));
            return (T)enumValues.GetValue(rand.Next(enumValues.Length));
        }

        public Problem()
        {
            do
            {
                a = rand.Next(-10, 10);
                b = rand.Next(-10, 10);
            }
            while (b == 0 || a % b != 0);

            operation = RandomOperation<Operation>();
            switch (operation)
            {
                case Operation.add: result = a + b; break;
                case Operation.subtract: result = a - b; break;
                case Operation.multiply: result = a * b; break;
                case Operation.divide: result = a / b; break;
            }
        }
    }
}
