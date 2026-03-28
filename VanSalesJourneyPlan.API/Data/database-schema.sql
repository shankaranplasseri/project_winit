-- Journey Plan Management System - SQLite Database Schema
-- Version: 1.0
-- Created: 2026-03-28

-- Drop existing tables if they exist (for clean initialization)
DROP TABLE IF EXISTS VisitLogs;
DROP TABLE IF EXISTS JourneyPlanItems;
DROP TABLE IF EXISTS JourneyPlans;
DROP TABLE IF EXISTS Customers;
DROP TABLE IF EXISTS Users;

-- ========================================
-- USERS TABLE
-- ========================================
CREATE TABLE Users (
    UserId INTEGER PRIMARY KEY AUTOINCREMENT,
    Username TEXT NOT NULL UNIQUE,
    Email TEXT NOT NULL UNIQUE,
    PasswordHash TEXT NOT NULL,
    FirstName TEXT,
    LastName TEXT,
    PhoneNumber TEXT,
    Role TEXT NOT NULL CHECK (Role IN ('Admin', 'VanSalesUser')),
    IsActive BOOLEAN NOT NULL DEFAULT 1,
    CreatedDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_Users_Username ON Users(Username);
CREATE INDEX idx_Users_Role ON Users(Role);

-- ========================================
-- CUSTOMERS TABLE
-- ========================================
CREATE TABLE Customers (
    CustomerId INTEGER PRIMARY KEY AUTOINCREMENT,
    CustomerCode TEXT NOT NULL UNIQUE,
    CustomerName TEXT NOT NULL,
    Address TEXT,
    City TEXT,
    PostalCode TEXT,
    ContactNumber TEXT,
    Email TEXT,
    LocationLatitude REAL,
    LocationLongitude REAL,
    Route TEXT,
    Status TEXT NOT NULL CHECK (Status IN ('Active', 'Inactive', 'Prospect')) DEFAULT 'Active',
    CreatedDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_Customers_CustomerCode ON Customers(CustomerCode);
CREATE INDEX idx_Customers_Status ON Customers(Status);

-- ========================================
-- JOURNEY PLANS TABLE
-- ========================================
CREATE TABLE JourneyPlans (
    JourneyPlanId INTEGER PRIMARY KEY AUTOINCREMENT,
    AssignedUserId INTEGER NOT NULL,
    PlanDate DATE NOT NULL,
    Title TEXT NOT NULL,
    Description TEXT,
    Status TEXT NOT NULL CHECK (Status IN ('Draft', 'Active', 'Completed', 'Cancelled')) DEFAULT 'Draft',
    CreatedUserId INTEGER NOT NULL,
    CreatedDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (AssignedUserId) REFERENCES Users(UserId) ON DELETE RESTRICT,
    FOREIGN KEY (CreatedUserId) REFERENCES Users(UserId) ON DELETE RESTRICT
);

CREATE INDEX idx_JourneyPlans_AssignedUserId ON JourneyPlans(AssignedUserId);
CREATE INDEX idx_JourneyPlans_PlanDate ON JourneyPlans(PlanDate);
CREATE INDEX idx_JourneyPlans_Status ON JourneyPlans(Status);

-- ========================================
-- JOURNEY PLAN ITEMS TABLE
-- ========================================
CREATE TABLE JourneyPlanItems (
    JourneyPlanItemId INTEGER PRIMARY KEY AUTOINCREMENT,
    JourneyPlanId INTEGER NOT NULL,
    CustomerId INTEGER NOT NULL,
    SequenceNumber INTEGER NOT NULL,
    Notes TEXT,
    PlannedVisitTime TIME,
    IsCompleted BOOLEAN NOT NULL DEFAULT 0,
    CreatedDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (JourneyPlanId) REFERENCES JourneyPlans(JourneyPlanId) ON DELETE CASCADE,
    FOREIGN KEY (CustomerId) REFERENCES Customers(CustomerId) ON DELETE RESTRICT,
    UNIQUE(JourneyPlanId, CustomerId)
);

CREATE INDEX idx_JourneyPlanItems_JourneyPlanId ON JourneyPlanItems(JourneyPlanId);
CREATE INDEX idx_JourneyPlanItems_CustomerId ON JourneyPlanItems(CustomerId);

-- ========================================
-- VISIT LOGS TABLE
-- ========================================
CREATE TABLE VisitLogs (
    VisitLogId INTEGER PRIMARY KEY AUTOINCREMENT,
    JourneyPlanItemId INTEGER NOT NULL,
    VisitDate DATE NOT NULL,
    VisitTime TIME,
    Notes TEXT,
    SalesAmount DECIMAL(10, 2),
    Status TEXT NOT NULL CHECK (Status IN ('Pending', 'Completed', 'Cancelled')) DEFAULT 'Pending',
    CreatedDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    UpdatedDate DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (JourneyPlanItemId) REFERENCES JourneyPlanItems(JourneyPlanItemId) ON DELETE CASCADE
);

CREATE INDEX idx_VisitLogs_JourneyPlanItemId ON VisitLogs(JourneyPlanItemId);
CREATE INDEX idx_VisitLogs_VisitDate ON VisitLogs(VisitDate);
CREATE INDEX idx_VisitLogs_Status ON VisitLogs(Status);

-- ========================================
-- SAMPLE TEST DATA
-- ========================================

-- Insert Admin User
INSERT INTO Users (Username, Email, PasswordHash, FirstName, LastName, Role, IsActive)
VALUES (
    'admin_test',
    'admin@test.com',
    -- Password: TestAdmin123! (will be hashed in application)
    '$2a$11$JXjKdJdlJdsKJlKdJdlJdJdlJdlKdJdlJdlKdlJdKdlJ',
    'Admin',
    'Test',
    'Admin',
    1
);

-- Insert Van Sales User
INSERT INTO Users (Username, Email, PasswordHash, FirstName, LastName, Role, IsActive)
VALUES (
    'vansales_test01',
    'vansales@test.com',
    -- Password: TestVanSales123! (will be hashed in application)
    '$2a$11$JXjKdJdlJdsKJlKdJdlJdJdlJdlKdJdlJdlKdlJdKdlJ',
    'John',
    'Smith',
    'VanSalesUser',
    1
);

-- Insert Sample Customers
INSERT INTO Customers (CustomerCode, CustomerName, Address, City, PostalCode, ContactNumber, Route, Status)
VALUES
('CUST001', 'ABC Trading Company', '123 Market Street', 'Jakarta', '12345', '+62-21-1234567', 'Route-A', 'Active'),
('CUST002', 'XYZ Retail Store', '456 Business Park', 'Jakarta', '12346', '+62-21-2345678', 'Route-A', 'Active'),
('CUST003', 'Premium Supermarket', '789 Shopping Mall', 'Jakarta', '12347', '+62-21-3345789', 'Route-A', 'Active'),
('CUST004', 'Local Convenience Store', '321 Neighborhood Center', 'Jakarta', '12348', '+62-21-4456890', 'Route-B', 'Active'),
('CUST005', 'Wholesale Distributor', '654 Industrial Zone', 'Jakarta', '12349', '+62-21-5567901', 'Route-B', 'Active'),
('CUST006', 'Restaurant Chain', '987 Food Court', 'Jakarta', '12350', '+62-21-6678012', 'Route-B', 'Active'),
('CUST007', 'Hotel & Resort', '111 Luxury Avenue', 'Jakarta', '12351', '+62-21-7789123', 'Route-C', 'Active'),
('CUST008', 'Shopping Complex', '222 Commercial Street', 'Jakarta', '12352', '+62-21-8890234', 'Route-C', 'Active'),
('CUST009', 'Pharmacy Chain', '333 Healthcare Plaza', 'Jakarta', '12353', '+62-21-9901345', 'Route-C', 'Active'),
('CUST010', 'Gas Station', '444 Highway Road', 'Jakarta', '12354', '+62-21-0012456', 'Route-D', 'Active');

-- Create a sample journey plan for today (to test TS-001)
INSERT INTO JourneyPlans (AssignedUserId, PlanDate, Title, Description, Status, CreatedUserId)
VALUES (
    2, -- vansales_test01 user
    DATE('now'),
    'Daily Journey Plan - Today',
    'Sample journey plan for testing with 5 customers',
    'Active',
    1 -- created by admin
);

-- Add customers to the journey plan (test TS-002, TS-003, TS-004)
INSERT INTO JourneyPlanItems (JourneyPlanId, CustomerId, SequenceNumber, PlannedVisitTime)
VALUES
(1, 1, 1, '09:00'),
(1, 2, 2, '10:00'),
(1, 3, 3, '11:00'),
(1, 4, 4, '12:00'),
(1, 5, 5, '13:00');

-- Create a future journey plan (test TS-007 - should not appear today)
INSERT INTO JourneyPlans (AssignedUserId, PlanDate, Title, Description, Status, CreatedUserId)
VALUES (
    2, -- vansales_test01 user
    DATE('now', '+1 day'),
    'Daily Journey Plan - Tomorrow',
    'Journey plan for tomorrow - should not appear in today view',
    'Draft',
    1
);

-- Add customers to future journey plan
INSERT INTO JourneyPlanItems (JourneyPlanId, CustomerId, SequenceNumber, PlannedVisitTime)
VALUES
(2, 6, 1, '09:00'),
(2, 7, 2, '10:00'),
(2, 8, 3, '11:00');

-- Create an empty journey plan (test TS-006 - empty state)
INSERT INTO JourneyPlans (AssignedUserId, PlanDate, Title, Description, Status, CreatedUserId)
VALUES (
    2,
    DATE('now'),
    'Empty Journey Plan',
    'Journey plan with no customers assigned (empty state)',
    'Active',
    1
);
