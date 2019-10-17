mkdir temp_binaries

copy /y attr\bin\Release\attr.exe temp_binaries
copy /y balloon\bin\Release\balloon.exe temp_binaries
copy /y clippy\bin\Release\clippy.exe temp_binaries
copy /y environment\bin\Release\environment.exe temp_binaries
copy /y ffm\bin\Release\ffm.exe temp_binaries
copy /y getpid\bin\Release\getpid.exe temp_binaries
copy /y injectkey\bin\Release\injectkey.exe temp_binaries
copy /y pathadd\bin\Release\pathadd.exe temp_binaries
copy /y pathdel\bin\Release\pathdel.exe temp_binaries
copy /y regfind\bin\Release\regfind.exe temp_binaries
copy /y regprint\bin\Release\regprint.exe temp_binaries
copy /y startupadd\bin\Release\startupadd.exe temp_binaries
copy /y startupdel\bin\Release\startupdel.exe temp_binaries
copy /y unzip\bin\Release\unzip.exe temp_binaries
copy /y zip\bin\Release\zip.exe temp_binaries
copy /y balloon\bin\Debug\Newtonsoft.Json.dll temp_binaries
copy /y balloon\bin\Debug\YellowLab.Windows.dll temp_binaries
copy /y clippy\bin\Debug\Mono.Options.dll temp_binaries
copy /y mmove\bin\Debug\policy.2.0.taglib-sharp.dll temp_binaries
copy /y mmove\bin\Debug\taglib-sharp.dll temp_binaries

powershell.exe -nologo -noprofile -command "& { Add-Type -A 'System.IO.Compression.FileSystem'; [IO.Compression.ZipFile]::CreateFromDirectory('temp_binaries', 'windowscli.binaries.zip'); }"

rd /s /q temp_binaries
