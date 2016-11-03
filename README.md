# CSharp.Utilities.Configuration
Strongly-typed configuration settings for .NET projects

## Let's get to the point right away
You are going to move from this:
```
int maxRetries = Convert.ToInt32(WebConfigurationManager.AppSettings["maxRetries"]);
```
to this:
```
int maxRetries = FrontEndSettings.Login.MaxRetries;
```

## It that sounds interesting, how does it happen?
This library allows you to create strongly-types application settings in your .NET projects.
Application settings are stored in XML files, much similar in form and intent to the appSettings tag in your web.config or app.config files.

The library allows you to read settings in two ways:
#### Array key
You can retrieve a values in the same way you use the ConfigurationManager.AppSettings["key"] to read settings from a config file.
This is useful when you start porting your existing code to the new library, as it allows you to move settings away from you web.config or app.config files without having to refactor too much code.

#### Strongly-typed
You can retrieve a value by using it's property in the auto-generated settings class.
This is the default way of using this library, and the reason why you're reading this.
