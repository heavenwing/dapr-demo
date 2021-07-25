dapr run `
    --app-id daprdemo-portal `
    --app-port 5000 `
    --log-level debug `
    --components-path ..\..\daprconfig\local `
    dotnet run