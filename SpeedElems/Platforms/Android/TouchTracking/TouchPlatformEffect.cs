﻿using Android.Views;
using Microsoft.Maui.Controls.Platform;
using Microsoft.Maui.Platform;

namespace SpeedElems.TouchTracking;

/// <summary>
/// Touch Platform Effect
/// </summary>
public partial class TouchPlatformEffect : PlatformEffect
{
    private Android.Views.View view;
    private Element formsElement;
    private TouchPlatformEffect libTouchEffect;
    private bool capture;
    private Func<double, double> fromPixels;
    private int[] twoIntArray = new int[2];

    private static Dictionary<Android.Views.View, TouchPlatformEffect> viewDictionary = new Dictionary<Android.Views.View, TouchPlatformEffect>();

    private static Dictionary<int, TouchPlatformEffect> idToEffectDictionary = new Dictionary<int, TouchPlatformEffect>();

    public event TouchActionEventHandler TouchAction;

    public bool Capture { set; get; }

    public void OnTouchAction(Element element, TouchActionEventArgs args)
    {
        TouchAction?.Invoke(element, args);
    }

    protected override void OnAttached()
    {
        // Get the Android View corresponding to the Element that the effect is attached to
        view = Control == null ? Container : Control;

        // Get access to the TouchEffect class in the .NET Standard library
        var touchEffect = (TouchPlatformEffect)Element.Effects.FirstOrDefault(e => e is TouchPlatformEffect);

        if (touchEffect != null && view != null)
        {
            viewDictionary.Add(view, this);
            formsElement = Element;
            libTouchEffect = touchEffect;
            // Save fromPixels function
            fromPixels = view.Context.FromPixels;
            // Set event handler on View
            view.Touch += OnTouch;
        }
    }

    protected override void OnDetached()
    {
        if (viewDictionary.ContainsKey(view))
        {
            viewDictionary.Remove(view);
            idToEffectDictionary.Clear();
            view.Touch -= OnTouch;
        }
    }

    private void OnTouch(object? sender, Android.Views.View.TouchEventArgs args)
    {
        // Two object common to all the events
        Android.Views.View senderView = sender as Android.Views.View;
        MotionEvent motionEvent = args.Event;

        // Get the pointer index
        int pointerIndex = motionEvent.ActionIndex;

        // Get the id that identifies a finger over the course of its progress
        int id = motionEvent.GetPointerId(pointerIndex);

        senderView.GetLocationOnScreen(twoIntArray);
        var screenPointerCoords = new Point(twoIntArray[0] + (int)motionEvent.GetX(pointerIndex), twoIntArray[1] + (int)motionEvent.GetY(pointerIndex));

        // Use ActionMasked here rather than Action to reduce the number of possibilities
        switch (args.Event.ActionMasked)
        {
            case MotionEventActions.Down:
            case MotionEventActions.PointerDown:
                FireEvent(this, id, TouchActionType.Pressed, screenPointerCoords, true);
                idToEffectDictionary.Add(id, this);
                capture = libTouchEffect.Capture;
                break;

            case MotionEventActions.Move:
                // Multiple Move events are bundled, so handle them in a loop
                for (pointerIndex = 0; pointerIndex < motionEvent.PointerCount; pointerIndex++)
                {
                    id = motionEvent.GetPointerId(pointerIndex);

                    if (capture)
                    {
                        senderView.GetLocationOnScreen(twoIntArray);
                        screenPointerCoords = new Point(twoIntArray[0] + (int)motionEvent.GetX(pointerIndex),
                                                        twoIntArray[1] + (int)motionEvent.GetY(pointerIndex));
                        FireEvent(this, id, TouchActionType.Moved, screenPointerCoords, true);
                    }
                    else
                    {
                        CheckForBoundaryHop(id, screenPointerCoords);
                        if (idToEffectDictionary[id] != null)
                            FireEvent(idToEffectDictionary[id], id, TouchActionType.Moved, screenPointerCoords, true);
                    }
                }
                break;

            case MotionEventActions.Up:
            case MotionEventActions.Pointer1Up:
            case MotionEventActions.Cancel:
                if (capture)
                    FireEvent(this, id, TouchActionType.Released, screenPointerCoords, false);
                else
                {
                    CheckForBoundaryHop(id, screenPointerCoords);
                    if (idToEffectDictionary[id] != null)
                        FireEvent(idToEffectDictionary[id], id, TouchActionType.Released, screenPointerCoords, false);
                }
                idToEffectDictionary.Remove(id);
                break;
        }
    }

    private void CheckForBoundaryHop(int id, Point pointerLocation)
    {
        TouchPlatformEffect touchEffectHit = null;

        foreach (Android.Views.View view in viewDictionary.Keys)
        {
            // Get the view rectangle
            try
            {
                view.GetLocationOnScreen(twoIntArray);
            }
            catch // System.ObjectDisposedException: Cannot access a disposed object.
            {
                continue;
            }
            Rect viewRect = new Rect(twoIntArray[0], twoIntArray[1], view.Width, view.Height);

            if (viewRect.Contains(pointerLocation))
            {
                touchEffectHit = viewDictionary[view];
            }
        }

        if (touchEffectHit != idToEffectDictionary[id])
        {
            if (idToEffectDictionary[id] != null)
            {
                FireEvent(idToEffectDictionary[id], id, TouchActionType.Exited, pointerLocation, true);
            }
            if (touchEffectHit != null)
            {
                FireEvent(touchEffectHit, id, TouchActionType.Entered, pointerLocation, true);
            }
            idToEffectDictionary[id] = touchEffectHit;
        }
    }

    private void FireEvent(TouchPlatformEffect touchEffect, int id, TouchActionType actionType, Point pointerLocation, bool isInContact)
    {
        // Get the method to call for firing events
        Action<Element, TouchActionEventArgs> onTouchAction = touchEffect.libTouchEffect.OnTouchAction;

        // Get the location of the pointer within the view
        touchEffect.view.GetLocationOnScreen(twoIntArray);
        double x = pointerLocation.X - twoIntArray[0];
        double y = pointerLocation.Y - twoIntArray[1];
        Point point = new Point(fromPixels(x), fromPixels(y));

        // Call the method
        onTouchAction(touchEffect.formsElement, new TouchActionEventArgs(id, actionType, point, isInContact));
    }
}