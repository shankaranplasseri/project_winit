# How to Run the Van Sales Journey Plan Project

## 📋 Prerequisites

### System Requirements
- **Windows 10/11** (API and Web can run on Linux/Mac, but mobile requires Android)
- **.NET 9.0 SDK** (Download from https://dotnet.microsoft.com/download)
- **Android SDK** (For mobile app - part of Visual Studio or installed separately)
- **Visual Studio 2022** or **Visual Studio Code** with C# extensions (recommended)

### Verify .NET Installation
```powershell
dotnet --version
# Output should show: 9.x.x
```

---

## 🚀 Quick Start (5 Minutes)

### Step 1: Start the API Backend
```powershell
# Open PowerShell/Command Prompt
cd C:\Project\VanSalesJourneyPlan.API

# Run the API server
dotnet run --configuration Release

# Expected output:
# Now listening on: http://localhost:5249
```

The API will start on **http://localhost:5249** with Swagger UI available at **http://localhost:5249/swagger**

### Step 2: Test Login in Browser or Postman
```
POST http://localhost:5249/api/auth/login
Content-Type: application/json

{
  "username": "vansales_test01",
  "password": "vansales123"
}

Response:
{
  "token": "eyJhbGciOiJIUzI1NiI...",
  "user": {
    "userId": 2,
    "username": "vansales_test01",
    "email": "vansales@test.com",
    "firstName": "John",
    "lastName": "Smith",
    "role": "VanSalesUser"
  },
  "success": true,
  "message": "Login successful"
}
```

### Step 3: Build Mobile App
```powershell
cd C:\Project\VanSalesJourneyPlan.Mobile

# Build for Android
dotnet build -f net8.0-android -c Release

# Or build in Visual Studio: Open VanSalesJourneyPlan.sln and click Build
```

### Step 4: Run Mobile App on Android Emulator
```powershell
cd C:\Project\VanSalesJourneyPlan.Mobile

# Run on Android emulator (requires emulator running first)
dotnet run -f net8.0-android
```

---

## 🔐 Test Users

| Username | Password | Role | Features |
|----------|----------|------|----------|
| `vansales_test01` | `vansales123` | VanSalesUser | View plans, log visits |
| `admin_test` | `admin123` | Admin | Full access to all endpoints |

---

## 📊 Sample API Workflow

### 1. Login and Get JWT Token
```powershell
$loginUrl = "http://localhost:5249/api/auth/login"
$loginBody = @{
    username = "vansales_test01"
    password = "vansales123"
} | ConvertTo-Json

$response = Invoke-WebRequest -Uri $loginUrl -Method POST -Body $loginBody -ContentType "application/json"
$data = $response.Content | ConvertFrom-Json

$token = $data.token
Write-Host "JWT Token: $token"
```

### 2. View Your Journey Plans
```powershell
$plansUrl = "http://localhost:5249/api/journeyplans/my"
$headers = @{ "Authorization" = "Bearer $token" }

$response = Invoke-WebRequest -Uri $plansUrl -Method GET -Headers $headers
$response.Content | ConvertFrom-Json | ConvertTo-Json
```

### 3. Get Plan Details with Customers
```powershell
$planUrl = "http://localhost:5249/api/journeyplans/1"
$headers = @{ "Authorization" = "Bearer $token" }

$response = Invoke-WebRequest -Uri $planUrl -Method GET -Headers $headers
$response.Content | ConvertFrom-Json | ConvertTo-Json
```

### 4. Log a Customer Visit
```powershell
$visitUrl = "http://localhost:5249/api/visitlogs"
$visitBody = @{
    journeyPlanItemId = 1
    visitDate = "2026-03-28"
    visitTime = "09:15"
    notes = "Great meeting with customer"
    salesAmount = 5500.00
} | ConvertTo-Json

$headers = @{ "Authorization" = "Bearer $token" }
$response = Invoke-WebRequest -Uri $visitUrl -Method POST -Body $visitBody -ContentType "application/json" -Headers $headers
$response.Content | ConvertFrom-Json | ConvertTo-Json
```

---

## 📱 Mobile App Usage

### Login Flow
1. App opens to LoginPage
2. Enter credentials: `vansales_test01` / `vansales123`
3. Click "Login"
4. On success, navigate to JourneyPlanListPage

### View Journey Plans
1. JourneyPlanListPage shows all your assigned plans
2. Plans display: Title, date, and status
3. Pull to refresh to sync with API

### View Plan Details
1. Tap a plan to open PlanDetailPage
2. See list of customers with visit times
3. Each customer shows: Name, visit time, location

### Log a Visit
1. Tap "Log Visit" button for a customer
2. VisitLogPage opens with form
3. Enter: Date, time (optional), sales amount (optional), notes (optional)
4. Tap "Log Visit" to submit
5. Success message confirms visit recorded
6. Plan item marked as completed in the API

### Offline Support
1. App caches all downloaded data locally (SQLite)
2. If API becomes unavailable, app uses cached data
3. Visits logged offline sync when connectivity restored

---

## 🛢️ Database Setup

### Database Location
```
C:\Project\VanSalesJourneyPlan.db
```

### Database Structure
```
Tables:
- Users (2 test users)
- Customers (10 test customers)
- JourneyPlans (3 test plans)
- JourneyPlanItems (8 items)
- VisitLogs (for recording visits)
```

### View Database
```powershell
# Using SQLite CLI (if installed)
sqlite3 C:\Project\VanSalesJourneyPlan.db

# Query test users
SELECT * FROM Users;

# Query today's plan
SELECT * FROM JourneyPlans WHERE PlanDate = DATE('now');

# Query visit logs
SELECT * FROM VisitLogs;
```

---

## 🏗️ Project Structure

```
C:\Project\
├── VanSalesJourneyPlan.sln           # Main solution file
│
├── VanSalesJourneyPlan.API/          # Backend REST API
│   ├── Controllers/                   # API endpoints
│   ├── Services/                      # Business logic (Auth, JWT)
│   ├── Data/                          # EF Core DbContext
│   ├── Models/                        # Domain models
│   ├── DTOs/                          # Data transfer objects
│   ├── Program.cs                     # DI configuration
│   ├── appsettings.json               # Connection strings & JWT config
│   └── VanSalesJourneyPlan.API.csproj # Project file
│
├── VanSalesJourneyPlan.Mobile/       # Android mobile app (.NET MAUI)
│   ├── Services/                      # Business logic
│   ├── Views/                         # XAML pages
│   ├── Models/                        # Local models
│   ├── MauiProgram.cs                # DI configuration
│   ├── App.xaml                       # App resources
│   ├── AppShell.xaml                  # Navigation shell
│   └── VanSalesJourneyPlan.Mobile.csproj # Project file
│
├── VanSalesJourneyPlan.Web/          # Admin dashboard (ASP.NET MVC)
│   ├── Controllers/                   # MVC controllers
│   ├── Views/                         # Razor views
│   └── VanSalesJourneyPlan.Web.csproj # Project file
│
├── VanSalesJourneyPlan.db            # SQLite database
├── appsettings.json                  # API configuration
├── RUNNING_THE_PROJECT.md            # This file
└── PROJECT_STATUS.md                 # Project status
```

---

## ⚙️ API Configuration

### appsettings.json
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=C:\\Project\\VanSalesJourneyPlan.db"
  },
  "Jwt": {
    "SecretKey": "YourSuperLongSecretKeyHere32PlusChars",
    "Issuer": "VanSalesJourneyPlan",
    "Audience": "VanSalesJourneyPlanUsers",
    "ExpirationMinutes": 1440
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  },
  "AllowedHosts": "*"
}
```

---

## 🧪 Testing Endpoints

### Using Swagger UI (Recommended)
1. Start API: `dotnet run` in VanSalesJourneyPlan.API folder
2. Open browser: http://localhost:5249/swagger
3. All endpoints documented and testable directly in browser

### Using PowerShell (Quick Testing)
```powershell
# Set base URL
$baseUrl = "http://localhost:5249/api"

