$fsiExitCode = 0
Push-Location
try {
    cd $PSScriptRoot
    $FSI = [IO.Path]::Combine($PSScriptRoot, ".." , "..", "packages", "docs","FSharp.Compiler.Tools","tools","fsi.exe")

    &$FSI "--exec" "generate.fsx" $args
    $fsiExitCode = $LastExitCode
}
finally {
    Pop-Location
}
exit($fsiExitCode)