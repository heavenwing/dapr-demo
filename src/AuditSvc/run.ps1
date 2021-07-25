dapr run `
    --app-id daprdemo-auditsvc `
    --app-port 5002 `
    --log-level debug `
    --components-path ..\..\daprconfig\local `
    -- dotnet run --urls http://0.0.0.0:5002