@echo OFF
REM Cannot load Counter Name data because an invalid index '♀ßÇü♦♂ ' was read from the registry.
REM origin: https://www.urtech.ca/2015/09/solved-cannot-load-counter-name-data-because-an-invalid-index-was-read-from-the-registry/
REM see also:
REM https://social.technet.microsoft.com/Forums/azure/en-US/9b01e1a6-d872-4f28-9280-f35d6ca02a9f/lodctr-r-error-code-2?forum=w7itprogeneral
C:\Windows\System32\lodctr.exe /r
REM Updates registry values related to performance counters.
REM the outcome varies. The success is logged as:
REM Info: Successfully rebuilt performance counter setting from system backup store

C:\Windows\System32\lodctr.exe /Q:System
REM NOTE specifying C:\Windows\System32\lodctr.exe /Q:PROCESSOR
REM produces similar output and probably is not the right way of calling it
