﻿using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SparModel;

namespace SparData
{
	public class GymRepository
	{
		private static ISession getSession()
		{
			return SessionProvider.SessionFactory.OpenSession();
		}

		public Gym GetGymById(int gymId)
		{
			Gym gym = null;
			using (var session = getSession())
			{
				gym = session.Get<Gym>(gymId);
			}

			return gym;
		}

		public IList<Gym> GetAllGyms()
		{
			IList<Gym> allGyms = null;
			using (var session = getSession())
			{
				allGyms = session.QueryOver<Gym>().Where(gg => gg.StreetAddress != null).OrderBy(gg => gg.Name).Asc.List();
			}

			return allGyms;
		}

		public IList<Gym> GetGymsWithProfilePics()
		{
			IList<Gym> gyms = new List<Gym>();

			using (var session = getSession())
			{
				gyms = session.QueryOver<Gym>().Where(gg => gg.GymPictureUploaded).OrderBy(ff => ff.InsertDate).Desc.List();
			}

			return gyms;
		}

		public Gym GetGymByStateAndName(string state, string name)
		{
			Gym gym = null;
			using (var session = getSession())
			{
				gym = session.QueryOver<Gym>().Where(m => m.State == state && m.Name == name.ToLower()).SingleOrDefault();
			}

			return gym;
		}

		public void SaveGym(Gym gym)
		{
			using (var session = getSession())
			{
				using (var transaction = session.BeginTransaction())
				{
					if (gym.Id == null)
						gym.InsertDate = gym.UpdateDate = DateTime.Now;
					else
						gym.UpdateDate = DateTime.Now;

					session.SaveOrUpdate(gym);
					transaction.Commit();
				}
			}
		}
	}
}
