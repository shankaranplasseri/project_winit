# ===============================
# MCP SETUP (FINAL WITH DB FIX)
# ===============================

Write-Host "[*] Starting MCP setup..." -ForegroundColor Cyan

$copilotDir = "C:\Users\Asus\.copilot"
$configPath = Join-Path $copilotDir "mcp-config.json"
$currentDir = Get-Location | Select-Object -ExpandProperty Path

# Format paths to use forward slashes for JSON compatibility
$formattedCurrentDir = $currentDir -replace "\\", "/"
$dbPath = "$formattedCurrentDir/MyAppDB.db"

# ---------------------------------------------------------
# THE FIX: Physically create the empty DB file so it doesn't crash
# ---------------------------------------------------------
$physicalDbPath = Join-Path $currentDir "MyAppDB.db"
Write-Host "[*] Checking for SQLite database file at $physicalDbPath..." -ForegroundColor Cyan

if (-not (Test-Path $physicalDbPath)) {
    New-Item -ItemType File -Path $physicalDbPath -Force | Out-Null
    Write-Host "[OK] Created empty MyAppDB.db file to prevent timeout." -ForegroundColor Green
} else {
    Write-Host "[OK] Database file already exists." -ForegroundColor Green
}

Write-Host "[*] Creating configuration..." -ForegroundColor Cyan

# Create directory if it does not exist
if (-not (Test-Path $copilotDir)) {
    New-Item -ItemType Directory -Path $copilotDir | Out-Null
}

# Build the config
$McpConfig = [ordered]@{
    mcpServers = [ordered]@{
        filesystem = [ordered]@{
            command = "npx"
            args = @("-y", "@modelcontextprotocol/server-filesystem", $formattedCurrentDir)
        }
        terminal = [ordered]@{
            command = "npx"
            args = @("-y", "mcp-server-commands")
        }
        git = [ordered]@{
            command = "npx"
            args = @("-y", "@modelcontextprotocol/server-github")
        }
        http = [ordered]@{
            command = "npx"
            args = @("-y", "@wordbricks/fetch-mcp")
        }
        playwright = [ordered]@{
            command = "npx"
            args = @("-y", "@playwright/mcp")
        }
        sql = [ordered]@{
            command = "npx"
            args = @("-y", "mcp-server-sqlite", $dbPath)
        }
    }
}

# Convert object to JSON and save
$configJson = $McpConfig | ConvertTo-Json -Depth 10
Set-Content -Path $configPath -Value $configJson -Encoding UTF8

Write-Host "[OK] Config successfully updated at $configPath" -ForegroundColor Green
Write-Host "[*] DONE. Please restart Copilot and your terminal." -ForegroundColor Magenta