# Helper function to make authenticated requests
function Invoke-ApiCall {
    param(
        [string]$Endpoint,
        [string]$Method = "GET",
        [object]$Body = $null,
        [string]$Token
    )
    
    $headers = @{}
    if ($Token) {
        $headers["Authorization"] = "Bearer $Token"
    }
    
    if ($Body) {
        $Body = $Body | ConvertTo-Json
        Invoke-WebRequest -Uri "$baseUrl$Endpoint" -Method $Method -Body $Body -ContentType "application/json" -Headers $headers
    } else {
        Invoke-WebRequest -Uri "$baseUrl$Endpoint" -Method $Method -Headers $headers
    }
}

# Test login
$login = Invoke-ApiCall -Endpoint "/auth/login" -Method POST -Body @{ username = "vansales_test01"; password = "vansales123" }
$token = ($login.Content | ConvertFrom-Json).token

# Test get plans
$plans = Invoke-ApiCall -Endpoint "/journeyplans/my" -Token $token
$plans.Content | ConvertFrom-Json
```

---

## 🔧 Common Issues & Solutions

### Issue: "Unable to locate the database file"
**Solution**: Verify database path in `appsettings.json`:
```
"DefaultConnection": "Data Source=C:\\Project\\VanSalesJourneyPlan.db"
```

### Issue: "Port 5249 already in use"
**Solution**: Stop existing process or use different port:
```powershell
# Find process using port 5249
netstat -ano | findstr :5249

