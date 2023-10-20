### Info


* this directory contains a replica of  [host-sflow](https://github.com/sflow/host-sflow)

### Original README.md and Notes
https://blog.sflow.com/2011/02/windows-load-average.html
https://github.com/sflow/host-sflow
Since load averages aren't natively supported by Windows, this article provides an overview of load measurement and describes how the Windows Host sFlow agent calculates load averages
Definition is:
System load averages is the average number of processes that are either in a runnable or uninterruptable state. A process in a runnable state is either using the CPU or waiting to use the CPU. A process in uninterruptable state is waiting for some I/O access, eg waiting for disk. The averages are taken over the three time intervals. Load averages are not normalized for the number of CPUs in a system, so a load average of 1 means a single CPU system is loaded all the time while on a 4 CPU system it means it was idle 75% of the time.


https://blog.sflow.com/2011/02/windows-load-average.html


https://github.com/sflow/host-sflow/blob/master/src/Windows/hsflowd/loadAverage.c
https://github.com/sflow/host-sflow/blob/master/src/Windows/hsflowd/readWindowsEnglishCounters.c

https://blog.sflow.com/2019/10/flow-metrics-with-prometheus-and-grafana.html
https://sflow.net/downloads.php
https://serverfault.com/questions/328260/what-is-the-closest-equivalent-of-load-average-in-windows-available-via-wmi/872048
https://github.com/sflow/sflow2graphite/blob/master/sflow2graphite.pl
Strting rom

```cmd
C:\Program Files\Host sFlow Project\Host sFlow Agent\hsflowd.exe 
``
Log File is used exlusively while service is running:

```cmd
copy  "%SystemDrive%\ProgramData\Host sFlow Project\Host sFlow Agent\hsflowd.log" %userprofile%\Desktop
```
```text
cannot copy
```
service

```cmd
SERVICE_NAME: hsflowd
DISPLAY_NAME: Host sFlow Agent
        TYPE               : 10  WIN32_OWN_PROCESS
        STATE              : 4  RUNNING
                                (STOPPABLE, NOT_PAUSABLE, IGNORES_SHUTDOWN)
        WIN32_EXIT_CODE    : 0  (0x0)
        SERVICE_EXIT_CODE  : 0  (0x0)
        CHECKPOINT         : 0x0
        WAIT_HINT          : 0x0
```

Win32_PerfFormattedData_PerfOS_System ProcessorQueueLength
https://docs.microsoft.com/en-us/previous-versions/aa394272(v=vs.85)

https://serverfault.com/questions/54753/common-wql-monitoring-queries
```cmd
wmic.exe path Win32_PerfFormattedData_PerfOS_System get ProcessorQueueLength
```

http://www.java2s.com/Code/CSharp/Development-Class/ComputerdetailsretrievedusingWindowsManagementInstrumentationWMI.htm
