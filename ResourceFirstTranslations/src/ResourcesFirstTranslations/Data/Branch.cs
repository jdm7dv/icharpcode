//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ResourcesFirstTranslations.Data
{
    using System;
    using System.Collections.Generic;
    
    public partial class Branch
    {
        public Branch()
        {
            this.BranchResourceFiles = new HashSet<BranchResourceFile>();
            this.ResourceStrings = new HashSet<ResourceString>();
            this.Translations = new HashSet<Translation>();
        }
    
        public int Id { get; set; }
        public string BranchDisplayName { get; set; }
        public string BranchRootUrl { get; set; }
    
        public virtual ICollection<BranchResourceFile> BranchResourceFiles { get; set; }
        public virtual ICollection<ResourceString> ResourceStrings { get; set; }
        public virtual ICollection<Translation> Translations { get; set; }
    }
}
