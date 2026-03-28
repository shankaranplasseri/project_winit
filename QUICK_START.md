# Quick Reference - Van Sales Journey Plan

## 🚀 Start the Project (2 Commands)

```powershell
# Terminal 1: Start API server
cd C:\Project\VanSalesJourneyPlan.API
dotnet run

# Terminal 2: Build mobile app
cd C:\Project\VanSalesJourneyPlan.Mobile
dotnet build -f net8.0-android -c Release
```

**API Running at**: http://localhost:5249  
**Swagger UI**: http://localhost:5249/swagger

---

## 🔑 Test Login

```
Username: vansales_test01
Password: vansales123
```

Or use Swagger UI to test endpoints directly.

---

## 📱 Mobile App Quick Test

### PowerShell Test (Without Emulator)
```powershell
# 1. Get JWT token
$resp = Invoke-WebRequest "http://localhost:5249/api/auth/login" `
  -Method POST `
  -Body '{"username":"vansales_test01","password":"vansales123"}' `
  -ContentType "application/json"

$token = ($resp.Content | ConvertFrom-Json).token
Write-Host "Token: $token"

# 2. Get your plans
$headers = @{"Authorization" = "Bearer $token"}
Invoke-WebRequest "http://localhost:5249/api/journeyplans/my" `
  -Headers $headers | Select-Object -ExpandProperty Content | ConvertFrom-Json

# 3. Get plan details
Invoke-WebRequest "http://localhost:5249/api/journeyplans/1" `
  -Headers $headers | Select-Object -ExpandProperty Content | ConvertFrom-Json

# 4. Log a visit
$visitBody = @{
  journeyPlanItemId = 1
  visitDate = "2026-03-28"
  visitTime = "09:15"
  notes = "Great visit"
  salesAmount = 5500
} | ConvertTo-Json

Invoke-WebRequest "http://localhost:5249/api/visitlogs" `
  -Method POST `
  -Body $visitBody `
  -ContentType "application/json" `
  -Headers $headers
```

---

## 🗂️ Project Structure

```
C:\Project\
├── VanSalesJourneyPlan.API/          ← Start this first
├── VanSalesJourneyPlan.Mobile/       ← Mobile app (builds successfully)
├── VanSalesJourneyPlan.Web/          ← Admin dashboard (Phase 5)
├── VanSalesJourneyPlan.db            ← SQLite database
├── RUNNING_THE_PROJECT.md            ← Full instructions
└── PROJECT_STATUS.md                 ← Project progress
```

---

## ✅ Status

- **API**: ✅ Complete & Tested (http://localhost:5249)
- **Mobile**: ✅ Compiles (ready to deploy to emulator)
- **Database**: ✅ Ready (3 plans, 5 customers in test data)
- **User Story**: ✅ 100% Implemented

---

## 🔗 Key URLs

| Resource | URL |
|----------|-----|
| **Swagger** | http://localhost:5249/swagger |
| **Login** | POST http://localhost:5249/api/auth/login |
| **My Plans** | GET http://localhost:5249/api/journeyplans/my |
| **Plan Details** | GET http://localhost:5249/api/journeyplans/1 |
| **Log Visit** | POST http://localhost:5249/api/visitlogs |

---

## 📚 See Also

- **RUNNING_THE_PROJECT.md** - Complete setup guide
- **PROJECT_STATUS.md** - Project progress & metrics
- **plan.md** - Development plan & todos

---

**User Story Implemented**: "As a Van Sales User, I want to view my assigned Journey Plan on my mobile device so that I can visit the right customers on the scheduled day and perform necessary sales activities in the field" ✅

**Current Status**: 71.4% Complete (10/14 todos) | User Story: 100% Complete
