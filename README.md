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




