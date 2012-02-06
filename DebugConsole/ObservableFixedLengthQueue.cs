using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using Microsoft.Research.DynamicDataDisplay.DataSources;
using System.Windows;

namespace Gearset
{
    /// <summary>
    /// This class is used to bind data to a DynamicDataDisplay 
    /// ChartPlotter from a FixedLengthQueue
    /// </summary>
    //public class FixedLengthPointSource : IPointDataSource
    //{
    //    public event EventHandler DataChanged;
    //    private DataSampler sampler;

    //    public FixedLengthPointSource(DataSampler sampler)
    //    {
    //        this.sampler = sampler;
    //        sampler.Values.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Values_CollectionChanged);
    //    }

    //    void Values_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    //    {
    //        if (DataChanged != null)
    //            DataChanged(this, EventArgs.Empty);
    //    }


    //    public IPointEnumerator GetEnumerator(System.Windows.DependencyObject context)
    //    {
    //        return new ObservableIterator(this);
    //    }

    //    private struct ObservableIterator : IPointEnumerator, IDisposable
    //    {
    //        private readonly FixedLengthPointSource dataSource;
    //        private readonly IEnumerator<float> enumerator;
    //        private int x;

    //        public ObservableIterator(FixedLengthPointSource dataSource)
    //        {
    //            this.dataSource = dataSource;
    //            this.enumerator = dataSource.sampler.Values.GetEnumerator();
    //            this.x = 0;
    //        }

    //        public void ApplyMappings(DependencyObject target)
    //        {
    //            //if (target != null)
    //            //{
    //            //    foreach (Mapping<T> mapping in this.mappings)
    //            //    {
    //            //        target.SetValue(mapping.Property, mapping.F.Invoke(GetCurrent));
    //            //    }
    //            //}

    //        }

    //        public void Dispose()
    //        {
    //            this.enumerator.Dispose();
    //            GC.SuppressFinalize(this);
    //        }

    //        public void GetCurrent(ref Point p)
    //        {
    //            p.X = x;
    //            p.Y = enumerator.Current;
    //        }

    //        public bool MoveNext()
    //        {
    //            x++;
    //            return this.enumerator.MoveNext();
    //        }
    //    }

    //}
}
