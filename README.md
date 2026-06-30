# PROYECTO-PA-G3

Programación avanzada, proyecto grupal



### Como ejecutar el proyecto

El proyecto de Backend se conecta a una base de datos local y el API levanta en el puerto 44333. El frontend (MvcClient) se levanta en el puerto 44300



Lo único que se debe hacer para correr el proyecto es levantar los dos proyectos en dos archivos separados al mismo tiempo



Reglas

Si van a hacer un cambio hay que hacer un nuevo Branch y hacer los cambios y luego hacer el pull request 


### Requerimiento 2 – Users and Roles
Objetivo

Implementar el catálogo de Roles y Usuarios del sistema UAM Lab Help Desk, estableciendo la base para la autenticación y autorización mediante JWT.

Funcionalidades Implementadas
Gestión de Roles

Se desarrolló el mantenimiento completo de Roles incluyendo:

Consulta de todos los roles.
Consulta de rol por identificador.
Creación de nuevos roles.
Actualización de roles existentes.
Eliminación lógica de roles mediante el campo IsActive.
Visualización de estado Activo/Inactivo.

Los roles principales definidos son:

Administrator
Technician
Instructor


Gestión de Usuarios

Se desarrolló el mantenimiento completo de Usuarios incluyendo:

Consulta de todos los usuarios.
Consulta de usuario por identificador.
Consulta de usuarios por rol.
Creación de usuarios.
Actualización de usuarios.
Eliminación lógica mediante el campo IsActive.
Asociación obligatoria de cada usuario a un rol activo.

La información administrada incluye:

Nombre
Apellido
Correo electrónico
Rol
Estado
Contraseña
Seguridad

Se implementó autenticación basada en JWT para proteger los endpoints del API.

Todos los endpoints del módulo Roles y Users requieren autenticación mediante token válido.

Además:

Las contraseñas no se almacenan en texto plano.
Se utiliza PasswordHash para almacenamiento seguro.
PasswordHash nunca se expone en respuestas del API.
PasswordHash no se muestra en las vistas MVC.
El campo contraseña se presenta como tipo Password en Create y Edit.
La contraseña nunca se carga automáticamente al editar un usuario.
Arquitectura Implementada

Se siguió el patrón solicitado:

Model → DTO → Interface → Repository → UnitOfWork → Controller

Además:

Los DTOs de salida fueron implementados como record.
Los DTOs de entrada fueron implementados como class.
Todas las respuestas utilizan ApiOperationResultDto.
La lógica de negocio se mantiene fuera de los controladores.
Reglas de Negocio Implementadas
Roles
El nombre del rol es único.
No se permite duplicar roles.
La validación se realiza en el repositorio.
Usuarios
El correo electrónico es único.
No se permite registrar usuarios con correos duplicados.
La validación se realiza en el repositorio.
No se permite asignar usuarios a roles inactivos.
Auditoría
CreatedAtUtc se asigna automáticamente.
UpdatedAtUtc se asigna automáticamente.
Las fechas de auditoría no son enviadas por el cliente.
Eliminación Lógica

La eliminación se realiza mediante:

IsActive = false

Los registros permanecen almacenados en la base de datos y no son eliminados físicamente.

Vistas MVC Implementadas
Roles
Index
Create
Edit
Detail

Incluyen:

Tabla de consulta
Estado del registro
Acciones de mantenimiento
Formularios mediante modales
Usuarios
Index
Create
Edit
Detail

Incluyen:

Tabla de consulta
Selección de rol mediante dropdown
Visualización de estado
Formularios mediante modales
Mejoras de Interfaz

Se modernizó la interfaz MVC mediante:

Bootstrap 5
Menú lateral corporativo
Dashboard principal
Modales profesionales
Diseño responsive
Estilo visual orientado a aplicaciones empresariales Help Desk
Resultado

El módulo Users and Roles quedó completamente integrado con la API REST y la base de datos SQL Server, permitiendo administrar usuarios y roles de forma segura, siguiendo las reglas de negocio, seguridad y arquitectura definidas en el requerimiento.

# PROYECTO-PA-G3

Programación avanzada, proyecto grupal



### Como ejecutar el proyecto

El proyecto de Backend se conecta a una base de datos local y el API levanta en el puerto 44333. El frontend (MvcClient) se levanta en el puerto 44300



Lo único que se debe hacer para correr el proyecto es levantar los dos proyectos en dos archivos separados al mismo tiempo



Reglas

Si van a hacer un cambio hay que hacer un nuevo Branch y hacer los cambios y luego hacer el pull request 


### Requerimiento 2 – Users and Roles
Objetivo

