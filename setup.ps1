#!/usr/bin/env pwsh
# ============================================================================
# TiendaUCN - Script de Configuración y Ejecución para Pruebas
# ============================================================================

Write-Host "╔════════════════════════════════════════════════════════════╗" -ForegroundColor Cyan
Write-Host "║   TiendaUCN - Configuración para Pruebas de Disertación    ║" -ForegroundColor Cyan
Write-Host "╚════════════════════════════════════════════════════════════╝" -ForegroundColor Cyan
Write-Host ""

# Variables
$projectPath = Split-Path -Parent $MyInvocation.MyCommand.Path
$dbPath = Join-Path $projectPath "database.db"
$appsettingsPath = Join-Path $projectPath "appsettings.json"
$appsettingsExample = Join-Path $projectPath "appsettings.example.json"
$envFile = Join-Path $projectPath ".env"

# Colores
$Success = "Green"
$Warning = "Yellow"
$Error = "Red"
$Info = "Cyan"

function Write-Success {
    Write-Host "✅ $args" -ForegroundColor $Success
}

function Write-Warning-Custom {
    Write-Host "⚠️  $args" -ForegroundColor $Warning
}

function Write-Error-Custom {
    Write-Host "❌ $args" -ForegroundColor $Error
}

function Write-Info {
    Write-Host "ℹ️  $args" -ForegroundColor $Info
}

# ============================================================================
# OPCIÓN 1: LIMPIAR BASE DE DATOS
# ============================================================================
function Clean-Database {
    Write-Host ""
    Write-Host "═══════════════════════════════════════════════════════════" -ForegroundColor Cyan
    Write-Host "OPCIÓN 1: Limpiar Base de Datos" -ForegroundColor Cyan
    Write-Host "═══════════════════════════════════════════════════════════" -ForegroundColor Cyan
    Write-Host ""
    
    if (Test-Path $dbPath) {
        Write-Warning-Custom "Base de datos actual será eliminada: $dbPath"
        $confirm = Read-Host "¿Continuar? (s/n)"
        
        if ($confirm -eq "s") {
            Remove-Item $dbPath -Force -ErrorAction SilentlyContinue
            Write-Success "Base de datos eliminada"
            
            # Eliminar archivos WAL
            Remove-Item "$dbPath-shm" -Force -ErrorAction SilentlyContinue
            Remove-Item "$dbPath-wal" -Force -ErrorAction SilentlyContinue
            Write-Success "Archivos WAL eliminados"
        } else {
            Write-Info "Operación cancelada"
            return
        }
    }
    
    Write-Info "Migrando base de datos..."
    dotnet ef database update
    
    if ($LASTEXITCODE -eq 0) {
        Write-Success "Base de datos creada exitosamente"
    } else {
        Write-Error-Custom "Error al crear base de datos"
    }
}

# ============================================================================
# OPCIÓN 2: RESTAURAR APPSETTINGS
# ============================================================================
function Restore-AppSettings {
    Write-Host ""
    Write-Host "═══════════════════════════════════════════════════════════" -ForegroundColor Cyan
    Write-Host "OPCIÓN 2: Restaurar appsettings.json" -ForegroundColor Cyan
    Write-Host "═══════════════════════════════════════════════════════════" -ForegroundColor Cyan
    Write-Host ""
    
    if (Test-Path $appsettingsExample) {
        Write-Info "Copiando appsettings.example.json → appsettings.json"
        Copy-Item $appsettingsExample $appsettingsPath -Force
        Write-Success "appsettings.json restaurado"
        
        Write-Warning-Custom "Importante: Debes configurar:"
        Write-Host "  - DATA_BASE_URL (variable de entorno)"
        Write-Host "  - JWT_SECRET (variable de entorno)"
        Write-Host "  - RESEND_API_KEY (variable de entorno)"
        Write-Host "  - Cloudinary credentials (en appsettings.json)"
    } else {
        Write-Error-Custom "No se encontró appsettings.example.json"
    }
}

