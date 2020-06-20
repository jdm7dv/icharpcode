﻿// Copyright (c) AlphaSierraPapa for the SharpDevelop Team (for details please see \doc\copyright.txt)
// This code is distributed under the GNU LGPL (for details please see \doc\license.txt)

using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace ICSharpCode.AspNet.Mvc
{
	public static class SlideBehaviour
	{
		public static readonly DependencyProperty FrameworkElementHiddenBySlideProperty =
			DependencyProperty.RegisterAttached(
				"FrameworkElementHiddenBySlide",
				typeof(FrameworkElement),
				typeof(SlideBehaviour),
				new UIPropertyMetadata(null));
		
		public static FrameworkElement GetFrameworkElementHiddenBySlide(FrameworkElement frameworkElement)
		{
			return frameworkElement.GetValue(FrameworkElementHiddenBySlideProperty) as FrameworkElement;
		}
		
		public static void SetFrameworkElementHiddenBySlide(FrameworkElement frameworkElement, FrameworkElement hiddenFrameworkElement)
		{
			frameworkElement.SetValue(FrameworkElementHiddenBySlideProperty, hiddenFrameworkElement);
		}
		
		public static readonly DependencyProperty IsDisplayedProperty =
			DependencyProperty.RegisterAttached(
				"IsDisplayed",
				typeof(bool),
				typeof(SlideBehaviour),
				new UIPropertyMetadata(false, OnIsDisplayedChanged));
		
		public static bool GetIsDisplayed(FrameworkElement frameworkElement)
		{
			return (bool)frameworkElement.GetValue(IsDisplayedProperty);
		}
		
		public static void SetIsDisplayed(FrameworkElement frameworkElement, bool value)
		{
			frameworkElement.SetValue(IsDisplayedProperty, value);
		}
		
		static void OnIsDisplayedChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs  e)
		{
			var frameworkElement = dependencyObject as FrameworkElement;
			if (frameworkElement == null) {
				return;
			}
			
			if (e.NewValue is bool) {
				if ((bool)e.NewValue) {
					SlideIn(frameworkElement);
				} else {
					SlideOut(frameworkElement);
				}
			}
		}
		
		static void SlideIn(FrameworkElement frameworkElement)
		{
			AnimationTimeline marginAnimation = CreateSlideInAnimation(frameworkElement);
			frameworkElement.BeginAnimation(FrameworkElement.MarginProperty, marginAnimation);
			frameworkElement.Visibility = Visibility.Visible;
			DisableHiddenFrameworkElement(frameworkElement);
		}
		
		static AnimationTimeline CreateSlideInAnimation(FrameworkElement frameworkElement)
		{
			var parent = frameworkElement.Parent as FrameworkElement;
			
			var marginAnimation = new ThicknessAnimation();
			marginAnimation.From = new Thickness(parent.ActualWidth, 0, 0, 0);
			marginAnimation.To = new Thickness(0);
			marginAnimation.Duration = duration;
			return marginAnimation;
		}
		
		static readonly Duration duration = new Duration(new TimeSpan(TimeSpan.TicksPerMillisecond * 100));
		
		static void DisableHiddenFrameworkElement(FrameworkElement frameworkElement)
		{
			EnableHiddenFrameworkElement(frameworkElement, enabled: false);
		}
		
		static void EnableHiddenFrameworkElement(FrameworkElement frameworkElement, bool enabled)
		{
			FrameworkElement hiddenFrameworkElement = GetFrameworkElementHiddenBySlide(frameworkElement);
			if (hiddenFrameworkElement != null) {
				hiddenFrameworkElement.IsEnabled = enabled;
			}
		}
		
		static void SlideOut(FrameworkElement frameworkElement)
		{
			AnimationTimeline marginAnimation = CreateSlideOutAnimation(frameworkElement);
			frameworkElement.BeginAnimation(FrameworkElement.MarginProperty, marginAnimation);
		}
		
		static AnimationTimeline CreateSlideOutAnimation(FrameworkElement frameworkElement)
		{
			var parent = frameworkElement.Parent as FrameworkElement;
			
			var marginAnimation = new ThicknessAnimation();
			marginAnimation.From = new Thickness(0);
			marginAnimation.To = new Thickness(parent.ActualWidth, 0, 0, 0);
			marginAnimation.Duration = duration;
			marginAnimation.Completed += (sender, e) => OnSlideOutCompleted(frameworkElement);
			
			return marginAnimation;
		}
		
		static void OnSlideOutCompleted(FrameworkElement frameworkElement)
		{
			frameworkElement.Visibility = Visibility.Collapsed;
			EnableHiddenFrameworkElement(frameworkElement);
		}

		static void EnableHiddenFrameworkElement(FrameworkElement frameworkElement)
		{
			EnableHiddenFrameworkElement(frameworkElement, true);
		}
	}
}
