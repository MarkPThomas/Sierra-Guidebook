nuget install NUnit.Runners -Version 2.6.4 -OutputDirectory ./DB-Location-XReferencer/tools
nuget install OpenCover -Version 4.6.519 -OutputDirectory ./DB-Location-XReferencer/tools

"C:\Program Files (x86)\MSBuild\14.0\Bin\MsBuild.exe" DB-Location-XReferencer.sln /t:Rebuild
.\DB-Location-XReferencer\tools\OpenCover.4.6.519\tools\OpenCover.Console.exe -target:".\DB-Location-XReferencer\tools\NUnit.Runners.2.6.4\tools\nunit-console.exe" -targetargs:"/nologo /noshadow %CD%\DB-Location-XReferences.Tests\bin\Debug\DB_Location_XReferences.Tests.dll" -filter:"+[*]* -[*.Tests]*" -output:"%CD%\opencover.xml" -register:user