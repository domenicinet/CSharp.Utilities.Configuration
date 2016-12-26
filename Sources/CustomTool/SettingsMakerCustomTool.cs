using Domenici.Utilities.Configuration;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Domenici.Utilities.Configuration
{
    public class SettingsMakerCustomTool
    {
        [Guid("79c47567-ff41-407f-af26-da9e97bc154a")]
        public class ToolBase : IVsSingleFileGenerator
        {
            public int DefaultExtension(out string InputfileRqExtension)
            {
                InputfileRqExtension = ".cs";  // the extension must include the leading period

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
                int result = VSConstants.S_FALSE;
                pcbOutput = 1;  // We are generating an output 

                Console.WriteLine("Generating Configuration code.");

                try
                {
                    StrongTyper st = new StrongTyper(bstrInputFileContents, wszDefaultNamespace, "Settings");
                    byte[] buffer = Encoding.Unicode.GetBytes(st.CreateStronglyTypedOutput());

                    pcbOutput = (uint)buffer.Length;
                    rgbOutputFileContents[0] = Marshal.AllocCoTaskMem(buffer.Length);
                    Marshal.Copy(buffer, 0, rgbOutputFileContents[0], buffer.Length);

                    result = VSConstants.S_OK;
                }
                catch (Exception e)
                {
                    result = VSConstants.S_FALSE;
                    Console.WriteLine("Error generating Configuration code: " + e.ToString());
                }

                Console.WriteLine("Done generating Configuration code.");

                return result;
            }
        }
    }
}
