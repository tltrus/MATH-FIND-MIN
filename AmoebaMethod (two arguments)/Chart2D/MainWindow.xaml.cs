using System.Globalization;
using System.Windows;
using System.Windows.Media;
using _Chart2D.Classes;


namespace _Chart2D
{
    // Test Run - Amoeba Method Optimization using C#
    // https://learn.microsoft.com/en-us/archive/msdn-magazine/2013/june/test-run-amoeba-method-optimization-using-csharp
    public partial class MainWindow : Window
    {
        System.Windows.Threading.DispatcherTimer timer;
        AmoebaOptimization Amoeba { get; set; }
        double bestX, bestY;

        DrawingVisual visual;
        DrawingContext dc;
        double width, height;
        Axis axis;
        FunctionXY Func3D;
        int state = 0;
        Solution sln;
        List<Point> slnPoints;
        double minX = -10.0;
        double maxX = 10.0;
        int dim = 2;  // problem dimension (number of variables to solve for)
        int amoebaSize = 3;  // number of potential solutions in the amoeba
        int maxLoop = 150;

        public MainWindow()
        {
            InitializeComponent();

            visual = new DrawingVisual();
            width = g.Width;
            height = g.Height;
            axis = new Axis(width, height);

            timer = new System.Windows.Threading.DispatcherTimer();
            timer.Tick += new EventHandler(timerMainTick);
            timer.Interval = new TimeSpan(0, 0, 0, 0, 20);

            Init();
        }

        private void Init()
        {
            state = 0;
            rtbConsole.Clear();
            slnPoints = new List<Point>();

            //Func<double, double, double> func1 = (x, y) => 5.0 * (x * x) + (y * y);
            //Func<double, double, double> func2 = (x, y) => 3 * Math.Pow((1 - x), 2) * Math.Exp(-x * x - (y + 1) * (y + 1)) - 10 * (0.2 * x - Math.Pow(x, 3) - Math.Pow(y, 5)) * Math.Exp(-x * x - y * y) - 1 / 3 * Math.Exp(-(x + 1) * (x + 1) - y * y);
            //Func<double, double, double> func3 = (x, y) => ((x * x) - (10 * Math.Cos(2 * Math.PI * x)) + 10)+ ((y * y) - (10 * Math.Cos(2 * Math.PI * y)) + 10);
            //Func<double, double, double> func4 = (x, y) => (x - 1) * (x - 1) + (y - 1) * (y - 1) - (x * y);
            //Func<double, double, double> func5 = (x, y) => (-x * Math.Sin(Math.Sqrt(Math.Abs(x)))) + (-y * Math.Sin(Math.Sqrt(Math.Abs(y))));
            Func<double, double, double> func6 = (x, y) => 100.0 * Math.Pow((y - x * x), 2) + Math.Pow(1 - x, 2);

            int contours = 20;
            Func3D = new FunctionXY(width, height, contours, minX, maxX, minX, maxX);
            Func3D.SetFunc(func6);
            tbCnum.Text = contours.ToString();

            Func3DControl();
            Drawing();
        }

        // Method like MoveNext
        private void AmoebaControl()
        {
            switch (state)
            {
                case 0:
                    rtbConsole.AppendText("\rBegin amoeba method optimization demo");
                    rtbConsole.AppendText("\rSolving Rosenbrock's function f(x,y) = 100*(y-x^2)^2 + (1-x)^2");
                    rtbConsole.AppendText("\rFunction has a minimum at x = 1.0, y = 1.0 when f = 0.0");

                    rtbConsole.AppendText("\r\rCreating amoeba with size = " + amoebaSize);
                    rtbConsole.AppendText("\rSetting maxLoop = " + maxLoop);

                    Amoeba = new AmoebaOptimization(amoebaSize, dim, minX, maxX, maxLoop);
                    Amoeba.BestSolutionNotify += (str) => { rtbConsole.AppendText(str); };
                    Amoeba.IterationSolutionNotify += (sln) =>
                    {
                        slnPoints.Clear();

                        // Get points from ameoba solutions
                        foreach (var s in sln)
                        {
                            var x = s.vector[0];
                            var y = s.vector[1];

                            Point point = new Point(x, y);
                            slnPoints.Add(point);
                        }
                    };

                    rtbConsole.AppendText("\rInitial amoeba is:\n");
                    rtbConsole.AppendText(Amoeba.ToString());

                    state = 1;
                    break;

                case 1:
                    rtbConsole.AppendText("\rBeginning reflect-expand-contract solve loop");

                    state = 2;
                    break;
                case 2:
                    sln = Amoeba.Solve();

                    if(sln is not null)
                        state = 3;
                    break;
                case 3:
                    rtbConsole.AppendText("\rSolve complete");

                    state = 4;
                    break;
                case 4:
                    rtbConsole.AppendText("\r\rFinal amoeba is:\n");
                    rtbConsole.AppendText(Amoeba.ToString());

                    rtbConsole.AppendText("\rBest solution found:");
                    rtbConsole.AppendText(sln.ToString());

                    rtbConsole.AppendText("\r\rEnd amoeba method optimization demo");

                    timer.Stop();
                    btnStart.Content = "Start timer";

                    state = -1;
                    break;
            }
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            var contour_num = int.Parse(tbCnum.Text);
            Func3D.SetNumberContours(contour_num);

            Drawing();
        }
        private void cbDrawFunc_Click(object sender, RoutedEventArgs e) => Drawing();
        private void cbDrawContour_Click(object sender, RoutedEventArgs e) => Drawing();
        private void Func3DControl()
        {
            var contour_num = int.Parse(tbCnum.Text);
            Func3D.SetNumberContours(contour_num);
            Func3D.Calculation();
        }
        private void Drawing()
        {
            g.RemoveVisual(visual);
            using (dc = visual.RenderOpen())
            {
                if (cbDrawFunc.IsChecked == true) Func3D.DrawFunc(dc);
                if (cbDrawContour.IsChecked == true) Func3D.DrawContour(dc);

                // Draw target
                var norm = Tools.Normalize(new Point(1, 1), width, height, minX, maxX, minX, maxX);
                dc.DrawEllipse(Brushes.Blue, null, norm, 5, 5);

                // Draw ameoba points
                foreach (var point in slnPoints)
                {
                    var normalize = Tools.Normalize(point, width, height, minX, maxX, minX, maxX);
                    dc.DrawEllipse(Brushes.Red, null, normalize, 4, 4);
                }

                axis.Draw(dc, visual);

                dc.Close();
                g.AddVisual(visual);
            }
        }

        private void btnStart_Click(object sender, RoutedEventArgs e)
        {
            if (!timer.IsEnabled)
            {
                Init();
                timer.Start();
                btnStart.Content = "Stop timer";
            } else
            {
                timer.Stop();
                btnStart.Content = "Start timer";
            }
        }

        private void timerMainTick(object sender, EventArgs e)
        {
            Func3DControl();
            AmoebaControl();
            Drawing();
        }
    }
}