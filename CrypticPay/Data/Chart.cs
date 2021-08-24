using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrypticPay.Data
{
    public class Chart
    {

        public class Dataset
        {
            public string label { get; set; }
            public string backgroundColor { get; set; }
            public string borderColor { get; set; }
            public List<double> data { get; set; }
            public int pointRadius { get; set; }
            public int pointHitRadius { get; set; }
            public string pointBackgroundColor { get; set; }
            public string pointHoverBackgroundColor { get; set; }
            public string pointBorderColor { get; set; }
        }

        public class Data
        {
            public List<string> labels { get; set; }
            public List<Dataset> datasets { get; set; }
        }

        public class Legend
        {
            public bool display { get; set; }
        }

        public class GridLines
        {
            public bool drawBorder { get; set; }
            public bool display { get; set; }
        }

        public class Ticks
        {
            public bool display { get; set; }
        }

        public class XAx
        {
            public GridLines gridLines { get; set; }
            public Ticks ticks { get; set; }
        }

        public class YAx
        {
            public GridLines gridLines { get; set; }
        }

        public class Scales
        {
            public List<XAx> xAxes { get; set; }
            public List<YAx> yAxes { get; set; }
        }

        public class Options
        {
            public Legend legend { get; set; }
            public Scales scales { get; set; }
        }

        public class Root
        {
            public string type { get; set; }
            public Data data { get; set; }
            public Options options { get; set; }
        }

    }

}