Implementar el catálogo de Roles y Usuarios del sistema UAM Lab Help Desk, estableciendo la base para la autenticación y autorización mediante JWT.

Funcionalidades Implementadas
Gestión de Roles

Se desarrolló el mantenimiento completo de Roles incluyendo:

Consulta de todos los roles.
Consulta de rol por identificador.
Creación de nuevos roles.
Actualización de roles existentes.
Eliminación lógica de roles mediante el campo IsActive.
Visualización de estado Activo/Inactivo.

Los roles principales definidos son:

Administrator
Technician
Instructor


Gestión de Usuarios

Se desarrolló el mantenimiento completo de Usuarios incluyendo:

Consulta de todos los usuarios.
Consulta de usuario por identificador.
Consulta de usuarios por rol.
Creación de usuarios.
Actualización de usuarios.
Eliminación lógica mediante el campo IsActive.
Asociación obligatoria de cada usuario a un rol activo.

La información administrada incluye:

Nombre
Apellido
Correo electrónico
Rol
Estado
Contraseña
Seguridad

Se implementó autenticación basada en JWT para proteger los endpoints del API.

Todos los endpoints del módulo Roles y Users requieren autenticación mediante token válido.

Además:

Las contraseñas no se almacenan en texto plano.
Se utiliza PasswordHash para almacenamiento seguro.
PasswordHash nunca se expone en respuestas del API.
PasswordHash no se muestra en las vistas MVC.
El campo contraseña se presenta como tipo Password en Create y Edit.
La contraseña nunca se carga automáticamente al editar un usuario.
Arquitectura Implementada

Se siguió el patrón solicitado:

Model → DTO → Interface → Repository → UnitOfWork → Controller

Además:

Los DTOs de salida fueron implementados como record.
Los DTOs de entrada fueron implementados como class.
Todas las respuestas utilizan ApiOperationResultDto.
La lógica de negocio se mantiene fuera de los controladores.
Reglas de Negocio Implementadas
Roles
El nombre del rol es único.
No se permite duplicar roles.
La validación se realiza en el repositorio.
Usuarios
El correo electrónico es único.
No se permite registrar usuarios con correos duplicados.
La validación se realiza en el repositorio.
No se permite asignar usuarios a roles inactivos.
Auditoría
CreatedAtUtc se asigna automáticamente.
UpdatedAtUtc se asigna automáticamente.
Las fechas de auditoría no son enviadas por el cliente.
Eliminación Lógica

La eliminación se realiza mediante:

IsActive = false

Los registros permanecen almacenados en la base de datos y no son eliminados físicamente.

Vistas MVC Implementadas
Roles
Index
Create
Edit
Detail

Incluyen:

Tabla de consulta
Estado del registro
Acciones de mantenimiento
Formularios mediante modales
Usuarios
Index
Create
Edit
Detail

Incluyen:

Tabla de consulta
Selección de rol mediante dropdown
Visualización de estado
Formularios mediante modales
Mejoras de Interfaz

Se modernizó la interfaz MVC mediante:

Bootstrap 5
Menú lateral corporativo
Dashboard principal
Modales profesionales
Diseño responsive
Estilo visual orientado a aplicaciones empresariales Help Desk
Resultado

El módulo Users and Roles quedó completamente integrado con la API REST y la base de datos SQL Server, permitiendo administrar usuarios y roles de forma segura, siguiendo las reglas de negocio, seguridad y arquitectura definidas en el requerimiento.

## Requerimiento 3 – Authentication y JWT

Se implementó un sistema de autenticación completo para el UAM Lab Help Desk usando los usuarios y roles del Requerimiento 2.

---

### Objetivo

Reemplazar la autenticación básica por un sistema con JWT de corta duración y Refresh Token para renovación de sesión.

---

### Flujo de autenticación

* Login con Email y Password
* Validación contra la tabla `Users`
* Generación de:

  * Access Token (JWT, 60 min)
  * Refresh Token (7 días)
* Uso de JWT para endpoints protegidos
* Renovación de sesión con Refresh Token
* Logout con revocación del Refresh Token

---

### Implementación

* Endpoint `/Login` (AllowAnonymous)
* Endpoint `/RefreshToken`
* Endpoint `/Logout`
* Tokens configurados desde `appsettings.json`
* Claims del JWT: `UserId`, `Email`, `Role`

---

### Refresh Token

* Guardado en tabla `RefreshTokens`
* Expira y puede ser revocado
* Se invalida automáticamente al hacer refresh o logout
* No se puede reutilizar

---

### Seguridad

