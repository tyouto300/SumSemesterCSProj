using ScanData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Windows.Forms;
using System.Net;

namespace SemesterCSProj {
    internal static class Program {

        static Dictionary<string, ScanDataResult> scannedIPs = new Dictionary<string, ScanDataResult>();

        private static string getDefaultGateway() {
            //Gets all open network interfaces(Should be only one, or if theres multiple they should all share the same default gateway,
            //unless the machine has multiple NICs and is configured to go to different networks
            var gatewayAddress = NetworkInterface.GetAllNetworkInterfaces()
                .Where(interf => interf.OperationalStatus == OperationalStatus.Up)
                .SelectMany(interf => interf.GetIPProperties().GatewayAddresses)
                .FirstOrDefault().Address;
            try {
                return gatewayAddress.ToString();
            }
            catch (Exception) {
                Console.WriteLine("Error getting default gateway address");
                return "";
            }
        }
        private static string getLocalAddress() {
            string localhost = Dns.GetHostEntry(Dns.GetHostName()).AddressList
                .FirstOrDefault(i => i.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).ToString();
            if (localhost != null) {
                return localhost;
            }
            else {
                Console.WriteLine("Error getting local address");
                return "";
            }
        }
        private static Form1 createForm() {
            Form1 temp = new Form1();
            foreach(string host in scannedIPs.Keys) {
                temp.copyIP(host, scannedIPs[host]);
            }
            return temp;
        }    
        //To make it reasonably possibly to finish, I will only handle cidr prefixes in 8 bit increments
        public static void recursiveScanning(ScanDataResult ipScan, int recursionCount, int prefixIncrement = 8) {
            foreach (string ip in ipScan.connectedAddresses) {
                ScanDataResult result = new ScanDataResult("", 32);
                if (!scannedIPs.TryGetValue(ip, out result)) {
                    //If we did not scan with a /24 or higher prefix, scan again with one less recursion 
                    //If we reach an ip address we've already seen, we don't want to scan it again, we already know whats behind it.
                    //We cannot find any new addresses either as we operate in a breadth first search, so the first time we get to an address
                    //we will have the highest recursionCount we could have.Technically not true, as nodes could connect during the scan, but I will not
                    //handle this case. By default, increase the network part by 8 bits every time
                    int newPrefix = Math.Min(32, ipScan.getPrefix() + prefixIncrement);
                    if (newPrefix < 32 && recursionCount > 0) {
                        ScanDataResult newScan = new ScanDataResult(ip, newPrefix );
                        newScan.getIPInfo(String.Join(",", scannedIPs.Keys));
                        scannedIPs[ip] = newScan;
                        recursiveScanning(scannedIPs[ip], recursionCount - 1, prefixIncrement);
                    }
                }
            }
        }
        static void Main() {
            //Main program handles organizing all scans, changing prefixes, and recursion. ScanData handles actually scannning of an ip and organizing those results
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            string gatewayAddress = getDefaultGateway();
            string localAddress = getLocalAddress();
            bool localScan = false;
            int cidrPrefix = 23;
            ScanDataResult initialScan = new ScanDataResult(localScan ? localAddress : gatewayAddress, cidrPrefix);
            initialScan.getIPInfo(String.Join(",", scannedIPs.Keys));
            scannedIPs[localScan ? localAddress : gatewayAddress] = initialScan;
            recursiveScanning(initialScan, 4);

            Form1 displayForm = createForm();
            displayForm.setStartAddress(localScan ? localAddress : gatewayAddress, initialScan);
            displayForm.updateMyAddress(localScan ? localAddress : gatewayAddress);
            Application.Run(displayForm);
        }

    }
}
