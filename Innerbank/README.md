# INFORMACIÓN SOBRE EL PROGRAMA
## Pantallas
El programa consta de 3 pantallas:
- **IdentificacionUsuario**: Esta pantalla se encarga de iniciar sesion de un usuario existente o registrar un nuevo usuario.
- **SeleccionarCuenta**: Esta pantalla se encarga de mostrar todas las cuentas en las cuales el DNI del usuario identificado coincida con el DNI del .titular o de alguno de los cotitulares en una lista para poder seleccionar la cuenta que desees o crear una nueva.
- **ConsultarCuenta**: Esta pantalla se encarga de mostrar toda la informacion de la cuenta, como el saldo, el titular y los cotitulares ademas de permitir ingresar y retirar dinero de la cuenta.

## Alamacenamiento de datos
El programa almacena los datos a través de ficheros cifrados, manejados en el servicio [ControlDatosFicheroService](Servicios/ControlDatosFicheroService.cs).

## Orientado a...
Este programa ha sido pensado para su uso interno de un banco o para ponerlo en ordenadores accesibles al publico sin capacidad de ver el codigo fuente.

## Aspectos a mejorar en su implementación
En [ControlDatosFicheroService](Servicios/ControlDatosFicheroService.cs) está a la vista tanto la variable [\_llaveEncriptado](Servicios/ControlDatosFicheroService.cs#L35), como [\_ivEncriptado](Servicios/ControlDatosFicheroService.cs#L36), además en [ConfirmarContraseñaCommand](Comandos/ConfirmarContraseñaCommand.cs#L16) la contraseña también es visible.
Se recomienda en caso de que el cliente pueda ver el codigo fuente, modificar el codigo para que se obtengan la llave, le iv y la contraseña de manera alternativa, y sin que se muestre en el codigo como una peticion http.