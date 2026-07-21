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

## Requerimiento 4 – Two-Factor Authentication con OTP

Se agregó verificación en dos pasos al flujo de autenticación implementado en el Requerimiento 3. Después de ingresar credenciales válidas, el sistema genera un código OTP de 6 dígitos, lo envía al correo del usuario y espera su confirmación antes de emitir el JWT y el RefreshToken.

---

### Objetivo

Implementar un segundo factor de autenticación mediante un código numérico temporal enviado por correo electrónico para robustecer la seguridad del acceso.

---

### Flujo de Autenticación Modificado

1. **Paso 1: Credenciales**: El usuario ingresa Email y Password en el formulario de Login.
2. **Generación de OTP y SessionToken**: Si las credenciales son válidas, se genera:
   - Un código OTP seguro de 6 dígitos numéricos usando `RandomNumberGenerator`.
   - Un `SessionToken` (GUID generado en el servidor, con duración de 10 minutos y sin información del usuario).
   - Se invalidan intentos previos de OTP/SessionToken pendientes.
3. **Envío por SMTP**: El OTP se envía al correo del usuario vía SMTP usando las credenciales configuradas en `appsettings.json`. Si el envío falla, no se completa el inicio de sesión.
4. **Paso 2: Verificación**: El cliente MVC almacena el `SessionToken` de forma segura en la sesión del servidor (no en campos visibles) y redirige a la pantalla de verificación (`VerifyOtp`).
5. **Validación del OTP**: El usuario introduce el código OTP. El API valida:
   - Que el `SessionToken` sea válido y corresponda al OTP.
   - Que el OTP no haya expirado (10 minutos), no haya sido usado previamente y coincida con el registro.
6. **Emisión de Tokens**: Si la verificación es exitosa, se marca el OTP como usado (`IsUsed = true`), se invalida el `SessionToken`, y se retornan el `AccessToken` (JWT) y el `RefreshToken` (que el cliente MVC almacena en cookies HttpOnly).

---

### Pasos para probar el requerimiento

#### 1. Configurar Base de Datos
Asegúrese de ejecutar las migraciones de EF Core para crear la tabla `OtpCodes`:
```bash
dotnet ef database update --project Api --startup-project Api --no-restore
```

#### 2. Configurar SMTP en `Api/appsettings.json`
Modifique el bloque `Smtp` en el archivo de configuración del API con sus credenciales SMTP válidas:
```json
  "Smtp": {
    "Host": "smtp.gmail.com",
    "Port": 587,
    "SenderEmail": "su-correo@gmail.com",
    "SenderName": "UAM Lab Help Desk Support",
    "Password": "su-contraseña-de-aplicación"
  },
  "OtpExpirationMinutes": 10,
  "SessionTokenExpirationMinutes": 10
```

#### 3. Ejecutar Proyectos
Inicie simultáneamente el API y el cliente MVC:
- **API**: Puerto `44333`
- **MVC Client**: Puerto `44300`

#### 4. Probar Flujo en la Interfaz
- Ingrese credenciales de un usuario existente.
- Verifique que sea redirigido a `/Account/VerifyOtp` y que reciba el correo electrónico con el código de 6 dígitos.
- Verifique que un OTP incorrecto o vencido retorne un mensaje de error correspondiente.
- Ingrese el código correcto y confirme el acceso exitoso al sistema.

## Requerimiento 6 – Ciclo de Vida de Reportes de Avería (Asignación y Estado)

Se completó el ciclo de vida de un reporte de avería agregando el flujo de asignación y cambio de estado, con un historial auditable de cada transición de estado en la base de datos local.

---

### Objetivo

Permitir que un técnico de laboratorio se asigne reportes pendientes (`Pending` → `InProgress`), atienda la avería y actualice su estado a resuelto (`InProgress` → `Resolved`), manteniendo un registro detallado e inmutable del historial de cambios (auditoría).

---

### Funcionalidades Implementadas

