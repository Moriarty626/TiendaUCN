# TiendaUCN - API REST

API REST desarrollada en ASP.NET Core 9.0 para un sistema de comercio electrónico que implementa identificación y seguridad, catálogo de productos, carrito de compras, procesamiento de órdenes e integración con servicios de terceros.

## Descripción del Proyecto

TiendaUCN es una API REST que soporta operaciones de una plataforma de tienda online. El sistema implementa autenticación JWT con tokenización segura, gestión de inventario con control de stock, integración con Cloudinary para almacenamiento de imágenes y Resend para notificaciones por correo electrónico.

### Características Principales

- Autenticación y autorización con JWT
- Validación de identidad (RUT chileno, mayoría de edad)
- Verificación de email mediante Resend
- Gestión administrativa de catálogo (CRUD, soft delete)
- Carrito de compras con validación de stock
- Procesamiento de órdenes con transacciones atómicas
- Integración Cloudinary para imágenes
- Jobs en segundo plano con Hangfire
- Logging centralizado con Serilog
- Arquitectura limpia con inyección de dependencias

## Requisitos Previos

- .NET 9.0 SDK
- SQLite (incluido)
- Postman (para pruebas)
- Variables de entorno configuradas

### Dependencias Principales

- ASP.NET Core 9.0
- Entity Framework Core 9.0
- JWT Bearer Authentication
- Hangfire 1.8.23
- Serilog
- Mapster
- BCrypt.Net-Next
- Cloudinary
- Resend API

## Instalación y Configuración

### Paso 1: Clonar el Repositorio

```bash
git clone https://github.com/tu-usuario/TiendaUCN.git
cd TiendaUCN
```

### Paso 2: Configurar Variables de Entorno

Crear archivo `.env` en la raíz del proyecto:

```
DATA_BASE_URL=Data Source=database.db
JWT_SECRET=TuClaveSeguraDeAlMenos32Caracteres123!@#
RESEND_API_KEY=re_tu_api_key_resend
```

### Paso 3: Configurar Cloudinary (Opcional)

Editar `appsettings.json` con tus credenciales:

```json
{
  "Cloudinary": {
    "CloudName": "tu-cloud-name",
    "ApiKey": "tu-api-key",
    "ApiSecret": "tu-api-secret"
  }
}
```

### Paso 4: Restaurar Dependencias

```powershell
dotnet restore
```

### Paso 5: Crear Base de Datos

```powershell
dotnet ef database update
```

### Paso 6: Ejecutar la Aplicación

```powershell
dotnet run
```

La API estará disponible en `http://localhost:5094`

## Configuración Automática

Alternativamente, ejecutar el script de configuración:

```powershell
.\setup.ps1
```

Seleccionar opción 8 "Configuración Completa (TODO)" para automatizar todos los pasos.

## Estructura del Proyecto

```
TiendaUCN/
├── src/
│   ├── API/
│   │   ├── Controllers/              # Endpoints REST
│   │   ├── Middlewares/              # Excepciones y blacklist
│   │   └── Program.cs
│   ├── Application/
│   │   ├── DTOs/                      # Data Transfer Objects
│   │   ├── Services/                  # Lógica de negocio
│   │   ├── Mappers/                   # Mapeo de datos
│   │   └── Validators/                # Validaciones
│   ├── Domain/
│   │   └── Models/                    # Entidades
│   └── Infrastructure/
│       ├── Data/
│       │   ├── DataContext.cs         # EF Core context
│       │   ├── Migrations/            # Migraciones
│       │   ├── Repository/            # Repositorios
│       │   └── DataSeeder.cs          # Datos de prueba
│       └── Services/
├── postman/
│   └── collections/
│       └── TiendaUCN_Testing_Complete.json
├── TESTING.md                         # Documentación de pruebas
├── README.md                          # Este archivo
├── setup.ps1                          # Script de configuración
├── Program.cs
├── appsettings.json
├── appsettings.example.json
└── TiendaUCN.csproj
```

## API Endpoints

### Autenticación

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| POST | `/api/auth/register` | Registro de usuario |
| POST | `/api/auth/login` | Inicio de sesión |
| POST | `/api/auth/logout` | Cierre de sesión |
| POST | `/api/auth/verify-email` | Verificación de email |
| GET | `/api/auth/profile` | Perfil del usuario autenticado |

### Productos

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/api/products` | Listar productos (paginado, filtrable) |
| GET | `/api/products/{id}` | Obtener detalle de producto |
| POST | `/api/products` | Crear producto (admin) |
| PUT | `/api/products/{id}` | Actualizar producto (admin) |
| PATCH | `/api/products/{id}/switch-status` | Cambiar estado (admin) |
| DELETE | `/api/products/{id}` | Eliminar producto (admin) |

### Carrito

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/api/cart` | Obtener carrito del usuario |
| POST | `/api/cart/items` | Agregar producto al carrito |
| PUT | `/api/cart/items/{id}` | Actualizar cantidad |
| DELETE | `/api/cart/items/{id}` | Eliminar item del carrito |
| POST | `/api/cart/checkout` | Procesar compra |

### Órdenes

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/api/orders` | Listar órdenes del usuario |
| GET | `/api/orders/{id}` | Obtener detalle de orden |
| GET | `/api/orders/stats` | Estadísticas (admin) |

### Categorías y Marcas

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/api/categories` | Listar categorías |
| POST | `/api/categories` | Crear categoría (admin) |
| GET | `/api/brands` | Listar marcas |
| POST | `/api/brands` | Crear marca (admin) |

