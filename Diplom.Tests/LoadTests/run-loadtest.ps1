# run-loadtest.ps1
echo
$jmx = "SignalR_Load.jmx"
$out = "results.jtl"
$report = "report"
echo
"D:\apache-jmeter-5.6.3\bin\jmeter.bat" -n -t $jmx -l $out -q jmeter.properties `
       -e -o $report

Write-Host "HTML-звіт ➜ $report/index.html"
echo