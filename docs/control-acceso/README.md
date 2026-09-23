# 1. Control de acceso

Semanas 2–4 · en progreso

Registro con activación por correo, sesión, roles, administración de usuarios, y recuperación/restablecimiento de contraseña.

## Variables de entorno

| Variable                            | Para qué sirve                                                                   |
| ----------------------------------- | -------------------------------------------------------------------------------- |
| `[DATABASE_URL]`                    | Conexión a la base de datos donde persisten usuarios, tokens y correos en cola   |
| `[JWT_SECRET]` / `[SESSION_SECRET]` | Firma de la credencial de sesión                                                 |
| `[SMTP_HOST]`                       | Host del servidor de correo saliente                                             |
| `[SMTP_PORT]`                       | Puerto del servidor de correo saliente                                           |
| `[SMTP_USER]`                       | Usuario/correo con el que se autentica el envío                                  |
| `[SMTP_PASS]`                       | Contraseña de aplicación del servidor de correo                                  |
| `[APP_BASE_URL]`                    | Base para construir el enlace de activación/recuperación que se envía por correo |
| `[ACTIVATION_TOKEN_TTL_MINUTES]`    | Vencimiento del token de activación (RF-CA-15)                                   |
| `[RECOVERY_CODE_TTL_MINUTES]`       | Vencimiento del código de recuperación (RF-CA-10)                                |

## Ejecutar el proceso de la cola de correos

```bash
# proceso aparte, ver RF-NOT-08/09
[comando]
```

## Cómo provocar cada criterio de aceptación

### Registro y activación

- [ ] **RF-CA-01** — Registrar un correo, luego repetir el registro con el mismo correo → segundo intento rechazado.
      `[comando/paso exacto]`
- [ ] **RF-CA-02** — Registrar dos usuarios con la misma contraseña → revisar la base de datos directamente: los valores almacenados no coinciden entre sí ni con la contraseña en texto plano.
      `[comando/paso exacto]`
- [ ] **RF-CA-14** — Registrar con contraseña de menos de 8 caracteres, y con una sin números o sin letras → rechazo controlado (no excepción sin manejar).
      `[comando/paso exacto]`
- [ ] **RF-CA-15** — Registrar un usuario e intentar iniciar sesión antes de abrir el enlace → rechazo indicando cuenta inactiva. Confirmar que el correo de activación llega de verdad.
      `[comando/paso exacto]`
- [ ] **RF-CA-16** — Abrir el enlace de activación → iniciar sesión funciona. Abrir el mismo enlace una segunda vez → rechazo, estado sin cambios.
      `[comando/paso exacto]`
- [ ] **RF-CA-17** — Pedir reenvío de activación con un correo que no existe y con uno que sí → misma respuesta en ambos casos. Confirmar que el enlace anterior queda invalidado.
      `[comando/paso exacto]`

### Sesión

- [ ] **RF-CA-03** — Iniciar sesión con credenciales correctas (recibe credencial de sesión) y con incorrectas (correo inexistente y contraseña incorrecta) → ambos rechazos con el mismo mensaje.
      `[comando/paso exacto]`
- [ ] **RF-CA-07** — Consultar el usuario autenticado sin sesión → rechazo. Con sesión válida → devuelve usuario y rol.
      `[comando/paso exacto]`
- [ ] **RF-CA-18** — Cerrar sesión y reutilizar la misma credencial → rechazo.
      `[comando/paso exacto]`
- [ ] **RF-CA-19** — Fallar el login 5 veces seguidas, luego intentar con la contraseña correcta durante el bloqueo → rechazo. Esperar o simular el vencimiento del bloqueo y repetir con la contraseña correcta → funciona, contador en cero.
      `[comando/paso exacto]`

### Roles y administración de usuarios

- [ ] **RF-CA-04 / RF-CA-05** — Ubicar en el código el punto único donde cada operación declara el rol que exige.
      `[ruta del archivo]`
