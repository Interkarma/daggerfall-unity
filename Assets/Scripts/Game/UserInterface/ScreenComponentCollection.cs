// Project:         Daggerfall Unity
// Copyright:       Copyright (C) 2009-2022 Daggerfall Workshop
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

namespace DaggerfallWorkshop.Game.UserInterface
{
    /// <summary>
    /// A list of screen components with events.
    /// The order components are added in determines the order they are updated and drawn.
    /// </summary>
    public class ScreenComponentCollection : IEnumerable<BaseScreenComponent>
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
            components.Add(component);
            component.Parent = parent;
            RaiseComponentAddedEvent(component);
        }

        public void Remove(BaseScreenComponent component)
        {
            components.Remove(component);
            RaiseComponentRemovedEvent(component);
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
        /// Gets <see cref="IEnumerator{BaseScreenComponent}"/> for the component collection
        /// for use with foreach.
        /// </summary>
        /// <returns><see cref="IEnumerator"/> of screen components.</returns>
        public IEnumerator<BaseScreenComponent> GetEnumerator()
        {
            return components.GetEnumerator();
        }

        /// <summary>
        /// Gets <see cref="IEnumerator"/> for the component collection.
        /// </summary>
        /// <returns>IEnumerator object.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return components.GetEnumerator();
        }

        #endregion

        #region ComponentAdded Event

        /// <summary>
        /// This event is fired whenever a component is added.
        /// </summary>
        public event ComponentAddedEventHandler OnComponentAdded;
        public delegate void ComponentAddedEventHandler(object sender, ComponentEventArgs e);

        /// <summary>
        /// This event is fired whenever a component is removed.
        /// </summary>
        public event ComponentAddedEventHandler OnComponentRemoved;
        public delegate void ComponentRemovedEventHandler(object sender, ComponentEventArgs e);

        /// <summary>
        /// Event arguments.
        /// </summary>
        public class ComponentEventArgs : EventArgs
        {
            public object Component;
        }

        /// <summary>
        /// Raise component added event.
        /// </summary>
        protected virtual void RaiseComponentAddedEvent(BaseScreenComponent component)
        {
            // Raise event
            if (OnComponentAdded != null)
            {
                // Popuate event arguments
                ComponentEventArgs e = new ComponentEventArgs()
                {
                    Component = component,
                };

                // Raise event
                OnComponentAdded(this, e);
            }
        }

        /// <summary>
        /// Raise component removed event.
        /// </summary>
        protected virtual void RaiseComponentRemovedEvent(BaseScreenComponent component)
        {
            // Raise event
            if (OnComponentRemoved != null)
            {
                // Popuate event arguments
                ComponentEventArgs e = new ComponentEventArgs()
                {
                    Component = component,
                };

                // Raise event
                OnComponentRemoved(this, e);
            }
        }

        #endregion
    }
}
