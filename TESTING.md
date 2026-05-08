# TESTING - TiendaUCN API REST

## Descripción General

Este documento describe el conjunto completo de pruebas para validar la API REST TiendaUCN. Las pruebas están organizadas en 5 flujos principales que abarcan identidad, catálogo, carrito, transacciones y manejo de errores, con un total de 27 casos de prueba.

## Importación de la Colección Postman

### Requisitos Previos

Descargar Postman desde https://www.postman.com/downloads/

### Pasos de Importación

1. Abrir Postman
2. Hacer clic en "Import" (botón superior izquierdo)
3. Seleccionar "Upload Files"
4. Navegar a: `postman/collections/TiendaUCN_Testing_Complete.json`
5. Hacer clic en "Import"
6. La colección aparecerá en el panel izquierdo bajo "Collections"

### Configuración de Variables

Las variables se capturan automáticamente mediante scripts en las pruebas:

- `token_admin`: JWT del usuario administrador
- `token_customer`: JWT del usuario cliente
- `product_id`: ID del producto creado
- `order_id`: ID de la orden generada
- `cart_id`: ID del carrito

No es necesario configurar variables manualmente. Los scripts de las pruebas las capturan automáticamente.

## Ejecución de las Pruebas

### Prerrequisitos

La API debe estar ejecutándose en `http://localhost:5094`

```powershell
# En la carpeta del proyecto
dotnet run
```

Esperar a que aparezca: "Now listening on: http://localhost:5094"

### Ejecución Individual

1. Expandir la colección "TiendaUCN_Testing_Complete"
2. Hacer clic derecho en una prueba
3. Seleccionar "Send"
4. Verificar el status code y el body de la respuesta

### Ejecución de Toda la Colección

1. Hacer clic en los tres puntos (...) junto al nombre de la colección
2. Seleccionar "Run collection"
3. Configurar valores si es necesario
4. Hacer clic en "Run TiendaUCN_Testing_Complete"

## Flujos de Prueba y Mapeo de Requisitos

### Flujo 1: Identidad y Seguridad (8 pruebas - 55 puntos)

#### 1.1 Registro - Usuario Válido

**Endpoint:** POST /api/auth/register

**Body:**
```json
{
  "firstName": "Juan",
  "lastName": "Pérez",
  "email": "juan@example.com",
  "password": "SecurePass123!",
  "rut": "12345678-9",
  "phone": "+56912345678",
  "birthDate": "2000-01-15"
}
```

**Validaciones:**
- Status: 201 Created
- Response contiene id del usuario
- El usuario no se encuentra verificado (isVerified: false)

**Requisitos Cumplidos:**
- Validación de RUT chileno
- Validación de mayoría de edad (18+)
- Creación de usuario en BD

#### 1.2 Registro - RUT Inválido

**Endpoint:** POST /api/auth/register

**Body:**
```json
{
  "firstName": "Pedro",
  "lastName": "García",
  "email": "pedro@example.com",
  "password": "SecurePass123!",
  "rut": "INVALID-RUT",
  "phone": "+56912345678",
  "birthDate": "2000-01-15"
}
```

**Validaciones:**
- Status: 400 Bad Request
- Message: "RUT inválido"

**Requisitos Cumplidos:**
- Validación estricta de formato RUT
- Manejo de errores controlado

#### 1.3 Registro - Menor de Edad

**Endpoint:** POST /api/auth/register

**Body:**
```json
{
  "firstName": "Carlos",
  "lastName": "López",
  "email": "carlos@example.com",
  "password": "SecurePass123!",
  "rut": "99999999-9",
  "phone": "+56912345678",
  "birthDate": "2010-01-15"
}
```

**Validaciones:**
- Status: 400 Bad Request
- Message: "Debes ser mayor de 18 años"

**Requisitos Cumplidos:**
- Validación de mayoría de edad
- Cálculo correcto de edad

#### 1.4 Registro - Email Duplicado

**Endpoint:** POST /api/auth/register

**Body:**
```json
{
  "firstName": "Otro",
  "lastName": "Usuario",
  "email": "juan@example.com",
  "password": "SecurePass123!",
  "rut": "87654321-9",
  "phone": "+56912345678",
  "birthDate": "1995-01-15"
}
```

**Precondición:** El email "juan@example.com" ya existe (de prueba 1.1)

**Validaciones:**
- Status: 400 Bad Request
- Message: "El email ya está registrado"

**Requisitos Cumplidos:**
- Validación de unicidad de email en BD

#### 1.5 Verificación de Email

**Endpoint:** POST /api/auth/verify-email

**Body:**
```json
{
  "email": "juan@example.com",
  "code": "123456"
}
```

**Precondición:** El código debe ser enviado por Resend en el registro (o usar el código de test en logs)

