using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Framework;
using System.Collections.Generic;
using System.Linq;

using Gestures.Objects;

namespace Framework
{
    internal class EventRequest
    {
        public Gesture Gesture { get; internal set; }
        public GestureEventHandler EventHandler { get; set; }
        public UIElement UIElement { get; set; }
    }

    internal class EventRequestDirectory
    {
        private static List<EventRequest> eventRequests = new List<EventRequest>();

        /// <summary>
        /// Adds the gesture event request to the end of the existing requests collection
        /// </summary>
        /// <param name="uiElement"></param>
        /// <param name="gestureName"></param>
        /// <param name="handler"></param>
        public static void Add(UIElement uiElement, string gestureName, GestureEventHandler handler)
        {
            Gesture gesture = GestureLanguageProcessor.GetGesture(gestureName);

            // Add new record in the list
            eventRequests.Add(new EventRequest()
            {
                UIElement = uiElement,
                Gesture = gesture,
                EventHandler = handler
            });
        }

        /// <summary>
        /// Removes the specified gesture event request for the specified UI element
        /// </summary>
        /// <param name="uiElement"></param>
        /// <param name="gestureName"></param>
        public static void Remove(UIElement uiElement, string gestureName)
        {
            var reqs = GetRequests(gestureName, uiElement);
            foreach (var req in reqs)
            {
                eventRequests.Remove(req);
            }
        }

        /// <summary>
        /// Returns the unique set of UI elements that has one or more gesture events attached to it
        /// </summary>
        /// <returns></returns>
        public static dynamic GetAllUIElements()
        {
            dynamic uiElements = (from r in eventRequests
                              select new { r.UIElement }).Distinct();

            return uiElements;
        }

        internal static List<EventRequest> GetRequests(string gestureName)
        {
            var requests = from r in eventRequests
                           where r.Gesture.Name == gestureName 
                           select r;

            return requests.ToList<EventRequest>();
        }

        internal static List<EventRequest> GetRequests(string gestureName, UIElement uiElement)
        {
            var requests = from r in eventRequests
                           where r.Gesture.Name == gestureName && r.UIElement == uiElement
                           select r;

            return requests.ToList<EventRequest>();
        }

        internal static List<EventRequest> GetRequests(UIElement uiElement)
        {
            var requests = from r in eventRequests
                           where r.UIElement == uiElement
                           select r;

            return requests.ToList<EventRequest>();
        }

        /// <summary>
        /// Determines whether the speficied UI element has any gestures attached to it.
        /// </summary>
        /// <param name="uiElement"></param>
        /// <returns></returns>
        public static bool Exists(UIElement uiElement)
        {
            var requests = from r in eventRequests
                           where r.UIElement == uiElement
                           select r;

            return (requests.Count() > 0) ;
        }
    }
}
