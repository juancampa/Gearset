using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gearset.Components;
using Microsoft.Xna.Framework;
using System.Collections.ObjectModel;

namespace Gearset.Components
{
    /// <summary>
    /// Keeps the history of a function.
    /// </summary>
    public class DataSampler
    {
        public String Name { get; private set; }
        public int SampleRate { get; set; }

        public Func<float> Function { get; private set; }
        public Func<float, float> Function2 { get; private set; }

        public FixedLengthQueue<float> Values;
        private int elapsedFrames;

        public DataSampler(String name)
        {
            this.Name = name;
            this.SampleRate = 1;
            Values = new FixedLengthQueue<float>(GearsetResources.Console.Settings.DataSamplerConfig.DefaultHistoryLength);
        }

        public DataSampler(String name, int historyLength)
        {
            this.Name = name;
            this.SampleRate = 1;
            Values = new FixedLengthQueue<float>(historyLength);
        }

        public DataSampler(String name, int historyLength, int sampleRate, Func<float, float> function)
        {
            this.Name = name;
            this.Function2 = function;
            this.SampleRate = sampleRate;
            Values = new FixedLengthQueue<float>(historyLength);
        }
        
        public DataSampler(String name, int historyLength, int sampleRate, Func<float> function)
        {
            this.Name = name;
            this.Function = function;
            this.SampleRate = sampleRate;
            Values = new FixedLengthQueue<float>(historyLength);
        }

        public void GetLimits(out float min, out float max)
        {
            max = float.MinValue;
            min = float.MaxValue;
            foreach (float value in Values)
            {
                if (value > max)
                    max = value;
                if (value < min)
                    min = value;
            }
            max = (float)Math.Ceiling(max);
            min = (float)Math.Floor(min);
        }

        public void Update(GameTime gameTime)
        {
            if (SampleRate == 0 || (Function == null && Function2 == null))
                return;

            elapsedFrames++;

            if (elapsedFrames >= SampleRate)
            {
                if (Function != null)
                    Values.Enqueue(Function());
                else
                    Values.Enqueue(Function2((float)gameTime.ElapsedGameTime.TotalSeconds));
                elapsedFrames = 0;
            }
        }

        /// <summary>
        /// Takes a sample from the bound function.
        /// </summary>
        public void TakeSample()
        {
            if (Function != null)
                Values.Enqueue(Function());
        }

        /// <summary>
        /// Inserts a sample. This method must be used with samplers that are
        /// not bound to a function.
        /// </summary>
        /// <param name="value"></param>
        public void InsertSample(float value)
        {
            Values.Enqueue(value);
        }
    }

    /// <summary>
    /// Keeps values of functions so they can be plotted.
    /// </summary>
    public class DataSamplerManager : Gear
    {
        private Dictionary<String, DataSampler> samplers;

        public DataSamplerConfig Config { get { return GearsetSettings.Instance.DataSamplerConfig; } }

#if WINDOWS || LINUX || MONOMAC
        public ObservableCollection<DataSampler> observableSamplers;
        public ReadOnlyObservableCollection<DataSampler> Samplers { get; private set; }
#else
        public List<DataSampler> observableSamplers;
        public List<DataSampler> Samplers { get; private set; }
#endif
        public DataSamplerManager()
            : base(GearsetSettings.Instance.DataSamplerConfig)
        {
            samplers = new Dictionary<string, DataSampler>();
#if WINDOWS || LINUX || MONOMAC
            observableSamplers = new ObservableCollection<DataSampler>();
            Samplers = new ReadOnlyObservableCollection<DataSampler>(observableSamplers);
#else
            observableSamplers = new List<DataSampler>();
            Samplers = new List<DataSampler>(observableSamplers);
#endif
        }

        /// <summary>
        /// Adds an sampler which is bound to a <c>function</c> that will be sampled
        /// every <c>sampleRate</c> frames. <c>historyLength</c> values will be kept.
        /// </summary>
        public void AddSampler(String name, int historyLength, int sampleRate, Func<float> function)
        {
            DataSampler sampler = new DataSampler(name, historyLength, sampleRate, function);
            InsertSampler(name, sampler);
        }

        private void InsertSampler(String name, DataSampler sampler)
        {
            samplers.Add(name, sampler);
            observableSamplers.Add(sampler);
        }

        /// <summary>
        /// Adds an sampler which is bound to a <c>function</c> that will be sampled
        /// every <c>sampleRate</c> frames. <c>historyLength</c> values will be kept.
        /// </summary>
        public void AddSampler(String name, int historyLength, int sampleRate, Func<float, float> function)
        {
            DataSampler sampler = new DataSampler(name, historyLength, sampleRate, function);
            InsertSampler(name, sampler);
        }

        /// <summary>
        /// Adds a single sample to the sampler of the specified name, if the sampler does
        /// not exists it will be created. This function is intended to be used with sampler that
        /// are not bound to a function.
        /// </summary>
        public void AddSample(String name, float value)
        {
            DataSampler sampler;
            if (!samplers.TryGetValue(name, out sampler))
            {
                sampler = new DataSampler(name);
                InsertSampler(name, sampler);
            }
            sampler.InsertSample(value);
        }

        /// <summary>
        /// Adds a single sample to the sampler of the specified name, if the sampler does
        /// not exists it will be created with the specified historyLength. 
        /// This function is intended to be used with sampler that are not bound to a function.
        /// </summary>
        public void AddSample(string name, float value, int historyLength)
        {
            DataSampler sampler;
            if (!samplers.TryGetValue(name, out sampler))
            {
                sampler = new DataSampler(name, historyLength, 0, default(Func<float>));
                InsertSampler(name, sampler);
            }
            else
            {
                sampler.Values.Capacity = historyLength;
            }
            sampler.InsertSample(value);
        }

        internal DataSampler GetSampler(String name)
        {
            DataSampler sampler;
            if (!samplers.TryGetValue(name, out sampler))
            {
                AddSampler(name, Config.DefaultHistoryLength, 0, (Func<float>)null);
                return samplers[name];
            }
            return sampler;
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            // Update all samplers.
            foreach (var sampler in samplers.Values)
            {
                sampler.Update(gameTime);
            }
        }

        
    }
}
