using DiagnosesRegistry.Common.Interfaces;

namespace DiagnosesRegistry.Common
{
	public class Specification<T> : ISpecification<T>
	{
		private readonly Func<T, bool> _predicate;

		public Specification(Func<T, bool> predicate)
		{
			_predicate = predicate;
		}

		public bool IsSatisfiedBy(T entity)
		{
			return _predicate(entity);
		}

		public Specification<T> And(ISpecification<T> other)
		{
			return new Specification<T>(entity => this.IsSatisfiedBy(entity) && other.IsSatisfiedBy(entity));
		}

		public Specification<T> Or(ISpecification<T> other)
		{
			return new Specification<T>(entity => this.IsSatisfiedBy(entity) || other.IsSatisfiedBy(entity));
		}
	}
}
