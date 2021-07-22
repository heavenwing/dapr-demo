dapr run --app-id sub `
         --app-protocol http `
         --app-port 8080 `
         --dapr-http-port 3500 `
         --log-level debug `
         --components-path ..\..\daprconfig\local `
         go run .\main.go