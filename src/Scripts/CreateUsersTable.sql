-- Users 資料表建表腳本
-- 執行時機：由 DatabaseService.Initialize() 自動執行
-- 若需手動重建，可刪除 app.db 後重新啟動應用程式

CREATE TABLE IF NOT EXISTS Users (
    Id TEXT PRIMARY KEY,
    Username TEXT NOT NULL UNIQUE,
    Password TEXT NOT NULL
);

-- 測試帳號（由 DatabaseService 在表為空時自動插入）
-- INSERT INTO Users (Id, Username, Password) VALUES ('0001', 'aimarliu', 'aimarliu');
