using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using static System.Windows.Forms.DataFormats;

namespace TowerOfHanoi
{
    public partial class Form1 : Form
    {
        static int n, select, NumberOfMoves = 0, XStep, YStep, CurrentMove = 0, size = 20;
        static int[] NumberOfPegs = new int[3]; //Represents the number of pegs on each rod. where NumberOfPegs[0] is rod 0, NumberOfPegs[1] is rod 1 and NumberOfPegs[2] is rod 2
        static Peg[] Pegs; //Used to store all available pegs.
        static Peg Current; //Current Peg to move.
        static Queue<Peg> Order;//Used to store order of moves.
        static Button StartStop = new Button(); //Start/Stop button.
        NumericUpDown nSelect = new NumericUpDown(); //Peg number selector.
        static Label TotalSteps = new Label(), StepNumber = new Label(); //Used to display total steps needed and current step respectively.
        static bool inAir = false, AboveTarget = false; //Used for moving pegs.
        public Form1()
        {
            InitializeComponent();
            WindowState = FormWindowState.Maximized;
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            AllocConsole();
            timer1.Interval = 50;
            //To Stop Flickering.
            this.SetStyle(
               System.Windows.Forms.ControlStyles.UserPaint |
               System.Windows.Forms.ControlStyles.AllPaintingInWmPaint |
               System.Windows.Forms.ControlStyles.OptimizedDoubleBuffer,
               true);

            //Button Setup.
            StartStop.Location = new Point(800, 850);
            StartStop.Size = new Size(300, 100);
            StartStop.Text = "Start";
            StartStop.Font = new Font(StartStop.Font.FontFamily, 30);
            StartStop.Click += StartStop_Click;

            //Selector Setup.
            nSelect.Location = new Point(300, 900);
            nSelect.Size = new Size(100, 600);
            nSelect.Maximum = 7;
            nSelect.Minimum = 3;
            n = (int)nSelect.Value;
            nSelect.ValueChanged += nSelect_ValueChanged;

            TowerSetup();

            //Step Counter Setup.
            TotalSteps.Location = new Point(30, 30);
            StepNumber.Location = new Point(30, 130);
            TotalSteps.Size = new Size(500, 100);
            StepNumber.Size = new Size(500, 100);
            TotalSteps.Font = new Font(TotalSteps.Font.FontFamily, 30);
            StepNumber.Font = new Font(TotalSteps.Font.FontFamily, 30);

            //Adding To Window.
            this.Controls.Add(nSelect);
            this.Controls.Add(StartStop);
            this.Controls.Add(TotalSteps);
            this.Controls.Add(StepNumber);
            timer1.Stop();

            Console.WriteLine(Height);

        }
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();
        static void TowerSetup()
        {
            inAir = false;
            AboveTarget = false;
            NumberOfMoves = 0;
            CurrentMove = 0;
            NumberOfPegs[0] = n; NumberOfPegs[1] = 0; NumberOfPegs[2] = 0;//Initialize towers 0, 1 and 2
            Order = new Queue<Peg>();
            Color[] Colors = { Color.Purple, Color.Blue, Color.Cyan, Color.Green, Color.Yellow, Color.Orange, Color.Red };//Possible Colors.
            Pegs = new Peg[n];
            Color[] UsedColors = new Color[n];
            for (int i = 0; i < n; i++)
            {
                UsedColors[i] = Colors[7 - n + i];
            }
            int color = 6;
            //Setup pegs.
            for (int i = n; i >= 1; i--)
            {
                //Starts setup from smallest to largest.
                Pegs[n - i] = new Peg(n,//Order of each peg starting from largest.
                    new Point(300 + (size * (n - i)), 780 - (size * (n - i))), //Starting point of each peg, decreasing slightly each peg.
                    new Point(740 - (size * (n - i)), 780 - (size * (n - i))), //Ending point of each peg.
                    UsedColors[n - i]);//Color used for each peg.
            }
            sortTower(n, 0, 2, 1);

            TotalSteps.Text = "Total Steps: " + NumberOfMoves;
            StepNumber.Text = "Current Step: " + CurrentMove;
            Current = Order.Dequeue();
        }
        static void sortTower(int i, int From, int To, int NotUsed)
        {

            if (i == 0)
                return;
            sortTower(i - 1, From, NotUsed, To);
            Order.Enqueue(new Peg(i - 1,
                new Point(300 + (size * (n - i)) + (440 * To), 780 - (size * NumberOfPegs[To])),
                new Point(740 - (size * (n - i)) + (440 * To), 780 - (size * NumberOfPegs[To])),
                Pegs[i - 1].Color
                ));
            (NumberOfPegs[From])--;
            (NumberOfPegs[To])++;
            NumberOfMoves++;
            sortTower(i - 1, NotUsed, To, From);

        }
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Pen pen = new Pen(Color.Black);
            pen.Width = size;
            //e.Graphics.DrawLine(pen to use, x1, y1, x2, y2);
            e.Graphics.DrawLine(pen, 300, 800, 1620, 800);//Base
            e.Graphics.DrawLine(pen, 520, 800, 520, 500);//Left Pole
            e.Graphics.DrawLine(pen, 960, 800, 960, 500);//Middle Pole
            e.Graphics.DrawLine(pen, 1400, 800, 1400, 500);//Right Pole
            for (int i = 0; i < n; i++)
            {
                pen.Color = Pegs[i].Color;
                e.Graphics.DrawLine(pen, Pegs[i].Start, Pegs[i].End);
            }

        }
        private void nSelect_ValueChanged(object sender, EventArgs e)
        {
            n = (int)nSelect.Value;
            timer1.Stop();
            StartStop.Text = "Start";
            TowerSetup();
            Invalidate();
        }
        private void StartStop_Click(object sender, EventArgs e)
        {

            if (StartStop.Text.Equals("Reset"))
            {
                TowerSetup();
                timer1.Stop();
                StartStop.Text = "Start";
                Invalidate();
            }
            else if (timer1.Enabled)
            {
                timer1.Stop();
                StartStop.Text = "Start";
            }
            else
            {
                if (CurrentMove == 0)
                    StepNumber.Text = "Current Step: " + (++CurrentMove);
                timer1.Start();
                StartStop.Text = "Stop";
            }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (Current.Start == Pegs[n - Current.Order - 1].Start && Current.End == Pegs[n - Current.Order - 1].End)
            {
                if (Order.Count == 0)
                {
                    timer1.Stop();
                    StartStop.Text = "Reset";
                    return;
                }
                Current = Order.Dequeue();
                StepNumber.Text = "Current Step: " + (++CurrentMove);
                inAir = false;
                AboveTarget = false;
            }

            if (!inAir && Pegs[n - Current.Order - 1].Start.Y > 480)
            {
                Pegs[n - Current.Order - 1].Start.Y -= 60;
                Pegs[n - Current.Order - 1].End.Y -= 60;
            }
            else if (!inAir)
            {
                inAir = true;
                XStep = (Current.Start.X - Pegs[n - Current.Order - 1].Start.X) / 5;
                YStep = (Current.Start.Y - Pegs[n - Current.Order - 1].Start.Y) / 5;
            }
            if (inAir && Pegs[n - Current.Order - 1].Start.X != Current.Start.X)
            {
                Pegs[n - Current.Order - 1].Start.X += XStep;
                Pegs[n - Current.Order - 1].End.X += XStep;
            }
            else if (Pegs[n - Current.Order - 1].Start.X == Current.Start.X)
            {
                AboveTarget = true;
            }

            if (AboveTarget)
            {
                Pegs[n - Current.Order - 1].Start.Y += YStep;
                Pegs[n - Current.Order - 1].End.Y += YStep;

            }

            Invalidate();

        }
    }
}