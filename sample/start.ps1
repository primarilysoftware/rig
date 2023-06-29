$env:HOSTIP = Get-NetIPAddress -AddressFamily IPv4 | 
    Where-Object { $_.AddressState -Eq "Preferred" } | 
    Where-Object { $_.InterfaceAlias -notlike "Loopback*" } | 
    Where-Object { $_.InterfaceAlias -notlike "vEthernet*" } | 
    Select-Object -First 1 | Select -ExpandProperty IPAddress

docker compose up -d