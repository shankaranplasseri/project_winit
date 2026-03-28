# Journey Plan Management System - Implementation Plan

## 📋 Project Overview
Build a **Van Sales Journey Plan Management System** with:
- **Web Admin Dashboard** (ASP.NET Core MVC) for admins to create and assign daily journey plans
- **Mobile App** (Xamarin.Forms with Prism MVVM) for field sales users to view and manage customer visits
- **ASP.NET Core REST API** for backend services
- **SQLite Database** for local data caching (critical data only)
- **Azure Deployment** for both API and web app

---

## 🎯 Confirmed Requirements
- **Target Platform**: Android only (via Xamarin)
- **MVVM Framework**: Prism for Xamarin
- **Offline Strategy**: Hybrid - cache critical data (customer list, current day's plan) locally
- **External Integration**: Azure for API and app deployment
- **User Roles**: 
  - Admin (Web Dashboard only)
  - Van Sales User (Mobile App only)
- **Key Non-Functional Requirements**:
  - Mobile-first, field-optimized UI
  - Performance: JP loads in 2s (3G), customer details in 1s
  - Field usability: bright sunlight contrast, one-handed operation, 44x44px touch targets

---

## 🏗️ System Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                     AZURE CLOUD                             │
├──────────────────┬──────────────────────┬──────────────────┤
│  Web App         │   API Server         │   SQLite Cloud   │
│ (ASP.NET MVC)    │ (ASP.NET Core)       │   (Backup)       │
└──────────────────┴──────────────────────┴──────────────────┘
         ↑                 ↑                       ↑
         │                 │                       │
┌────────┴────────┬────────┴────────────┬─────────┴─────────┐
│  Admin Browser  │  Mobile App (Android) │  Local SQLite   │
│                 │  (Xamarin.Forms)      │  (Cache Layer)  │
└─────────────────┴───────────────────────┴─────────────────┘
```

### Layer Architecture
1. **Presentation Layer**
   - Admin Web UI (ASP.NET MVC Razor Views)
   - Mobile UI (Xamarin.Forms XAML with Prism)

2. **Domain Layer**
   - Models (User, JourneyPlan, Customer, Visit)
   - Business Logic & Validation

3. **Data Layer**
   - API Service (HTTP client for mobile)
   - SQLite Repository (local caching)
   - EF Core DbContext (API backend)

---

## 📊 Database Schema

### Core Tables
```
Users
  ├── UserId (PK)
  ├── Username
  ├── PasswordHash
  ├── Email
  ├── Role (Admin / VanSalesUser)
  └── IsActive

JourneyPlans
  ├── JourneyPlanId (PK)
  ├── AssignedUserId (FK → Users)
  ├── ScheduleDate
  ├── CreatedDate
  ├── Status
  └── Notes

JourneyPlanItems
  ├── JourneyPlanItemId (PK)
  ├── JourneyPlanId (FK)
  ├── CustomerId (FK)
  ├── SequenceNo
  └── Notes

Customers
  ├── CustomerId (PK)
  ├── CustomerCode
  ├── CustomerName
  ├── Address
  ├── ContactNumber
  ├── Location/Route
  └── Status

VisitLogs
  ├── VisitLogId (PK)
  ├── JourneyPlanItemId (FK)
  ├── VisitDate
  ├── VisitNotes
  ├── SalesAmount
  ├── Status (Pending/Completed)
  └── CreatedDate
```

---

## 🔌 API Endpoints (REST)

### Authentication
- `POST /api/auth/login` - User login (returns JWT token)
- `POST /api/auth/logout` - User logout
- `GET /api/auth/validate` - Token validation

### Journey Plans (Mobile)
- `GET /api/journeyplans/today` - Get today's journey plan for logged-in user
- `GET /api/journeyplans/{id}` - Get journey plan details
- `GET /api/journeyplans/{id}/customers` - Get customers in plan

### Journey Plans (Admin)
- `GET /api/admin/journeyplans` - List all plans (paginated)
- `POST /api/admin/journeyplans` - Create new plan
- `PUT /api/admin/journeyplans/{id}` - Update plan
- `POST /api/admin/journeyplans/{id}/assign` - Assign plan to user
- `DELETE /api/admin/journeyplans/{id}` - Delete plan

### Customers
- `GET /api/customers` - List customers (paginated)
- `GET /api/customers/{id}` - Get customer details
- `POST /api/customers` - Create customer (Admin)
- `PUT /api/customers/{id}` - Update customer (Admin)

### Visits
- `POST /api/visits` - Log visit (Mobile)
- `PUT /api/visits/{id}` - Update visit status
- `GET /api/visits/report` - Get visit reports (Admin)

---

## 📱 Mobile App Features (MVP)

### Screens
1. **Login Screen**
   - Username/password fields
   - "Remember me" checkbox
   - Error handling
   - Form validation

2. **Journey Plan List Screen**
   - Today's journey plan card
   - Customer list (optimized for mobile)
   - Pull-to-refresh
   - Offline indicator

3. **Customer Detail Screen**
   - Customer info (name, code, address, contact)
   - Location map (if available)
   - Visit actions: Mark Complete, Add Notes, Record Sales
   - Previous visit history

4. **Visit Log Screen**
   - Form to log visit (notes, sales amount)
   - Camera access (capture photos)
   - Submit & sync to API

5. **Settings Screen**
   - User profile
   - Logout
   - Sync status
   - Offline data management

### MVVM Structure
```
Mobile App (Xamarin.Forms)
├── Models/
│   ├── User.cs
│   ├── JourneyPlan.cs
│   ├── Customer.cs
│   └── VisitLog.cs
├── Views/
│   ├── LoginView.xaml
│   ├── JourneyPlanListView.xaml
│   ├── CustomerDetailView.xaml
│   ├── VisitLogView.xaml
│   └── SettingsView.xaml
├── ViewModels/ (Prism)
│   ├── LoginViewModel.cs
│   ├── JourneyPlanListViewModel.cs
│   ├── CustomerDetailViewModel.cs
│   ├── VisitLogViewModel.cs
│   └── SettingsViewModel.cs
├── Services/
│   ├── ApiService.cs (HTTP client)
│   ├── LocalStorageService.cs (SQLite)
│   ├── AuthenticationService.cs
│   └── SyncService.cs
└── App.xaml.cs
```

---

## 🌐 Web Admin Dashboard

### Pages (ASP.NET MVC)
1. **Login Page**
   - Admin authentication
   - Session management

2. **Dashboard**
   - Summary stats (total plans, users, visits)
   - Recent activity

3. **Journey Plans Management**
   - List all plans (table view)
   - Create new plan (form)
   - Edit plan
   - Assign to user
   - Delete plan

4. **Users Management**
   - List van sales users
   - Enable/disable users
   - View user's assigned plans

5. **Customers Management**
   - List customers
   - Add/edit customer
   - Search & filter

6. **Reports**
   - Visit completion reports
   - User performance metrics
   - Sales by user/period

---

## 🔐 Security Implementation
- Password hashing (BCrypt)
- JWT token-based authentication
- CORS configuration (API)
- Input validation on all endpoints
- HTTPS/TLS for all communications
- Secure local storage (encrypted SQLite)

---

## 📋 Development Phases & Todos

### Phase 0: Setup ✓
- Clarify requirements ✓
- Understand project scope ✓

### Phase 1: Project Structure
- Create .NET solution structure
- Create ASP.NET Core API project
- Create ASP.NET MVC Web project
- Create Xamarin.Forms mobile project
- Setup git repository

### Phase 2: Database
- Create SQLite database schema
- Setup EF Core DbContext for API
- Create sample test data
- Create local SQLite for mobile caching

### Phase 3: API Backend
- Implement authentication endpoints (login, logout)
- Implement JourneyPlan APIs
- Implement Customer APIs
- Implement Visit logging APIs
- Add input validation
- Test all endpoints

### Phase 4: Web Admin Dashboard
- Build login page & authentication
- Build journey plan management pages
- Build customer management pages
- Build user management pages
- Implement API integration (HTTP client)
- Add UI styling & responsiveness

### Phase 5: Mobile App - Architecture
- Setup Prism MVVM framework
- Create service layer (API + SQLite)
- Implement dependency injection
- Setup navigation

### Phase 6: Mobile App - UI & Features
- Build login screen
- Build journey plan list screen
- Build customer detail screen
- Build visit log screen
- Build settings screen
- Implement field-optimized styling

### Phase 7: Mobile App - Data Sync
- Implement local caching (SQLite)
- Implement API integration
- Implement sync mechanism
- Handle offline scenarios
- Performance optimization (2s load time)

### Phase 8: Integration & Testing
- Test API endpoints (Postman/HTTP)
- Test web dashboard with API
- Test mobile app with API
- Test offline scenarios
- Playwright UI tests

### Phase 9: Azure Deployment
- Configure Azure App Service for API
- Configure Azure App Service for Web
- Setup database on Azure
- Deploy and verify

### Phase 10: Performance & Optimization
- Profile and optimize API response times
- Mobile app startup optimization
- Network optimization (3G testing)
- Caching strategies

### Phase 11: Documentation & Delivery
- Create API documentation
- Create user guides
- Create deployment guide
- Test summary report

---

## 📦 Project Structure (To Be Created)

```
VanSalesJourneyPlan/
├── VanSalesJourneyPlan.sln
├── VanSalesJourneyPlan.API/
│   ├── Controllers/
│   ├── Models/
│   ├── Services/
│   ├── Data/ (EF Core)
│   ├── appsettings.json
│   └── Startup.cs
├── VanSalesJourneyPlan.Web/
│   ├── Controllers/
│   ├── Views/
│   ├── wwwroot/
│   ├── Models/
│   └── Startup.cs
├── VanSalesJourneyPlan.Mobile/
│   ├── Models/
│   ├── Views/
│   ├── ViewModels/
│   ├── Services/
│   ├── App.xaml
│   └── VanSalesJourneyPlan.Mobile.csproj
└── docs/
    ├── API_Documentation.md
    ├── Database_Schema.md
    └── Deployment_Guide.md
```

---

## 🎯 Key Success Criteria

1. **Functionality**: All test scenarios (TS-001 to TS-010) pass
2. **Performance**: Journey plans load in ≤2s, customer details in ≤1s
3. **Usability**: Mobile UI works on various screen sizes, one-handed operation
4. **Offline**: Critical data syncs and works offline
5. **Deployment**: Successfully deployed to Azure
6. **Documentation**: Complete API docs, user guides, deployment guide

---

## 📌 Notes & Assumptions
- Using Xamarin.Forms (not native Android) for cross-platform capability
- SQLite for local caching (built-in, no external dependencies)
- JWT for stateless authentication
- Prism DI container for dependency injection
- Async/await pattern throughout for responsiveness
- MVVM data binding for UI updates

---

## 🚀 Next Steps
Awaiting approval to proceed with Phase 1 (Project Setup).
