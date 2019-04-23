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

namespace SPT_Reaction
{
    public static class RichTextBoxExtensions
    {
        public static void AppendText(this RichTextBox box, string text, Color color)
        {
            box.SelectionStart = box.TextLength;
            box.SelectionLength = 0;

            box.SelectionColor = color;
            box.AppendText(text);
            box.SelectionColor = box.ForeColor;
        }
    }

    public class StopwatchProxy
    {
        private Stopwatch _stopwatch;
        private static readonly StopwatchProxy _stopwatchProxy = new StopwatchProxy();

        private StopwatchProxy()
        {
            _stopwatch = new Stopwatch();
        }

        public Stopwatch Stopwatch { get { return _stopwatch; } }

        public static StopwatchProxy Instance
        {
            get { return _stopwatchProxy; }
        }
    }
    public partial class Form1 : Form
    {
        public static Problem problem = new Problem();
        public static int counter = 1;
        public static long[] reactionTimes = new long[5];
        public static long reactionTime = 0;
        public static long totalTime = 0;
        public static int result = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void richTextBox1_Enter(object sender, EventArgs e)
        {
            label1.Focus();
        }


        private void startButton_Click(object sender, EventArgs e)
        {
            if (StopwatchProxy.Instance.Stopwatch.IsRunning) return;
            stopButton.Enabled = true;
            startButton.Enabled = false;
            richTextBox1.Text = "";
            richTextBox2.Text = "";
            richTextBox3.Text = "";
            StopwatchProxy.Instance.Stopwatch.Start();
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

        private void stopButton_Click(object sender, EventArgs e)
        {
            stopButton.Enabled = false;
            startButton.Enabled = true;
            StopwatchProxy.Instance.Stopwatch.Stop();
            richTextBox2.Text = $"Reaction time: {TimeSpan.FromMilliseconds(totalTime / 5).Seconds}s {TimeSpan.FromMilliseconds(totalTime / 5).Milliseconds}ms.";
            richTextBox1.Text = "";
            richTextBox4.Text = "";
            problem = new Problem();
            totalTime = 0;
        }

        private void richTextBox4_KeyUp(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                try
                {
                    result = Convert.ToInt32(richTextBox4.Text);
                }
                catch { richTextBox4.Text = "";  return; }
                if (problem.result == Convert.ToInt32(richTextBox4.Text))
                {
                    reactionTimes[counter - 1] = StopwatchProxy.Instance.Stopwatch.ElapsedMilliseconds;
                    StopwatchProxy.Instance.Stopwatch.Restart();
                    problem = new Problem();
                    totalTime += reactionTimes[counter - 1];
                    richTextBox2.Text = $"Reaction time: {TimeSpan.FromMilliseconds(totalTime / counter).Seconds}s {TimeSpan.FromMilliseconds(totalTime / counter).Milliseconds}ms.{Environment.NewLine}";
                    richTextBox3.Text = $"Correct Answers: {counter}";
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
                    if (counter == 5) { stopButton.PerformClick(); return; }
                    counter++;
                    richTextBox4.Text = "";
                }
                else
                {
                    richTextBox4.Text = "";
                }
            }
        }
    }
}
