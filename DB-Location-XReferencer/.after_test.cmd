nuget install NUnit.Runners -Version 2.6.4 -OutputDirectory DB-Location-XReferencer/tools
nuget install OpenCover -Version 4.6.519 -OutputDirectory DB-Location-XReferencer/tools
nuget install coveralls.net -Version 0.412.0 -OutputDirectory DB-Location-XReferencer/tools
  
.\DB-Location-XReferencer\tools\OpenCover.4.6.519\tools\OpenCover.Console.exe -target:".\DB-Location-XReferencer\tools\NUnit.Runners.2.6.4\tools\nunit-console.exe" -targetargs:"/nologo /noshadow .\DB-Location-XReferencer\DB-Location-XReferences.Tests\bin\Debug\DB_Location_XReferences.Tests.dll" -filter:"+[*]* -[*.Tests]*" -register:user
 
.\DB-Location-XReferencer\tools\coveralls.net.0.412\tools\csmacnz.Coveralls.exe --opencover -i .results.xml