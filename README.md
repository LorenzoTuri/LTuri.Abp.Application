# LTuri.Abp.Application

## How to use it:

install package LTuri.Abp.Application from nuget

>>> dotnet add package LTuri.Abp.Application


## Usage

Refer to the guides in the docs folder to use the various features of this package.


## Suggestions

Create the permission definition -> Contracts, always like this:

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


Configure swagger with docs -> Host or application
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