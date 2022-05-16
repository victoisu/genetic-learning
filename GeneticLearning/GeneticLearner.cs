using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GeneticLearning
{
	public abstract class GeneticLearner<T>
	{
		private bool _isSorted = false;
		private List<LearningInstance<T>> _population;
		public List<T> Population
		{
			get
			{
				return _population.Select(p => p.Instance).ToList();
			}
		}

		public T BestInstance
		{
			get
			{
				if (!_isSorted)
					Sort();
				return _population[0].Instance;
			}
		}

		private readonly Random _rng;

		public abstract int GenomeSize { get; }
		public GeneticLearner() {
			_rng = new Random();
		}

		public void GeneratePopulation(int size)
		{
			_population = new(size);
			for (int i = 0; i < size; i++)
			{
				_population.Add(GenerateIndividual());
			}
			_isSorted = false;
		}

		private LearningInstance<T> GenerateIndividual()
		{
			var genes = new BitArray(GenomeSize);
			for (int i = 0; i < GenomeSize; i++)
			{
				genes[i] = (_rng.Next() & 1) == 0;
			}
			return CreateInstance(genes, default);
		}

		private BitArray Cross(BitArray p1, BitArray p2, double mutation = 0.1)
		{
			bool take1 = (_rng.Next() & 1) == 0;
			var child = new BitArray(take1 ? p1 : p2);
			for (int i = 0; i < GenomeSize; i++)
			{
				var flip = _rng.NextDouble() < (2.0/GenomeSize);
				take1 ^= flip;
				if (_rng.NextDouble() < mutation)
				{
					child[i] = (_rng.Next() & 1) == 0;
				}
				else
				{
					child[i] = take1 ? p1[i] : p2[i];
				}
			}
			return child;
		}

		private (BitArray, BitArray) GetParents(int maxIndex)
		{
			return (_population[_rng.Next(maxIndex)].Genes, _population[_rng.Next(maxIndex)].Genes);
		} 

		public void NextGeneration(double keep, double children)
		{
			if (!_isSorted)
				Sort();
			_isSorted = false;
			int lastParent = (int)(keep * _population.Count);
			int lastChild = (int)((keep + children) * _population.Count);
			for (int i = lastParent; i < lastChild; i++)
			{
				var parents = GetParents(lastParent);
				_population[i].Genes = Cross(parents.Item1, parents.Item2);
				_population[i].Instance = default;
			}
			for (int i = lastChild; i < _population.Count; i++)
			{
				_population[i] = GenerateIndividual();
			}
		}

		public abstract double Fitness(T instance);

		protected abstract BitArray InstanceToGenes(T instance);

		private BitArray InstanceToGenesIntern(T instance)
		{
			var genes = InstanceToGenes(instance);
			if (genes.Count != GenomeSize)
				throw new ArgumentException($"Genes produced do not match expected genome size. Given: {genes.Count}, Expected: {GenomeSize}.");

			return genes;
		}

		protected abstract T GenesToInstance(BitArray genes);

		private T GenesToInstanceIntern(BitArray genes)
		{
			if (genes.Count != GenomeSize)
				throw new ArgumentException($"Genes given do not match expected genome size. Given: {genes.Count}, Expected: {GenomeSize}.");

			return GenesToInstance(genes);
		}

		private void Sort()
		{
			_population = _population.OrderByDescending(p => Fitness(p.Instance)).ToList();
			_isSorted = true;
		}

		private LearningInstance<T> CreateInstance(BitArray genes, T instance)
		{
			return new LearningInstance<T>(this, genes, instance);
		}

		private class LearningInstance<N> where N : T
		{
			private GeneticLearner<N> _learner;
			private BitArray _genes;
			public BitArray Genes
			{
				get
				{
					if (_genes == null)
						_genes = _learner.InstanceToGenesIntern(_instance);
					return _genes;
				}
				set => _genes = value;
			}

			private N _instance;
			public N Instance
			{
				get
				{
					if (_instance == null)
						_instance = _learner.GenesToInstanceIntern(_genes);
					return _instance;
				}
				set => _instance = value;
			}

			public LearningInstance(GeneticLearner<N> learner, BitArray genes, N instance)
			{
				_learner = learner;
				_genes = genes;
				_instance = instance;
			}
		}
	}
}
