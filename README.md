# PROYECTO-PA-G3

Programación avanzada, proyecto grupal



### Como ejecutar el proyecto

El proyecto de Backend se conecta a una base de datos local y el API levanta en el puerto 44385. El frontend (MvcClient) se levanta en el puerto 44300



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

El proyecto de Backend se conecta a una base de datos local y el API levanta en el puerto 44385. El frontend (MvcClient) se levanta en el puerto 44300



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

## Requerimiento 3 – Autenticación, Autorización y Gestión de Tokens JWT

### Objetivo

Implementar un mecanismo seguro de autenticación y autorización para el sistema UAM Lab Help Desk mediante JSON Web Tokens (JWT), incorporando además la gestión de Refresh Tokens para renovación de sesiones y revocación segura durante el cierre de sesión.

---

### Funcionalidades Implementadas

#### Autenticación de Usuarios

Se implementó un proceso de autenticación basado en credenciales almacenadas en la base de datos.

Características principales:

* Validación de usuarios registrados en la tabla `Users`.
* Verificación segura de contraseñas mediante BCrypt.
* Validación del estado activo del usuario.
* Generación de Access Token JWT.
* Generación de Refresh Token persistido en base de datos.
* Mensajes de autenticación centralizados mediante archivos de recursos (`.resx`).

---

#### Autorización Basada en JWT

El sistema protege los recursos mediante tokens JWT firmados digitalmente.

Los tokens contienen información relevante del usuario autenticado:

* Identificador del usuario.
* Correo electrónico.
* Rol asignado.

Los endpoints protegidos utilizan el atributo:

```csharp
[Authorize]
```

garantizando que únicamente usuarios autenticados puedan acceder a los recursos restringidos.

---

#### Refresh Token

Se implementó un mecanismo de renovación de sesión mediante Refresh Tokens.

Características:

* Generación automática durante el Login.
* Almacenamiento en base de datos.
* Fecha de expiración configurable.
* Validación de vigencia.
* Rotación automática de tokens.
* Revocación durante el cierre de sesión.

Esto permite mantener sesiones seguras sin obligar al usuario a autenticarse constantemente.

---

#### Logout Seguro

Se desarrolló un proceso de cierre de sesión que:

* Revoca el Refresh Token almacenado.
* Impide reutilización de tokens previamente emitidos.
* Elimina la sesión activa del cliente MVC.
* Redirige al usuario a la pantalla de autenticación.

---

### Arquitectura Implementada

Se mantuvo la arquitectura definida para el proyecto:

Model → DTO → Interface → Repository → UnitOfWork → Controller

Adicionalmente se incorporaron los siguientes componentes:

* AuthRepository
* IAuthRepository
* RefreshTokenRepository
* AuthenticationDelegatingHandler
* AuthService
* DTOs específicos para autenticación

La lógica de autenticación permanece desacoplada de los controladores siguiendo principios de responsabilidad única.

---

### Seguridad Implementada

#### Contraseñas

Las contraseñas nunca se almacenan en texto plano.

Se utiliza:

```text
BCrypt Password Hashing
```

para almacenar y validar credenciales de forma segura.

---

#### Tokens

Los JWT son emitidos utilizando:

```text
HMAC SHA-256
```

y contienen información mínima necesaria para la autorización.

Se valida:

* Firma digital.
* Emisor.
* Audiencia.
* Tiempo de expiración.

---

#### Refresh Tokens

Cada Refresh Token:

* Es único.
* Se almacena en base de datos.
* Posee fecha de expiración.
* Puede ser revocado.
* No puede reutilizarse una vez invalidado.

---

### Base de Datos

Se incorporó la entidad:

```text
RefreshToken
```

para la gestión de sesiones persistentes.

Información almacenada:

* Id
* UserId
* Token
* ExpiresAtUtc
* IsRevoked

Esta estructura permite controlar la vigencia y revocación de sesiones de manera centralizada.

---

### Integración MVC

El cliente MVC se integró completamente con la API.

Características:

* Pantalla de Login.
* Persistencia de sesión mediante Cookies Authentication.
* Almacenamiento seguro de Access Token y Refresh Token.
* Protección de vistas mediante autorización.
* Cierre de sesión integrado con la API.

---

### Casos de Prueba Realizados

#### Login Exitoso

* Usuario válido.
* Contraseña correcta.
* Generación de Access Token.
* Generación de Refresh Token.
* Acceso autorizado al sistema.

#### Login Fallido

* Credenciales inválidas.
* Mensaje genérico de error.
* Sin filtración de información sensible.

#### Renovación de Token

* Refresh Token válido.
* Emisión de nuevo Access Token.
* Emisión de nuevo Refresh Token.

#### Logout

* Revocación del Refresh Token.
* Cierre de sesión exitoso.

#### Reutilización de Token Revocado

* Intento de uso posterior al Logout.
* Acceso denegado correctamente.

---

### Resultado

El sistema de autenticación quedó completamente integrado con la arquitectura del proyecto, proporcionando un mecanismo seguro de identificación, autorización y administración de sesiones mediante JWT y Refresh Tokens.

La implementación cumple con los requisitos de seguridad, escalabilidad y separación de responsabilidades definidos para el proyecto UAM Lab Help Desk.
