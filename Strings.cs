using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Resources;
using System.Runtime.CompilerServices;

namespace TypeScript.Tasks
{
    [GeneratedCode("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [CompilerGenerated]
    [DebuggerNonUserCode]
    internal class Strings
    {
        private static ResourceManager resourceMan;
        private static CultureInfo resourceCulture;

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal static ResourceManager ResourceManager
        {
            get
            {
                if (object.ReferenceEquals((object)Strings.resourceMan, (object)null))
                    Strings.resourceMan = new ResourceManager("TypeScript.Tasks.Strings", typeof(Strings).Assembly);
                return Strings.resourceMan;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        internal static CultureInfo Culture
        {
            get
            {
                return Strings.resourceCulture;
            }
            set
            {
                Strings.resourceCulture = value;
            }
        }

        internal static string ErrorListBuildPrefix
        {
            get
            {
                return Strings.ResourceManager.GetString("ErrorListBuildPrefix", Strings.resourceCulture);
            }
        }

        internal static string NoConfigurations
        {
            get
            {
                return Strings.ResourceManager.GetString("NoConfigurations", Strings.resourceCulture);
            }
        }

        internal static string NoFilesToCompile
        {
            get
            {
                return Strings.ResourceManager.GetString("NoFilesToCompile", Strings.resourceCulture);
            }
        }

        internal static string ToolsVersionWarning
        {
            get
            {
                return Strings.ResourceManager.GetString("ToolsVersionWarning", Strings.resourceCulture);
            }
        }

        internal Strings()
        {
        }
    }
}