**Validaciones:**
- Status: 200 OK
- Response: isVerified: true
- El usuario ahora puede hacer login

**Requisitos Cumplidos:**
- Integración con Resend
- Validación de código temporal
- Marcado de cuenta como verificada

#### 1.6 Login - Credenciales Válidas

**Endpoint:** POST /api/auth/login

**Body:**
```json
{
  "email": "juan@example.com",
  "password": "SecurePass123!"
}
```

**Precondición:** Prueba 1.5 completada (usuario verificado)

**Validaciones:**
- Status: 200 OK
- Response contiene: token (JWT)
- El token se captura en variable `token_customer` automáticamente

**Script de captura (incluido):**
```javascript
var jsonData = pm.response.json();
pm.collectionVariables.set("token_customer", jsonData.token);
```

**Requisitos Cumplidos:**
- Autenticación con JWT
- Token seguro de sesión

#### 1.7 Logout - Invalidación de Token

**Endpoint:** POST /api/auth/logout

**Headers:**
```
Authorization: Bearer {token_customer}
```

**Validaciones:**
- Status: 200 OK
- Message: "Sesión cerrada correctamente"
- El token se añade a la blacklist en BD

**Requisitos Cumplidos:**
- Implementación de blacklist
- Invalidación segura de tokens

#### 1.8 Acceso con Token en Blacklist

**Endpoint:** GET /api/auth/profile

**Headers:**
```
Authorization: Bearer {token_que_acaba_de_ser_invalidado}
```

**Precondición:** Prueba 1.7 completada (token en blacklist)

**Validaciones:**
- Status: 401 Unauthorized
- Message: "Token inválido o expirado"
- El middleware rechaza el token

**Requisitos Cumplidos:**
- Validación de blacklist en cada petición
- Seguridad de logout efectivo

### Flujo 2: Gestión de Catálogo y Productos (6 pruebas - 30 puntos)

#### 2.1 Crear Producto (Admin)

**Endpoint:** POST /api/products

**Headers:**
```
Authorization: Bearer {token_admin}
Content-Type: multipart/form-data
```

**Form Data:**
```
name: "Laptop Gaming"
description: "Laptop de alto rendimiento"
price: 1500.00
stock: 10
categoryId: 1
brandId: 1
image: [archivo.jpg]
```

**Validaciones:**
- Status: 201 Created
- Response contiene: productId
- La imagen se sube a Cloudinary
- El producto se crea en BD

**Requisitos Cumplidos:**
- CRUD administrativo funcional
- Integración con Cloudinary
- Upload de archivos multipart

#### 2.2 Actualizar Producto (Admin)

**Endpoint:** PUT /api/products/{productId}

**Headers:**
```
Authorization: Bearer {token_admin}
Content-Type: application/json
```

**Body:**
```json
{
  "name": "Laptop Gaming PRO",
  "description": "Laptop actualizada",
  "price": 1800.00,
  "stock": 15
}
```

**Validaciones:**
- Status: 200 OK
- Los cambios se reflejan en BD

**Requisitos Cumplidos:**
- Actualización de productos
- Control de acceso por rol Admin

#### 2.3 Cambiar Estado de Producto

**Endpoint:** PATCH /api/products/{productId}/switch-status

**Headers:**
```
Authorization: Bearer {token_admin}
```

**Body:**
```json
{
  "isActive": false
}
```

**Validaciones:**
- Status: 200 OK
- Product.isActive se actualiza en BD

**Requisitos Cumplidos:**
- Control de estado de productos

#### 2.4 Eliminar Producto (Soft Delete)

**Endpoint:** DELETE /api/products/{productId}

**Headers:**
```
Authorization: Bearer {token_admin}
```

**Validaciones:**
- Status: 204 No Content
- El producto sigue en BD pero deletedAt contiene timestamp
- El producto no aparece en listados públicos

**Requisitos Cumplidos:**
- Soft delete implementado
- Eliminación lógica sin borrar físicamente

#### 2.5 Listar Productos Públicos con Paginación

**Endpoint:** GET /api/products?pageNumber=1&pageSize=10

**Validaciones:**
- Status: 200 OK
- Response contiene: items[], totalCount, pageNumber, pageSize
- Solo productos activos y no eliminados aparecen
- La paginación funciona correctamente

**Requisitos Cumplidos:**
- Visualización pública de catálogo
- Paginación funcional

#### 2.6 Buscar Productos con Filtros

**Endpoint:** GET /api/products?searchTerm=laptop&categoryId=1&brandId=1&pageNumber=1&pageSize=10

**Validaciones:**
- Status: 200 OK
- Los resultados coinciden con los filtros aplicados
- La búsqueda textual funciona

**Requisitos Cumplidos:**
- Búsqueda dinámica funcional
- Filtros combinables

