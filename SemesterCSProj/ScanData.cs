
using SaltwaterTaffy;
using SaltwaterTaffy.Container;
using System;
using System.Collections.Generic;
namespace ScanData {
    public class ScanDataResult {
        //Each object represents an nmap scan on an ip
        string scanAddress;
        int cidrPrefix;
        public HashSet<string> connectedAddresses;
        public ScanDataResult(string scanAddress,int prefix) {
            this.scanAddress = scanAddress;
            cidrPrefix = prefix;
            connectedAddresses = new HashSet<string>();
        }
        public void printConnectedAddresses() {
            foreach(string i in connectedAddresses) {
                Console.WriteLine(i);
            }
        }
        public void AddConnectedIP(string address) {
            if (address != scanAddress) {
                connectedAddresses.Add(address);
            }     
        }
        public int getPrefix() {
            return cidrPrefix;
        }
        //static Dictionary<string, HostDataResult> scannedIPs = new Dictionary<string, HostDataResult>();
        public HashSet<string> getIPInfo(string seenHosts = "") {
            //Console.WriteLine($"Starting scan with IP {scanAddress} and prefix {cidrPrefix}");
            
            Scanner scanner = new Scanner(new Target(scanAddress + $"/{cidrPrefix}"));
            //standard scanner optionset. Later change this to be a constant and allow for dynamic choosing of options
            //Chosen for speed and privacy over information gathered. Port info doesn't matter for this application, OS could be interesting
            scanner.PersistentOptions = new NmapOptions {
                {NmapFlag.MaxRttTimeout,"200ms"},
                {NmapFlag.MaxRetries, "2"},
                {NmapFlag.NeverDnsResolve, "-n"},
                {NmapFlag.HostScan, "-sn"},
                
            };
            if(seenHosts != "") {//Adding all seen hosts to the exclude flag. Doesnt work but it should
                scanner.PersistentOptions.Add(NmapFlag.ExcludeHosts, seenHosts);
            }
            var scanResult = scanner.Scan();

            foreach (Host i in scanResult.Hosts) {
                string scannedIP = i.Address.ToString();
                //Add scanned IP to current addresses connected list
                AddConnectedIP(scannedIP);
                Console.WriteLine(scannedIP);
            }
            //After getting ip info, return the set of connected ips so that the main program can add them to the dictionary
            return connectedAddresses;
        }
    }
}
