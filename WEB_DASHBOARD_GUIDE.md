# Web Dashboard Implementation Guide

## 📊 Current Status

**User Story**: ✅ **100% COMPLETE** (Mobile + API fully working)

**Remaining Optional Work**: 4 todos (28.6%)
- [ ] **web-project-setup** - Configure Web to call API
- [ ] **web-admin-dashboard** - Build admin UI pages
- [ ] system-integration-test - End-to-end testing
- [ ] azure-deployment-setup - Azure resources

---

## 🎯 What the Web Dashboard Does

Admin users can:
1. **Manage Customers** - Add, edit, delete customer records
2. **Create Journey Plans** - Create daily plans with customer lists
3. **Assign Plans** - Assign plans to van sales users
4. **View Assignments** - See which plans are assigned to which users
5. **Monitor Activity** - See completed visits and sales data

---

## 🏗️ Implementation Plan

### Phase 5a: Web Project Setup (2 hours)
**Goal**: Configure Web project to communicate with API

#### Step 1: Add NuGet Packages
```xml
<!-- VanSalesJourneyPlan.Web.csproj -->
<ItemGroup>
  <PackageReference Include="System.Net.Http.Json" Version="4.5.0" />
</ItemGroup>
```

#### Step 2: Create ApiClient Service
```csharp
// Services/ApiClient.cs
public interface IApiClient
{
    Task<T?> GetAsync<T>(string endpoint);
    Task<T?> PostAsync<T>(string endpoint, object data);
    Task<T?> PutAsync<T>(string endpoint, object data);
    Task<bool> DeleteAsync(string endpoint);
    void SetToken(string token);
}

public class ApiClient : IApiClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;
    private string? _token;

    public ApiClient(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _config = config;
        _httpClient.BaseAddress = new Uri(config["ApiSettings:BaseUrl"] ?? "http://localhost:5249");
    }

    public void SetToken(string token)
    {
        _token = token;
        _httpClient.DefaultRequestHeaders.Authorization = 
            new("Bearer", token);
    }

    public async Task<T?> GetAsync<T>(string endpoint)
    {
        var response = await _httpClient.GetAsync(endpoint);
        if (response.IsSuccessStatusCode)
            return await response.Content.ReadAsAsync<T>();
        return null;
    }

    public async Task<T?> PostAsync<T>(string endpoint, object data)
    {
        var json = JsonSerializer.Serialize(data);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(endpoint, content);
        if (response.IsSuccessStatusCode)
            return await response.Content.ReadAsAsync<T>();
        return null;
    }

    public async Task<T?> PutAsync<T>(string endpoint, object data)
    {
        var json = JsonSerializer.Serialize(data);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PutAsync(endpoint, content);
        if (response.IsSuccessStatusCode)
            return await response.Content.ReadAsAsync<T>();
        return null;
    }

    public async Task<bool> DeleteAsync(string endpoint)
    {
        var response = await _httpClient.DeleteAsync(endpoint);
        return response.IsSuccessStatusCode;
    }
}
```

#### Step 3: Configure Program.cs
```csharp
// Program.cs
builder.Services.AddHttpClient<IApiClient, ApiClient>();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(24);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

app.UseSession();
```

#### Step 4: Add Configuration
```json
// appsettings.json
{
  "ApiSettings": {
    "BaseUrl": "http://localhost:5249/api"
  }
}
```

---

### Phase 5b: Build Admin Pages (3-4 hours)

#### Page 1: Login (New)
**File**: `Views/Account/Login.cshtml`

Features:
- Admin login form
- Store JWT token in session
- Redirect to dashboard on success

```csharp
// Controllers/AccountController.cs
[HttpPost("login")]
public async Task<IActionResult> Login(LoginViewModel model)
{
    var response = await _apiClient.PostAsync<LoginResponse>(
        "/auth/login", 
        new { username = model.Username, password = model.Password }
    );

    if (response?.Success == true)
    {
        HttpContext.Session.SetString("jwt_token", response.Token);
        HttpContext.Session.SetString("user", JsonSerializer.Serialize(response.User));
        _apiClient.SetToken(response.Token);
        return RedirectToAction("Index", "Dashboard");
    }

    ModelState.AddModelError("", "Invalid credentials");
    return View(model);
}
```

#### Page 2: Dashboard (Home)
**File**: `Views/Dashboard/Index.cshtml`

Shows:
- Summary cards (Total Plans, Customers, Visits Today)
- Recent activity
- Quick action buttons

#### Page 3: Customers List
**File**: `Views/Customers/Index.cshtml`

Features:
- Table with all customers
- Add, Edit, Delete buttons
- Search/filter by route or name

```csharp
// Controllers/CustomersController.cs
public async Task<IActionResult> Index()
{
    var token = HttpContext.Session.GetString("jwt_token");
    _apiClient.SetToken(token);
    
    var customers = await _apiClient.GetAsync<List<CustomerDto>>("/customers");
    return View(customers);
}

[HttpPost("create")]
public async Task<IActionResult> Create(CustomerDto model)
{
    var token = HttpContext.Session.GetString("jwt_token");
    _apiClient.SetToken(token);
    
    var result = await _apiClient.PostAsync<CustomerDto>("/customers", model);
    if (result != null)
        return RedirectToAction("Index");
    
    ModelState.AddModelError("", "Failed to create customer");
    return View(model);
}
```

