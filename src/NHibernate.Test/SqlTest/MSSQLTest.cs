using System;
using System.Collections;
using NUnit.Framework;

namespace NHibernate.Test.SqlTest
{
	[TestFixture]
	public class MSSQLTest : HandSQLTest
	{
		protected override IList Mappings
		{
			get { return new string[] {"SqlTest.MSSQLEmployment.hbm.xml"}; }
		}

		protected override System.Type GetDialect()
		{
			return typeof(NHibernate.Dialect.MsSql2000Dialect);
		}
	}
}