### Flujo 3: Carrito de Compras y Checkout (6 pruebas - 20 puntos)

#### 3.1 Agregar Producto al Carrito

**Endpoint:** POST /api/cart/items

**Headers:**
```
Authorization: Bearer {token_customer}
```

**Body:**
```json
{
  "productId": 1,
  "quantity": 2
}
```

**Validaciones:**
- Status: 201 Created
- CartItem se crea en BD
- Quantity se establece correctamente

**Requisitos Cumplidos:**
- Gestión de carrito
- Asociación con usuario autenticado

#### 3.2 Ver Carrito

**Endpoint:** GET /api/cart

**Headers:**
```
Authorization: Bearer {token_customer}
```

**Validaciones:**
- Status: 200 OK
- Response contiene: items[], totalPrice, totalItems
- El totalPrice se calcula correctamente desde BD

**Requisitos Cumplidos:**
- Consulta de carrito
- Cálculo de totales desde BD

#### 3.3 Actualizar Cantidad en Carrito

**Endpoint:** PUT /api/cart/items/{cartItemId}

**Headers:**
```
Authorization: Bearer {token_customer}
```

**Body:**
```json
{
  "quantity": 5
}
```

**Validaciones:**
- Status: 200 OK
- La quantity se actualiza en BD
- El totalPrice se recalcula

**Requisitos Cumplidos:**
- Actualización dinámica de carrito

#### 3.4 Validación de Stock en Checkout

**Endpoint:** POST /api/cart/checkout

**Headers:**
```
Authorization: Bearer {token_customer}
```

**Precondición:** El carrito contiene un producto con cantidad que excede el stock disponible

**Validaciones:**
- Status: 400 Bad Request
- Message: "Stock insuficiente para producto X. Disponible: Y"

**Requisitos Cumplidos:**
- Validación de disponibilidad
- Manejo de errores en transacción

#### 3.5 Checkout Exitoso

**Endpoint:** POST /api/cart/checkout

**Headers:**
```
Authorization: Bearer {token_customer}
Content-Type: application/json
```

**Body:**
```json
{
  "shippingAddress": "Calle Principal 123",
  "city": "Santiago",
  "country": "Chile"
}
```

**Precondición:** El carrito tiene items con stock disponible

**Validaciones:**
- Status: 201 Created
- Response contiene: orderId
- El stock se descuenta en BD
- El carrito se vacía
- orderId se captura en variable

**Requisitos Cumplidos:**
- Transacción atómica
- Atomicidad y descuento de stock
- Limpieza de carrito

#### 3.6 Eliminar Item del Carrito

**Endpoint:** DELETE /api/cart/items/{cartItemId}

**Headers:**
```
Authorization: Bearer {token_customer}
```

**Validaciones:**
- Status: 204 No Content
- CartItem se elimina de BD
- totalPrice se recalcula

**Requisitos Cumplidos:**
- Gestión completa de carrito

### Flujo 4: Transacciones y Pedidos (4 pruebas - 20 puntos)

#### 4.1 Obtener Historial de Órdenes

**Endpoint:** GET /api/orders

**Headers:**
```
Authorization: Bearer {token_customer}
```

**Validaciones:**
- Status: 200 OK
- Response contiene: orders[]
- Solo órdenes del usuario autenticado aparecen
- Cada orden contiene: orderId, orderDate, totalPrice, status

**Requisitos Cumplidos:**
- Historial de pedidos accesible
- Aislamiento de datos por usuario

#### 4.2 Obtener Detalle de Orden

**Endpoint:** GET /api/orders/{orderId}

**Headers:**
```
Authorization: Bearer {token_customer}
```

**Precondición:** Prueba 3.5 completada (orden existe)

**Validaciones:**
- Status: 200 OK
- Response contiene: orderDetails[] con productName, productPrice, quantity
- El productPrice es el precio capturado en el momento de la compra (estático)
- No es el precio actual del catálogo

**Requisitos Cumplidos:**
- Detalle estático de pedido
- Registro histórico de precios

#### 4.3 Validación de Acceso a Orden Ajena

**Endpoint:** GET /api/orders/{orderId_de_otro_usuario}

**Headers:**
```
Authorization: Bearer {token_customer}
```

**Validaciones:**
- Status: 403 Forbidden
- Message: "No tienes permiso para acceder a esta orden"

**Requisitos Cumplidos:**
- Validación de autorización
- Seguridad de datos

#### 4.4 Estadísticas de Órdenes (Admin)

**Endpoint:** GET /api/orders/stats

**Headers:**
```
Authorization: Bearer {token_admin}
```

**Validaciones:**
- Status: 200 OK
- Response contiene: totalOrders, totalRevenue, averageOrderValue
- Los datos se calculan desde BD

**Requisitos Cumplidos:**
- Endpoint administrativo funcional

