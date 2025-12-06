@echo off  
REM origin: https://www.cyberforum.ru/cmd-bat/thread3000311.html
chcp 1251 
title Автоматическое обновление Windows
>nul 2>&1 dism.exe|| (
 echo/
 echo  Ошибка! Требуются права Администратора.
 echo/
 pause
 exit
)
if defined PROCESSOR_ARCHITEW6432 (
 start %windir%\Sysnative\cmd.exe /c %0
 exit
)
cls
echo/
set TrustedInstaller=*S-1-5-80-956008885-3418522649-1831038044-1853292631-2271478464
set var=%windir%\System32\wuaueng.dll
icacls.exe %var%| >nul find "BUILTIN"|| goto next
call :status РАЗРЕШЕНО 0a
call :msg "Запретить автоматическое обновления Windows?"&& (
 cls
 echo/
 echo Запрещаю автоматическое обновление...
 echo --------------------------------------------------------------
 2>nul net stop wuauserv
 takeown.exe /f %var% /a
 icacls.exe %var% /reset
 icacls.exe %var% /inheritance:r
 echo --------------------------------------------------------------
 echo Готово.
 call :status ЗАПРЕЩЕНО 0c
 pause
)
exit
:next
set donor=%windir%\System32\ntdll.dll
for %%i in (%var%) do set fname=%%~nxi
call :status ЗАПРЕЩЕНО 0c
call :msg "Разрешить автоматическое обновление Windows?"&& (
 cls
 echo/
 echo Разрешаю автоматическое обновление...
 echo --------------------------------------------------------------
 echo f| >nul xcopy /oxy %do:wnor% %tmp%\%fname%
 pushd %tmp%
 >nul icacls.exe %fname% /save %fname%.acl
 popd
 icacls.exe %var% /reset
 icacls.exe %var% /setowner %TrustedInstaller%
 icacls.exe %windir%\System32 /restore %tmp%\%fname%.acl
 del /q %tmp%\%fname%*
 echo --------------------------------------------------------------
 echo Готово.
 call :status РАЗРЕШЕНО 0a
 pause
)
exit
:status
color F0
echo/
echo --------------------------------------------------------------
echo        Статус автоматического обновления: * %1 *
echo --------------------------------------------------------------
echo/
exit /b
:msg
set ask=& set /p "ask=%~1 [1=Да/0=Нет]:"
if "%ask%" neq "1" exit /b 1
