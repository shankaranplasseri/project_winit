#!/usr/bin/env python3
"""
Database Setup Script for Journey Plan Management System
Creates SQLite database with schema and test data
"""

import sqlite3
import os
import sys
from pathlib import Path

def create_database(db_path):
    """Create SQLite database with complete schema and test data"""
    
    # Remove old database if exists
    if os.path.exists(db_path):
        os.remove(db_path)
        print(f"✓ Removed old database: {db_path}")
    
    # Connect to database
    conn = sqlite3.connect(db_path)
    cursor = conn.cursor()
    
    print("Creating database schema...")
    
    # Create Users table
    cursor.execute("""
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
    )
    """)
    cursor.execute("CREATE INDEX idx_Users_Username ON Users(Username)")
    cursor.execute("CREATE INDEX idx_Users_Role ON Users(Role)")
    print("  ✓ Users table created")
    
    # Create Customers table
    cursor.execute("""
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
    )
    """)
    cursor.execute("CREATE INDEX idx_Customers_CustomerCode ON Customers(CustomerCode)")
    cursor.execute("CREATE INDEX idx_Customers_Status ON Customers(Status)")
    print("  ✓ Customers table created")
    
    # Create JourneyPlans table
    cursor.execute("""
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
    )
    """)
    cursor.execute("CREATE INDEX idx_JourneyPlans_AssignedUserId ON JourneyPlans(AssignedUserId)")
    cursor.execute("CREATE INDEX idx_JourneyPlans_PlanDate ON JourneyPlans(PlanDate)")
    cursor.execute("CREATE INDEX idx_JourneyPlans_Status ON JourneyPlans(Status)")
    print("  ✓ JourneyPlans table created")
    
    # Create JourneyPlanItems table
    cursor.execute("""
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
    )
    """)
    cursor.execute("CREATE INDEX idx_JourneyPlanItems_JourneyPlanId ON JourneyPlanItems(JourneyPlanId)")
    cursor.execute("CREATE INDEX idx_JourneyPlanItems_CustomerId ON JourneyPlanItems(CustomerId)")
    print("  ✓ JourneyPlanItems table created")
    
    # Create VisitLogs table
    cursor.execute("""
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
    )
    """)
    cursor.execute("CREATE INDEX idx_VisitLogs_JourneyPlanItemId ON VisitLogs(JourneyPlanItemId)")
    cursor.execute("CREATE INDEX idx_VisitLogs_VisitDate ON VisitLogs(VisitDate)")
    cursor.execute("CREATE INDEX idx_VisitLogs_Status ON VisitLogs(Status)")
    print("  ✓ VisitLogs table created")
    
    print("\nInserting test data...")
    
    # Insert Admin User
    cursor.execute("""
    INSERT INTO Users (Username, Email, PasswordHash, FirstName, LastName, Role, IsActive)
    VALUES (?, ?, ?, ?, ?, ?, ?)
    """, ('admin_test', 'admin@test.com', 
          '$2a$11$JXjKdJdlJdsKJlKdJdlJdJdlJdlKdJdlJdlKdlJdKdlJ', 
          'Admin', 'Test', 'Admin', 1))
    
    # Insert Van Sales User
    cursor.execute("""
    INSERT INTO Users (Username, Email, PasswordHash, FirstName, LastName, Role, IsActive)
    VALUES (?, ?, ?, ?, ?, ?, ?)
    """, ('vansales_test01', 'vansales@test.com',
          '$2a$11$JXjKdJdlJdsKJlKdJdlJdJdlJdlKdJdlJdlKdlJdKdlJ',
          'John', 'Smith', 'VanSalesUser', 1))
    print("  ✓ Users inserted (admin_test, vansales_test01)")
    
    # Insert Sample Customers
    customers = [
        ('CUST001', 'ABC Trading Company', '123 Market Street', 'Jakarta', '12345', '+62-21-1234567', 'Route-A'),
        ('CUST002', 'XYZ Retail Store', '456 Business Park', 'Jakarta', '12346', '+62-21-2345678', 'Route-A'),
        ('CUST003', 'Premium Supermarket', '789 Shopping Mall', 'Jakarta', '12347', '+62-21-3345789', 'Route-A'),
        ('CUST004', 'Local Convenience Store', '321 Neighborhood Center', 'Jakarta', '12348', '+62-21-4456890', 'Route-B'),
        ('CUST005', 'Wholesale Distributor', '654 Industrial Zone', 'Jakarta', '12349', '+62-21-5567901', 'Route-B'),
        ('CUST006', 'Restaurant Chain', '987 Food Court', 'Jakarta', '12350', '+62-21-6678012', 'Route-B'),
        ('CUST007', 'Hotel & Resort', '111 Luxury Avenue', 'Jakarta', '12351', '+62-21-7789123', 'Route-C'),
        ('CUST008', 'Shopping Complex', '222 Commercial Street', 'Jakarta', '12352', '+62-21-8890234', 'Route-C'),
        ('CUST009', 'Pharmacy Chain', '333 Healthcare Plaza', 'Jakarta', '12353', '+62-21-9901345', 'Route-C'),
        ('CUST010', 'Gas Station', '444 Highway Road', 'Jakarta', '12354', '+62-21-0012456', 'Route-D'),
    ]
    
    for code, name, address, city, postal, contact, route in customers:
        cursor.execute("""
        INSERT INTO Customers (CustomerCode, CustomerName, Address, City, PostalCode, ContactNumber, Route, Status)
        VALUES (?, ?, ?, ?, ?, ?, ?, ?)
        """, (code, name, address, city, postal, contact, route, 'Active'))
    print(f"  ✓ Customers inserted ({len(customers)} customers)")
    
    # Insert sample journey plan for today
    cursor.execute("""
    INSERT INTO JourneyPlans (AssignedUserId, PlanDate, Title, Description, Status, CreatedUserId)
    VALUES (?, DATE('now'), ?, ?, ?, ?)
    """, (2, 'Daily Journey Plan - Today', 'Sample journey plan for testing with 5 customers', 'Active', 1))
    
    # Insert journey plan items
    journey_plan_items = [
        (1, 1, 1, '09:00'),
        (1, 2, 2, '10:00'),
        (1, 3, 3, '11:00'),
        (1, 4, 4, '12:00'),
        (1, 5, 5, '13:00'),
    ]
    
    for jp_id, cust_id, seq, time in journey_plan_items:
        cursor.execute("""
        INSERT INTO JourneyPlanItems (JourneyPlanId, CustomerId, SequenceNumber, PlannedVisitTime)
        VALUES (?, ?, ?, ?)
        """, (jp_id, cust_id, seq, time))
    print("  ✓ Journey Plan for today inserted (5 customers)")
    
    # Insert future journey plan (TS-007 test)
    cursor.execute("""
    INSERT INTO JourneyPlans (AssignedUserId, PlanDate, Title, Description, Status, CreatedUserId)
    VALUES (?, DATE('now', '+1 day'), ?, ?, ?, ?)
    """, (2, 'Daily Journey Plan - Tomorrow', 'Journey plan for tomorrow - should not appear in today view', 'Draft', 1))
    
    future_jp_items = [
        (2, 6, 1, '09:00'),
        (2, 7, 2, '10:00'),
        (2, 8, 3, '11:00'),
    ]
    
    for jp_id, cust_id, seq, time in future_jp_items:
        cursor.execute("""
        INSERT INTO JourneyPlanItems (JourneyPlanId, CustomerId, SequenceNumber, PlannedVisitTime)
        VALUES (?, ?, ?, ?)
        """, (jp_id, cust_id, seq, time))
    print("  ✓ Journey Plan for tomorrow inserted (3 customers)")
    
    # Insert empty journey plan (TS-006 test)
    cursor.execute("""
    INSERT INTO JourneyPlans (AssignedUserId, PlanDate, Title, Description, Status, CreatedUserId)
    VALUES (?, DATE('now'), ?, ?, ?, ?)
    """, (2, 'Empty Journey Plan', 'Journey plan with no customers assigned (empty state)', 'Active', 1))
    print("  ✓ Empty Journey Plan inserted (for testing empty state)")
    
    # Commit all changes
    conn.commit()
    print("\n✓ All data committed to database")
    
    # Verify
    print("\nVerifying database content...")
    
    cursor.execute("SELECT COUNT(*) FROM Users")
    user_count = cursor.fetchone()[0]
    print(f"  ✓ Users: {user_count}")
    
    cursor.execute("SELECT COUNT(*) FROM Customers")
    cust_count = cursor.fetchone()[0]
    print(f"  ✓ Customers: {cust_count}")
    
    cursor.execute("SELECT COUNT(*) FROM JourneyPlans")
    jp_count = cursor.fetchone()[0]
    print(f"  ✓ Journey Plans: {jp_count}")
    
    cursor.execute("SELECT COUNT(*) FROM JourneyPlanItems")
    jpi_count = cursor.fetchone()[0]
    print(f"  ✓ Journey Plan Items: {jpi_count}")
    
    # Show test users
    print("\nTest User Credentials:")
    cursor.execute("SELECT UserId, Username, Role FROM Users")
    for row in cursor.fetchall():
        print(f"  - UserId: {row[0]}, Username: {row[1]}, Role: {row[2]}")
    
    conn.close()
    
    print(f"\n✅ Database setup complete!")
    print(f"📁 Database location: {os.path.abspath(db_path)}")
    print(f"📊 Database size: {os.path.getsize(db_path) / 1024:.2f} KB")
    
    return True

if __name__ == "__main__":
    db_path = "VanSalesJourneyPlan.db"
    
    try:
        create_database(db_path)
        sys.exit(0)
    except Exception as e:
        print(f"\n❌ Error: {e}", file=sys.stderr)
        import traceback
        traceback.print_exc()
        sys.exit(1)
