@echo off
setlocal
setlocal enabledelayedexpansion
@echo off
for /r %%i in (plugins) do (
  @if exist "%%i" (
    @set _variable=%%i
	@RD /S /Q %%i
    @echo !_variable!
    )
  )

for /r %%i in (bin) do (
  @if exist "%%i" (
    @set _variable=%%i
	@RD /S /Q %%i
    @echo !_variable!
    )
  )
  
for /r %%i in (obj) do (
  @if exist "%%i" (
    @set _variable=%%i
	@RD /S /Q %%i
    @echo !_variable!
    )
  )
  
DEL /S /Q storage.json
endlocal