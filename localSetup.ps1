param(
    [string]$CertPass = "crypticpassword"
)

dotnet dev-certs https -ep $env:APPDATA\asp.net\https\Regis.Pay.Api.pfx -p $CertPass
dotnet dev-certs https --trust