# Tienda UCN 

API REST  desarrollada en APS.NET 9 para la gestion de una tienda online llamada "Tienda UCN". Se permitira manejar usuarios, catalogo de los productos, autentucacion, carritode compras y tambien pedidos.

## Tecnologías utilizadas

- **Framework:** ASP.NET Core 9.0
- **Versionado:** Git + Conventional Commits
- **Base de Datos:** Sqlite
- **ORM:** Entity Framework Core

## Instalación y COnfiguracion Local

### Requisitos Previos
- **.NET 9 SDK**: [Download](https://dotnet.microsoft.com/download/dotnet/9.0)
- **Visual Studio Code**: [Download](https://code.visualstudio.com/)
- **Git**: [Download](https://git-scm.com/install/windows)

### Instalar extensiones en VSCode

- **C# Dev Kit**
- **C#**
- **.NET Install Tool**
- **C# Extensions**
- **SQLite**

### Clonar el Repositorio

Abre una terminal en el directorio que sees guardar este proyecto y ejecuta el siguiente comando:
``` bash
git code https://github.com/Moriarty626/TiendaUCN.git
```

Navega a la carpeta del proyecto clonado
``` bash
cd . \TiendaUCN\
```

Abre VsCode con el siguiente comando:
``` bash
code .
```

### 2. Cambiar la rama

Abrir la terminal en VsCode y cambiar la rama development
``` bash
git checkout development
```

### 3. Establecer las variables de entorno 

Crear el archivo **.env**, desde la terminal en VsCode ejecuta este comando:
``` bash
cp .env.example .env
```

Configurar las variables de **.env**:
``` bash
DATA_BASE_URL = Data Source=<nombreBD>.db
```

Crear el archivo **appsettings.json**:
``` bash
cp appsettings.example.json appsettings.json
```

### 4. Instalar Entity Framework
``` bash
dotnet tool install --global dotnet-ef
```

### 5. Instalar Dpendencias
``` bash
dotnet restore
```
### 6. Compilar el proyecto
``` bash
dotnet build
```
### 7. Crear Base de datos 
``` bash
dotnet ef database update
```

### 8. Ejecutar el proyecto 
``` bash
dotnet run
```
El servicio estará disponible en: http://localhost:5094

### 9. Visualizar Base de datos
Abrir opciones en VsCode:
``` bash
Shift + Ctrl + p
```
Buscar y presionar SQLite: Open DataBase

Finalmente abrir database.db