## Documentación de Pruebas

Consulta `TESTING.md` para:

- Importación de colección Postman
- Descripción de 27 casos de prueba
- Mapeo de pruebas a requisitos
- Secuencia recomendada para presentación

Resumen rápido:

```powershell
# 1. Iniciar API
dotnet run

# 2. Importar en Postman
postman/collections/TiendaUCN_Testing_Complete.json

# 3. Ejecutar colección
Run collection > TiendaUCN_Testing_Complete
```

## Tecnologías Utilizadas

### Backend

- **Framework:** ASP.NET Core 9.0
- **ORM:** Entity Framework Core 9.0
- **Base de Datos:** SQLite
- **Autenticación:** JWT (JSON Web Tokens)
- **Mapeo:** Mapster 10.0.7
- **Encriptación:** BCrypt.Net-Next 4.1.0
- **Jobs:** Hangfire 1.8.23
- **Logging:** Serilog
- **Email:** Resend API
- **Almacenamiento:** Cloudinary
- **Data Seeding:** Bogus 35.6.5

### Herramientas

- **Version Control:** Git
- **Commits:** Conventional Commits
- **Testing:** Postman
- **Hooks:** Husky (pre-commit validation)

## Configuración de Git

El proyecto utiliza GitHub Flow:

### Crear Feature

```bash
git checkout development
git pull origin development
git checkout -b feature/descripcion-breve
```

### Hacer Commit

```bash
git add .
git commit -m "tipo: descripcion

- Cambio 1
- Cambio 2

Relacionado con: Taller 1"
```

**Tipos válidos:** feat, fix, docs, style, refactor, perf, test, chore

### Push y Pull Request

```bash
git push -u origin feature/descripcion-breve
# Crear PR en GitHub UI
# En GitHub: Merge pull request > Squash and merge
```

## Variables de Entorno

| Variable | Descripción | Ejemplo |
|----------|-------------|---------|
| `DATA_BASE_URL` | Connection string SQL | `Data Source=database.db` |
| `JWT_SECRET` | Clave para firmar JWT (mín. 32 chars) | `MySecureKeyAtLeast32Characters!` |
| `RESEND_API_KEY` | API key de Resend para emails | `re_xxxxxxxxxxxxx` |
| `Cloudinary:CloudName` | Nombre de cloud Cloudinary | `tu-cloud-name` |
| `Cloudinary:ApiKey` | API key Cloudinary | `xxxxxxxxxxxxx` |
| `Cloudinary:ApiSecret` | API secret Cloudinary | `xxxxxxxxxxxxx` |

## Seguridad

- Contraseñas hasheadas con BCrypt
- Tokens JWT con expiración configurada
- Blacklist de tokens al logout
- Validación de roles en endpoints administrativos
- Data Annotations para validación de entrada
- Soft delete para eliminación segura
- Logging de eventos críticos
- Variables de entorno para secretos

## Mantenimiento Automático

Hangfire ejecuta jobs periódicamente:

- **Limpieza de usuarios no verificados:** Diariamente a las 2:00 AM
- **Purga de blacklist de JWT:** Diariamente a las 3:00 AM

Dashboard disponible en `http://localhost:5094/hangfire`

## Logging

Serilog registra todos los eventos en:

- **Consola:** En tiempo real
- **Archivos:** `logs/myapp-YYYYMMDD.txt` (rotación diaria)
- **Eventos:** Errores, accesos denegados, transacciones críticas

## Defensa del Proyecto

Para presentación de defensa (máximo 15 minutos):

1. **Introducción (2 min):** Equipo, metodología, responsabilidades
2. **Arquitectura (2 min):** Diagrama de BD y Clases
3. **Demo en Vivo (8 min):** Ejecutar pruebas en Postman
   - Flujos de éxito
   - Flujos de error controlados
   - Validaciones de seguridad
4. **Problemas y Soluciones (2 min):** Obstáculos técnicos reales
5. **Conclusiones (1 min):** Aprendizajes clave

Referencias en TESTING.md: "Ejecución para Presentación de Defensa"

## Problemas Comunes

### La aplicación no inicia

- Verificar que puerto 5094 no está en uso
- Asegurar que .NET 9.0 está instalado: `dotnet --version`
- Restaurar dependencias: `dotnet restore`

### Base de datos no se crea

- Eliminar `database.db` si existe
- Ejecutar: `dotnet ef database update`
- Verificar que el archivo `appsettings.json` tiene la connection string correcta

### Variables de entorno no se cargan

- Crear archivo `.env` en raíz del proyecto
- Asegurar que las claves coinciden exactamente (case-sensitive)
- Reiniciar la aplicación después de crear `.env`

### Cloudinary no funciona

- Configurar credenciales en `appsettings.json`
- Usar Resend dummy si no tienes cuenta: `re_dummy_key_for_testing`

## Licencia

Este proyecto es parte del Taller 1 de la asignatura Introducción al Desarrollo Web y Móvil de la Universidad Católica del Norte.

## Contacto

Para consultas sobre el desarrollo, contactar al profesor Jorge Rivera o los ayudantes del curso.

## Historial de Cambios

Ver commits en rama `development` y `main` para historial detallado.

Última actualización: 2026-05-08

