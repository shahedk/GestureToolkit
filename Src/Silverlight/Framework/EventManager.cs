using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using TouchToolkit.Framework.Exceptions;
using TouchToolkit.Framework.Utility;
using TouchToolkit.GestureProcessor.Feedbacks.TouchFeedbacks;
using System.Windows.Controls;
using TouchToolkit.GestureProcessor.PrimitiveConditions.Validators;
using TouchToolkit.GestureProcessor.Feedbacks.GestureFeedbacks;
using System.Windows.Media;
using System.Diagnostics;
using TouchToolkit.GestureProcessor.Objects;
using TouchToolkit.Framework.TouchInputProviders;
using TouchToolkit.GestureProcessor.ReturnTypes;

namespace TouchToolkit.Framework.GestureEvents
{
    //public enum TouchInputType
    //{
    //    Silverlight,
    //    Surface,
    //    SmartTableTop
    //}

    public class EventManager
    {
        public event TouchToolkit.Framework.TouchInputProviders.TouchInputProvider.SingleTouchChangeEventHandler SingleTouchChanged;
        public event TouchToolkit.Framework.TouchInputProviders.TouchInputProvider.MultiTouchChangeEventHandler MultiTouchChanged;

        #region Constructor & singleton instance
        internal EventManager()
        {

        }

        #endregion

        #region public methods & properties


        bool _bubbleUpUnhandledEvents = false;
        public bool BubbleUpUnhandledEvents
        {
            get { return _bubbleUpUnhandledEvents; }
            set { _bubbleUpUnhandledEvents = value; }
        }

        /// <summary>
        /// Adds gesture event to specified UI element
        /// </summary>
        /// <param name="uiElement"></param>
        /// <param name="gestureName"></param>
        /// <param name="handler"></param>
        public void AddEvent(UIElement uiElement, string gestureName, GestureEventHandler handler)
        {
            Gesture g = GestureLanguageProcessor.GetGesture(gestureName);
            AddEvent(uiElement, g, handler, g.ValidationBlocks.Count - 1);
        }

        /// <summary>
        /// Register callback event for a specific step of a multi-step gesture on the specified UI element
        /// </summary>
        /// <param name="uiElement"></param>
        /// <param name="gestureName"></param>
        /// <param name="handler"></param>
        /// <param name="stepNo">The step no of a multi-step gesture definition</param>
        public void AddEvent(UIElement uiElement, string gestureName, GestureEventHandler handler, int stepNo)
        {
            Gesture g = GestureLanguageProcessor.GetGesture(gestureName);
            AddEvent(uiElement, g, handler, stepNo);
        }

        private void AddEvent(UIElement uiElement, Gesture gesture, GestureEventHandler handler, int stepNo)
        {
            if (GestureFramework.IsInitialized)
            {
                EventRequestDirectory.Add(uiElement, gesture, handler, stepNo);
            }
            else
            {
                throw new FrameworkException("You need to initialize the framework first!. Call GestureEventManager.Initialize(...) at application startup.");
            }
        }

        /// <summary>
        /// Removes the specified gesture event request for the specified UI element
        /// </summary>
        /// <param name="uiElement"></param>
        /// <param name="gestureName"></param>
        public void RemoveEvent(UIElement uiElement, string gestureName)
        {
            EventRequestDirectory.Remove(uiElement, gestureName);
        }

        #endregion


        internal void SubscribeTouchEvents()
        {
            // First, unsubscribe any exising subscription
            UnSubscribeTouchEvents();

            // Subscribe to events
            TouchInputManager.ActiveTouchProvider.FrameChanged += ActiveHardware_FrameChanged;
            TouchInputManager.ActiveTouchProvider.MultiTouchChanged += ActiveHardware_MultiTouchChanged;
            TouchInputManager.ActiveTouchProvider.SingleTouchChanged += ActiveTouchProvider_SingleTouchChanged;
        }

        private void UnSubscribeTouchEvents()
        {
            TouchInputManager.ActiveTouchProvider.FrameChanged -= ActiveHardware_FrameChanged;
            TouchInputManager.ActiveTouchProvider.MultiTouchChanged -= ActiveHardware_MultiTouchChanged;
            TouchInputManager.ActiveTouchProvider.SingleTouchChanged -= ActiveTouchProvider_SingleTouchChanged;
        }



