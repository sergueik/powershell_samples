### Info

this directory contains System Tray Application skeleton project (Form version)
based on [codeproject article](https://www.codeproject.com/Articles/290013/Formless-System-Tray-Application) code but converted to  extending `System.Windows.Forms.Form` (for sake of have `DisplayBallonMessage` work)
itegrated with Named Pipes [codeproject article](https://www.codeproject.com/Articles/810030/IPC-with-Named-Pipes).
When application is run and is in the tray, a custom installer can close it by

```powershell
.\client.ps1 -message exit
```

### Author
[Serguei Kouzmine](kouzmine_serguei@yahoo.com)
