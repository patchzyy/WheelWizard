﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CT_MKWII_WPF.Resources.Languages {
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
    public class Phrases {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Phrases() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        public static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("CT_MKWII_WPF.Resources.Languages.Phrases", typeof(Phrases).Assembly);
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
        public static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Talk to us!.
        /// </summary>
        public static string Sidebar_Link_Discord {
            get {
                return ResourceManager.GetString("Sidebar_Link_Discord", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Source code.
        /// </summary>
        public static string Sidebar_Link_Github {
            get {
                return ResourceManager.GetString("Sidebar_Link_Github", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Support us!.
        /// </summary>
        public static string Sidebar_Link_Support {
            get {
                return ResourceManager.GetString("Sidebar_Link_Support", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Made by: {$1} \n And {$2}.
        /// </summary>
        public static string Sidebar_MadeByString {
            get {
                return ResourceManager.GetString("Sidebar_MadeByString", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Thanks a lot to all the translators:.
        /// </summary>
        public static string Text_ThanksTranslators {
            get {
                return ResourceManager.GetString("Text_ThanksTranslators", resourceCulture);
            }
        }
    }
}