        const int CommonBehaviourInterval = 3;
        private void ActiveHardware_FrameChanged(object sender, FrameInfo frameInfo)
        {
            foreach (var cb in GestureFramework.CommonBehaviors)
            {
                try
                {
                    cb.FrameChanged(frameInfo);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }
        }

        private void ActiveTouchProvider_SingleTouchChanged(object sender, SingleTouchEventArgs e)
        {
            if (SingleTouchChanged != null)
            {
                SingleTouchChanged(sender, e);
            }
        }

        private void ActiveHardware_MultiTouchChanged(Object sender, MultiTouchEventArgs e)
        {
            if (MultiTouchChanged != null)
                MultiTouchChanged(sender, e);

            if (e.TouchPoints.Count > 0 && e.TouchPoints[0].Action == TouchAction.Up)
            //TODO: For test only. REMOVE the above if (..) after test
            {
            // Validate pre-conditions
            ValidSetOfTouchPoints availableTouchPoints = e.TouchPoints.Copy();

                ValidSetOfPointsCollection dataToEvaluate = new ValidSetOfPointsCollection();
                dataToEvaluate.Add(availableTouchPoints);

                // Validate gestures that are in progess
                ValidateBlockResult[] list = PartiallyEvaluatedGestures.GetAll();
                foreach (ValidateBlockResult item in list)
                {
                    Gesture gesture = GestureLanguageProcessor.ActiveGestures[item.GestureName];
                    dataToEvaluate.ExpectedGestureName = gesture.Name;
                    int stepToValidate = item.ValidateBlockNo + 1;

                    if (gesture != null)
                    {
                        if (gesture.ValidationBlocks.Count > stepToValidate)
                        {
                            var result = ValidateGesture(gesture, dataToEvaluate, stepToValidate);

                            // If its the second last step then also run the last step
                            if (gesture.ValidationBlocks.Count == stepToValidate + 2)
                            {
                                stepToValidate += 1;
                                result = ValidateGesture(gesture, dataToEvaluate, stepToValidate);
                            }

                            // If its the final step of a gesture, then delete all related partial results from cache
                            if (gesture.ValidationBlocks.Count == stepToValidate + 1)
                            {
                                PartiallyEvaluatedGestures.Remove(item);
                            }

                            // Remove touch points that are used in multi-step gesture from 
                            // touch points list for new gestures
                            dataToEvaluate.Remove(result);
                        }
                    }
                }

                // Validate gestures from the first validate block as if new gestures
            foreach (Gesture gesture in GestureLanguageProcessor.ActiveGestures.Values)
            {
                    if (dataToEvaluate.Count > 0)
                        ValidateGesture(gesture, dataToEvaluate, 0);
                }
            }
        }

        /*
        private void ActiveHardware_MultiTouchChanged2(Object sender, MultiTouchEventArgs e)
        {
            if (MultiTouchChanged != null)
                MultiTouchChanged(sender, e);

            // Validate pre-conditions
            ValidSetOfTouchPoints availableTouchPoints = e.TouchPoints.Copy();

            ValidSetOfPointsCollection dataToEvaluate = new ValidSetOfPointsCollection();
            dataToEvaluate.Add(availableTouchPoints);

            /* TODO: This is a temporary implementation. Although it works properly 
             * but is not an efficient implementation. Consider using RETE algorithm
             *

            // Step 1: Validate any multi-step gestures that are in progress
            ValidateBlockResult[] list = PartiallyEvaluatedGestures.GetAll();
            foreach (ValidateBlockResult item in list)
            {
                // Get the gesture definition
                var gesture = GestureLanguageProcessor.ActiveGestures[item.GestureName];
                int blockToExecute = item.ValidateBlockNo + 1;

                if (item.Data == null)
                    dataToEvaluate.History.Clear();
                else
                    dataToEvaluate.History.Add(item.ValidateBlockName, item.Data);

                // Validate the required block
                ValidSetOfPointsCollection touchPointsUsed = ValidateGesture(gesture, dataToEvaluate, blockToExecute);

                // If gesture is detected
                if (touchPointsUsed.Count > 0)
                {
                    // Add to "PartiallyEvaludatedGestures" if its an intermediate step and has alias defined
                    if (gesture.ValidationBlocks.Count > blockToExecute)
                    {
                        string blockName = gesture.ValidationBlocks[blockToExecute].Name;

                        if (!string.IsNullOrEmpty(blockName))
                            PartiallyEvaluatedGestures.Add(gesture.Name, blockToExecute, touchPointsUsed, blockName);
                    }

                    // Remove the touch points that are used to detect a multi-step gesture
                    availableTouchPoints.Remove(touchPointsUsed);
                }
            }

            // Step 2: Validate all active gestures (as new gestures rather than partially validate gesture)
            foreach (Gesture gesture in GestureLanguageProcessor.ActiveGestures.Values)
            {
                ValidSetOfPointsCollection touchPointsUsed = ValidateGesture(gesture, dataToEvaluate, 0);

                // Remove the touch points that where used to detect this gesture
                //availableTouchPoints.Remove(touchPointsUsed);
            }
        } */

        private ValidSetOfPointsCollection ValidateGesture(Gesture gesture, ValidSetOfPointsCollection validResultSets, int blockNo)
        {
            var validateBlock = gesture.ValidationBlocks[blockNo];

            // Validate the specified block
            validResultSets = validateBlock.PrimitiveConditions.Validate(validResultSets);
            
            if (validResultSets.Count > 0)
            // current dataset satisfies the validation block
            {
                // Building return objects for each valid sets
                foreach (var validSetOfPoint in validResultSets)
                {
                    List<EventRequest> eventReqs = EventRequestDirectory.GetRequests(gesture.Name, validSetOfPoint.GetUIElements(), blockNo);

                    if (eventReqs.Count > 0)
                {
                    // Build return objects
                    List<IReturnType> returnObjs = gesture.ReturnTypes.Calculate(validSetOfPoint);

                    // Execute gesture effects, if any
                    if (GestureFramework.GestureFeedbacks.ContainsKey(gesture.Name))
                    {
                        var gestureFeeds = GestureFramework.GestureFeedbacks[gesture.Name];
                        foreach (Type gestureFeedbackTyoe in gestureFeeds)
                        {
                            ExecuteFeedback(gestureFeedbackTyoe, returnObjs);
                        }
                    }

                    // Invoke the callback to notify the subscriber(s)
                    foreach (var eventRequest in eventReqs)
                    {
                        GestureEventArgs e = new GestureEventArgs() { Values = returnObjs };
                        eventRequest.EventHandler(eventRequest.UIElement, e);
                    }
                }
                }

                // Its one of the validate blocks of a multi-step gesture (and it is not the last block)

                if (!string.IsNullOrEmpty(validateBlock.Name) && gesture.ValidationBlocks.Count != blockNo + 1)
                {
                    // Save the partial result in cache
                    PartiallyEvaluatedGestures.Add(gesture.Name, blockNo, validResultSets, validateBlock.Name);
                }
            }

            return validResultSets;
        }

        //        private bool IsPointOriginatedOnElement(TouchPoint2 point, UIElement uIElement)
        //        {
        //            if (point.Tags.ContainsKey(uIElement))
        //            {
        //                return (point.Tags[uIElement] as bool?) == true;
        //            }
        //            else
        //            {
        //                // Get all elements on the position of the point
        //                bool result = false;

        //                //TODO: Find a way to do hit-test in WPF 4.0
        //#if SILVERLIGHT
        //                var elements = VisualTreeHelper.FindElementsInHostCoordinates(point.StartPoint.ToPoint(), GestureFramework.LayoutRoot);
        //                foreach (var uiItem in elements)
        //                {
        //                    if (uiItem == uIElement)
        //                    {
        //                        result = true;
        //                        break;
        //                    }
        //                }
        //#else
        //                // Code block for : .NET Framework 4.0 / WPF 4.0 
        //#endif
        //                point.Tags[uIElement] = result;

        //                return result;
        //            }
        //        }

        private void ExecuteFeedback(Type type, List<IReturnType> returnObjects)
        {
            (Activator.CreateInstance(type) as IGestureFeedback).RenderUI(GestureFramework.LayoutRoot.Dispatcher, GestureFramework.LayoutRoot, returnObjects);
        }

    }
}
