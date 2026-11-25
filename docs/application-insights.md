# Application Insights

App insights is the platform we are using for measuring telemetry. By default app insights is on in dev and test environments and off when running locally or building in a pipeline.

App insights can be enabled locally by including the conncection string in your secrets file. The format should be `"APPLICATIONINSIGHTS_CONNECTION_STRING": "<Connection String here>"`
You can copy the connection string from Azure. Go the the app insights instance (Current instance name is `s184d01-fiat-insights`) and on the overview panel you can copy the connection string. Always use the development environment when testing locally.

## Tracking events

Both Academies API HTTP client factories set `x-correlationId` to the current .NET `Activity` trace ID. If no activity is active, they generate a GUID. Incoming `x-correlationId` headers are not propagated; correlation uses .NET tracing rather than CoreLibs HTTP middleware.

We are tracking page views and requests automatically. If you would like to track your own events then us the TrackEvent method.
You can track events in the client using javascript or on the server using the app insights module.

More info about app insights can be found [on learn.microsoft.com](https://learn.microsoft.com/en-us/azure/azure-monitor/app/app-insights-overview)
