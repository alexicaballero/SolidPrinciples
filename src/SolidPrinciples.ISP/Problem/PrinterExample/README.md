# Problema: Interfaces Gordas - Ejemplo de Impresora Multifunción

## El Problema

[IMultiFunctionDevice.cs](IMultiFunctionDevice.cs) es una interfaz gorda que obliga a todas las impresoras a implementar 5 métodos, incluso si solo soportan uno.

## La Violación

[BasicInkjetPrinter.cs](BasicInkjetPrinter.cs) solo puede imprimir, pero se ve forzada a:

- Implementar 5 métodos (solo 1 funciona)
- Lanzar `NotSupportedException` para 4 de ellos
- "Mentir" sobre sus capacidades en la firma

## Consecuencias

1. **Excepciones en tiempo de ejecución**: El código compila pero falla cuando se ejecuta
2. **API poco confiable**: Los clientes no saben qué métodos son seguros
3. **Verificación de tipos necesaria**: `if (device is BasicInkjetPrinter)` - code smell
4. **Pruebas complicadas**: Mockear 5 métodos cuando solo se usa 1

## Enseñanza

Cuando una clase implementa una interfaz y **la mitad de los métodos lanzan NotImplementedException**,
la interfaz viola ISP.

## Referencia

Ver [artículo ISP](https://calm-field-0d87ced10.6.azurestaticapps.net/post/es/solid-principles/interface-segregation) -
Sección: "El Problema: Interfaces Gordas - Un Ejemplo Simple: Impresora Multifunción"