* Contraseñas con BCrypt
* Usuario inactivo no puede autenticarse
* Mensajes sin información sensible (genéricos)
* JWT firmado con HMAC SHA-256
* Validación de expiración, firma y audiencia

---

### MVC

* Login con almacenamiento de tokens en cookies HttpOnly
* Logout desde layout principal
* Renovación automática si el token expira (401 → refresh)
* Redirección a Login si falla la renovación

---

### Reglas cumplidas

* Sin strings hardcodeados (todo en `.resx`)
* Eliminación lógica (`IsActive = false`)
* Sin lógica de negocio en controladores
* Cliente MVC consume solo API

---

### Resultado

Sistema de autenticación seguro con JWT y Refresh Tokens, cumpliendo el flujo completo de login, renovación de sesión y logout según los requisitos del proyecto.


# Requerimiento 4 - Two-Factor Authentication (OTP)

## Flujo implementado

El proceso de autenticación fue ampliado para incorporar verificación en dos pasos mediante un código OTP enviado por correo electrónico.

### Pasos para probar en Swagger o Postman

1. Ejecutar la API y el cliente MVC.
2. Enviar una solicitud a:

```
POST /api/Auth/Login
```

Ejemplo:

```json
{
  "email": "usuario@correo.com",
  "password": "Password123*"
}
```

3. Si las credenciales son correctas, el sistema enviará un código OTP al correo del usuario y devolverá un `SessionToken`.

4. Verificar el código recibido utilizando:

```
POST /api/Auth/VerifyOtp
```

Ejemplo:

```json
{
  "sessionToken": "<SessionToken>",
  "code": "123456"
}
```

5. Si el código es válido, el sistema devuelve:

* AccessToken (JWT)
* RefreshToken

Estos tokens permiten acceder a los endpoints protegidos.

---

## Prueba desde el Cliente MVC

1. Iniciar sesión desde la pantalla **Login**.
2. Ingresar correo y contraseña.
3. El sistema redirige automáticamente a **VerifyOtp**.
4. Introducir el código recibido por correo.
5. Si el código es válido, el usuario ingresa al sistema.


# Requerimiento 5 - Password Recovery and Session Management

## Recuperación de contraseña

### Swagger / Postman

### 1. Solicitar recuperación

```
POST /api/Auth/ForgotPassword
```

```json
{
  "email": "usuario@correo.com"
}
```

El sistema envía un código OTP de recuperación al correo del usuario y devuelve un mensaje genérico por motivos de seguridad.

### 2. Restablecer contraseña

```
POST /api/Auth/ResetPassword
```

```json
{
  "sessionToken": "<SessionToken>",
  "code": "123456",
  "newPassword": "NuevaPassword123*",
  "confirmPassword": "NuevaPassword123*"
}
```

Si el código es válido:

* Se actualiza la contraseña.
* Se almacena utilizando hash.
* Se revocan todos los Refresh Tokens activos.

---

## Cambio de contraseña

Con un usuario autenticado ejecutar:

```
POST /api/Auth/ChangePassword
```

```json
{
  "currentPassword": "PasswordActual123*",
  "newPassword": "NuevaPassword123*",
  "confirmPassword": "NuevaPassword123*"
}
```

Si la contraseña actual es correcta:

* Se actualiza la contraseña.
* Se revocan todas las sesiones activas excepto la sesión actual.

---

## Gestión de sesiones

### Consultar sesiones activas

```
GET /api/Auth/sessions
```

Retorna la lista de Refresh Tokens activos del usuario autenticado.

### Revocar una sesión

```
POST /api/Auth/revoke-session/{id}
```

Revoca únicamente la sesión seleccionada.

### Revocar todas las sesiones

```
POST /api/Auth/RevokeAllSessions
```

Revoca todas las sesiones activas del usuario autenticado.

---

## Prueba desde el Cliente MVC

### Recuperación de contraseña

1. Abrir la vista **ForgotPassword**.
2. Ingresar el correo electrónico.
3. Revisar el correo y obtener el código OTP.
4. Abrir la vista **ResetPassword**.
5. Ingresar el código OTP y la nueva contraseña.
6. Iniciar sesión nuevamente con la contraseña actualizada.

### Cambio de contraseña

1. Iniciar sesión.
2. Abrir la vista **ChangePassword**.
3. Ingresar la contraseña actual.
4. Ingresar la nueva contraseña y confirmarla.
5. Guardar los cambios.

### Sesiones activas

1. Iniciar sesión.
2. Abrir la vista **MySessions**.
3. Visualizar las sesiones activas.
4. Utilizar **Cerrar sesión** para revocar una sesión específica.
5. Utilizar **Cerrar todas las sesiones** para revocar todas las sesiones activas.