# Kill the process (replace PID with actual number)
Stop-Process -Id <PID> -Force
```

### Issue: "Invalid JWT token"
**Solution**: Ensure token is correctly passed in Authorization header:
```
Authorization: Bearer eyJhbGciOiJIUzI1NiI...
```

### Issue: "Mobile app won't compile"
**Solution**: Ensure prerequisites installed:
```powershell
# Check .NET version
dotnet --version

# Reinstall workload if needed
dotnet workload restore
```

---

## 📈 What to Test After Running

### API Tests (PowerShell)
- ✅ Login returns JWT token
- ✅ GET /api/journeyplans/my returns 3 plans
- ✅ GET /api/journeyplans/1 returns 5 customers
- ✅ POST /api/visitlogs creates visit record
- ✅ Plan item marked as completed

### Mobile App Tests
- ✅ App starts and shows LoginPage
- ✅ Login with correct credentials succeeds
- ✅ JourneyPlanListPage displays plans
- ✅ PlanDetailPage shows customers
- ✅ VisitLogPage form works
- ✅ Can log a visit and see success
- ✅ App offline access works (kill API, check cached data)

---

## 📞 API Endpoints Reference

### Authentication
- `POST /api/auth/login` - Get JWT token
- `POST /api/auth/logout` - Invalidate session

### Journey Plans
- `GET /api/journeyplans/my` - Get your assigned plans
- `GET /api/journeyplans/{id}` - Get plan details with customers
- `POST /api/journeyplans` - Create new plan (Admin)
- `PUT /api/journeyplans/{id}` - Update plan (Admin)
- `DELETE /api/journeyplans/{id}` - Delete plan (Admin)

### Customers
- `GET /api/customers` - List all customers (Admin)
- `GET /api/customers/{id}` - Get customer details (Admin)
- `POST /api/customers` - Create customer (Admin)
- `PUT /api/customers/{id}` - Update customer (Admin)
- `DELETE /api/customers/{id}` - Delete customer (Admin)

### Visit Logs
- `GET /api/visitlogs` - Get visit history (Admin)
- `POST /api/visitlogs` - Log a visit (VanSalesUser)

---

## 🎓 Complete End-to-End Workflow

```
1. START API
   $ cd C:\Project\VanSalesJourneyPlan.API
   $ dotnet run

2. OPEN MOBILE APP
   $ cd C:\Project\VanSalesJourneyPlan.Mobile
   $ dotnet run -f net8.0-android
   (Or launch Android emulator first, then open app in Visual Studio)

3. LOGIN IN MOBILE
   Username: vansales_test01
   Password: vansales123

4. VIEW PLANS
   See "Daily Journey Plan - Today" and other plans

5. SELECT PLAN
   Tap first plan to see 5 customers

6. LOG VISIT
   Tap "Log Visit" for "ABC Trading Company"
   Fill in form and submit

7. VERIFY IN API
   Visit Swagger at http://localhost:5249/swagger
   GET /journeyplans/1 shows first customer as completed

8. TEST OFFLINE
   Stop API server (Ctrl+C)
   Mobile app still shows cached plans
   Restart API to sync visits
```

---

## 📚 Next Steps

1. **For Development**: Open `VanSalesJourneyPlan.sln` in Visual Studio
2. **For Testing**: Use Swagger UI or PowerShell tests
3. **For Deployment**: See `azure-deployment-setup` plan (Phase 6)
4. **For Web Admin**: Build out Phase 5 - Web Admin Dashboard

---

## ✅ Checklist for First Run

- [ ] .NET 9.0 SDK installed
- [ ] Android SDK installed (for mobile)
- [ ] Database file exists at C:\Project\VanSalesJourneyPlan.db
- [ ] API starts on http://localhost:5249
- [ ] Swagger UI loads at /swagger
- [ ] Login works with test credentials
- [ ] Journey plans returned for user
- [ ] Mobile app builds successfully
- [ ] Mobile app runs on emulator
- [ ] Can log a visit and see completion

---

**Happy coding! 🚀**
