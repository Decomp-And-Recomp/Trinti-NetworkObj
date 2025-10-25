# NetworkObj implementation
Written by @newxans
## How to run
Custom port:  
./NetworkObj -p 67  
Without custom port, it will default on port 4201
## Get the server to connect
Open up SFSServerVersion and replace the Start function with this code:  
```cs
	private IEnumerator Start()
	{
		string IP = "144.202.21.189";
		int Port = 4201;

		CoopDomain = IP;
		CoopStandbyServerIP = IP;
		CoopServerPort = Port;
		VsDomain = IP;
		VsStandbyServerIP = IP;
		VsServerPort = 4200; // test
		VsGroupIdMin = 1;
		VsGroupIdMax = 6741;
		callback(true);
		yield break;
	}
```
IP of course being your VPS IP, and Port being the port you ran the program with.
## Support
email: f0nee@proton.me, I will usually respond within 12-24 hours.  
additionally, if your in the Call of Mini server, ping @newxans
