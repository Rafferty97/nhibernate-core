﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by AsyncGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------


using System.Collections.Generic;
using NHibernate.Criterion;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.ManyToOneFilters20Behaviour
{
	using System.Threading.Tasks;
	using System.Threading;
	[TestFixture]
	public class FixtureAsync : BugTestCase
	{
		private static Task<IList<Parent>> JoinGraphUsingHqlAsync(ISession s, CancellationToken cancellationToken = default(CancellationToken))
		{
			try
			{
				const string hql = @"select p from Parent p
			                     join p.Child c";
				return s.CreateQuery(hql).ListAsync<Parent>(cancellationToken);
			}
			catch (System.Exception ex)
			{
				return Task.FromException<IList<Parent>>(ex);
			}
		}

		private static Task<IList<Parent>> JoinGraphUsingCriteriaAsync(ISession s, CancellationToken cancellationToken = default(CancellationToken))
		{
			try
			{
				return s.CreateCriteria(typeof(Parent)).Fetch("Child").ListAsync<Parent>(cancellationToken);
			}
			catch (System.Exception ex)
			{
				return Task.FromException<IList<Parent>>(ex);
			}
		}

		private static Parent CreateParent()
		{
			var ret = new Parent { Child = new Child() };
			ret.Address = new Address { Parent = ret };
			return ret;
		}

		private static void EnableFilters(ISession s)
		{
			var f = s.EnableFilter("activeChild");
			f.SetParameter("active", true);
			var f2 = s.EnableFilter("alwaysValid");
			f2.SetParameter("always", true);
		}

		protected override void OnTearDown()
		{
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				s.Delete("from Parent");
				tx.Commit();
			}
		}

		[Test]
		public async Task VerifyAlwaysFilterAsync()
		{
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var p = CreateParent();
				p.Child.Always = false;
				await (s.SaveAsync(p));
				await (tx.CommitAsync());
			}

			using (var s = OpenSession())
			{
				EnableFilters(s);
				var resCriteria = await (JoinGraphUsingCriteriaAsync(s));
				var resHql = await (JoinGraphUsingHqlAsync(s));

				Assert.That(resCriteria.Count, Is.EqualTo(0));
				Assert.That(resHql.Count, Is.EqualTo(0));
			}
		}

		[Test]
		public async Task VerifyFilterActiveButNotUsedInManyToOneAsync()
		{
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				await (s.SaveAsync(CreateParent()));
				await (tx.CommitAsync());
			}

			using (var s = OpenSession())
			{
				EnableFilters(s);
				var resCriteria = await (JoinGraphUsingCriteriaAsync(s));
				var resHql = await (JoinGraphUsingHqlAsync(s));

				Assert.That(resCriteria.Count, Is.EqualTo(1));
				Assert.That(resCriteria[0].Child, Is.Not.Null);

				Assert.That(resHql.Count, Is.EqualTo(1));
				Assert.That(resHql[0].Child, Is.Not.Null);
			}
		}

		[Test]
		public async Task VerifyQueryWithWhereClauseAsync()
		{
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var p = CreateParent();
				p.ParentString = "a";
				p.Child.ChildString = "b";
				await (s.SaveAsync(p));
				await (tx.CommitAsync());
			}
			IList<Parent> resCriteria;
			IList<Parent> resHql;
			using (var s = OpenSession())
			{
				EnableFilters(s);
				resCriteria = await (s.CreateCriteria(typeof(Parent), "p")
				               .CreateCriteria("Child", "c")
				               .Fetch("Child")
				               .Add(Restrictions.Eq("p.ParentString", "a"))
				               .Add(Restrictions.Eq("c.ChildString", "b"))
				               .ListAsync<Parent>());

				resHql = await (s.CreateQuery(
					          @"select p from Parent p
				                join fetch p.Child c
				                where p.ParentString='a' and c.ChildString='b'")
				          .ListAsync<Parent>());
			}
			Assert.That(resCriteria.Count, Is.EqualTo(1));
			Assert.That(resCriteria[0].Child, Is.Not.Null);

			Assert.That(resHql.Count, Is.EqualTo(1));
			Assert.That(resHql[0].Child, Is.Not.Null);
		}

		[Test]
		public async Task VerifyAlwaysFiltersOnPropertyRefAsync()
		{
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var p = CreateParent();
				await (s.SaveAsync(p));
				await (tx.CommitAsync());
			}

			using (var s = OpenSession())
			{
				EnableFilters(s);
				var resCriteria = await (JoinGraphUsingCriteriaAsync(s));
				var resHql = await (JoinGraphUsingHqlAsync(s));

				Assert.That(resCriteria.Count, Is.EqualTo(1));
				Assert.That(resCriteria[0].Address, Is.Not.Null);
				Assert.That(resHql.Count, Is.EqualTo(1));
				Assert.That(resHql[0].Address, Is.Not.Null);
			}
		}

		[Test]
		public async Task ExplicitFiltersOnCollectionsShouldBeActiveAsync()
		{
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var p = CreateParent();
				p.Children = new List<Child>
				{
					new Child {IsActive = true},
					new Child {IsActive = false},
					new Child {IsActive = true}
				};
				await (s.SaveAsync(p));
				await (tx.CommitAsync());
			}

			using (var s = OpenSession())
			{
				var f = s.EnableFilter("active");
				f.SetParameter("active", true);
				var resCriteria = await (JoinGraphUsingCriteriaAsync(s));
				var resHql = await (JoinGraphUsingHqlAsync(s));

				Assert.That(resCriteria.Count, Is.EqualTo(1));
				Assert.That(resCriteria[0].Children.Count, Is.EqualTo(2));
				Assert.That(resHql.Count, Is.EqualTo(1));
				Assert.That(resHql[0].Children.Count, Is.EqualTo(2));
			}
		}

		[Test]
		public async Task ExplicitFiltersOnCollectionsShouldBeActiveWithEagerLoadAsync()
		{
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				var p = CreateParent();
				p.Children = new List<Child>
				{
					new Child {IsActive = true},
					new Child {IsActive = false},
					new Child {IsActive = true}
				};
				await (s.SaveAsync(p));
				await (tx.CommitAsync());
			}

			using (var s = OpenSession())
			{
				var f = s.EnableFilter("active");
				f.SetParameter("active", true);
				var resCriteria = await (s.CreateCriteria(typeof(Parent)).Fetch("Children").ListAsync<Parent>());
				var resHql = await (s.CreateQuery("select p from Parent p join fetch p.Children").ListAsync<Parent>());

				Assert.That(resCriteria[0].Children.Count, Is.EqualTo(2));
				Assert.That(resHql[0].Children.Count, Is.EqualTo(2));
			}
		}

		[Test]
		public async Task Verify20BehaviourForPropertyRefAndFilterAsync()
		{
			using (var s = OpenSession())
			using (var tx = s.BeginTransaction())
			{
				await (s.SaveAsync(CreateParent()));
				await (tx.CommitAsync());
			}
			using (var s = OpenSession())
			using (s.BeginTransaction())
			{
				s.EnableFilter("active")
				 .SetParameter("active", true);

				var resCriteria = await (s.CreateCriteria(typeof(Parent))
				                   .Fetch("Address")
				                   .ListAsync<Parent>());

				var resHql = await (s.CreateQuery("select p from Parent p join p.Address")
				              .ListAsync<Parent>());

				Assert.That(resCriteria.Count, Is.EqualTo(1));
				Assert.That(resCriteria[0].Address, Is.Not.Null);

				Assert.That(resHql.Count, Is.EqualTo(1));
				Assert.That(resHql[0].Address, Is.Not.Null);
			}
		}

	}
}
