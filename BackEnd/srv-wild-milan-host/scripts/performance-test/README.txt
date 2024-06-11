Setup
-----
Download and extract JMeter
 - https://jmeter.apache.org/download_jmeter.cgi

Install the JMeter Plugins Manager
 - https://jmeter-plugins.org/wiki/PluginsManager/    

Install the Parallel Controller & Sampler plugin:
 - https://github.com/Blazemeter/jmeter-bzm-plugins/blob/master/parallel/Parallel.md

Create the following environment variables:
 - JMETER_PHOENIX_HOSTNAME e.g. api.phoenix.dev.madness.games
 - JMETER_OAUTH_KEY

GUI
---
jmeter -t WildCatPerformance.jmx

Command Line Execution
----------------------
jmeter -n -t WildCatPerformance.jmx -l <path-to-output-log>

Note: the log generated can be loaded back into 'View Aggregate Report'

Debugging
---------
If you run into any errors do the following:

    1) Open up the test plan in the JMeter GUI. 
    2) Enable 'Debug Sampler'
    3) Enable 'View Results Tree'
    4) Execute test
    5) In the 'View Results Tree' check the output from 'Debug Sampler' and any failing steps.
