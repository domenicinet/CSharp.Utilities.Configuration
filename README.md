<I>NOTE TO THE READER: 
WE'RE SETTING UP THIS REPOSITORY. WE'LL BE UPLOADING THE LIBRARY VERY SOON.
THE LIBRARY (IN ITS PROPRIETARY FORM) HAS BEEN USED FOR SEVERAL MONTHS IN MANY LIVE PROJECTS IN THE FINANCE SECTOR.
PLEASE BEAR WITH US AND IF YOU LIKE THIS IDEA THEN FEEL FREE TO COMMENT: PLEASE PUT PRESSURE ON US!</I>

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

## How does it happen?
This library allows you to create strongly-types application settings in your .NET projects.
Application settings are stored in XML files, much similar in form and intent to the appSettings tag in your web.config or app.config files.

### Typical web/app.config scenario
Your typical web.config/appSettings section could look like this:
```
<appSettings>
    <add key="login:maxReties" value="3" /> 
    <add key="login:minPasswordLength" value="8" />  
    <add key="welcomeMessage" value="welcome back,\r\n&lt;b&gt;We missed you!&lt;/b&gt;" />          
</appSettings>
```
You will then reference these settings as follows:
```
int maxRetries = Convert.ToInt32(WebConfigurationManager.AppSettings["login:maxRetries"]);
int minPwdLength = Convert.ToInt32(WebConfigurationManager.AppSettings["login:minPasswordLength"]);
this.welcomeDiv.Html = WebConfigurationManager.AppSettings["welcomeMessage"];
```

### New Domenici.Utilities.Configuration scenario
This library's settings would look like this:
```
<appSettings>
    <section name="Login">
        <item key="MaxReties" value="3" type="int" /> 
        <item key="MinPasswordLength" value="8" type="int" /> 
    </section>
    <add key="WelcomeMessage"><![CDATA[welcome back,
<b>We missed you!</b>]]></item>
</appSettings>
```
You will then reference these settings as follows:
```
int maxRetries = FrontEndSettings.Login.MaxRetries;
int minPwdLength = FrontEndSettings.Login.MinPasswordLength;
this.welcomeDiv.Html = FrontEndSettings.WelcomeMessage;
```

## Transitioning
The library allows you to read settings in two ways:
#### Array key
You can retrieve a values in the same way you use the ConfigurationManager.AppSettings["key"] to read settings from a config file.
This is useful when you start porting your existing code to the new library, as it allows you to move settings away from you web.config or app.config files without having to refactor too much code.

#### Strongly-typed
You can retrieve a value by using it's property in the auto-generated settings class.
This is the default way of using this library, and the reason why you're reading this.