- [ ] **RF-CA-06** — Con sesión de Estándar, construir a mano una petición a una operación de Administrador (sin pasar por la interfaz) → rechazo explícito.
      `[comando/paso exacto]`
- [ ] **RF-CA-08** — Como Administrador, cambiar el rol de otro usuario → funciona. Como Estándar, intentar cambiar el propio rol o el de otro → rechazo en ambos casos.
      `[comando/paso exacto]`
- [ ] **RF-CA-20** — Como Administrador, desactivar un usuario con sesión abierta → esa sesión deja de servir y no puede iniciar sesión de nuevo. Reactivarlo → vuelve a poder. Intentar que el Administrador se desactive a sí mismo → rechazo.
      `[comando/paso exacto]`
- [ ] **RF-CA-21** — Como Administrador, listar usuarios con rol y estado → no incluye hashes ni tokens. Como Estándar, intentar el mismo listado → rechazo.
      `[comando/paso exacto]`

### Contraseñas: recuperación, cambio y restablecimiento

- [ ] **RF-CA-09** — Pedir recuperación con un correo inexistente y con uno existente → misma respuesta en ambos casos.
      `[comando/paso exacto]`
- [ ] **RF-CA-10** — Usar el código de recuperación dos veces, y usarlo después de vencido → ambos casos rechazados, la contraseña no cambia.
      `[comando/paso exacto]`
- [ ] **RF-CA-11 / RF-CA-12** — Cambiar la contraseña con un código válido → la contraseña anterior deja de servir para iniciar sesión, y una credencial de sesión emitida antes del cambio queda rechazada.
      `[comando/paso exacto]`
- [ ] **RF-CA-13** — Como Administrador, forzar el restablecimiento de la contraseña de un usuario → la contraseña anterior deja de servir y llega por la cola el correo con el código nuevo.
      `[comando/paso exacto]`
- [ ] **RF-CA-22** — Con sesión, cambiar la propia contraseña indicando la actual incorrecta → rechazo. Con la actual correcta → funciona, aplicando RF-CA-14 y RF-CA-12.
      `[comando/paso exacto]`

### Correo por cola

- [ ] **RF-NOT-08** — Apagar el acceso al servidor SMTP y registrar un usuario → la operación termina bien, el correo queda en la cola en estado pendiente.
      `[comando/paso exacto]`
- [ ] **RF-NOT-09 / RF-NOT-12** — Restaurar el acceso SMTP y ejecutar el proceso enviador → el correo llega de verdad. Ejecutar el enviador una segunda vez → no duplica el envío.
      `[comando/paso exacto]`
- [ ] **RF-NOT-13** — Confirmar que las credenciales SMTP no aparecen en el repositorio ni en su historial (`git log -p`).

### Estructura de la máquina de estados de negocio

- [ ] **RF-NEG-03 / RD-04** — Ubicar en el código el punto único donde están declarados los estados (entre 3 y 5) y las transiciones permitidas.
      `[ruta del archivo]`
- [ ] **RF-NEG-04** — Confirmar la transición explícitamente prohibida documentada.
- [ ] **RF-NEG-05** — Confirmar el estado terminal documentado.
- [ ] Tabla de transiciones en [`../maquina-de-estados.md`](../maquina-de-estados.md).

### Persistencia

- [ ] **RD-09** — Reiniciar la aplicación → los usuarios registrados siguen existiendo.

## Pull requests de esta pieza

Cada uno con las cuatro secciones (Qué cambia, Por qué, Cómo probarlo, Qué NO incluye):

- [ ] Registro y activación (RF-CA-01, 02, 14, 15, 16, 17)
- [ ] Sesión (RF-CA-03, 07, 18, 19)
- [ ] Recuperación de contraseña (RF-CA-09 a 13, 22)
- [ ] Administración de usuarios (RF-CA-04, 05, 06, 08, 20, 21)