# ============================================================================
# OPCIÓN 3: CONFIGURAR VARIABLES DE ENTORNO
# ============================================================================
function Setup-Environment {
    Write-Host ""
    Write-Host "═══════════════════════════════════════════════════════════" -ForegroundColor Cyan
    Write-Host "OPCIÓN 3: Configurar Variables de Entorno" -ForegroundColor Cyan
    Write-Host "═══════════════════════════════════════════════════════════" -ForegroundColor Cyan
    Write-Host ""
    
    $dbUrl = Read-Host "Database URL (default: Data Source=database.db)"
    if ([string]::IsNullOrEmpty($dbUrl)) {
        $dbUrl = "Data Source=database.db"
    }
    
    $jwtSecret = Read-Host "JWT Secret (mínimo 32 caracteres)"
    if ([string]::IsNullOrEmpty($jwtSecret)) {
        $jwtSecret = "MySecureJWTKeyThatIsAtLeast32Characters123!@#"
    }
    
    $resendKey = Read-Host "Resend API Key (o presiona Enter para dummy)"
    if ([string]::IsNullOrEmpty($resendKey)) {
        $resendKey = "re_dummy_key_for_testing"
    }
    
    # Crear o actualizar .env
    @"
DATA_BASE_URL=$dbUrl
JWT_SECRET=$jwtSecret
RESEND_API_KEY=$resendKey
"@ | Set-Content $envFile
    
    Write-Success ".env creado/actualizado"
    Write-Info "Variables exportadas:"
    Write-Host "  - DATA_BASE_URL: $dbUrl"
    Write-Host "  - JWT_SECRET: ••••••••••••••••"
    Write-Host "  - RESEND_API_KEY: ••••••••••••••••"
}

# ============================================================================
# OPCIÓN 4: INSTALAR DEPENDENCIAS
# ============================================================================
function Install-Dependencies {
    Write-Host ""
    Write-Host "═══════════════════════════════════════════════════════════" -ForegroundColor Cyan
    Write-Host "OPCIÓN 4: Instalar Dependencias" -ForegroundColor Cyan
    Write-Host "═══════════════════════════════════════════════════════════" -ForegroundColor Cyan
    Write-Host ""
    
    Write-Info "Ejecutando dotnet restore..."
    dotnet restore
    
    if ($LASTEXITCODE -eq 0) {
        Write-Success "Dependencias instaladas exitosamente"
    } else {
        Write-Error-Custom "Error al instalar dependencias"
    }
}

# ============================================================================
# OPCIÓN 5: COMPILAR PROYECTO
# ============================================================================
function Build-Project {
    Write-Host ""
    Write-Host "═══════════════════════════════════════════════════════════" -ForegroundColor Cyan
    Write-Host "OPCIÓN 5: Compilar Proyecto" -ForegroundColor Cyan
    Write-Host "═══════════════════════════════════════════════════════════" -ForegroundColor Cyan
    Write-Host ""
    
    Write-Info "Compilando proyecto..."
    dotnet build --configuration Debug
    
    if ($LASTEXITCODE -eq 0) {
        Write-Success "Proyecto compilado exitosamente"
    } else {
        Write-Error-Custom "Error al compilar proyecto"
    }
}

# ============================================================================
# OPCIÓN 6: VERIFICAR CONFIGURACIÓN
# ============================================================================
function Verify-Setup {
    Write-Host ""
    Write-Host "═══════════════════════════════════════════════════════════" -ForegroundColor Cyan
    Write-Host "OPCIÓN 6: Verificar Configuración" -ForegroundColor Cyan
    Write-Host "═══════════════════════════════════════════════════════════" -ForegroundColor Cyan
    Write-Host ""
    
    $checks = @{
        ".NET SDK" = { dotnet --version }
        "Entity Framework Core" = { dotnet ef --version }
        "appsettings.json" = { Test-Path $appsettingsPath }
        ".env" = { Test-Path $envFile }
        "database.db" = { Test-Path $dbPath }
    }
    
    foreach ($check in $checks.GetEnumerator()) {
        $name = $check.Name
        $result = $check.Value
        
        try {
            $test = & $result
            if ($test) {
                Write-Success "${name}: OK"
            } else {
                Write-Warning-Custom "${name}: No configurado"
            }
        } catch {
            Write-Warning-Custom "${name}: No disponible"
        }
    }
}

