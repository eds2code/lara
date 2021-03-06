﻿/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 8/2019
Author: Pablo Carbonell
*/

using System.Threading.Tasks;
using Integrative.Lara;
using SampleProject.Main;

namespace SampleProject.Reactive
{
    [LaraPage(Address = PageAddress)]
    internal class ReactiveSimplePage : IPage
    {
        public const string PageAddress = "/reactor1";

        private readonly SimpleData _data = new SimpleData();

        public Task OnGet()
        {
            var document = LaraUI.Page.Document;
            SampleAppBootstrap.AppendTo(document.Head);
            var builder = new LaraBuilder(document.Body);
            builder.Push("div", "p-2")
                .Push("span")
                    .BindInnerText(_data, x => x.Counter.ToString())
                .Pop()
            .Pop()
            .Push("div", "p-2")
                .Push("button", "btn btn-primary")
                    .On("click", () => _data.IncreaseCounter())
                    .AppendText("increase")
                .Pop()
            .Pop();
            return Task.CompletedTask;
        }
    }

    internal class SimpleData : BindableBase
    {
        private int _counter;

        public int Counter
        {
            get => _counter;
            set => SetProperty(ref _counter, value);
        }

        public void IncreaseCounter()
        {
            Counter++;
        }
    }
}
