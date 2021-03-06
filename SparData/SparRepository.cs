﻿using NHibernate;
using NHibernate.Transform;
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

		public List<SparNote> GetSparNotes(string SparRequestId)
		{
			using (var session = getSession())
			{
				return session.GetNamedQuery("SparRequest_GetNotesBySparRequestId")
					.SetString("SparRequestId", SparRequestId)
					.SetResultTransformer(Transformers.AliasToBean(typeof(SparNote)))
					.List<SparNote>().ToList();
			}
		}

		public void CreateSparRequest(SparRequest sparRequest)
		{
			sparRequest.LastUpdateDate = sparRequest.RequestDate;

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
			sparRequest.LastUpdateDate = DateTime.Now;

			using (var session = getSession())
			{
				session.Evict(sparRequest);
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
						&& (sr.Status == SparRequestStatus.DateLocationNegotiation || sr.Status == SparRequestStatus.Requested)
					).OrderBy(sr => sr.LastUpdateDate).Desc.List();
			}

			return sparRequests;
		}

		public IList<SparActivity> GetSparRequestActivitiesSince(DateTime sinceDate)
		{
			using (var session = getSession())
			{
				return session.GetNamedQuery("SparRequest_GetAllActiveSince")
					.SetString("SinceDate", sinceDate.ToShortDateString())
					.SetResultTransformer(Transformers.AliasToBean(typeof(SparActivity)))
					.List<SparActivity>().ToList();
			}
		}
	}
}
