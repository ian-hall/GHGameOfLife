﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.34014
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GHGameOfLife {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class SmallPops {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal SmallPops() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("GHGameOfLife.SmallPops", typeof(SmallPops).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 0100000
        ///0001000
        ///1100111.
        /// </summary>
        internal static string Acorn {
            get {
                return ResourceManager.GetString("Acorn", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 11101
        ///10000
        ///00011
        ///01101
        ///10101.
        /// </summary>
        internal static string BlockLayer {
            get {
                return ResourceManager.GetString("BlockLayer", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 010
        ///001
        ///111.
        /// </summary>
        internal static string Glider {
            get {
                return ResourceManager.GetString("Glider", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to 01001
        ///10000
        ///10001
        ///11110.
        /// </summary>
        internal static string Smallship {
            get {
                return ResourceManager.GetString("Smallship", resourceCulture);
            }
        }
    }
}
