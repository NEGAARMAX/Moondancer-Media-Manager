# Project Moondancer — Media Manager

Extensión de Playnite para seleccionar los recursos visuales mostrados por **temas compatibles** en Desktop y Fullscreen. No modifica las imágenes ni los metadatos originales de los juegos.

## Características
- Cinco modalidades: **Cover**, **Icon**, **Background**, **Logo** y **Background + Logo**.
- Preferencias independientes para **Desktop** y **Fullscreen**.
- Marcos transparentes de referencia **16:9** y **1:1** para logotipos.
- Migración de preferencias anteriores de la serie 0.4.x al perfil Fullscreen.

## Requisitos
- Playnite con soporte de extensiones genéricas y temas compatibles.
- Un tema que integre explícitamente los recursos de Moondancer. La extensión **no funciona automáticamente con todos los temas**.
- Para los logotipos, disponer de recursos adecuados mediante Extra Metadata Loader y la integración que use el tema.
- Para compilar: Visual Studio con herramientas de .NET Framework 4.6.2/WPF y restauración del paquete `PlayniteSDK` 6.17.0.

## Instalación de la extensión (.pext)
1. Descarga el archivo `.pext` correspondiente a la versión 1.0.0 desde la sección **Releases** del repositorio.
2. Abre el archivo `.pext` con Playnite para instalar la extensión.
3. Instala un tema compatible con Moondancer y selecciónalo en el modo correspondiente (Desktop o Fullscreen).
4. Abre la configuración de Moondancer y elige los recursos visuales que deseas mostrar.

La extensión y los temas de prueba se distribuyen por separado. Instalar Moondancer **no modifica automáticamente** el tema que ya utilizas.

## Instalación desde código fuente
1. Haz una copia de seguridad de tu configuración y de la versión anterior.
2. Abre `Moondancer.csproj` en Visual Studio y restaura los paquetes NuGet.
3. Compila el proyecto. El resultado principal es `Moondancer.dll`.
4. Para instalar manualmente, coloca `Moondancer.dll`, `extension.yaml`, `icon.png` y `Localization/en_US.xaml` en la raíz de una carpeta de extensión de Playnite. Alternativamente, empaqueta esos archivos en la raíz de un ZIP y cambia la extensión a `.pext`.
5. Instala y selecciona por separado un tema compatible con Moondancer.

**Importante:** este archivo es el paquete de **código fuente**, no un instalador `.pext` precompilado.

## Configuración
Abre los ajustes de extensiones de Playnite y busca **Moondancer - Media Manager**. Cada perfil (Desktop o Fullscreen) permite seleccionar modalidad y marco de logo independientemente.

### Relación de aspecto
La proporción de la cuadrícula se cambia desde **Configuración → Apariencia → Vista de cuadrícula → Relación de aspecto objetivo**.

| Modalidad | Proporción recomendada |
| --- | --- |
| Cover | 3:4 |
| Icon | 1:1 |
| Background | 16:9 |
| Logo | 1:1 o 16:9 |
| Background + Logo | 16:9 |

**Limitación:** esa proporción es global en Playnite y afecta a Desktop y Fullscreen. Moondancer **no cambia automáticamente** la proporción ni sustituye el control de cuadrícula de Playnite. Las proporciones indicadas son recomendaciones, no presets aplicables desde la extensión.

## Temas de prueba y referencia

Se proporcionan dos temas de ejemplo, basados en la presentación predeterminada de Playnite y modificados para integrar Moondancer:

| Paquete | Modo | Archivo principal de integración |
| --- | --- | --- |
| `Moondancer_Desktop_Test_Theme_Alpha_0_5_0.pthm` | Desktop | `DerivedStyles/GridViewItemTemplate.xaml` |
| `Moondancer_Test_Theme_Alpha_0_5_0.pthm` | Fullscreen | `DerivedStyles/ListGameItemTemplate.xaml` |

Estos temas **no son requisitos** de la extensión: sirven para probar su funcionamiento y como referencia para adaptar otros temas. Los paquetes de ejemplo tienen versión de tema **0.5.0**, distinta de la versión **1.0.0** de la extensión. La implementación incluida debe comprobarse en cada tema derivado; la compatibilidad no se hereda automáticamente.

## Ejemplos visuales


