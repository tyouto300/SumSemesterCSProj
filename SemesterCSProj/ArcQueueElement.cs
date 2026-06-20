using System;
using ScanData;
using System.Windows.Forms;
namespace ArcQueue {
    class ArcQueueElement {
        //Represents an arc node waiting to be drawn after circle is finished. Contains info needed at time of creation to determine placement
        public (int x, int y) arcHostPos { get; set; }
        public (int x, int y) parentPos { get; set; }
        public double currentAngle { get; set; }
        public double angleAllowed { get; set; }
        public string scanAddress { get; set; }
        public Label hostAddressDisplay { get; set; }
        public ArcQueueElement() {
            ;
        }
    }
}