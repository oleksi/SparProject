﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SparModel;
using NHibernate;

namespace SparData
{
    public class FighterRepository
    {
		private static ISession getSession()
		{
			return SessionProvider.SessionFactory.OpenSession();
		}

		public IList<Fighter> GetAllFighters()
		{
			IList<Fighter> fighters = new List<Fighter>();
			using (var session = getSession())
			{
				fighters = session.QueryOver<Fighter>().OrderBy(ff => ff.ProfilePictureUploaded).Desc.OrderBy(ff => ff.InsertDate).Desc.JoinQueryOver<SparIdentityUser>(ff => ff.SparIdentityUser).Where(iu => iu.EmailConfirmed == true).List();
			}

			return fighters;
		}

		public IList<Fighter> GetRegisterredFightersSince(DateTime sinceDate)
		{
			IList<Fighter> fighters = new List<Fighter>();
			using (var session = getSession())
			{
				fighters = session.QueryOver<Fighter>().Where(ff => ff.InsertDate > sinceDate).OrderBy(ff => ff.InsertDate).Desc.JoinQueryOver<SparIdentityUser>(ff => ff.SparIdentityUser).Where(iu => iu.EmailConfirmed == true).List();
			}

			return fighters;
		}

		public IList<Fighter> GetFightersWithProfilePics()
		{
			IList<Fighter> fighters = new List<Fighter>();
			using (var session = getSession())
			{
				fighters = session.QueryOver<Fighter>().Where(ff => ff.ProfilePictureUploaded && ff.IsDemo == false).OrderBy(ff => ff.InsertDate).Desc.JoinQueryOver<SparIdentityUser>(ff => ff.SparIdentityUser).Where(iu => iu.EmailConfirmed == true).List();
			}

			return fighters;
		}

		public Fighter GetFighterById(int fighterId)
		{
			Fighter fighter = null;
			using (var session = getSession())
			{
				fighter = session.Get<Fighter>(fighterId);
			}

			return fighter;
		}

		public Fighter GetFighterByIdentityUserId(string identityUserId)
		{
			Fighter fighter = null;
			using (var session = getSession())
			{
				fighter = session.QueryOver<Fighter>().Where(m => m.SparIdentityUser.Id == identityUserId).SingleOrDefault();
			}

			return fighter;
		}

		public Fighter GetFighterByStateAndName(string state, string name)
		{
			Fighter fighter = null;
			using (var session = getSession())
			{
				fighter = session.QueryOver<Fighter>().Where(m => m.State == state && m.Name == name.ToLower()).SingleOrDefault();
			}

			return fighter;
		}

		public void SaveFighter(Fighter fighter)
		{
			using (var session = getSession())
			{
				using (var transaction = session.BeginTransaction())
				{
					if (fighter.Id == null)
						fighter.InsertDate = fighter.UpdateDate = DateTime.Now;
					else
						fighter.UpdateDate = DateTime.Now;

					session.SaveOrUpdate(fighter);
					transaction.Commit();
				}
			}
		}
    }
}
