using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatchDataEntry.Components
{
    public class NavigationList<T> : List<T>
    {
        private int _currentIndex = 0;
        public int CurrentIndex
        {
            get
            {
                if (_currentIndex > Count - 1)
                {
                    _currentIndex = Count - 1;
                }
                if(_currentIndex < 0 ) { _currentIndex = 0; }
                return _currentIndex;
            }
            set { _currentIndex = value; }
        }

        public T MoveNext
        {
            get {
                if(hasNext)
                    _currentIndex++;
                return this[CurrentIndex]; }
        }

        public bool hasNext
        {
          get { return (_currentIndex < this.Count - 1); }         
        }

        public bool hasPrevious
        {
            get { return (_currentIndex > 0); }
        }

        public T MovePrevious
        {
            get {
                if (hasPrevious)
                    _currentIndex--;
                 return this[CurrentIndex];
            }
        }

        public T Current
        {
            get { return this[CurrentIndex]; }
        }
    }
}
