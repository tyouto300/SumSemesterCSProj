Attempt at mapping network topology recursively using SaltwaterTaffy, an nmap wrapper for .Net, and Windows Forms.
To use, open in IDE and run SemesterCSProj,(Command line does not work), wait a little for scanning to finish, and the result will automatically display.
As command line does not work, to change initial scan parameters it must be done in the IDE, unfortunately.

Initial program stars with a /23 prefix scan on either the local address or the gateway address. The detected hosts are displayed in a circle around the center of the screen.
For each hosts, a recursive scan with a decreased prefix(default is 8 bits) is performed, and new hosts detected from this are displayed in an arc outwards from the node whose address
was used for the scan. Nodes will only be displayed in an arc if they hae not already been displayed.