1. **Flujo de Asignación**: Un usuario con el rol `Technician` puede asignarse un reporte de avería en estado `Pending`. El reporte pasa automáticamente a `InProgress`, registrando su `AssignedToUserId` y guardando el log histórico correspondiente.
2. **Actualización de Estado**: El técnico asignado puede actualizar el estado del reporte de `InProgress` a `Resolved`. Esta acción es exclusiva para el técnico asignado, requiere ingresar notas de resolución obligatorias en la vista de cliente, y guarda un log detallado.
3. **Auditoría de Estados**: La tabla `FaultReportStatusLogs` almacena:
   - Identificador del reporte (`FaultReportId`)
   - Técnico que realizó el cambio (`ChangedByUserId`)
   - Estado anterior (`PreviousStatus`)
   - Estado nuevo (`NewStatus`)
   - Notas de trabajo (`Notes`)
   - Fecha de cambio (`ChangedAtUtc`)
4. **Inmutabilidad y Restricciones**:
   - Un reporte en estado `Closed` es inmutable y no acepta más cambios.
   - Las transiciones de estado no permitidas (ej. `Pending` a `Resolved` directamente) son rechazadas por el Web API con `Success: false`.

---

### Pasos para probar el requerimiento

#### 1. Iniciar Sesión con Técnico
- Ingrese al portal MVC con credenciales de un técnico de laboratorio:
  - **Correo**: `karinabril19@gmail.com`
  - **Contraseña**: `Admin123!`

#### 2. Asignar un Reporte
- Vaya a la sección **Reportes de Averías**.
- En la tabla de reportes, identifique uno en estado **Pendiente** y haga clic en el botón **Asignar**.
- El reporte se actualizará a **En Progreso** y se mostrará su nombre en la columna "Técnico Asignado".

#### 3. Ver Detalle e Historial
- Haga clic en el botón **Ver** para entrar al detalle del reporte asignado.
- Observe que en la sección inferior aparece la tabla **Historial de Cambios de Estado** mostrando el primer log (la asignación automática).

#### 4. Resolver Avería
- Haga clic en el botón **Actualizar Estado** (este botón solo se muestra para el técnico asignado al reporte).
- Complete el campo **Notas de la Resolución** describiendo las acciones tomadas y haga clic en **Resolver Reporte**.
- Será redirigido al detalle, donde verá el estado actualizado a **Resuelto** y el nuevo registro de log de auditoría reflejando sus notas y fecha exacta.

## Requerimiento 7 – Pruebas Unitarias y CI Pipeline con GitHub Actions

Se implementó un suite completo de pruebas unitarias desacoplado de la base de datos física mediante **xUnit**, **Moq** y **EF Core InMemory**, junto con un pipeline automatizado en **GitHub Actions**.

---

### Estructura de Pruebas Unitarias

El proyecto de pruebas está ubicado en `Tests/Uam.LabHelpDesk.Tests/Uam.LabHelpDesk.Tests.csproj` e incluye **16 pruebas unitarias** distribuidas en 5 módulos clave:

1. **Módulo de Reportes de Avería (`FaultReportTests.cs`)**:
   - `CreateFaultReport_ShouldFail_WhenEquipmentIsUnderRepair`: Impide crear reportes para equipos en estado `UnderRepair`.
   - `UpdateFaultReportStatus_ShouldFail_WhenReportIsClosed`: Impide modificar reportes en estado `Closed`.
   - `UpdateFaultReportStatus_ShouldFail_WhenTransitionIsInvalid`: Rechaza transiciones no válidas.
   - `AssignFaultReport_ShouldSucceed_WhenReportIsPending`: Asigna el técnico y cambia estado a `InProgress`.
   - `CloseFaultReport_ShouldFail_WhenReportIsNotInResolvedState`: Exige que el reporte esté `Resolved` antes de cerrar.

2. **Módulo de Usuarios (`UserTests.cs`)**:
   - `CreateUser_ShouldFail_WhenEmailAlreadyExists`: Valida unicidad de correo electrónico (case-insensitive).
   - `CreateUser_ShouldFail_WhenRoleIdIsInactiveOrNotFound`: Rechaza asignación a roles inactivos.
   - `GetUserById_ShouldReturnUserDto_WhenUserExists`: Verifica mapeo correcto de datos de usuario.