#### Page 4: Journey Plans
**File**: `Views/JourneyPlans/Index.cshtml` & `Create.cshtml`

Features:
- List all plans
- Create new plan
- Select customers for plan
- Assign to user
- View assignments

```csharp
// Controllers/JourneyPlansController.cs
[HttpGet("create")]
public async Task<IActionResult> Create()
{
    var token = HttpContext.Session.GetString("jwt_token");
    _apiClient.SetToken(token);
    
    // Get list of users to assign to
    var users = await _apiClient.GetAsync<List<UserDto>>("/users");
    // Get list of customers to add to plan
    var customers = await _apiClient.GetAsync<List<CustomerDto>>("/customers");
    
    var model = new CreateJourneyPlanViewModel
    {
        Users = users,
        Customers = customers
    };
    
    return View(model);
}

[HttpPost("create")]
public async Task<IActionResult> Create(CreateJourneyPlanDto model)
{
    var token = HttpContext.Session.GetString("jwt_token");
    _apiClient.SetToken(token);
    
    var result = await _apiClient.PostAsync<JourneyPlanDto>(
        "/journeyplans", 
        model
    );
    
    if (result != null)
        return RedirectToAction("Index");
    
    ModelState.AddModelError("", "Failed to create plan");
    return View(model);
}
```

#### Page 5: View Details/Edit
**File**: `Views/JourneyPlans/Details.cshtml`

Shows:
- Plan details
- List of customers
- Edit/delete options
- View visit logs

---

## 📋 Required DTOs & Models

```csharp
// Models/DTOs
public class LoginResponse
{
    public string Token { get; set; }
    public UserDto User { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; }
}

public class UserDto
{
    public int UserId { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
}

public class CustomerDto
{
    public int CustomerId { get; set; }
    public string CustomerCode { get; set; }
    public string CustomerName { get; set; }
    public string Address { get; set; }
    public string ContactNumber { get; set; }
    public double LocationLatitude { get; set; }
    public double LocationLongitude { get; set; }
    public string Route { get; set; }
}

public class JourneyPlanDto
{
    public int JourneyPlanId { get; set; }
    public int AssignedUserId { get; set; }
    public DateTime PlanDate { get; set; }
    public string Title { get; set; }
    public string Status { get; set; }
    public List<JourneyPlanItemDto> Items { get; set; }
}

public class CreateJourneyPlanDto
{
    public int AssignedUserId { get; set; }
    public DateTime PlanDate { get; set; }
    public string Title { get; set; }
    public List<int> CustomerIds { get; set; }
}
```

---

## 🎨 UI Pages Summary

| Page | Purpose | Features |
|------|---------|----------|
| **Login** | Admin authentication | Form, error messages |
| **Dashboard** | Home/overview | Stats, recent activity |
| **Customers** | Manage customers | List, add, edit, delete |
| **Journey Plans** | Manage plans | List, create, assign |
| **Plan Details** | View/edit plan | Details, customers, visits |

---

## 🔐 Authentication Flow

1. Admin opens Web app
2. Redirected to Login page
3. Enters credentials (admin_test / admin123)
4. Web calls `POST /api/auth/login` to API
5. API returns JWT token
6. Web stores token in session
7. All subsequent API calls include JWT in Authorization header
8. Admin sees Dashboard

---

## ✅ Testing the Web Dashboard

### Step 1: Start API
```powershell
cd C:\Project\VanSalesJourneyPlan.API
dotnet run
```

### Step 2: Run Web Project
```powershell
cd C:\Project\VanSalesJourneyPlan.Web
dotnet run
# Opens at http://localhost:5000
```

### Step 3: Test Admin Flow
1. Navigate to http://localhost:5000
2. Login: admin_test / admin123
3. View dashboard
4. View customers
5. Create journey plan
6. Assign customers
7. Assign to user

### Step 4: Verify in API
- Go to http://localhost:5249/swagger
- Call GET /journeyplans to verify plan was created
- Call GET /visitlogs to see if any visits were logged

---

## 🚀 Implementation Timeline

| Task | Time | Priority |
|------|------|----------|
| Setup ApiClient | 30 min | Critical |
| Configure Program.cs | 15 min | Critical |
| Login page | 30 min | Critical |
| Dashboard page | 30 min | Medium |
| Customers CRUD | 60 min | High |
| Journey Plans CRUD | 90 min | High |
| Testing & debugging | 30 min | Medium |
| **Total** | **~4 hours** | - |

---

## 📝 Notes

- All API endpoints are already working (verified)
- Test data is ready (10 customers, 2 users)
- JWT authentication is secure
- Bootstrap CSS is available in wwwroot
- Can be deployed to Azure App Service when ready

---

## 🎓 Decision Point

**The user story is 100% complete.** The web dashboard is an optional enhancement.

### Choose One:
1. **Skip Web Dashboard** - Project is done, user story fulfilled
2. **Build Web Dashboard** - ~4 hours of development
3. **Build Partial Dashboard** - Focus on key pages (Customers + Plans)

**Recommendation**: 
- If time is limited: Skip (user story is complete)
- If want full system: Build complete dashboard (~4 hours)
- If want quick admin tool: Build partial dashboard (Customers + Plans only)
