@echo off
echo Starting ArcaneVault API and Web Projects...

echo [1/2] Starting ArcaneVaultAPI...
start cmd /k "cd ArcaneVaultAPI && title ArcaneVault API && dotnet run"

echo [2/2] Starting ArcaneVault.Web...
start cmd /k "cd ArcaneVault.Web && title ArcaneVault Web && dotnet run"

echo Applications are starting in new windows.
echo API will be on http://localhost:5001
echo Web will be on http://localhost:5000
