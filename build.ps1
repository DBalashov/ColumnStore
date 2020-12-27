$CD = $PSScriptRoot

cd $CD\ColumnStore

& "dotnet" @("build", "--configuration", "Release")

cd $CD

nuget pack ColumnStore.nuspec