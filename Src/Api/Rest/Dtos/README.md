# API DTOs
In most use cases, the request and response DTOs used by the controllers
are the same DTOs defined in the Application Layer. This avoids unnecessary
duplication and abstractions when the HTTP contract does not introduce
additional concerns.

However, when an HTTP-specific type or concern is required, a separate API
DTO should be created.

For example, when an endpoint receives a file through `multipart/form-data`,
the controller may need to use `IFormFile`. Since `IFormFile` is an
ASP.NET Core type and should not leak into the Application Layer, the API
layer should define its own request DTO and map it to the corresponding
Application request.

In these cases, the API DTO should follow the same name as the Application
DTO, with the `Api` suffix to clearly distinguish both types.

Example:

Application Layer: `CreateXRequest`
API Layer: `CreateXApiRequest`

The API DTO is responsible for representing the HTTP contract, while the
Application DTO represents the input required by the use case.

API DTOs should only be introduced when the API representation differs
from the Application representation. They should not be created merely
for the sake of maintaining a strict one-to-one separation between layers.