3. **Módulo de Equipos (`EquipmentTests.cs`)**:
   - `CreateEquipment_ShouldFail_WhenCodeAlreadyExists`: Valida unicidad del código de equipo.
   - `CreateEquipment_ShouldFail_WhenLaboratoryDoesNotExist`: Valida existencia de laboratorio foráneo.
   - `DeleteEquipment_ShouldPerformLogicalDelete`: Confirma baja lógica (`IsActive = false`).

4. **Módulo de Roles (`RoleTests.cs`)**:
   - `CreateRole_ShouldFail_WhenRoleNameAlreadyExists`: Valida unicidad del nombre del rol.
   - `DeleteRole_ShouldPerformLogicalDelete_SettingIsActiveFalse`: Confirma baja lógica del rol.

5. **Módulo de Autenticación y Notificaciones (`AuthAndNotificationTests.cs`)**:
   - `LoginAsync_ShouldInvokeSmtpService_WhenCredentialsAreValid`: Verifica con **Moq** la invocación de `ISmtpService` al generar OTP sin enviar correos reales.
   - `LoginAsync_ShouldNotInvokeSmtpService_WhenPasswordIsInvalid`: Confirma que NO se invoque el servicio con credenciales inválidas.
   - `SmtpService_SendEmailAsync_ShouldBypass_InDevelopmentEnvironment`: Valida el bypass seguro en modo desarrollo.

---

### Instrucciones para Ejecutar las Pruebas Localmente

Para ejecutar la suite de pruebas unitarias desde la terminal en el directorio raíz de la solución:

```bash
dotnet test
```

O ejecutando el proyecto de pruebas directamente:

```bash
dotnet test Tests/Uam.LabHelpDesk.Tests/Uam.LabHelpDesk.Tests.csproj
```

**Resultado esperado:**
```
Pruebas totales: 25
     Correcto: 25
 Tiempo total: ~5.8 Segundos
```

---

### Integración Continua (GitHub Actions)

El workflow se encuentra configurado en `.github/workflows/ci.yml` y se ejecuta automáticamente en cada `push` y `pull_request` hacia la rama `main`.

**Pasos del Pipeline:**
1. Checkout del código fuente (`actions/checkout@v4`).
2. Configuración de .NET SDK 10.0 (`actions/setup-dotnet@v4`).
3. Restauración de dependencias (`dotnet restore`).
4. Compilación en modo Release (`dotnet build --no-restore`).
5. Ejecución de pruebas unitarias (`dotnet test --no-build`).

**Comportamiento en Caso de Fallo:**
Si alguna prueba unitaria falla, `dotnet test` devuelve un código de salida distinto de cero (Exit Code 1), provocando que el job de GitHub Actions detenga el pipeline y se marque en rojo **FAILED**, bloqueando la integración de código defectuoso a `main`. Al corregir la aserción o lógica, el pipeline vuelve a estar en verde **PASSED**.

## Requerimiento 8 – Notificaciones por Correo Electrónico del Ciclo de Vida de Averías

Se implementó un sistema de notificaciones automáticas por correo electrónico desacoplado y localizado que informa a técnicos e instructores sobre los eventos clave en el ciclo de vida de los reportes de averías.

---

### Eventos Notificados

1. **Reporte Creado (`Created`)**: Se envía una notificación a todos los usuarios activos con rol `Technician`.
2. **Reporte Asignado (`Assigned`)**: Se notifica al técnico asignado cuando se vincula a la avería (Estado `InProgress`).
3. **Estado Cambiado a Resuelto (`Resolved`)**: Se notifica al instructor que creó el reporte cuando la avería ha sido resuelta.
4. **Reporte Cerrado (`Closed`)**: Se notifica al instructor que creó el reporte cuando la avería se cierra definitivamente.

---

### Arquitectura y Reglas de Negocio

- **Servicio `IEmailNotificationService`**: Registrado mediante Inyección de Dependencias en `Program.cs` con ámbito `Scoped`.
- **Desacoplamiento y Resiliencia**: Los correos se envían de forma asíncrona tras confirmar la transacción en la base de datos. Si el servidor SMTP falla, la operación principal no se revierte y el error se registra mediante `ILogger`.
- **Cero Strings Hardcodeados**: Asuntos y cuerpos HTML localizados en archivos de recursos `.resx` (`EmailNotificationService.resx` / `.en.resx`).
- **Filtro de Inactividad**: Los usuarios o técnicos marcados como inactivos (`IsActive = false`) son omitidos automáticamente.

