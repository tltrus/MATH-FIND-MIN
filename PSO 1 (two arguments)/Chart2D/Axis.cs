using System.Configuration.Internal;
using System.Globalization;
using System.Numerics;
using System.Security.RightsManagement;
using System.Windows;
using System.Windows.Media;


namespace _Chart2D
{
    internal class Axis
    {
        double width, height;
        Pen pen;
        int factor;

        public Axis(double w, double h)
        {
            width = w;
            height = h;

            pen = new Pen(Brushes.White, 1);
        }

        public void Draw(DrawingContext dc, DrawingVisual visual)
        {
            // axis X
            dc.DrawLine(pen, new Point(0, height / 2), new Point(width, height / 2));

            // axis Y
            dc.DrawLine(pen, new Point(width / 2, 0), new Point(width / 2, height));

            // X ortha text
            FormattedText formattedText = new FormattedText("X", CultureInfo.GetCultureInfo("en-us"),
                                                FlowDirection.LeftToRight, new Typeface("Verdana"), 12, Brushes.White,
                                                VisualTreeHelper.GetDpi(visual).PixelsPerDip);
            Point textPos = new Point(width - 14, height / 2 - 20);
            dc.DrawText(formattedText, textPos);

            // Y ortha text
            formattedText = new FormattedText("Y", CultureInfo.GetCultureInfo("en-us"),
                                                FlowDirection.LeftToRight, new Typeface("Verdana"), 12, Brushes.White,
                                                VisualTreeHelper.GetDpi(visual).PixelsPerDip); 
            textPos = new Point(width / 2 + 6, 6);
            dc.DrawText(formattedText, textPos);

            //
            for(int i = -8; i < 8; i++)
            {
                Point px = new Point(Xto(i), Yto(0));
                Point py = new Point(Xto(0), Yto(i));

                dc.DrawEllipse(null, new Pen(Brushes.White, 1), px, 1.1, 1.1);
                dc.DrawEllipse(null, new Pen(Brushes.White, 1), py, 1.1, 1.1);
            }
        }

        public void SetFactor(int factor) => this.factor = factor;

        public double scrX(double x) => Math.Round(x - width / 2);
        public double scrY(double y) => Math.Round((y - height / 2) * -1);

        public double Xto(double x) => Math.Round(x * factor + width / 2);
        public double Yto(double y) => Math.Round(height / 2 - y * factor);
    }
}
