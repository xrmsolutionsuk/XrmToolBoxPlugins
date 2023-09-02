using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace XrmSolutionsUK.XrmToolBoxPlugins.ManagedSolutionLayerRaiser.BusinessLogic
{
    internal class SortableBindingList<T> : BindingList<T> where T : class
    {
        private bool _isSorted;
        private ListSortDirection _sortDirection = ListSortDirection.Ascending;
        private PropertyDescriptor _sortyProperty;

        public SortableBindingList() 
        { 
        
        }

        public SortableBindingList(IList<T> list) : base(list)
        { 
        }

        protected override bool SupportsSortingCore
        {
            get { return true; }
        }

        protected override bool IsSortedCore
        {
            get { return _isSorted; }
        }

        protected override ListSortDirection SortDirectionCore
        {
            get { return _sortDirection; }
        }

        protected override PropertyDescriptor SortPropertyCore
        {
            get { return _sortyProperty; }
        }

        protected override void RemoveSortCore()
        {
            _sortDirection = ListSortDirection.Ascending;
            _sortyProperty = null;
            _isSorted = false;
        }

        protected override void ApplySortCore(PropertyDescriptor prop, ListSortDirection direction)
        {
            _sortyProperty = prop;
            _sortDirection = direction;

            List<T> list = Items as List<T>;
            if (list == null) return;
            list.Sort(Compare);
            _isSorted = true;
            OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }

        private int Compare(T lhs, T rhs)
        {
            var result = OnComparison(lhs, rhs);
            if (_sortDirection == ListSortDirection.Descending)
                result = -result; ;
            return result;
        }

        private int OnComparison(T lhs, T rhs)
        {
            object lhsValue = lhs == null ? null:_sortyProperty.GetValue(lhs);
            object rhsValue = rhs == null ? null :_sortyProperty.GetValue(rhs);

            if (lhsValue == null)
            {
                return (rhsValue == null) ? 0: -1;
            }

            if (rhsValue == null)
            {
                return 1;
            }

            if (lhsValue is IComparable)
            {
                return ((IComparable)lhsValue).CompareTo(rhsValue);
            }

            if (lhsValue.Equals(rhsValue))
            {
                return 0;
            }
            return lhsValue.ToString().CompareTo(rhsValue.ToString());
        }
    }
}
