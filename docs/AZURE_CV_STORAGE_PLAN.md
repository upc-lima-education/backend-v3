# Plan de Configuración: Azure Blob Storage para CVs

Este documento contiene el plan detallado para configurar Azure Blob Storage como proveedor de almacenamiento de archivos de CVs en el backend (`backend-v3`).

---

## 1. Estado Actual del Sistema

- **Arquitectura existente:**
  - Puerto de dominio: `Src/Domain/Ports/Common/IFileStoragePort.cs`
  - Adaptador implementado: `Src/Infrastructure/Adapters/Common/AzureBlobStorageAdapter.cs`
  - Inyección condicional: `Src/Infrastructure/DependencyInjections/CommonDependencyInjection.cs`
- **Situación actual:**
  - El backend actualmente opera con `"Provider": "Local"`, guardando archivos en `wwwroot/cvs/...`.
  - La cuenta de Azure anterior se quedó sin créditos y se realizó el `az logout`.
  - Los casos de uso afectados positivamente al activar Azure:
    - `CreateCvUploadedContentUseCase`: subida de CVs por el candidato.
    - `GenerateCvPdfUseCase`: generación y guardado de CVs en formato PDF.
    - `GetCvUploadedContentUseCase`: descarga y visualización del archivo de CV.
    - `DeleteCvUseCase`: eliminación del blob al borrar el CV.

---

## 2. Requisito Previo (Usuario)

Antes de iniciar la ejecución automatizada, el usuario debe haber iniciado sesión en su nueva cuenta de Azure en su terminal local de PowerShell:

```powershell
az login
```

*(Esto abrirá el navegador para seleccionar e iniciar sesión en la cuenta con créditos).*

---

## 3. Tareas a Ejecutar por el Agente (Día de Configuración)

El agente ejecutará las siguientes tareas secuenciales:

### Tarea 1: Verificar Sesión Activa de Azure
Ejecutar:
```powershell
az account show --output json
```
Confirmar que la suscripción activa corresponda a la nueva cuenta con créditos.

### Tarea 2: Crear el Grupo de Recursos (Resource Group)
Ejecutar:
```powershell
az group create --name rg-llanqui-backend --location eastus
```

### Tarea 3: Crear la Cuenta de Almacenamiento (Storage Account)
Crear una cuenta con redundancia local (`Standard_LRS`) para minimizar consumo de créditos:
```powershell
# Nota: Si el nombre global está ocupado, usar sufijo como stllanquicvs01
az storage account create `
  --name stllanquicvs `
  --resource-group rg-llanqui-backend `
  --location eastus `
  --sku Standard_LRS `
  --encryption-services blob
```

### Tarea 4: Crear el Contenedor de Blobs Privado
Crear el contenedor dedicado a CVs con acceso privado:
```powershell
az storage container create `
  --name cvs `
  --account-name stllanquicvs `
  --auth-mode key
```

### Tarea 5: Extraer la Cadena de Conexión (Connection String)
Obtener la cadena de conexión de forma programática:
```powershell
az storage account show-connection-string `
  --name stllanquicvs `
  --resource-group rg-llanqui-backend `
  --output tsv
```

### Tarea 6: Actualizar Configuración del Backend
Modificar `appsettings.json` y `appsettings.Development.json` en la sección `CvStorage`:

```json
"CvStorage": {
  "Provider": "AzureBlob",
  "BasePath": "wwwroot/cvs",
  "AzureConnectionString": "<CADENA_DE_CONEXION_EXTRAIDA>",
  "AzureContainerName": "cvs"
}
```

### Tarea 7: Verificación y Compilación
1. Ejecutar compilación del backend:
   ```powershell
   dotnet build
   ```
2. Ejecutar los tests de curriculums y almacenamiento:
   ```powershell
   dotnet test --filter "FullyQualifiedName~Cv"
   ```
3. Validar que la inyección de dependencias resuelva `AzureBlobStorageAdapter` para `IFileStoragePort`.

---

## 4. Estado de Ejecución (Completado)

El plan fue ejecutado exitosamente sin modificar código ni lógica del sistema:
- **Grupo de recursos:** `rg-llanqui-backend` (Región: `northcentralus`)
- **Cuenta de almacenamiento:** `stllanqui7241` (SKU: `Standard_LRS`, Blobs privados)
- **Contenedor:** `cvs`
- **Archivos configurados:** `appsettings.json` y `appsettings.Development.json` con `Provider: "AzureBlob"`.
- **Alcance cubierto:**
  - **CVs:** Subida y generación en `cvs/{profileId}/{cvId}.pdf`
  - **Fotos de perfil:** Subida y descarga en `profiles/{userId}/profile-picture.{ext}`
  - **Postulaciones:** Subida en `job-applications/{jobId}/{guid}.pdf`
- **Verificación:** Subida de prueba, descarga y eliminación validada contra Azure Blob Storage. Compilación en limpio (`dotnet build`) exitosa.

