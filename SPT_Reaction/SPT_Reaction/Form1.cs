using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading;

/*! \mainpage Reaction Time Game
 *
 * \section intro_sec Introdukcia
 *
 * Reakčná hra pre zistenie vašich matematických schopností.
 *
 * \section install_sec Vstupy
 *
 * \subsection step1 Vstup 1:
 * Vstupom je reakcia uživatela na príklady v podobe zadaných výsledkov.
 *
 * \section install_sec Výstupy
 *
 * \subsection vystup1 Výstup 1: 
 * Informacie o úspěšnosti (kolkokrát bylo odpovedané).
 *
 * \subsection vystup2 Výstup 2: 
 * Informacie o priemernej reakčnej dobe.
 */

namespace SPT_Reaction
{
    /// <summary>
    /// Stopwatch class pre multithread podporu, počítanie času medzi funkciami.
    /// </summary>
    public class StopwatchProxy
    {
        private Stopwatch _stopwatch; //!< Privátna instancia stopwatch

        private static readonly StopwatchProxy _stopwatchProxy = new StopwatchProxy();
        private StopwatchProxy()
        {
            _stopwatch = new Stopwatch();
        }

        public Stopwatch Stopwatch { get { return _stopwatch; } } //!< Verejná instancia stopwatch

        /// <summary>
        /// Stopwatch inicializátor.
        /// <return>Instancia stopwatch.</return>
        /// </summary>
        public static StopwatchProxy Instance
        {
            get { return _stopwatchProxy; }
        }
    }

    /// <summary>
    /// Main form trieda.
    /// </summary>
    public partial class Form1 : Form
    {
        //vytvorenie premenných
        public static Problem problem = new Problem();  //!< Objekt problému
        public static int counter = 1;  //!< Počítadlo pre počet úspešných príkladov
        public static long[] reactionTimes = new long[5];  //!< Pole reakčných časov
        public static long reactionTime = 0;  //!< Priebežný reakčný čas
        public static long totalTime = 0;  //!< Celkový uplynutý čas
        public static int result = 0;  //!< Výsledok príkladu

        /// <summary>
        /// Inicializácia formy, disable stopButton pre zamedzenie pokusu o vypnutie nebežiacej hry.
        /// </summary>
        public Form1()
        {
            InitializeComponent();
            stopButton.Enabled = false;
        }

        /// <summary>
        /// Zmena focusu pri spustení formy.
        /// <param name="sender">Objekt daného eventu.</param>
        /// <param name="e">Parametre eventu.</param>
        /// </summary>
        private void richTextBox1_Enter(object sender, EventArgs e)
        {
            label1.Focus();
        }

        /// <summary>
        /// Event pre button pre začiatok reakčnej hry, spustí sa stopwatch pre meranie reakčnej doby a zobrazí sa prvý príklad.
        /// <param name="sender">Objekt daného eventu.</param>
        /// <param name="e">Parametre eventu.</param>
        /// </summary>
        private void startButton_Click(object sender, EventArgs e)
        {
            //kontrola či ešte hra nie je v behu
            if (StopwatchProxy.Instance.Stopwatch.IsRunning) return;
            //zabránenie aby bol start button stlačený znova
            stopButton.Enabled = true;
            startButton.Enabled = false;
            //vymazanie polí
            richTextBox1.Text = "";
            richTextBox2.Text = "";
            richTextBox3.Text = "";
            StopwatchProxy.Instance.Stopwatch.Start();
            //vypísanie príkladu
            switch (problem.operation)
            {
                case Operation.add:
                    richTextBox1.Text = $"({problem.a}) + ({problem.b})";
                    break;
                case Operation.subtract:
                    richTextBox1.Text = $"({problem.a}) - ({problem.b})";
                    break;
                case Operation.multiply:
                    richTextBox1.Text = $"({problem.a}) * ({problem.b})";
                    break;
                case Operation.divide:
                    richTextBox1.Text = $"({problem.a}) / ({problem.b})";
                    break;
            }
        }

        /// <summary>
        /// Event pre button pre ukončenie, vymažú sa všetky textboxy a zobrazí sa konečná reakčná doba
        /// <param name="sender">Objekt daného eventu.</param>
        /// <param name="e">Parametre eventu.</param>
        /// </summary>
        private void stopButton_Click(object sender, EventArgs e)
        {
            //zabránenie aby bol stop button stlačený znova
            stopButton.Enabled = false;
            startButton.Enabled = true;
            StopwatchProxy.Instance.Stopwatch.Stop();
            //vypísanie koncovej reakčnej doby
            richTextBox2.Text = $"Reaction time: {TimeSpan.FromMilliseconds(totalTime / 5).Seconds}s {TimeSpan.FromMilliseconds(totalTime / 5).Milliseconds}ms.";
            richTextBox1.Text = "";
            richTextBox4.Text = "";
            problem = new Problem();
            totalTime = 0;
            counter = 1;
        }

        /// <summary>
        /// Event pri zmene textového poľa pre zadávanie výsledkov
        /// <param name="sender">Objekt daného eventu.</param>
        /// <param name="e">Parametre eventu.</param>
        /// </summary>
        private void richTextBox4_KeyUp(object sender, KeyPressEventArgs e)
        {
            //Ak je stlačený enter skontroluj výsledok
            if (e.KeyChar == (char)Keys.Enter)
            {
                //Ak text v textboxe nie je číselný výsledok, zmaže sa pole a nekontroluje sa
                try
                {
                    result = Convert.ToInt32(richTextBox4.Text);
                }
                catch { richTextBox4.Text = "";  return; }
                //Ak sa výsledok v poli rovná výsledku príkladu
                if (problem.result == Convert.ToInt32(richTextBox4.Text))
                {
                    //Zapíš výsledný reakčný čas do pola
                    reactionTimes[counter - 1] = StopwatchProxy.Instance.Stopwatch.ElapsedMilliseconds;
                    StopwatchProxy.Instance.Stopwatch.Restart();
                    //vytvor nový príklad
                    problem = new Problem();
                    //pridanie času do celkového času
                    totalTime += reactionTimes[counter - 1];
                    //vypísanie výsledkov reakcie do textboxov
                    richTextBox2.Text = $"Reaction time: {TimeSpan.FromMilliseconds(totalTime / counter).Seconds}s {TimeSpan.FromMilliseconds(totalTime / counter).Milliseconds}ms.{Environment.NewLine}";
                    richTextBox3.Text = $"Correct Answers: {counter}";
                    //vypísanie nového problému
                    switch (problem.operation)
                    {
                        case Operation.add:
                            richTextBox1.Text = $"({problem.a}) + ({problem.b})";
                            break;
                        case Operation.subtract:
                            richTextBox1.Text = $"({problem.a}) - ({problem.b})";
                            break;
                        case Operation.multiply:
                            richTextBox1.Text = $"({problem.a}) * ({problem.b})";
                            break;
                        case Operation.divide:
                            richTextBox1.Text = $"({problem.a}) / ({problem.b})";
                            break;
                    }
                    //ak už bolo 5 príkladov, ukonči hru
                    if (counter == 5) { stopButton.PerformClick(); return; }
                    counter++;
                    richTextBox4.Text = "";
                }
                else
                {
                    //ak sa výsledok nerovnal, vymaž pole pre výsledok
                    richTextBox4.Text = "";
                }
            }
        }
    }
}