### Flujo 5: Pruebas de Error Controladas (6 pruebas - Middleware)

#### 5.1 Compra sin Stock

**Endpoint:** POST /api/cart/checkout

**Precondición:** El carrito contiene producto con stock = 0

**Validaciones:**
- Status: 400 Bad Request
- Message: "Stock insuficiente"
- El middleware de excepciones captura el error

**Requisitos Cumplidos:**
- Manejo de error controlado
- Validación de transacción

#### 5.2 Acceso Admin sin Rol

**Endpoint:** POST /api/products (crear producto)

**Headers:**
```
Authorization: Bearer {token_customer}
```

**Validaciones:**
- Status: 403 Forbidden
- Message: "Acceso denegado. Se requiere rol de administrador"
- El middleware de autorización rechaza la petición

**Requisitos Cumplidos:**
- Control de acceso por rol
- Restricción de endpoints administrativos

#### 5.3 JWT Inválido

**Endpoint:** GET /api/cart

**Headers:**
```
Authorization: Bearer invalid_token_xyz
```

**Validaciones:**
- Status: 401 Unauthorized
- Message: "Token inválido"
- El middleware valida la firma del JWT

**Requisitos Cumplidos:**
- Validación de JWT
- Seguridad de endpoints protegidos

#### 5.4 Datos Incompletos en Registro

**Endpoint:** POST /api/auth/register

**Body:**
```json
{
  "firstName": "Juan",
  "email": "juan@example.com"
}
```

**Validaciones:**
- Status: 400 Bad Request
- Message: "Campos requeridos faltantes: lastName, password, rut, phone, birthDate"
- Data Annotations validan los campos

**Requisitos Cumplidos:**
- Validación de Data Annotations
- Requerimiento de campos obligatorios

#### 5.5 Email Inválido en Registro

**Endpoint:** POST /api/auth/register

**Body:**
```json
{
  "firstName": "Juan",
  "lastName": "Pérez",
  "email": "invalid-email",
  "password": "SecurePass123!",
  "rut": "12345678-9",
  "phone": "+56912345678",
  "birthDate": "2000-01-15"
}
```

**Validaciones:**
- Status: 400 Bad Request
- Message: "Email inválido"

**Requisitos Cumplidos:**
- Validación de formato email
- Data Annotations email format

#### 5.6 Contraseña Débil

**Endpoint:** POST /api/auth/register

**Body:**
```json
{
  "firstName": "Juan",
  "lastName": "Pérez",
  "email": "juan@example.com",
  "password": "123",
  "rut": "12345678-9",
  "phone": "+56912345678",
  "birthDate": "2000-01-15"
}
```

**Validaciones:**
- Status: 400 Bad Request
- Message: "La contraseña debe contener al menos 8 caracteres, una mayúscula, un número y un carácter especial"

**Requisitos Cumplidos:**
- Validación de seguridad de contraseña
- Requerimientos de complejidad

## Tabla de Mapeo: Pruebas vs Requisitos de Pauta

| Número | Prueba | Requisito | Puntaje |
|--------|--------|-----------|---------|
| 1.1 | Registro Usuario Válido | Validación RUT + Mayoría de edad | 5 |
| 1.2-1.4 | Errores de Registro | Validaciones Data Annotations | 5 |
| 1.5 | Verificación Email | Integración Resend | 10 |
| 1.6 | Login JWT | Seguridad JWT | 10 |
| 1.7-1.8 | Logout + Blacklist | Cierre de sesión seguro | 5 |
| 2.1-2.4 | CRUD + Soft Delete | Operaciones Admin | 10 |
| 2.5-2.6 | Búsqueda y Paginación | Visualización Pública | 10 |
| 3.1-3.6 | Carrito y Validaciones | Lógica del Carrito | 10 |
| 3.5 | Checkout Exitoso | Atomicidad y Stock | 5 |
| 4.1-4.2 | Órdenes e Historial | Orden y Limpieza | 10 |
| 5.1-5.6 | Manejo de Errores | Control de Excepciones | 5 |

## Ejecución para Presentación de Defensa

### Duración Recomendada: 8 minutos

### Secuencia Sugerida

1. Registrar usuario (1.1)
2. Verificar email (1.5)
3. Login (1.6)
4. Ver catálogo de productos (2.5)
5. Agregar producto a carrito (3.1)
6. Ver carrito (3.2)
7. Checkout (3.5)
8. Ver historial de órdenes (4.1)
9. Intentar compra sin stock (5.1)
10. Intentar acceso admin sin rol (5.2)
11. Logout (1.7)
12. Intentar acceso con token en blacklist (1.8)

Esta secuencia demuestra:
- Flujo de éxito completo (registro hasta compra)
- Flujos de error controlados
- Validaciones y seguridad
- Manejo del middleware de excepciones

