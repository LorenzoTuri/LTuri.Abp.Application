# Utilities

## IsOneOfValidationAttribute

This validation attribute can be used to validate an input variable
that should be one of the provided values.

Ex.: 
``` c#
class RequestInput {
	[IsOneOfValidationAttribute("a,b,c")]
	public string name { get; set; }
}
```