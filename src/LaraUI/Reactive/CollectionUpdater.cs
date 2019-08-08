﻿/*
Copyright (c) 2019 Integrative Software LLC
Created: 8/2019
Author: Pablo Carbonell
*/

using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Integrative.Lara.Reactive
{
    class CollectionUpdater<T>
        where T : INotifyPropertyChanged
    {
        readonly BindChildrenOptions<T> _options;
        readonly Element _element;
        readonly NotifyCollectionChangedEventArgs _args;

        public CollectionUpdater(BindChildrenOptions<T> options,
            Element element,
            NotifyCollectionChangedEventArgs args)
        {
            _options = options;
            _element = element;
            _args = args;
        }

        public void Run()
        {
            switch (_args.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    CollectionAdd();
                    break;
                case NotifyCollectionChangedAction.Move:
                    CollectionMove();
                    break;
                case NotifyCollectionChangedAction.Remove:
                    CollectionRemove();
                    break;
                case NotifyCollectionChangedAction.Replace:
                    CollectionReplace();
                    break;
                case NotifyCollectionChangedAction.Reset:
                default:
                    CollectionReset(_options, _element);
                    break;
            }
        }

        private void CollectionAdd()
        {
            var item = (T)_args.NewItems[0];
            var childElement = _options.CreateCallback(item);
            _element.AppendChild(childElement);
        }

        private void CollectionMove()
        {
            var value = (T)_args.NewItems[0];
            var removeIndex = _args.OldStartingIndex;
            var addIndex = _args.NewStartingIndex;
            var child = _options.CreateCallback(value);
            RemoveAt(removeIndex);
            InsertAt(addIndex, child);
        }

        private void CollectionRemove()
        {
            var index = _args.OldStartingIndex;
            RemoveAt(index);
        }

        private void CollectionReplace()
        {
            var value = (T)_args.NewItems[0];
            var index = _args.OldStartingIndex;
            var childElement = _options.CreateCallback(value);
            RemoveAt(index);
            InsertAt(index, childElement);
        }

        private void RemoveAt(int index)
        {
            var child = _element.GetChildAt(index);
            if (child is Element element)
            {
                _options.DisposeCallback(element);
            }
            _element.RemoveAt(index);
        }

        private void InsertAt(int index, Element child)
        {
            _element.InsertChildAt(index, child);
        }

        private static void CollectionReset(BindChildrenOptions<T> options, Element element)
        {
            NotifyDeleted(options, element);
            element.ClearChildren();
        }

        private static void NotifyDeleted(BindChildrenOptions<T> options, Element element)
        {
            if (options.DisposeCallback == null)
            {
                return;
            }
            var list = new List<Node>();
            list.AddRange(element.Children);
            foreach (var node in list)
            {
                if (node is Element childElement)
                {
                    options.DisposeCallback(childElement);
                }
            }
        }

        public static void CollectionLoad(BindChildrenOptions<T> options, Element element)
        {
            CollectionReset(options, element);
            foreach (var item in options.Collection)
            {
                var child = options.CreateCallback(item);
                element.AppendChild(child);
            }
        }
    }
}
