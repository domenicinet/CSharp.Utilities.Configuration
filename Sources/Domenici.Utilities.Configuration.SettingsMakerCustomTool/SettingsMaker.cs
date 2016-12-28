using System;

/*
After compilation, there are two steps to be performed: You must register the assembly for COM interop, and place it in the Global Assembly Cache (GAC). 
The order of these two operations is unimportant.

You can edit the registry to register the assembly:
[HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\VisualStudio\VS_VERSION\CLSID\GENERATOR_GUID]
@="COM+ class: NAMESPACE_NAME.GENERATOR_TYPE_NAME"
"InprocServer32"="C:\\WINDOWS\\system32\\mscoree.dll"
"ThreadingModel"="Both"
"Class"="NAMESPACE_NAME.GENERATOR_TYPE_NAME"
"Assembly"="NAMESPACE_NAME, Version=ASSEMBLY_VERSION, Culture=Neutral, PublicKeyToken=PUBLIC_TOKEN_KEY"

[HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\VisualStudio\VS_VERSION\Generators]

[HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\VisualStudio\VS_VERSION\Generators\PROJECT_TYPE_GUID]

[HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\VisualStudio\VS_VERSION\Generators\PROJECT_TYPE_GUID\\.FILE_EXTENSTION]
@="GENERATOR_TYPE_NAME"

[HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\VisualStudio\VS_VERSION\Generators\PROJECT_TYPE_GUID\GENERATOR_TYPE_NAME]
@="Code generator for whatever you like"
"CLSID"="GENERATOR_GUID"
"GeneratesDesignTimeSource"=dword:00000001

For VS to see your tool, it needs to be in the GAC when VS starts. 
Thus, before starting up VS, place your custom tool in the GAC with: gacutil /i YourCustomTool.dll
WARNING: You must place ALL dependencies in the GAC, too

Assembly removal is done with the /u switch, and you must remember to remove the .dll ending, as the tool wants the assembly display name, not the file name.
gacutil /u YourCustomTool

Important note: VS loads your custom tool into memory when it runs. This means that even if you unregister it, recompile and re-register the new version, 
VS will not see it immediately. You will need to:
Close Visual Studio
Run command: devenv /setup 
NOTE: The command will run for a few of minutes and it's needed to make sure that Visual Studio's tools cache is reloaded.
Launch Visual Studio again, your custom tool will be ready for use.
 */
namespace Domenici.Utilities.Configuration.CustomTools
{
    using System;
    using Microsoft.VisualStudio;
    using Microsoft.VisualStudio.Shell.Interop;
    using System.Runtime.InteropServices;
    using System.Text;
    using Configuration;
    using System.Diagnostics;

    [Guid("E29E2A80-292A-4CCB-9433-1F6100CC6F77")]
    public class SettingsMaker : IVsSingleFileGenerator
    {
        public int DefaultExtension(out string pbstrDefaultExtension)
        {
            pbstrDefaultExtension = ".cs";
            return VSConstants.S_OK;
        }
        
        public int Generate(
            string wszInputFilePath, 
            string bstrInputFileContents, 
            string wszDefaultNamespace,
            IntPtr[] rgbOutputFileContents, 
            out uint pcbOutput, 
            IVsGeneratorProgress pGenerateProgress)
        {
            Debug.WriteLine("Generating configuration code.");
            Console.WriteLine("Generating configuration code.");
            Console.WriteLine("Source path: " + wszInputFilePath);
            Console.WriteLine("Source namespace: " + wszDefaultNamespace);

            string output = null;

            try
            {
                StrongTyper st = new StrongTyper(bstrInputFileContents, wszDefaultNamespace, "Settings.cs");
                output = st.CreateStronglyTypedOutput();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error generating Configuration code: " + e.ToString());
                
                pcbOutput = 0;
                return VSConstants.S_FALSE;
            }

            byte[] bytes = Encoding.UTF8.GetBytes(output);
            int length = bytes.Length;

            rgbOutputFileContents[0] = Marshal.AllocCoTaskMem(length);
            Marshal.Copy(bytes, 0, rgbOutputFileContents[0], length);

            Console.WriteLine("Done generating Configuration code.");

            pcbOutput = (uint)length;
            return VSConstants.S_OK;   
        }

        #region Registry self-registration (COMMENTED)
        //private const string CUSTOM_TOOL_NAME = "Domenici.Utilities.Configuration.CustomTools.SettingsMaker";
        //private const string REGISTRY_PATH = @"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\VisualStudio\14.0\Generators\{FAE04EC1-301F-11D3-BF4B-00C04F79EFBC}\"; // C#
        ////private const string REGISTRY_PATH = @"HKEY_LOCAL_MACHINE\SOFTWARE\WOW6432Node\Microsoft\VisualStudio\14.0\Generators\{164B10B9-B200-11D0-8C61-00A0C91E29D5}\"; // VB

        //[ComRegisterFunction]
        //public static void RegisterClass(Type t)
        //{
        //    using (RegistryKey key = Registry.LocalMachine.CreateSubKey(REGISTRY_PATH + CUSTOM_TOOL_NAME))
        //    {
        //        key.SetValue("", "Domenici.Utilities.Configuration custom tool that generates strongly typed settings.");
        //        key.SetValue("CLSID", new GuidAttribute("{E29E2A80-292A-4CCB-9433-1F6100CC6F77}"));
        //        key.SetValue("GeneratesDesignTimeSource", 1);
        //    }
        //}

        //[ComUnregisterFunction]
        //public static void UnregisterClass(Type t)
        //{
        //    Registry.LocalMachine.DeleteSubKey(REGISTRY_PATH + CUSTOM_TOOL_NAME, false);
        //}
        #endregion
    }
}