﻿/*
Copyright (c) 2019-2020 Integrative Software LLC
Created: 8/2019
Author: Pablo Carbonell
*/

using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Integrative.Lara;
using SampleProject.Main;

namespace SampleProject.Reactive
{
    [LaraPage(Address = PageAddress)]
    internal class RactiveCollectionPage : IPage
    {
        public const string PageAddress = "/reactor2";

        private readonly MyDataTable _data = new MyDataTable();

        public Task OnGet()
        {
            var document = LaraUI.Page.Document;
            SampleAppBootstrap.AppendTo(document.Head);
            var builder = new LaraBuilder(document.Body);
            builder.Push("div", "p-2")
                .Push("button", "btn btn-primary")
                    .AppendText("add row")
                    .On("click", () => _data.AddRow())
                .Pop()
            .Pop()
            .Push("div", "p-2")
                .Push("table")
                    .Push("tbody")
                        .BindChildren(_data.Rows, CreateRowCallback)
                    .Pop()
                .Pop()
            .Pop();
            return Task.CompletedTask;
        }

        private Element CreateRowCallback(MyDataRow dataRow)
        {
            var row = Element.Create("tr");
            var builder = new LaraBuilder(row);
            builder.Push("td")
                .Push("span")
                    .BindInnerText(dataRow, x => x.Counter.ToString())
                .Pop()
            .Pop()
            .Push("td")
                .Push("button", "btn btn-primary ml-3 mr-2 mb-1")
                    .AppendText("increase")
                    .On("click", () => dataRow.Increase())
                .Pop()
            .Pop()
            .Push("td")
                .Push("button", "btn btn-secondary mr-2")
                    .AppendText("remove")
                    .On("click", () => _data.Remove(dataRow))
                .Pop()
            .Pop()
            .Push("td")
                .Push("button", "btn btn-secondary")
                    .AppendText("Move up")
                    .BindToggleAttribute("disabled", _data.Rows, () => !dataRow.CanMoveUp())
                    .On("click", () => _data.MoveUp(dataRow))
                .Pop()
            .Pop()
            .Push("td")
                .Push("button", "btn btn-secondary")
                    .AppendText("Move down")
                    .BindToggleAttribute("disabled", _data.Rows, () => !dataRow.CanMoveDown())
                    .On("click", () => _data.MoveDown(dataRow))
                .Pop()
            .Pop();
            return row;
        }
    }

    internal class MyDataTable : BindableBase
    {
        public ObservableCollection<MyDataRow> Rows { get; } = new ObservableCollection<MyDataRow>();

        public void AddRow()
        {
            Rows.Add(new MyDataRow(this));
        }

        public void Remove(MyDataRow row)
        {
            Rows.Remove(row);
        }

        public void MoveUp(MyDataRow row) => Move(row, -1);

        public void MoveDown(MyDataRow row) => Move(row, 1);

        private void Move(MyDataRow row, int offset)
        {
            var index = row.GetIndex();
            Rows.Move(index, index + offset);
        }
    }

    internal class MyDataRow : BindableBase
    {
        private readonly MyDataTable _parent;

        public MyDataRow(MyDataTable parent)
        {
            _parent = parent;
        }

        private int _counter;
        public int Counter
        {
            get => _counter;
            set => SetProperty(ref _counter, value);
        }

        public void Increase()
        {
            Counter++;
        }

        public int GetIndex()
        {
            return _parent.Rows.IndexOf(this);
        }

        public bool CanMoveUp()
        {
            return GetIndex() > 0;
        }

        public bool CanMoveDown()
        {
            return GetIndex() < _parent.Rows.Count - 1;
        }
    }
}
