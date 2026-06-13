using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using ScanData;

namespace SemesterCSProj{
    public partial class Form1 : Form {
    //Display will take the form of circles, wit hthe first one being centered around  the gateway ip. Routers/hosts with extra connections
    //from the gateway will be arranged in an arc facing away from the rest of the circle as to make viewing easier.
    Brush hostColorDefault = new SolidBrush(Color.Lime);
    const int circleRadius = 100;//Radius of circle/arc drawn for a host.
    const int startCirclePosition = 250;
    const int hostCircleSize = 10;//Size of the actual graphic in h,w for each host
    formDrawer setupDrawer() {
        formDrawer temp = new formDrawer();
        temp.circleRadius = circleRadius;
        temp.hostDefaultColor = hostColorDefault;
        temp.nodeDims = (hostCircleSize, hostCircleSize);
        temp.startPos = (startCirclePosition, startCirclePosition);
        return temp;    
    }
        formDrawer linkedDrawer;
        public Form1() {
            linkedDrawer = setupDrawer();
            InitializeComponent();

            this.Paint += drawCircle;
            //this.Paint += drawArc;
        }
        public void copyIP(string addr, ScanDataResult data) {
            linkedDrawer.IPsToDisplay[addr] = data;
        }
        public void updateMyAddress(string addr) {
            linkedDrawer.myAddress = addr;
        }
        public void setGatewayAddress(string address, ScanDataResult data) {
            linkedDrawer.gatewayAddress = (address, data);
        }
        //Drawing a circle is the same as drawing an arc with 360 degrees
        Label buildLabel(int x, int y, string name) {
            Label label = new Label();
            label.Location = new Point(x - 10, y - 15);
            label.Size = new Size(50, 15);
            label.Text = name;
            label.AutoSize = true;
            label.Name = name;
            return label;
        }
        private void drawCircle(object sender, PaintEventArgs e) {
            
            Graphics g = e.Graphics;
            Label newLabel = new Label();

            newLabel.Location = new Point(startCirclePosition, startCirclePosition);
            newLabel.Size = new Size(50, 15);
            newLabel.Text = linkedDrawer.gatewayAddress.Item1;
            newLabel.AutoSize = true;
            this.Controls.Add(newLabel);

            var theta = (Math.PI * 2) / linkedDrawer.gatewayAddress.Item2.connectedAddresses.Count;//How much of an angle each node gets
            double angleAccumulated = 0.0;
            foreach (string i in linkedDrawer.gatewayAddress.Item2.connectedAddresses) {
                //Circle radius is 100(const) so each new host will have (x,y)
                //x = 250 + (100 * cos(anglePerHost))
                //y = 250 + (100 * sin(anglePerHost))
                int x = (int)(linkedDrawer.startPos.x + (circleRadius * Math.Cos(angleAccumulated)));
                int y = (int)(linkedDrawer.startPos.y + (circleRadius * Math.Sin(angleAccumulated)));

                this.Controls.Add(buildLabel(x, y, i));
                //The center of the line to draw to will be x + 5, y - 5. Circles are drawn after the line, so don't have to worry about stopping before overlap
                //Improve detection/placement of end of line
                g.DrawLine(new Pen(Brushes.Black), startCirclePosition, startCirclePosition, x, y);

                linkedDrawer.currentWorkingAddr = (i, linkedDrawer.IPsToDisplay[i]);
                //Console.WriteLine($"Currently drawing results for address {i}");
                //Console.WriteLine("Connected addresses: ");
                //linkedDrawer.currentWorkingAddr.Item2.printConnectedAddresses();
                linkedDrawer.arcHostPos = (x, y);
                linkedDrawer.arcAngle = angleAccumulated;//Each arc gets to display its elements in an angle of 2 * theta
                angleAccumulated += theta;

                //Console.WriteLine($"Drawing ARC for address {linkedDrawer.currentWorkingAddr.Item1}");
                int point = 0;

                //This is awful. I cannot move this into a method because if I did, then they would not be called when the line is read, but their call would be queued,
                //And only after the circle is finished would it be draw, WITH the last value the linkedDrawer had as its arc parameters. Unreal as to why it works like that
                double totalArcAngle = 2 * theta;
                foreach (string arcHost in linkedDrawer.currentWorkingAddr.Item2.connectedAddresses) {
                    //if (this.Controls.Find(arcHost, true).Length == 0) {
                        double arcTheta = totalArcAngle / linkedDrawer.currentWorkingAddr.Item2.connectedAddresses.Count;
                        double newAngle = linkedDrawer.arcAngle + (point * arcTheta);
                        int arcX = (int)(linkedDrawer.arcHostPos.x + (linkedDrawer.circleRadius * Math.Cos(newAngle)));
                        int arcY = (int)(linkedDrawer.arcHostPos.y + (linkedDrawer.circleRadius * Math.Sin(newAngle)));

                        point += 1;
                        this.Controls.Add(buildLabel(arcX, arcY, arcHost));

                        //linkedDrawer.currentWorkingAddr = (i, linkedDrawer.IPsToDisplay[i]);
                        //linkedDrawer.arcHostPos = (x, y);
                        //linkedDrawer.arcAngle = 2 * theta;
                        //angleAccumulated += theta;
                        //this.Paint += drawArc;
                        //Console.WriteLine($"Drawing new node at  x = {x2}, y = {y2}, with label {j}");
                        //Adjust drawing of line so that it fits to host better
                        //Fix drawing of hosts who have already been drawn
                        g.DrawLine(new Pen(Brushes.Black), linkedDrawer.arcHostPos.x, linkedDrawer.arcHostPos.y, arcX, arcY + 5);
                        g.FillEllipse(arcHost == linkedDrawer.myAddress ? new SolidBrush(Color.Purple) : linkedDrawer.hostDefaultColor, arcX, arcY, linkedDrawer.nodeDims.h, linkedDrawer.nodeDims.w);
                   // }
                }

                //When doing arc drawing, draw arc nodes and lines before the node the arc from
                g.FillEllipse(i == linkedDrawer.myAddress ? new SolidBrush(Color.Purple) : hostColorDefault, x, y, hostCircleSize, hostCircleSize);
                //Update calculation to be better
            }
            g.FillEllipse(hostColorDefault, startCirclePosition, startCirclePosition, hostCircleSize, hostCircleSize);
        }
    }
}
