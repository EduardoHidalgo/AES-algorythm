CIFRADO AES

ES UN ALGORITMO DE BLOQUES

TEXTO PLANO
16x 16 bits = 128 bits = 16 bytes

LLAVE K
 16 x 16 = 128  -> rondas = 10
 24 x 24 = 192  -> rondas = 12
 32 x 32 = 256  -> rondas = 14

4 PROCESOS POR CADA CICLO

 s box
 shift rows
 mix columns
 add round key  (XOR)

 en el último ciclo no hace mix columns

ETAPA A

A.1) el texto plano se pone en un array 4x4 y es en hexadecimal
A.2) el mensaje (de 128) se pone en un array 4x4 y en hexadecimal
A.3) convertir ambos bloques a binario
A.4) hacer un XOR de los valores 
A.5) convertir a hexadecimal el bloque resultante

ETAPA B

B.1) aplicar (TABLA S-BOX hexadecimal)
B.2) aplicar shift rows 
B.3) aplicar MixColumns (tabla CONSTANTS)
B.4) aplicar XOR

repetir 9 veces

en la 10ma repetición se repite todo excepto el paso B.3


ETAPA PARA LLAVES

K[i] -> llave previa al proceso

LL.1) RotRow a la ultima columna  (toma el valor 4,1 y lo intercambia con 4,4)
LL.2) aplicar (tabla S-box) a la columna tomada
LL.3) aplicar XOR de la columna transformada con (i-3) columna de la llave previa al proceso = K[i]
LL.4) aplicar tabla RCON de 128bits a la columna. es un XOR entre la tabla y la columna transformada

LL.5) aplicar XOR[i - 3] a las otras 3 columnas (tomando en cuenta las generadas)

 COLUMNAS        COLUMNAS TRANSFORMADAS
1C 2C 3C 4C  - 5C 
.  .  .  .     .
.  .  .  .     .
.  .  .  .     .
.  .  .  .     .

XOR PARA GENERAR C6 = XOR (2C X 5C)
XOR PARA GENERAR C7 = XOR (3C X 6C)
XOR PARA GENERAR C8 = XOR (4C X 7C)

shift rows

c3 x 02 = cb

195 x 2 = 203