---

### Pasos para Probar el Envío de Correos

#### 1. Configurar Credenciales SMTP en `Api/appsettings.json`
Modifique las variables en la sección `Smtp`:
```json
"Smtp": {
  "Host": "smtp.gmail.com",
  "Port": 587,
  "SenderEmail": "su-correo@gmail.com",
  "SenderName": "UAM Lab Help Desk Support",
  "Password": "su-contraseña-de-aplicación"
}
```
*Nota: Si la contraseña permanece como `"pass"`, el sistema funciona en modo desarrollo haciendo un bypass seguro que imprime los registros en la consola sin enviar correos reales.*

#### 2. Probar el Flujo
1. **Creación**: Ingrese como Instructor y cree un reporte de avería. Los técnicos activos recibirán la alerta de nuevo reporte.
2. **Asignación**: Ingrese como Técnico (`karinabril19@gmail.com`) y presione **Asignar**. El técnico recibirá la confirmación de asignación.
3. **Resolución**: Presione **Actualizar Estado**, ingrese las notas y resuelva el reporte. El instructor creador recibirá el correo con el estado **Resolved**.
4. **Cierre**: Cierre el reporte. El instructor creador recibirá la notificación de reporte **Closed**.

## Requerimiento 9 – Dashboard de Métricas Operativas para Administradores

Se implementó el módulo de Dashboard de Métricas Operativas de acceso exclusivo para usuarios con rol `Administrator`. Permite visualizar métricas consolidadas del parque tecnológico, reportes por laboratorio, carga de trabajo por técnico e indicadores de tiempo de resolución.

---

### Endpoints API Registrados

- `GET /api/Dashboard/GeneralSummary`: Retorna el conteo total de reportes, desglose por estado (`Pending`, `InProgress`, `Resolved`, `Closed`), total de equipos y equipos en reparación (`UnderRepair`).
- `GET /api/Dashboard/ReportsByLab`: Agrupa y calcula la distribución de reportes y sus estados por cada laboratorio activo.
- `GET /api/Dashboard/ReportsByStatus`: Agrupa el número total de reportes por cada estado del ciclo de vida.
- `GET /api/Dashboard/ReportsByTechnician`: Muestra la carga de trabajo actual (reportes asignados en `InProgress`) e histórico de reportes resueltos por técnico de laboratorio.
- `GET /api/Dashboard/AverageResolutionTime`: Calcula el tiempo promedio, resolución más rápida y más lenta (en horas) basándose en la diferencia entre `ReportedAtUtc` y el log con `NewStatus = "Resolved"`.

---

### Arquitectura y Reglas de Negocio

- **Repositorio Dedicado `IDashboardRepository`**: Separado completamente de los repositorios de entidades individuales.
- **Consultas Agregadas en Base de Datos**: Todas las agrupaciones y conteos se ejecutan mediante LINQ/EF Core directamente en SQL sin iterar listas en memoria.
- **Seguridad basada en Roles**: Restringido con `[Authorize(Roles = "Administrator")]`. Los intentos de acceso por otros roles devuelven `Success: false` y HTTP 403 Forbidden.
- **Cero Strings Hardcodeados**: Asuntos y respuestas localizados mediante `DashboardRepository.resx` y `DashboardRepository.en.resx`.

---

### Pasos para Probar el Dashboard en MVC

1. **Iniciar Sesión**: Acceda con el usuario Administrador (`admin@uam.edu` / `Admin123!`).
2. **Navegar al Dashboard**: Seleccione el enlace **Dashboard Métricas** en el menú lateral o diríjase a `/Dashboard`.
3. **Verificación de Datos**:
   - Compruebe las 7 tarjetas de conteo resumen en la parte superior.
   - Verifique los indicadores de tiempo promedio, mínimo y máximo de resolución.
   - Analice la tabla de distribución por laboratorio y la tabla de carga de trabajo por técnico.
4. **Prueba de Control de Acceso**: Inicie sesión con un usuario Técnico (`karinabril19@gmail.com`) e intente acceder a `/Dashboard`. El sistema redirigirá mostrando un mensaje de acceso denegado.

