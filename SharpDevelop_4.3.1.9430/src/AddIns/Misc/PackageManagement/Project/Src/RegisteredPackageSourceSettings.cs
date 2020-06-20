﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

using NuGet;

namespace ICSharpCode.PackageManagement
{
	public class RegisteredPackageSourceSettings
	{
		public static readonly string PackageSourcesSectionName = "packageSources";
		public static readonly string ActivePackageSourceSectionName = "activePackageSource";
		public static readonly string DisabledPackageSourceSectionName = "disabledPackageSources";
		
		public static readonly PackageSource AggregatePackageSource = 
			new PackageSource("(Aggregate source)", "All");
		
		ISettings settings;
		PackageSource defaultPackageSource;
		RegisteredPackageSources packageSources;
		PackageSource activePackageSource;
		
		public RegisteredPackageSourceSettings(ISettings settings)
			: this(settings, RegisteredPackageSources.DefaultPackageSource)
		{
		}
		
		public RegisteredPackageSourceSettings(ISettings settings, PackageSource defaultPackageSource)
		{
			this.settings = settings;
			this.defaultPackageSource = defaultPackageSource;
			ReadActivePackageSource();
		}
		
		void ReadActivePackageSource()
		{
			IList<KeyValuePair<string, string>> packageSources = settings.GetValues(ActivePackageSourceSectionName);
			activePackageSource = PackageSourceConverter.ConvertFromFirstKeyValuePair(packageSources);
		}
		
		public RegisteredPackageSources PackageSources {
			get {
				if (packageSources == null) {
					ReadPackageSources();
				}
				return packageSources;
			}
		}
		
		void ReadPackageSources()
		{
			IEnumerable<PackageSource> savedPackageSources = GetPackageSourcesFromSettings();
			packageSources = new RegisteredPackageSources(savedPackageSources, defaultPackageSource);
			packageSources.CollectionChanged += PackageSourcesChanged;
			
			if (!savedPackageSources.Any()) {
				UpdatePackageSourceSettingsWithChanges();
			}
		}
		
		IEnumerable<PackageSource> GetPackageSourcesFromSettings()
		{
			IList<KeyValuePair<string, string>> savedPackageSources = settings.GetValues(PackageSourcesSectionName);
			foreach (PackageSource packageSource in PackageSourceConverter.ConvertFromKeyValuePairs(savedPackageSources)) {
				packageSource.IsEnabled = IsPackageSourceEnabled(packageSource);
				yield return packageSource;
			}
		}
		
		bool IsPackageSourceEnabled(PackageSource packageSource)
		{
			string disabled = settings.GetValue(DisabledPackageSourceSectionName, packageSource.Name);
			return String.IsNullOrEmpty(disabled);
		}
		
		void PackageSourcesChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			UpdatePackageSourceSettingsWithChanges();
		}
		
		void UpdatePackageSourceSettingsWithChanges()
		{
			IList<KeyValuePair<string, string>> newPackageSourceSettings = GetSettingsFromPackageSources();
			SavePackageSourceSettings(newPackageSourceSettings);
			IList<KeyValuePair<string, string>> disabledPackageSourceSettings = GetSettingsForDisabledPackageSources();
			SaveDisabledPackageSourceSettings(disabledPackageSourceSettings);
		}
		
		IList<KeyValuePair<string, string>> GetSettingsFromPackageSources()
		{
			return PackageSourceConverter.ConvertToKeyValuePairList(packageSources);
		}
		
		KeyValuePair<string, string> CreateKeyValuePairFromPackageSource(PackageSource source)
		{
			return new KeyValuePair<string, string>(source.Name, source.Source);
		}
		
		void SavePackageSourceSettings(IList<KeyValuePair<string, string>> newPackageSourceSettings)
		{
			settings.DeleteSection(PackageSourcesSectionName);
			settings.SetValues(PackageSourcesSectionName, newPackageSourceSettings);
		}
		
		IList<KeyValuePair<string, string>> GetSettingsForDisabledPackageSources()
		{
			return packageSources
				.Where(source => !source.IsEnabled)
				.Select(source => new KeyValuePair<string, string>(source.Name, "true"))
				.ToList();
		}
		
		void SaveDisabledPackageSourceSettings(IList<KeyValuePair<string, string>> disabledPackageSourceSettings)
		{
			settings.DeleteSection(DisabledPackageSourceSectionName);
			if (disabledPackageSourceSettings.Any()) {
				settings.SetValues(DisabledPackageSourceSectionName, disabledPackageSourceSettings);
			}
		}
		
		public PackageSource ActivePackageSource {
			get {
				if (activePackageSource != null) {
					if (activePackageSource.IsAggregate()) {
						return activePackageSource;
					}
					if (PackageSources.Contains(activePackageSource)) {
						return activePackageSource;
					}
				}
				return null;
			}
			set {
				activePackageSource = value;
				if (activePackageSource == null) {
					RemoveActivePackageSourceSetting();
				} else {
					UpdateActivePackageSourceSetting();
				}
			}
		}
		
		void RemoveActivePackageSourceSetting()
		{
			settings.DeleteSection(ActivePackageSourceSectionName);
		}
		
		void UpdateActivePackageSourceSetting()
		{
			RemoveActivePackageSourceSetting();
			
			KeyValuePair<string, string> activePackageSourceSetting = PackageSourceConverter.ConvertToKeyValuePair(activePackageSource);
			SaveActivePackageSourceSetting(activePackageSourceSetting);
		}
		
		void SaveActivePackageSourceSetting(KeyValuePair<string, string> activePackageSource)
		{
			settings.SetValue(ActivePackageSourceSectionName, activePackageSource.Key, activePackageSource.Value);
		}
	}
}
