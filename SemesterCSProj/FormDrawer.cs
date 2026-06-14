
using System.Collections.Generic;
using System.Drawing;
using ScanData;
//Helper class for drawing output of scan
namespace SemesterCSProj {
    class formDrawer {
        //Display config(size,color,start coords....
        public int circleRadius { get; set; }
        public (int h, int w) nodeDims { get; set; }
        public (int x, int y) startPos { get; set; }
        public Brush hostDefaultColor { get; set; }
        //Variables to control drawing arc. Current pos,connected addresses...
        public double arcAngle { get; set; }
        public (string, ScanDataResult) currentWorkingAddr { get; set; }
        public (int x, int y) arcHostPos { get; set; }
        public int arcRadius { get; set; }
        //Management and variables for changing display of certain variables
        public string myAddress { get; set; }
        public (string, ScanDataResult) startAddress { get; set; }

        public Dictionary<string, ScanDataResult> IPsToDisplay;


        public formDrawer() {
            IPsToDisplay = new Dictionary<string, ScanDataResult>();

        }
    }
}
