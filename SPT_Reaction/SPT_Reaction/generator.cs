using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPT_Reaction
{
    /// <summary>
    /// Enum operácie pre príklad.
    /// </summary>
    public enum Operation
    {
        add = '+',
        subtract = '-',
        multiply = '*',
        divide = '/'
    }
    /// <summary>
    /// Trieda Príkladu.
    /// </summary>
    public class Problem
    {

        public Operation operation { get; set; } //!< Enum operácie pre príklad
        public int a { get; set; } //!< Prvá hodnota príkladu
        public int b { get; set; } //!< Druhá hodnota príkladu
        public int result { get; set; } //!< Výsledok operácie 
        public static Random rand { get; private set; } = new Random(); //!< Funkica Random
        
        /// <summary>
        /// Funkcia RandomOperation vráti náhodnú operáciu pre príklad.
        /// </summary>
        /// <returns>Enum s danou náhodne vybranou operáciou</returns>
        public static T RandomOperation<T>()
        {
            Array enumValues = Enum.GetValues(typeof(T));
            return (T)enumValues.GetValue(rand.Next(enumValues.Length));
        }

        /// <summary>
        /// Inicializátor objektu problém, ktorý vytvorí dve náhodné čísla, zvolí pre ne operáciu
        /// a vypočíta výsledok pre spätnú kontrolu.
        /// </summary>
        /// <returns>Nový objekt problému</returns>
        public Problem()
        {
            //generovanie random čísel pre premenné pre zabránenie celočíselnému deleniu a deleniu nulou
            do
            {
                a = rand.Next(-10, 10);
                b = rand.Next(-10, 10);
            }
            while (b == 0 || a % b != 0);
            
            //vygenerovanie random operácie
            operation = RandomOperation<Operation>();
            
            //vypočítanie výsledku pre príklad
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
