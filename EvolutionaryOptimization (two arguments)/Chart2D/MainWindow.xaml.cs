using System.Globalization;
using System.Windows;
using System.Windows.Media;
using static _Chart2D.EvolutionaryOptimization;


namespace _Chart2D
{
    // Test Run - Evolutionary Optimization Algorithms
    // https://learn.microsoft.com/en-us/archive/msdn-magazine/2012/june/test-run-evolutionary-optimization-algorithms

    public partial class MainWindow : Window
    {
        System.Windows.Threading.DispatcherTimer timer;
        EvolutionaryOptimization EvolutionaryOptimization { get; set; }
        double bestX, bestY;

        DrawingVisual visual;
        DrawingContext dc;
        double width, height;
        Axis axis;
        FunctionXY Func3D;
        int state = 0;

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

        void Init()
        {
            state = 0;
            rtbConsole.Clear();

            rtbConsole.AppendText("\rBegin Evolutionary Optimization demo");
            rtbConsole.AppendText("\r\rGoal is to find the (x,y) that minimizes Schwefel's function");
            rtbConsole.AppendText("\rf(x,y) = (-x * sin(sqrt(abs(x)))) + (-y * sin(sqrt(abs(y))))");
            rtbConsole.AppendText("\rKnown solution is x = y = 420.9687 when f = -837.9658");

            EvolutionaryOptimization = new EvolutionaryOptimization();
            EvolutionaryOptimization.Calculate();
            EvolutionaryOptimization.GenerationNotify += (value) =>
            {
                lbGen.Content = value + " / " + EvolutionaryOptimization.maxGeneration;
            };
            EvolutionaryOptimization.BestSolutionNotify += (array) =>
            {
                if (array is null)
                {
                    timer.Stop();
                    rtbConsole.AppendText("\r\rBest position is at [ " + bestX.ToString("F3") + "  " + bestY.ToString("F3") + " ]");

                    return;
                }
                
                if (array.Length != 2) return;

                bestX = array[0];
                bestY = array[1];
            };

            rtbConsole.AppendText("\r\rPopulation size = " + EvolutionaryOptimization.ev.popSize);
            rtbConsole.AppendText("\rNumber genes = " + EvolutionaryOptimization.ev.numGenes);
            rtbConsole.AppendText("\rminGene value = " + EvolutionaryOptimization.ev.minGene.ToString("F1"));
            rtbConsole.AppendText("\rmaxGene value = " + EvolutionaryOptimization.ev.maxGene.ToString("F1"));
            rtbConsole.AppendText("\rMutation rate = " + EvolutionaryOptimization.ev.mutateRate.ToString("F4"));
            rtbConsole.AppendText("\rMutation precision = " + EvolutionaryOptimization.ev.precision.ToString("F4"));
            rtbConsole.AppendText("\rSelection pressure tau = " + EvolutionaryOptimization.ev.tau.ToString("F2"));
            rtbConsole.AppendText("\rMaximum generations = " + EvolutionaryOptimization.ev.maxGeneration);

            //Func<double, double, double> func1 = (x, y) => 5.0 * (x * x) + (y * y);
            //Func<double, double, double> func2 = (x, y) => 3 * Math.Pow((1 - x), 2) * Math.Exp(-x * x - (y + 1) * (y + 1)) - 10 * (0.2 * x - Math.Pow(x, 3) - Math.Pow(y, 5)) * Math.Exp(-x * x - y * y) - 1 / 3 * Math.Exp(-(x + 1) * (x + 1) - y * y);
            //Func<double, double, double> func3 = (x, y) => ((x * x) - (10 * Math.Cos(2 * Math.PI * x)) + 10)+ ((y * y) - (10 * Math.Cos(2 * Math.PI * y)) + 10);
            //Func<double, double, double> func4 = (x, y) => (x - 1) * (x - 1) + (y - 1) * (y - 1) - (x * y);
            Func<double, double, double> func5 = (x, y) => (-x * Math.Sin(Math.Sqrt(Math.Abs(x)))) + (-y * Math.Sin(Math.Sqrt(Math.Abs(y))));

            Func3D = new FunctionXY(width, height, 5, -500, 500, -500, 500);
            Func3D.SetFunc(func5);

            Func3DControl();
            Drawing();
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

                // Draw points
                for (int i = 0; i < EvolutionaryOptimization.ev.population.Length; ++i)
                {
                    var X = EvolutionaryOptimization.ev.population[i].chromosome[0]; // X
                    var Y = EvolutionaryOptimization.ev.population[i].chromosome[1]; // Y
                    
                    var point = new Point(X, Y);
                    var normalize = Tools.Normalize(point, width, height, -500, 500, -500, 500);

                    dc.DrawEllipse(Brushes.Red, null, normalize, 4, 4);
                }

                // Target
                var x = 420.9687; // X
                var y = 420.9687; // Y
                var p = new Point(x, y);
                var norm = Tools.Normalize(p, width, height, -500, 500, -500, 500);
                dc.DrawEllipse(Brushes.Blue, null, norm, 5, 5);

                // Best solution
                p = new Point(bestX, bestY);
                norm = Tools.Normalize(p, width, height, -500, 500, -500, 500);
                dc.DrawEllipse(Brushes.WhiteSmoke, null, norm, 4, 4);

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

            EvolutionaryOptimization.Calculate();

            Drawing();
        }
    }
}