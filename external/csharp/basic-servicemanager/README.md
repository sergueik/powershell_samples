### Info

Replica of [service optimizer](https://github.com/imribiy/service-optimizer) aplication that lists the services that are actively running on Windows and whose startup status is automatic and allows to disable them

### NOTE

The application uses to execute when run by non admin user. This temporarily been commented out

### Usage
```sh
curl -skLo ~/Downloads/PSTools.zip https://download.sysinternals.com/files/PSTools.zip
```
```sh
unzip -l ~/Downloads/PSTools.zip | grep -i psexec
```
```txt
  716176  2023-04-11 18:10   PsExec.exe
   833472  2023-04-11 18:10   PsExec64.exe
```
```sh
unzip -x ~/Downloads/PSTools.zip PsExec.exe PsExec64.exe
```
### See Also
   * `psexec` [documentation](https://learn.microsoft.com/en-us/sysinternals/downloads/psexec)
   * `PsExec.exe` [command line options](https://ss64.com/nt/psexec.html)
   * https://github.com/tylerdotrar/RGBwiki/blob/main/vault/Red%20Cell/04.%20Privilege%20Escalation/Windows/PsExec.md
   * https://github.com/megsystem/DebloaterTool
   * https://github.com/imribiy/group-policy-library
   * [serviwin](https://www.nirsoft.net/utils/serviwin.html)
   * https://github.com/imribiy/useful-regs-bats
   * [open and lightweight modification to Windows, designed to optimize performance, privacy and usability](https://github.com/Atlas-OS/Atlas)
   * [forge-postinstall.ps1](https://github.com/imribiy/Forged-NTLite/blob/main/Forge-PostInstall.ps1) - stripped-down, aggressively optimized Windows 11 24H2 build for dedicated gaming and special-purpose PCs. This project removes bloat, disables telemetry, maximizes system performance, and applies privacy-focused settings. It is intended for isolated, non-production systems where security trade-offs are acceptable
   * [useful group policy dwords for Windows 10 & 11](https://github.com/imribiy/group-policy-library)
   * https://github.com/dkmilan/service-master



