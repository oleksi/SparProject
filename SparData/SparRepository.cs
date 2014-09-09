﻿using NHibernate;
using SparModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SparData
{
	public class SparRepository
	{
		private static ISession getSession()
		{
			return SessionProvider.SessionFactory.OpenSession();
		}

		public SparRequest GetSparRequestById(string SparRequestId)
		{
			using (var session = getSession())
			{
				return session.Get<SparRequest>(SparRequestId);
			}
		}

		public void CreateSparRequest(SparRequest sparRequest)
		{
			using (var session = getSession())
			{
				using (var transaction = session.BeginTransaction())
				{
					sparRequest.Id = Guid.NewGuid().ToString();

					session.Save(sparRequest);
					transaction.Commit();
				}
			}
		}
	}
}