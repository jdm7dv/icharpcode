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
    
    public partial class TranslationsAllBranchesView
    {
        public int Id { get; set; }
        public int FK_BranchId { get; set; }
        public int FK_ResourceFileId { get; set; }
        public string ResourceIdentifier { get; set; }
        public string Culture { get; set; }
        public string TranslatedValue { get; set; }
        public System.DateTime LastUpdated { get; set; }
        public string LastUpdatedBy { get; set; }
        public Nullable<long> RowNumber { get; set; }
        public string OriginalResxValueAtTranslation { get; set; }
        public bool OriginalResxValueChangedSinceTranslation { get; set; }
        public string ResxValue { get; set; }
        public string ResxComment { get; set; }
        public System.DateTime LastUpdatedFromRepository { get; set; }
    }
}
