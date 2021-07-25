dapr run `
    --app-id daprdemo-auditsvc `
    --app-protocol grpc `
    --app-port 50001 `
    --log-level debug `
    --components-path ..\..\daprconfig\local `
    AuditSvc.exe