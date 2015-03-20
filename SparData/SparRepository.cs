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

		public void SaveSparRequest(SparRequest sparRequest)
		{
			using (var session = getSession())
			{
				using (var transaction = session.BeginTransaction())
				{
					session.Update(sparRequest);
					transaction.Commit();
				}
			}
		}

		public IList<SparRequest> GetSparRequestsForFighter(int fighterId)
		{
			IList<SparRequest> sparRequests = null;

			using (var session = getSession())
			{
				sparRequests = session.QueryOver<SparRequest>().Where(sr => (sr.RequestorFighter.Id == fighterId || sr.OpponentFighter.Id == fighterId)
						&& ((sr.Status == SparRequestStatus.Confirmed && sr.SparDateTime > DateTime.Now)
							|| ((sr.Status == SparRequestStatus.DateLocationNegotiation || sr.Status == SparRequestStatus.Requested) && sr.RequestDate > DateTime.Now.AddDays(-14))
						)
					).OrderBy(sr => sr.RequestDate).Desc.List();
			}

			return sparRequests;
		}
	}
}