# ============================================================================
# OPCIÓN 7: EJECUTAR APLICACIÓN
# ============================================================================
function Run-Application {
    Write-Host ""
    Write-Host "═══════════════════════════════════════════════════════════" -ForegroundColor Cyan
    Write-Host "OPCIÓN 7: Ejecutar Aplicación" -ForegroundColor Cyan
    Write-Host "═══════════════════════════════════════════════════════════" -ForegroundColor Cyan
    Write-Host ""
    
    Write-Info "Iniciando aplicación..."
    Write-Host ""
    Write-Host "La aplicación estará disponible en: http://localhost:5094" -ForegroundColor Green
    Write-Host "Hangfire Dashboard: http://localhost:5094/hangfire" -ForegroundColor Green
    Write-Host "Swagger/OpenAPI: http://localhost:5094/openapi" -ForegroundColor Green
    Write-Host ""
    Write-Info "Presiona Ctrl+C para detener la aplicación"
    Write-Host ""
    
    dotnet run
}

# ============================================================================
# OPCIÓN 8: MENÚ COMPLETO DE CONFIGURACIÓN
# ============================================================================
function Setup-Complete {
    Write-Host ""
    Write-Host "═══════════════════════════════════════════════════════════" -ForegroundColor Cyan
    Write-Host "OPCIÓN 8: Configuración Completa (Paso a Paso)" -ForegroundColor Cyan
    Write-Host "═══════════════════════════════════════════════════════════" -ForegroundColor Cyan
    Write-Host ""
    
    Write-Host "Este proceso realizará:" -ForegroundColor Yellow
    Write-Host "  1. Restaurar appsettings.json"
    Write-Host "  2. Configurar variables de entorno"
    Write-Host "  3. Instalar dependencias"
    Write-Host "  4. Compilar proyecto"
    Write-Host "  5. Crear base de datos"
    Write-Host ""
    
    $confirm = Read-Host "¿Continuar? (s/n)"
    if ($confirm -ne "s") {
        return
    }
    
    Restore-AppSettings
    Write-Host ""
    Setup-Environment
    Write-Host ""
    Install-Dependencies
    Write-Host ""
    Build-Project
    Write-Host ""
    Clean-Database
    
    Write-Host ""
    Write-Success "¡Configuración completada!"
    Write-Info "Ahora puedes ejecutar: dotnet run"
}

# ============================================================================
# MENÚ PRINCIPAL
# ============================================================================
function Show-Menu {
    Write-Host ""
    Write-Host "Selecciona una opción:" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "  1️⃣  Limpiar Base de Datos"
    Write-Host "  2️⃣  Restaurar appsettings.json"
    Write-Host "  3️⃣  Configurar Variables de Entorno"
    Write-Host "  4️⃣  Instalar Dependencias"
    Write-Host "  5️⃣  Compilar Proyecto"
    Write-Host "  6️⃣  Verificar Configuración"
    Write-Host "  7️⃣  Ejecutar Aplicación"
    Write-Host "  8️⃣  Configuración Completa (TODO)"
    Write-Host "  9️⃣  Salir"
    Write-Host ""
}

# ============================================================================
# LOOP PRINCIPAL
# ============================================================================
do {
    Show-Menu
    $choice = Read-Host "Opción"
    
    switch ($choice) {
        "1" { Clean-Database }
        "2" { Restore-AppSettings }
        "3" { Setup-Environment }
        "4" { Install-Dependencies }
        "5" { Build-Project }
        "6" { Verify-Setup }
        "7" { Run-Application }
        "8" { Setup-Complete }
        "9" { 
            Write-Host ""
            Write-Success "¡Hasta luego!"
            exit
        }
        default { 
            Write-Error-Custom "Opción no válida"
        }
    }
} while ($true)

