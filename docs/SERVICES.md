# Services

This package allows the use of different ApplicationServices.
Like the already existing in the Volo.Abp framework, ApplicationService are used
to generate AutoApi. The ApplicationServices listed here are used to implement
various other implementations revolving around this Feature.

All the services listed here provides AutoFilters along with the AutoApi feature.

Also all the services implements fully the permissions, like in Volo.Abp framework.
Check the end of the file for a complete example of the implemented ApplicationService.

## AbstractApplicationService

AbstractApplicationService is the simplest service implementable to have auto-api with auto-filters.
Also is compatible with events, if wanted.

Routes provided:
- Get -> get an entity by id,		route GET:		{commonPrefix}/{entity}/{id}
- List -> get a list of entitites,	route GET:		{commonPrefix}/{entity}
- Search -> get a list of entities, route POST:		{commonPrefix}/{entity}/search
- Create -> creates an entity,		route POST:		{commonPrefix}/{entity}
- Update -> updates an entity,		route PUT:		{commonPrefix}/{entity}/{id}
- Delete -> delets and entity,		route DELETE:	{commonPrefix}/{entity}/{id}

List and search endpoints supports auto-filters by using a url-encoded (list) or json (search) rappresentation
of the criterias, explained in the CRITERIA.md guide.

## AbstractTranslatableApplicationService

This service extends AbstractApplicationService to provides also a way to manage translations.

All routes are of the AbstractApplicationService exists, along with:
- GetTranslations -> get list,		route GET:		{commonPrefix}/{entity}/{id}/translations

## Example

All the example shown below are similar, the only thing changed is the signature of the created class.

By changing the extended class, you can provide events, translations etc...

### Simple application service
``` c#
[Authorize(ErpCrmPermissions.Products.Default)]
public class ProductService: AbstractApplicationService<
    // Entity
    Product,
    // Dto for get requests
    ProductDto,
    // Dto for list requests
    ProductDto,
    // input Dto for creation
    ProductCreateDto,
    // input Dto for update
    ProductUpdateDto
    >
{
    public ProductService(
        IAbpLazyServiceProvider lazyServiceProvider,
        IProductRepository repository,
        IObjectMapper objectMapper
    ) : base(lazyServiceProvider, repository, objectMapper)
    {
        ParentAppService.SetPolicy(
            PolicyType.Get,
            ErpCrmPermissions.Products.Default
        );
        ParentAppService.SetPolicy(
            PolicyType.List, 
            ErpCrmPermissions.Products.Default
        );
        ParentAppService.SetPolicy(
            PolicyType.Create, 
            ErpCrmPermissions.Products.Create
        );
        ParentAppService.SetPolicy(
            PolicyType.Update, 
            ErpCrmPermissions.Products.Edit
        );
        ParentAppService.SetPolicy(
            PolicyType.Delete, 
            ErpCrmPermissions.Products.Delete
        );
    }
}
```

### Application service with events
``` c#
[Authorize(ErpCrmPermissions.Products.Default)]
public class ProductService: AbstractApplicationService<
    // Entity
    Product,
    // Dto for get requests
    ProductDto, 
    // Dto for list requests
    ProductDto,
    // input Dto for creation
    ProductCreateDto,
    // input Dto for update
    ProductUpdateDto,
    // event triggered on creation
    ProductCreateEvent,
    // event triggered on update
    ProductUpdateEvent,
    // event entity in database
    WebhookQueue
    >
{
    public ProductService(
        IAbpLazyServiceProvider lazyServiceProvider,
        IProductRepository repository,
        IObjectMapper objectMapper,
        AggregatedEventBus<WebhookQueue> eventBus
    ) : base(lazyServiceProvider, repository, objectMapper, eventBus)
    {
        ParentAppService.SetPolicy(
            PolicyType.Get,
            ErpCrmPermissions.Products.Default
        );
        ParentAppService.SetPolicy(
            PolicyType.List, 
            ErpCrmPermissions.Products.Default
        );
        ParentAppService.SetPolicy(
            PolicyType.Create, 
            ErpCrmPermissions.Products.Create
        );
        ParentAppService.SetPolicy(
            PolicyType.Update, 
            ErpCrmPermissions.Products.Edit
        );
        ParentAppService.SetPolicy(
            PolicyType.Delete, 
            ErpCrmPermissions.Products.Delete
        );
    }
}
```

### Application service with translations
``` c#
[Authorize(ErpCrmPermissions.Products.Default)]
public class ProductService: AbstractTranslatableApplicationService<
    // Entity
    Product,
    // Entity of the translation
    ProductTranslation,
    // Dto for get requests
    ProductDto, 
    // Dto for list requests
    ProductDto,
    // Dto for translations request
    ProductTranslationDto,
    // Input Dto for creation
    ProductCreateDto,
    // input Dto for update
    ProductUpdateDto
    >
{
    public ProductService(
        IAbpLazyServiceProvider lazyServiceProvider,
        IProductRepository repository,
        IObjectMapper objectMapper
    ) : base(lazyServiceProvider, repository, objectMapper)
    {
        ParentAppService.SetPolicy(
            PolicyType.Get,
            ErpCrmPermissions.Products.Default
        );
        ParentAppService.SetPolicy(
            PolicyType.List, 
            ErpCrmPermissions.Products.Default
        );
        ParentAppService.SetPolicy(
            PolicyType.Create, 
            ErpCrmPermissions.Products.Create
        );
        ParentAppService.SetPolicy(
            PolicyType.Update, 
            ErpCrmPermissions.Products.Edit
        );
        ParentAppService.SetPolicy(
            PolicyType.Delete, 
            ErpCrmPermissions.Products.Delete
        );
    }
}
```

### Application service with translations and events
``` c#
[Authorize(ErpCrmPermissions.Products.Default)]
public class ProductService: AbstractTranslatableApplicationService<
    // Entity
    Product,
    // Entity of the translation
    ProductTranslation,
    // Dto for get requests
    ProductDto, 
    // Dto for list requests
    ProductDto,
    // Dto for translations
    ProductTranslationDto,
    // input Dto for creation
    ProductCreateDto,
    // input Dto for update
    ProductUpdateDto,
    // event triggered on creation
    ProductCreateEvent,
    // event triggered on update
    ProductUpdateEvent,
    // event entity in database
    WebhookQueue
    >
{
    public ProductService(
        IAbpLazyServiceProvider lazyServiceProvider,
        IProductRepository repository,
        IObjectMapper objectMapper,
        AggregatedEventBus<WebhookQueue> eventBus
    ) : base(lazyServiceProvider, repository, objectMapper, eventBus)
    {
        ParentAppService.SetPolicy(
            PolicyType.Get,
            ErpCrmPermissions.Products.Default
        );
        ParentAppService.SetPolicy(
            PolicyType.List, 
            ErpCrmPermissions.Products.Default
        );
        ParentAppService.SetPolicy(
            PolicyType.Create, 
            ErpCrmPermissions.Products.Create
        );
        ParentAppService.SetPolicy(
            PolicyType.Update, 
            ErpCrmPermissions.Products.Edit
        );
        ParentAppService.SetPolicy(
            PolicyType.Delete, 
            ErpCrmPermissions.Products.Delete
        );
    }
}
```