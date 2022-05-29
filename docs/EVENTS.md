# Events and WebhookEvents

This package provides also a way to implement custom Events (for local use or other use)
directly in the ApplicationServices.

When implementing the AbstractApplicationService or AbstractTranslatableApplicationService, if wanted,
you can choose to implement the version that triggers events.

## How does the event work

Events works like this:
When an event is triggered, multiple EventBus are called.

The simplest one is the Volo.Abp default eventbus, that is used to trigger local events.
The second is the WebhookEventBus, that saves the event to a database, in order to call the respective Webhook
asyncronously.

## Setup

The passages needed to fully implement the event system are the following:

### Create the entity that rappresents the Event in the database (and add it to DbContext)

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

### Add the required services to manage WebhookEvents, linking them to the created entity

Add the call to AddWebhookEventBus (in the module of the Application or the Domain) to add the required services, like:

``` c#
context.Services.AddWebhookEventBus<WebhookQueue>();
context.Services.AddAggregatedEventBus<WebhookQueue>(serviceProvider =>
    new System.Collections.Generic.List<Volo.Abp.EventBus.IEventBus> {
        serviceProvider.GetService<ILocalEventBus>()
    }
);
```

### Implement events

ApplicationServices triggers specific events, depending on it's signature.
This is an example of ApplicationService complete and working.

``` c#
[EventAttribute("app_product_create", "product")]
public class ProductCreateEvent : AbstractEvent
{
}
[EventAttribute("app_product_create", "product")]
public class ProductUpdateEvent : AbstractEvent
{
}
```

``` c#
public class ProductService: AbstractTranslatableApplicationService<
    Product,
    ProductTranslation,
    ProductDto, 
    ProductDto,
    ProductTranslationDto,
    ProductCreateDto,
    ProductUpdateDto,
    ProductCreateEvent,
    ProductUpdateEvent,
    WebhookQueue
    >
{
}
```

The provided ApplicationService, will now trigger the ProductCreateEvent and ProductUpdateEvent.
Right now events are triggered only on create and update. 