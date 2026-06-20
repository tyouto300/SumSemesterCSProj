using System;
using ScanData;
using System.Windows.Forms;
namespace ArcQueue {
    class ArcQueueElement {
        public (int x, int y) arcHostPos { get; set; }
        public (int x, int y) parentPos { get; set; }
        public double currentAngle { get; set; }
        public double angleAllowed { get; set; }
        //public ScanDataResult hostResults { get; set; }
        public string scanAddress { get; set; }
        public Label hostAddressDisplay { get; set; }
        public ArcQueueElement() {
            ;
        }
    }
}