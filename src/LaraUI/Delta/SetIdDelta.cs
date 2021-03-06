﻿/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 5/2019
Author: Pablo Carbonell
*/

using System.Runtime.Serialization;

namespace Integrative.Lara
{
    [DataContract]
    internal sealed class SetIdDelta : BaseDelta
    {
        [DataMember]
        public ElementLocator? Locator { get; set; }

        [DataMember]
        public string? NewId { get; set; }

        public SetIdDelta() : base(DeltaType.SetId)
        {
        }

        public static void Enqueue(Element element, string? newValue)
        {
            if (!element.TryGetQueue(out var document)) return;
            var locator = ElementLocator.FromElement(element);
            document.Enqueue(new SetIdDelta
            {
                Locator = locator,
                NewId = newValue
            });
        }
    }
}
