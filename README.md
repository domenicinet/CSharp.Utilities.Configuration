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

#### Typical web/app.config scenario
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

#### New Domenici.Utilities.Configuration scenario
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

```
int maxRetries = Convert.ToInt32(WebConfigurationManager.AppSettings["maxRetries"]);
```
becomes:
```
int maxRetries = Convert.ToInt32(SettingsManager.GetValue("maxRetries"));
```

#### Strongly-typed
You can retrieve a value by using it's property in the auto-generated settings class.
This is the default way of using this library, and the reason why you're reading this.


```
int maxRetries = Convert.ToInt32(WebConfigurationManager.AppSettings["maxRetries"]);
```
becomes:
```
int maxRetries = FrontEndSettings.MaxRetries;
```

## Setting up your Visual Studio project
This library's companion app, SettingsMaker.exe, can generate your strongly-typed classes as source code within your project or as DLL files that you can reference from your project (this is my preferred way).

### Step 1
You start by creating a folder in your project that will hold all of your settings files. Yes, you may split your settings into different files and SettingsMaker will merge them into a single project class :D

All settings files must have extension <b>.appsettings</b>

In this example, the folder is called <b>domenici.settings</b>

### Step 2
You add the following key to your web.config or app.config files:
```
<appSettings>
    <add key="Domenici.Net:Configuration:AppSettingsPath" value="domenici.settings" />          
</appSettings>
```
"value" is a relative (my favorite) or absolute path to the folder where you hold your settings files.

### Step 3
Download Domenici.Utilities.Configuration from NUGet, it will be located in your solution's "packages" folder.

### Step 4
In Visual Studio, select your project then ALT+ENTER to view the project's settings. Select "Build Events" then enter the following pre-build event:
```
$(SolutionDir)packages\Domenici.Utilities\Domenici.Utilities.Configuration.SettingsMaker\domenicisettingsmaker.exe $(ProjectDir)domenici.settings $(SolutionDir)packages\domenici.settings $(ProjectName) /library
```
