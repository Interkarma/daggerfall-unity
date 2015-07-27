// Project:         Daggerfall Tools For Unity
// Copyright:       Copyright (C) 2009-2015 Daggerfall Workshop
// Web Site:        http://www.dfworkshop.net
// License:         MIT License (http://www.opensource.org/licenses/mit-license.php)
// Source Code:     https://github.com/Interkarma/daggerfall-unity
// Original Author: Gavin Clayton (interkarma@dfworkshop.net)
// Contributors:    
// 
// Notes:
//

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace DaggerfallWorkshop.Demo.UserInterface
{
    /// <summary>
    /// A list of screen components with events.
    /// The order components are added in determines the order they are updated and drawn.
    /// </summary>
    public class ScreenComponentCollection : IEnumerable
    {
        #region Fields

        BaseScreenComponent parent;
        List<BaseScreenComponent> components = new List<BaseScreenComponent>();

        #endregion

        #region Properties

        /// <summary>
        /// Gets the number of components contained in collection.
        /// </summary>
        public int Count
        {
            get { return components.Count; }
        }

        /// <summary>
        /// Gets component at index.
        /// </summary>
        /// <param name="index">Index of component.</param>
        /// <returns>Reference to component.</returns>
        public BaseScreenComponent this[int index]
        {
            get { return components[index]; }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public ScreenComponentCollection()
        {
            this.parent = null;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="parent">Base component hosting this collection.</param>
        public ScreenComponentCollection(BaseScreenComponent parent)
            : base()
        {
            this.parent = parent;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Adds a component to collection.
        /// </summary>
        /// <param name="component">Component to add.</param>
        public void Add(BaseScreenComponent component)
        {
            // Add component
            components.Add(component);

            // Assign new parent
            component.Parent = parent;

            // Raise event
            RaiseComponentAddedEvent(component);
        }

        /// <summary>
        /// Clears the collection.
        /// </summary>
        public void Clear()
        {
            components.Clear();
        }

        #endregion

        #region IEnumerable

        /// <summary>
        /// Gets IEnumerator for the component collection.
        /// </summary>
        /// <returns>IEnumerator object.</returns>
        public IEnumerator GetEnumerator()
        {
            return (components as IEnumerable).GetEnumerator();
        }

        #endregion

        #region ComponentAdded Event

        /// <summary>
        /// This event is fired whenever a component is added, allowing the entity to overlay
        ///  any special handling required.
        /// </summary>
        public event ComponentAddedEventHandler ComponentAdded;
        public delegate void ComponentAddedEventHandler(object sender, ComponentAddedEventArgs e);

        /// <summary>
        /// Event arguments.
        /// </summary>
        public class ComponentAddedEventArgs : EventArgs
        {
            public object Component;
        }

        /// <summary>
        /// Raise event.
        /// </summary>
        protected virtual void RaiseComponentAddedEvent(BaseScreenComponent component)
        {
            // Raise event
            if (null != ComponentAdded)
            {
                // Popuate event arguments
                ComponentAddedEventArgs e = new ComponentAddedEventArgs()
                {
                    Component = component,
                };

                // Raise event
                ComponentAdded(this, e);
            }
        }

        #endregion
    }
}