|  Ejemplos | Desktop  | Fullscreen  |
|---|---|---|
| Cover  | ![Cover Desktop](screenshots/D%20Cover.png) | ![Cover Fullscreen](screenshots/FS%20Cover.png) |
| Icon  | ![Icon Desktop](screenshots/D%20Icon.png) | ![Icon Fullscreen](screenshots/FS%20Icon.png) |
|  Logo | ![Logo Desktop](screenshots/D%20Logo.png) | ![Logo Fullscreen](screenshots/FS%20logo.png) |
|  BG | ![BG Desktop](screenshots/D%20BG.png) | ![BG Fullscreen](screenshots/FS%20BG.png) |
| BG + Logo  | ![BG + Logo Desktop](screenshots/D%20BG%2BLogo.png) | ![BG + Logo Fullscreen](screenshots/FS%20BG%2BIcon.png) |
|  example in compatible theme | ![example in compatible theme Desktop](screenshots/Ejemplo%20con%20tema.png) | ![example in compatible theme Fullscreen](screenshots/Ejemplo%20con%20tema%20FS.png) |

### Integración en un tema propio (XAML)

Los ejemplos conservan el elemento `PART_ImageCover` utilizado por Playnite y superponen elementos inicialmente ocultos para mostrar iconos, fondos y logotipos. Un `DataTrigger` consulta el modo seleccionado en Moondancer y controla qué imágenes son visibles. Ejemplo de Fullscreen:

```xml
<DataTrigger Binding="{PluginSettings Plugin=Moondancer, Path=Fullscreen.VisualMode}" Value="Icon">
    <Setter TargetName="PART_ImageCover" Property="Visibility" Value="Collapsed" />
    <Setter TargetName="MoondancerIcon" Property="Visibility" Value="Visible" />
</DataTrigger>
```

En Desktop, sustituye `Fullscreen.VisualMode` por `Desktop.VisualMode`. Los valores utilizados en los temas de prueba son `Icon`, `Background`, `Logo` y `BackgroundLogo`; cuando no se activa ninguno de estos disparadores, permanece la carátula (`Cover`). Los elementos `MoondancerIcon`, `MoondancerBackground` y `MoondancerLogoFrame` deben existir en la plantilla del tema: el fragmento anterior **no funciona por sí solo**.

Los ejemplos obtienen los recursos de las siguientes fuentes:

- Icono: `Game.Icon`.
- Fondo: `Game.BackgroundImage`.
- Logotipo: ruta construida a partir de `{Api Paths.ConfigurationPath}`, el identificador `Game.Id` y `ExtraMetadata\games\{Game.Id}\logo.png`, utilizando los convertidores de imágenes del tema.

Para el marco del logotipo, ambos ejemplos utilizan un `Viewbox` y un lienzo transparente de **160 × 90**; un disparador cambia su altura a **160** cuando `LogoAspectRatio` es `1:1`:

```xml
<DataTrigger Binding="{PluginSettings Plugin=Moondancer, Path=Fullscreen.LogoAspectRatio}" Value="1:1">
    <Setter TargetName="MoondancerLogoCanvas" Property="Height" Value="160" />
</DataTrigger>
```

En Desktop se usa `Desktop.LogoAspectRatio`. Este cambio modifica el **marco de referencia del logotipo**, no la relación de aspecto de las celdas de Playnite. Para la implementación completa, consulta los archivos XAML de los temas de prueba.

**Nota sobre recursos y licencias:** los ejemplos están basados en temas predeterminados de Playnite. Antes de redistribuir temas derivados, comprueba los avisos y licencias aplicables a los recursos de Playnite y a cualquier otro componente incorporado.

## Compatibilidad y alcance
La extensión expone sus ajustes a los temas mediante el soporte de configuración de Playnite. Los temas que no implementen esa integración no mostrarán cambios. La compatibilidad real debe comprobarse con cada tema.

## Autoría y licencia
**Autor:** NEGAARMAX.

Código fuente bajo **GNU General Public License, versión 3 (GPL-3.0-only)**. Consulta el archivo [`LICENSE`](LICENSE) para conocer los permisos y obligaciones. Esta licencia permite la redistribución y el uso comercial si se respetan sus condiciones. Las dependencias y recursos de terceros mantienen sus propias licencias.

Los nombres y personajes de *My Little Pony* mencionados en los agradecimientos son referencias humorísticas; no implican afiliación ni patrocinio.

## Agradecimientos especiales 🌙✨
**Moondancer**, por su dedicación, paciencia y obsesión por encontrar la solución correcta, incluso cuando las proporciones de Playnite parecían desafiar las leyes de la geometría.

**La Grande y Poderosa Trixie**, por inspirar indirectamente este proyecto con sus espectaculares problemas de cuadrícula. Aunque insiste en que todo formaba parte de su magnífico plan desde el principio.

Y **Twilight Sparkle**, quien probablemente habría encontrado la casilla de configuración en cinco minutos y después habría escrito una carta a la princesa Celestia:

> Querida princesa Celestia: hoy aprendí que la verdadera magia de la amistad consiste en dejar que tus amigas pasen semanas intentando resolver un problema antes de señalarles una casilla del menú de configuración.

*P. D. de Trixie: ¡La Grande y Poderosa Trixie exige que conste que ella descubrió el problema primero!*
