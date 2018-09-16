using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.WindowsForms;
namespace MURRDesktop
{
    public partial class PaintWindow : Form
    {
        public PaintWindow()
        {
            InitializeComponent();
            
        }

        public PaintWindow(Dictionary<DateTime, decimal> dictionary)
            :this()
        {
            Painter(dictionary);
        }

        public PaintWindow(Dictionary<decimal, decimal> dictionary)
            :this()
        {
            Painter(dictionary);
        }

        private void Painter(Dictionary<decimal, decimal> dictionary)
        {
            if (dictionary.Count == 0)
                return;

            PlotView plotView = new PlotView();
            plotView.Dock = DockStyle.Fill;
            panel1.Controls.Add(plotView);

            LinearAxis XAxis = new LinearAxis()
            {
                Position = AxisPosition.Bottom,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.None,
            };


            //plotView.Model.Series.Add(functionSeries);
            PlotModel plotModel = new PlotModel();
            plotView.Model = plotModel;
            plotModel.Axes.Add(XAxis);
            FunctionSeries functionSeries = new FunctionSeries();
            Series series = new LineSeries();
            foreach (var x in dictionary)
            {
                DataPoint dataPoint = new DataPoint(Convert.ToDouble(x.Key), Convert.ToDouble(x.Value));
                functionSeries.Points.Add(dataPoint);
            }
            plotView.Model.Series.Add(functionSeries);
        }

        private void PaintWindow_Load(object sender, EventArgs e)
        {

        }

        private void Painter(Dictionary<DateTime, decimal> dictionary)
        {
            if (dictionary.Count == 0)
                return;

            PlotView plotView = new PlotView();
            plotView.Dock = DockStyle.Fill;
            panel1.Controls.Add(plotView);

            LinearAxis XAxis = new DateTimeAxis
            {
                Position = AxisPosition.Bottom,
                Minimum = DateTimeAxis.ToDouble(dictionary.Keys.Min()),
                Maximum = DateTimeAxis.ToDouble(dictionary.Keys.Max()),
                MinorIntervalType = DateTimeIntervalType.Days,
                IntervalType = DateTimeIntervalType.Days,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.None,
            };


            //plotView.Model.Series.Add(functionSeries);
            PlotModel plotModel = new PlotModel();
            plotView.Model = plotModel;
            plotModel.Axes.Add(XAxis);
            FunctionSeries functionSeries = new FunctionSeries();
            Series series = new LineSeries();
            foreach(var x in dictionary)
            {
                DataPoint dataPoint = new DataPoint(DateTimeAxis.ToDouble(x.Key), Convert.ToDouble(x.Value));
                functionSeries.Points.Add(dataPoint);
            }
            plotView.Model.Series.Add(functionSeries);
        }
    }
}
