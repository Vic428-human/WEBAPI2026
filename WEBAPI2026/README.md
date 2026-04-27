```mermaid
flowchart TD
    A[Client / Swagger<br/>POST /api/so] --> B[SalesOrderController.cs<br/>Controllers/SalesOrderController.cs]

    B --> C[DateRangeRequest.cs<br/>Models/Requests/DateRangeRequest.cs<br/>接收 request body]

    B --> D{驗證 request<br/>dateTimestampGTE 是否有值?}

    D -- No --> E[ApiResponse.cs<br/>Models/Responses/ApiResponse.cs<br/>回傳錯誤格式]
    E --> F[HTTP 400 BadRequest<br/>Message / Status / Data]

    D -- Yes --> G[建立假資料<br/>SalesOrderDto.cs<br/>Models/Dtos/SalesOrderDto.cs]

    G --> H[ApiResponse.cs<br/>Models/Responses/ApiResponse.cs<br/>包裝成統一 response]

    H --> I[回傳 HTTP 200 OK<br/>Message / Status / Data]

    J[Startup.cs<br/>Startup.cs] --> B
    J --> H
    J -. 設定 JSON 欄位大小寫保留 .-> H
```