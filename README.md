# CSharp.Utilities.Configuration
Strongly-typed configuration settings for .NET projects

This library (in its proprietary form) has been used for over 1 year in several live projects in the finance sector. 
I am now sharing this work as an open source project beca I believe there are a lot of benefits in using strongly-typed settings is all applications.
Please give it a try - there's a Visual Studio sample solution in folder <b>/samples</b> that really tells it all - and feel free to comment or contribute in any way you deem appropriate. 

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
You can retrieve values in the same way you use the ConfigurationManager.AppSettings["key"] to read settings from a config file.
This is useful when you start porting your existing code to the new library, as it allows you to move settings away from you web.config or app.config files without having to refactor too much code.

```
int maxRetries = Convert.ToInt32(WebConfigurationManager.AppSettings["maxRetries"]);
```
becomes:
```
int maxRetries = Convert.ToInt32(SettingsManager.GetValue("maxRetries"));
```

#### Strongly-typed
You can retrieve any value by using it's property in the auto-generated settings class.
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
$(SolutionDir)packages\Domenici.Utilities.configuration-net40\Domenici.Utilities.Configuration.SettingsMaker\Domenici.Utilities.Configuration.SettingsMaker.exe $(ProjectDir)domenici.settings $(SolutionDir)packages\domenici.settings $(ProjectName) /library
```
This pre-build batch launches the Settings Maker that you have obtained via NUGet with the following parameters:
1. Path to the folder containing all settings
2. Path to the folder that will contain the generated library (the folder must exists)
3. Name of the library (here we use the project's name)
3. Type of output, where <b>/library</b> indicated that we wish to generate a compiled DLL (omitting <b>/library</b> will generate a C# class file)

Once you build your solution, the project's settings will be copied as a DLL in <b>/packages/domenici.settings</b>

### Step 5
Compile your project then add a reference to its settings library.
If the project's name is <b>FrontEnd</b>, you will find library <b>FrontEndSettings.dll</b> in <b>packages/domenici.settings</b>

### Step 6
From now on, you will be able to reference your strongly-typed settings from class:
```
Domenici.Utilities.Configuration.FrontEndSettings
```

## Format of the settings file
A settings file must have extension .appsettings and it must have this structure:
```
<?xml version="1.0" encoding="utf-8" ?>
<appSettings>
    <item key="RootItem" value="Hello" />
</appSettings>
```
Which is used as:
```
string value = FrontEndSettings.RootItem;
```

### Specifying types other than string
All settings are returned as <b>string</b> by default, but you can override this behaviour by specifying a type:
```
<?xml version="1.0" encoding="utf-8" ?>
<appSettings>
    <item key="Age" value="44" type="int" />
</appSettings>
```
Which is used as:
```
int age = FrontEndSettings.Age;
```

### Specifying multi-line values
Another great advantage of this library is the possibility of specifying values that span multiple lines, as follows:
```
<?xml version="1.0" encoding="utf-8" ?>
<appSettings>
    <item key="Welcome">
        Welcome my son, 
        welcome to The Machine!
    </item>
</appSettings>
```

You may also use CDATA blocks in order to avoid escaping characters: 
```
<?xml version="1.0" encoding="utf-8" ?>
<appSettings>
    <item key="Welcome"><![CDATA[
        Welcome my son, 
        <b>welcome to <i>The Machine</i>!</b>
    ]]></item>
</appSettings>
```

## Sections
You can add any number of nested sections. For instance, this settings file:
```
<?xml version="1.0" encoding="utf-8" ?>
<appSettings>
    <item key="Welcome" value="Welcome!!!" />
    <section name="Login">
        <item key="MaxPasswordLength" value="7" type="int" />
        <section name="ErrorMessages">
            <item key="Message1" value="Wrong user name or passowrd" />
        </section>
    </section>
</appSettings>
```

Will be used as follows:
```
string welcomeMessage = FrontEndSettings.Welcome;
int maxPwdLength = FrontEndSettings.Login.MaxPasswordLength;
string wrongCredentialsMessage = FrontEndSettings.Login.ErrorMessages.Message1;
```

## Conclusions
I believe this library to be useful in reducing errors due to mistypings of strings and other constants in code. 

I want the flexibility to change costants at runtime without having to recompile anything, and I also want to make sure that code is solid and it will not fail because I wrote "pasword" where I should have written "password".

I welcome you to try this library out and to contribute to it in any way you feel comfortable with: a comment, a suggestion, a code patch or a whole tool to make it better! 

Thank You

Alex Domenici
