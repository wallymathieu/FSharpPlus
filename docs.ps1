
Push-Location
try {
    $loc = [IO.Path]::Combine($PSScriptRoot,  ".\docsTool\bin\Debug\netcoreapp2.2")
Set-Location $loc 
dotnet docsTool.dll build --projectglob "../../../../src/**/*.??proj" --sitebaseurl http://fsprojects.github.io/FSharpPlus/ --docsoutputdirectory ..\..\..\..\docs\ --docssourcedirectory ..\..\..\..\docsrc-api --githubrepourl https://github.com/fsprojects/FSharpPlus/ --projectname FSharpPlus --releaseversion 1.1.0
}
finally {
    Pop-Location
}