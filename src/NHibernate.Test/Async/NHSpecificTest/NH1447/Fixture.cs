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
using NHibernate.Dialect.Function;
using NUnit.Framework;

namespace NHibernate.Test.NHSpecificTest.NH1447
{
	using System.Threading.Tasks;
	[TestFixture]
	public class FixtureAsync : BugTestCase
	{
		protected override void OnTearDown()
		{
			base.OnTearDown();
			using (ISession session = OpenSession())
			{
				using (ITransaction tx = session.BeginTransaction())
				{
					session.Delete("from Person");
					tx.Commit();
				}
			}
		}

		protected override void OnSetUp()
		{
			using (ISession s = OpenSession())
			{
				using (ITransaction tx = s.BeginTransaction())
				{
					var e1 = new Person("Tuna Toksoz",false);
					var e2 = new Person("Oguz Kurumlu", true);
					s.Save(e1);
					tx.Commit();
				}
			}
		}

		[Test]
		public async Task CanQueryByConstantProjectionWithTypeAsync()
		{
			using (ISession s = OpenSession())
			{
				ICriteria c = s.CreateCriteria(typeof (Person))
					.Add(Restrictions.EqProperty("WantsNewsletter", Projections.Constant(false,NHibernateUtil.Boolean)));
				IList<Person> list = await (c.ListAsync<Person>());
				Assert.AreEqual(1, list.Count);
			}
		}
	}
}
