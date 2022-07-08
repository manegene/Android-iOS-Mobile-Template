﻿using habahabamall.Models;
using habahabamall.ViewModels.Onboarding;
using Syncfusion.SfRotator.XForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace habahabamall.Behaviors.Onboarding
{
    /// <summary>
    /// This class extends the behavior of the SfRotator control to animate the view.
    /// </summary>
    [Preserve(AllMembers = true)]
    public class SfRotatorBehavior : Behavior<SfRotator>
    {
        #region Fields

        private int previousIndex;

        #endregion

        #region Methods

        /// <summary>
        /// Invoke when initialize animate to view.
        /// </summary>
        /// <param name="rotator">The SfRotator</param>
        /// <param name="selectedIndex">Selected Index</param>
        public void Animation(SfRotator rotator, double selectedIndex)
        {
            if (rotator != null && rotator.ItemsSource != null && rotator.ItemsSource.Count() > 0)
            {
                int itemsCount = rotator.ItemsSource.Count();
                _ = int.TryParse(selectedIndex.ToString(), out int index);

                OnBoardingAnimationViewModel viewModel = rotator.BindingContext as OnBoardingAnimationViewModel;
                if (selectedIndex == itemsCount - 1)
                {
                    viewModel.NextButtonText = "DONE";
                    viewModel.IsSkipButtonVisible = false;
                }
                else
                {
                    viewModel.NextButtonText = "NEXT";
                    viewModel.IsSkipButtonVisible = true;
                }

                if (Device.RuntimePlatform != Device.UWP)
                {
                    List<object> items = rotator.ItemsSource.ToList();

                    // Start animation to selected view.
                    object currentItem = items[index];
                    List<View> childElement =
                        (((currentItem as Boarding).RotatorItem as ContentView).Children[0] as StackLayout).Children
                        .ToList();
                    if (childElement != null && childElement.Count > 0)
                    {
                        StartAnimation(childElement, currentItem as Boarding);
                    }

                    // Set default value to previous view.
                    if (index != previousIndex)
                    {
                        object previousItem = items[previousIndex];
                        List<View> previousChildElement =
                            (((previousItem as Boarding).RotatorItem as ContentView).Children[0] as StackLayout)
                            .Children.ToList();
                        if (previousChildElement != null && previousChildElement.Count > 0)
                        {
                            _ = previousChildElement[0].FadeTo(0);
                            _ = previousChildElement[1].FadeTo(0);
                            _ = previousChildElement[1].TranslateTo(0, 60);
                            _ = previousChildElement[1].ScaleTo(1, 0);
                            _ = previousChildElement[2].FadeTo(0);
                            _ = previousChildElement[2].TranslateTo(0, 60);
                        }
                    }

                    previousIndex = index;
                }
            }
        }

        /// <summary>
        /// Animation start to view.
        /// </summary>
        /// <param name="childElement">The Child Element</param>
        /// <param name="item">The Item</param>
        public async void StartAnimation(List<View> childElement, Boarding item)
        {
            Task<bool> fadeAnimationImage = childElement[0].FadeTo(1);
            Task<bool> fadeAnimationtaskTitleTime = childElement[1].FadeTo(1, 1000);
            Task<bool> translateAnimation = childElement[1].TranslateTo(0, 0, 500);
            Task<bool> scaleAnimationTitle = childElement[1].ScaleTo(1.5, 500, Easing.SinIn);
            Task<bool> fadeAnimationTaskDescriptionTime = childElement[2].FadeTo(1, 1000);
            Task<bool> translateDescriptionAnimation = childElement[2].TranslateTo(0, 0, 500);

            Animation animation = new Animation();
            Animation scaleDownAnimation = new Animation(v => childElement[0].Scale = v, 0.5, 1, Easing.SinIn);
            animation.Add(0, 1, scaleDownAnimation);
            animation.Commit(item.RotatorItem as ContentView, "animation", 16, 500);

            _ = await Task.WhenAll(fadeAnimationTaskDescriptionTime, fadeAnimationtaskTitleTime, translateAnimation,
                scaleAnimationTitle, translateDescriptionAnimation);
        }

        /// <summary>
        /// Invoke when adding rotator to view.
        /// </summary>
        /// <param name="rotator">The Rotator</param>
        protected override void OnAttachedTo(SfRotator rotator)
        {
            base.OnAttachedTo(rotator);
            rotator.SelectedIndexChanged += Rotator_SelectedIndexChanged;
            rotator.BindingContextChanged += Rotator_BindingContextChanged;
        }

        /// <summary>
        /// Invoke when exit from the view.
        /// </summary>
        /// <param name="rotator"></param>
        protected override void OnDetachingFrom(SfRotator rotator)
        {
            base.OnDetachingFrom(rotator);
            rotator.SelectedIndexChanged -= Rotator_SelectedIndexChanged;
            rotator.BindingContextChanged -= Rotator_BindingContextChanged;
        }

        /// <summary>
        /// Invoked when rotator binding context is changed.
        /// </summary>
        /// <param name="sender">The Sender</param>
        /// <param name="e">The event args</param>
        private void Rotator_BindingContextChanged(object sender, EventArgs e)
        {
            _ = Task.Delay(500).ContinueWith(t => Animation(sender as SfRotator, 0));
        }

        /// <summary>
        /// Invoked when selected index is changed.
        /// </summary>
        /// <param name="sender">The rotator</param>
        /// <param name="e">The selection changed event args</param>
        private void Rotator_SelectedIndexChanged(object sender, SelectedIndexChangedEventArgs e)
        {
            SfRotator rotator = sender as SfRotator;
            Animation(rotator, e.Index);
        }
    }

    #endregion
}