using System.Windows;
using ScottPlot;

namespace WpfApp
{

    public partial class MainWindow : Window
    {
        IPalette palette;
        BFO bfo;
        System.Windows.Threading.DispatcherTimer timer;

        public MainWindow()
        {
            InitializeComponent();

            Init();
        }


        private void Init()
        {
            bfo = new BFO();
            bfo.TimerNotify += () => { timer.Stop(); };
            bfo.ContersNotify += (array) => 
            {
                if (array.Length != 3) return;

                lbNed.Content = array[0].ToString();
                lbNre.Content = array[1].ToString();
                lbNc.Content = array[2].ToString();
            };

            timer = new System.Windows.Threading.DispatcherTimer();
            timer.Tick += new EventHandler(timerTick);
            timer.Interval = new TimeSpan(0, 0, 0, 0, 50);

            // ScotPlot draw
            WpfPlot1.Plot.Clear();

            palette = new ScottPlot.Palettes.Category10();

            for (int i = 0; i < bfo.colony.bacteria.Length; ++i)
            {
                var x = bfo.colony.bacteria[i].position[0]; // X
                var y = bfo.colony.bacteria[i].position[1]; // Y
                var c = WpfPlot1.Plot.Add.Circle(x, y, .1);

                c.FillStyle.Color = Colors.Blue;
                c.LineWidth = 0;
            }

            // force circles to remain circles
            ScottPlot.AxisRules.SquareZoomOut squareRule = new(WpfPlot1.Plot.Axes.Bottom, WpfPlot1.Plot.Axes.Left);
            WpfPlot1.Plot.Axes.Rules.Add(squareRule);

            // legends
            LegendItem legendBacteria = new LegendItem()
            {
                LabelText = "Bacteria",
                FillColor = Colors.Blue,
            };
            LegendItem legendBest = new LegendItem()
            {
                LabelText = "Best solution",
                FillColor = Colors.Red,
            };
            var legends = new List<LegendItem>() { legendBacteria, legendBest };

            WpfPlot1.Plot.ShowLegend(legends);

            WpfPlot1.Refresh();
        }

        private void timerTick(object sender, EventArgs e)
        {
            bfo.Calculate();

            WpfPlot1.Plot.Clear();

            double x = 0, y = 0;
            ScottPlot.Plottables.Ellipse c;

            for (int i = 0; i < bfo.colony.bacteria.Length; ++i)
            {
                x = bfo.colony.bacteria[i].position[0]; // X
                y = bfo.colony.bacteria[i].position[1]; // Y
                c = WpfPlot1.Plot.Add.Circle(x, y, .1);

                c.FillStyle.Color = Colors.Blue;
                c.LineWidth = 0;
            }

            x = bfo.bestPosition[0]; // X
            y = bfo.bestPosition[1]; // Y
            c = WpfPlot1.Plot.Add.Circle(x, y, .1);

            c.FillStyle.Color = Colors.Red;
            c.LineWidth = 0;

            WpfPlot1.Refresh();
        }

        private void btnStart_Click(object sender, RoutedEventArgs e) => timer.Start();
        private void btnClear_Click(object sender, RoutedEventArgs e) => Init();
    }
}