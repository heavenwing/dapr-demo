dapr run `
    --app-id daprdemo-helloapi `
    --app-port 5001 `
    --log-level debug `
    --components-path ..\..\daprconfig\local `
    -- dotnet run --urls http://0.0.0.0:5001