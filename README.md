# LTuri.Abp.Application

# How to use it:

install package LTuri.Abp.Application from nuget

>>> dotnet add package LTuri.Abp.Application

# Usage

## Create your entities (ex. here) -> Domain, and configure them in DbContext

``` c#
    [Table("app_products")]
    public class Product : AuditedAggregateRoot<Guid>, IMultiTenant, IIdentifiableEntity
    {
        [Column("number")]
        [Required]
        [MinLength(5), MaxLength(50)]
        public string ProductNumber { get; set; }
        [Column("quantity")]
        [Required]
        public long Quantity { get; set; }
        [Column("price")]
        [Required]
        public double Price { get; set; }
        [Column("tenant_id")]
        [Required]
        public Guid? TenantId { get; set; }

        public string Identifier => ProductNumber;
    }
```

## Create your Dto/Dtos -> Contracts

``` c#
    public class ProductDto : AuditedEntityDto<Guid>
    {
        [Required]
        [MinLength(5), MaxLength(50)]
        public string ProductNumber { get; set; }
        public long Quantity { get; set; }
        public double Price { get; set; }
    }
```

## Configure the automapping -> Application

``` c#
    public MyApplicationApplicationAutoMapperProfile()
    {
        // Product
        CreateMap<Entities.Product, ProductDto>();
        CreateMap<ProductCreateDto, Product>();
        CreateMap<ProductUpdateDto, Product>();
    }
```

## Create your events -> Application, and EventEntity -> Domain
## TODO: this part should not be required but optional

``` c#
    [EventAttribute("app_product_create", "product")]
    public class ProductCreateEvent : AbstractEvent
    {
    }
    
    [EventAttribute("app_product_update", "product")]
    public class ProductUpdateEvent : AbstractEvent
    {
    }
```
``` c#
    [Table("app_webhook_queue")]
    public class WebhookQueue : Entity<Guid>, IMultiTenant, IWebhookQueueEntity
    {
        [Column("event_name")]
        [Required]
        public string EventName { get; set; }

        [Column("event_status")]
        [Required]
        public WebhookQueueStatuses EventStatus { get; set; } = WebhookQueueStatuses.Pending;
        
        [Column("entity_name")]
        [Required]
        public string EntityName { get; set; }
        
        [Column("entity_id")]
        [Required]
        public Guid? EntityId { get; set; }

        [Column("changed")]
        [Required]
        public string[] Changed { get; set; } = new string[0];
        
        [Column("tenant_id")]
        [Required]
        public Guid? TenantId { get; set; }
    }
```

## Create the permission definition -> Contracts, ex.:

``` c#
public static class MyApplicationPermissions
{
    public const string GroupName = "MyApplication";

    public const string CreateSuffix = "_create";
    public const string EditSuffix = "_edit";
    public const string DeleteSuffix = "_delete";

    public static class Products
    {
        public const string Default = GroupName + "_products";
        public const string Create = Default + CreateSuffix;
        public const string Edit = Default + EditSuffix;
        public const string Delete = Default + DeleteSuffix;
    }
}
```

## Create the Service -> Application
``` c#
    [Authorize(MyApplicationPermissions.Products.Default)]
    public class ProductService : AbstractApplicationService<
        Product,
        ProductDto,
        ProductDto,
        ProductCreateDto,
        ProductUpdateDto,
        ProductCreateEvent,
        ProductUpdateEvent,
        WebhookQueue
        >
    {
        public ProductService(
            IAbpLazyServiceProvider lazyServiceProvider,
            IEfCoreRepository<Product, Guid> repository,
            IObjectMapper objectMapper,
            AggregatedEventBus<WebhookQueue> eventBus
        ) : base(lazyServiceProvider, repository, objectMapper, eventBus)
        {
            ParentAppService.SetPolicy(
                PolicyType.Get,
                MyApplicationPermissions.Products.Default
            );
            ParentAppService.SetPolicy(
                PolicyType.List,
                MyApplicationPermissions.Products.Default
            );
            ParentAppService.SetPolicy(
                PolicyType.Create,
                MyApplicationPermissions.Products.Create
            );
            ParentAppService.SetPolicy(
                PolicyType.Update,
                MyApplicationPermissions.Products.Edit
            );
            ParentAppService.SetPolicy(
                PolicyType.Delete,
                MyApplicationPermissions.Products.Delete
            );
        }
    } 
```


## SUGGESTION: configure swagger with docs -> Host or application
``` c#
        context.Services.AddAbpSwaggerGenWithOAuth(
            ...
            options => {
                ...
                
                options.CustomSchemaIds(
                    type => type.FullName.Replace("Volo.Abp", "Base").Replace("Lturi.MyApplication", "App").ToSnakeCase()
                );

                // Load all documentations attributes for swagger
                var files = Directory.GetFiles(AppContext.BaseDirectory, "*.xml", SearchOption.AllDirectories);
                foreach (var file in files)
                {
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, file);
                    options.IncludeXmlComments(xmlPath);
                }
                ...
            }
        );
```