-- Phase 6 訂單製作：Orders 表
-- 執行時機：由 DatabaseService.Initialize() 呼叫 EnsureOrdersTable

CREATE TABLE IF NOT EXISTS Orders (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    CreatedAt TEXT NOT NULL,
    OrderNo TEXT NOT NULL,
    VersionNo TEXT NOT NULL,
    OrderQuantity INTEGER NOT NULL,
    BoxType TEXT NOT NULL,
    Category TEXT NOT NULL,
    Phase1 TEXT NOT NULL,
    Phase2 TEXT NOT NULL,
    Phase3 TEXT NOT NULL,
    Length INTEGER NOT NULL,
    Width INTEGER NOT NULL,
    CustomerName TEXT NOT NULL,
    Remarks TEXT NOT NULL
);

CREATE INDEX IF NOT EXISTS IX_Orders_VersionNo ON Orders(VersionNo);
CREATE INDEX IF NOT EXISTS IX_Orders_CreatedAt ON Orders(CreatedAt);
