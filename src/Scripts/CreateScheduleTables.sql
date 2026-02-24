-- 生產排程與完成訂單資料表
-- 執行時機：由 DatabaseService.Initialize() 或專用初始化邏輯執行

-- 生產排程（上區 DataGrid）
CREATE TABLE IF NOT EXISTS ScheduleOrders (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    OrderNo TEXT NOT NULL,
    VersionNo TEXT NOT NULL,
    OrderQuantity INTEGER NOT NULL,
    BoxType TEXT NOT NULL,
    Category TEXT NOT NULL,
    CustomerName TEXT NOT NULL,
    Remarks TEXT NOT NULL,
    SortOrder INTEGER NOT NULL DEFAULT 0
);

-- 生產完成訂單（下區 DataGrid）
CREATE TABLE IF NOT EXISTS CompletedOrders (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    CompletedAt TEXT NOT NULL,
    OrderNo TEXT NOT NULL,
    VersionNo TEXT NOT NULL,
    OrderQuantity INTEGER NOT NULL,
    BoxType TEXT NOT NULL,
    Category TEXT NOT NULL,
    CustomerName TEXT NOT NULL,
    Remarks TEXT NOT NULL
);

-- 索引
CREATE INDEX IF NOT EXISTS IX_ScheduleOrders_SortOrder ON ScheduleOrders(SortOrder);
CREATE INDEX IF NOT EXISTS IX_CompletedOrders_CompletedAt ON CompletedOrders(CompletedAt);
