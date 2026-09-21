# NextDnsIpMonitor
Simple Windows Service to dynamically link an IP with a NextDNS profile.

Run this as a Windows Service on a computer on your network with your personal NextDNS link IP endpoint in the `appsettings.json` file. Every cycle, the service will check if your public IP address has changed, and if so, it will automatically call the endpoint to link your NextDNS profile to the current IP address.