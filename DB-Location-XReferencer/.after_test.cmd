nuget install NUnit.Runners -Version 2.6.4 -OutputDirectory tools
nuget install OpenCover -Version 4.6.519 -OutputDirectory tools
nuget install coveralls.net -Version 0.412.0 -OutputDirectory tools
  
.toolsOpenCover.4.6.519toolsOpenCover.Console.exe -target.toolsNUnit.Runners.2.6.4toolsnunit-console.exe -targetargsnologo noshadow .DB-Location-XReferences.TestsbinDebugDB_Location_XReferences.Tests.dll -filter+[] -[.Tests] -registeruser
 
.toolscoveralls.net.0.412toolscsmacnz.Coveralls.exe --opencover -i .results.xml