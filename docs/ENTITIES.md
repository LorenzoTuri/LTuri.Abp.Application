# Entities

This package contains the definition for different Entity interfaces, used by the services

## IIdentifiableEntity -> deprecated, unused, can be removed

Rappresents an entity identifiable by a property.

This interface requires a property "Identifier" (string) to be defined. 

This allows the ApplicationService to create an endpoint to get an entity by using a property different from Id with a single Get.

Is useful for entities who have something like a ProductCode (or any other identifier) that is commonly
used to get the entity without knowing the Id. In case of non-unique Identifier, the first valid entity found is returned.

## ITranslatableEntity

Rappresents an entity that can be translated and Thus has a property Translation.
Translation entity can be customized with generics, but must implementa ITranslationEntity.

It's used by AbstractTranslatableApplicationService to provide an endpoint used to load translations

## ITranslationEntity

Rappresents an entity that is a translation of another. If implemented, it only requires to implement a
string property LanguageCode. Type of language code is not important, can be iso, or a custom one, what the language
code is, is up to the developer



### Example for entities

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