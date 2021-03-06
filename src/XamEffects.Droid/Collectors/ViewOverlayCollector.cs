﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using Android.Views;
using Android.Widget;
using Java.Util;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using View = Android.Views.View;

namespace XamEffects.Droid.Collectors
{
    public static class ViewOverlayCollector
    {
        private static Dictionary<ViewGroup, ViewContainer> Collection { get; } = new Dictionary<ViewGroup, ViewContainer>();

	    public static void Init()
	    {
		    
	    }

        public static FrameLayout AddOrGet(ViewGroup group, object instance)
        {
            if (Collection.ContainsKey(group))
            {
                Collection[group].Objects.Add(instance);
                return Collection[group].OverlayView;
            }

			var view = new FrameLayout(group.Context)
            {
                LayoutParameters = new ViewGroup.LayoutParams(-1, -1)
            };
            group.LayoutChange += ViewOnLayoutChange;
            //group.AddView(view);
            //view.BringToFront();

	        var vc = new ViewContainer
	        {
		        OverlayView = view,
		        Objects =
		        {
			        instance
		        }
	        };

			Collection.Add(group, vc);

            return view;
        }

        public static void TryDelete(ViewGroup group, object instance)
        {
            if (Collection.ContainsKey(group))
            {
                Collection[group].Objects.Remove(instance);
                if (Collection[group].Objects.Count == 0)
                {
                    group.LayoutChange -= ViewOnLayoutChange;
                    group.RemoveView(Collection[group].OverlayView);
                    Collection.Remove(group);
                }
            }
        }

        private static void ViewOnLayoutChange(object sender, View.LayoutChangeEventArgs layoutChangeEventArgs)
        {
            var group = ((ViewGroup)sender);
            if (Collection.ContainsKey(group))
            {
                Collection[group].OverlayView.Right = group.Width;
                Collection[group].OverlayView.Bottom = group.Height;
            }
        }

		private class ViewContainer
        {
            public FrameLayout OverlayView { get; set; }

            public HashSet<object> Objects { get; } = new HashSet<object>();
        }
    }
}