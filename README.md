# SampleDataCollector

This is more or less a simple test harness for me to run simple benchmarks on to get an idea of process/system usage of a .NET Core application running inside of a linux container.

It is also meant to show a simple data collection process that I've found help in periodically reporting metrics of the application.

Yes, I know the rebuild is a bummer right now, I need to make a template to base my projects off of

| Commands       | Description                                           |
| -------------- | ----------------------------------------------------- |
| `make run`     | Starts the application as well as a metrics dashboard |
| `make stop`    | Tears down the local setup                            |
| `make rebuild` | Rebuilds the image so you can run latest code         |

### Usage

 App is running on `8000` and the metrics dash can be seen on `localhost:80`. The default login for these boards is user:pass `admin:admin`

### Notes

I've been trying to run benchmarks with varying configurations (environment variables, different systems, different versions, etc.) so I'll post the run results in this section

### Tests

All tests are being run with [bombardier](https://github.com/codesenberg/bombardier) sending load to the single endpoint I have. The endpoint is mostly just creating a new dumb thing that simulates an IO operation and creates a large object.
